using System;
using System.Collections.Generic;
using System.Threading;
using ColonyDefender.Core;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ColonyDefender.Infrastructure.Services
{
    public class BuildingService : IBuildingService, IDisposable
    {
        private readonly ColonyData _data;
        private readonly IResourceService _resourceService;
        private readonly MapGrid _grid;
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<BuildingType, BuildingConfig> _buildingConfigs = new();
        private readonly Dictionary<Building, GameObject> _buildingObjects = new();
        
        private readonly Subject<BuildingPlaced> _buildingPlaced = new();
        private readonly Subject<BuildingRemoved> _buildingRemoved = new();
        
        public IObservable<BuildingPlaced> OnBuildingPlaced => _buildingPlaced;
        public IObservable<BuildingRemoved> OnBuildingRemoved => _buildingRemoved;
        
        public BuildingService(ColonyData data, IResourceService resourceService, MapGrid grid, BuildingConfig[] configs)
        {
            _data = data;
            _resourceService = resourceService;
            _grid = grid;
            
            foreach (var config in configs)
            {
                _buildingConfigs[config.Type] = config;
            }
            
            MessageBroker.Default.Receive<BuildingDamaged>()
                .Subscribe(OnBuildingDamaged)
                .AddTo(_disposables);
        }
        
        public bool CanPlaceBuilding(BuildingType type, Vector2Int position)
        {
            if (!_buildingConfigs.TryGetValue(type, out var config))
            {
                Debug.LogError($"Building config not found for type: {type}");
                return false;
            }
            
            if (!_resourceService.HasEnoughResources(config.EnergyCost, config.MineralCost))
            {
                return false;
            }
            
            return _grid.CanPlaceBuilding(position, config.Size);
        }
        
        public async UniTask<bool> PlaceBuildingAsync(BuildingType type, Vector2Int position, CancellationToken token)
        {
            if (!CanPlaceBuilding(type, position))
            {
                return false;
            }
            
            var config = _buildingConfigs[type];
            
            if (!_resourceService.TrySpendResources(config.EnergyCost, config.MineralCost))
            {
                return false;
            }
            
            var building = config.CreateBuilding(position);
            if (!_grid.PlaceBuilding(building))
            {
                // refund resources if placement fails
                _resourceService.AddEnergy(config.EnergyCost);
                _resourceService.AddMinerals(config.MineralCost);
                return false;
            }
            
            // create visual representation
            var buildingObj = Object.Instantiate(
                config.Prefab, 
                new Vector3(position.x + config.Size.x / 2f, 0, position.y + config.Size.y / 2f),
                Quaternion.identity
            );
            
            _buildingObjects[building] = buildingObj;
            
            // simulate building time
            await UniTask.Delay(TimeSpan.FromSeconds(config.BuildTime), cancellationToken: token);
            
            // update production
            UpdateResourceProduction(building, true);
            
            _buildingPlaced.OnNext(new BuildingPlaced { Building = building, Position = position });
            
            return true;
        }
        
        public bool RemoveBuilding(Vector2Int position)
        {
            var building = _grid.GetBuildingAt(position);
            if (building == null)
            {
                return false;
            }
            
            if (!_grid.RemoveBuilding(position))
            {
                return false;
            }
            
            // update production
            UpdateResourceProduction(building, false);
            
            // destroy visual representation
            if (_buildingObjects.TryGetValue(building, out var buildingObj))
            {
                Object.Destroy(buildingObj);
                _buildingObjects.Remove(building);
            }
            
            _buildingRemoved.OnNext(new BuildingRemoved { Position = position });
            
            return true;
        }
        
        private void OnBuildingDamaged(BuildingDamaged evt)
        {
            var building = _grid.GetBuildingAt(evt.Position);
            if (building == null)
            {
                return;
            }
            
            building.TakeDamage(evt.Damage);
            
            if (building.IsDestroyed)
            {
                RemoveBuilding(evt.Position);
            }
        }
        
        private void UpdateResourceProduction(Building building, bool add)
        {
            var multiplier = add ? 1 : -1;
            
            // update resource production
            _data.EnergyProduction.Value += building.EnergyProduction * multiplier;
            _data.MineralProduction.Value += building.MineralProduction * multiplier;
            _data.MaxPopulation.Value += building.PopulationSupport * multiplier;
        }
        
        public void Dispose() => _disposables.Dispose();
    }
} 