using System;
using ColonyDefender.Core;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using System.Threading;

namespace ColonyDefender.Infrastructure.Services
{
    public class ResourceService : IResourceService, IDisposable
    {
        private readonly ColonyData _data;
        private readonly CompositeDisposable _disposables = new();
        private readonly float _resourceUpdateInterval = 5f; // seconds

        public ResourceService(ColonyData data)
        {
            _data = data;
            
            _data.Population
                .Where(p => p >= 100)
                .Subscribe(_ => Debug.Log("Population milestone!"))
                .AddTo(_disposables);
                
            // start resource update loop
            UpdateResourcesLoop().Forget();
        }

        public bool TrySpendEnergy(int amount)
        {
            if (_data.Energy.Value < amount) return false;
            _data.Energy.Value -= amount;
            return true;
        }
        
        public bool TrySpendMinerals(int amount)
        {
            if (_data.Minerals.Value < amount) return false;
            _data.Minerals.Value -= amount;
            return true;
        }
        
        public bool TrySpendResources(int energyAmount, int mineralAmount)
        {
            if (!HasEnoughResources(energyAmount, mineralAmount)) return false;
            
            _data.Energy.Value -= energyAmount;
            _data.Minerals.Value -= mineralAmount;
            return true;
        }
        
        public bool HasEnoughResources(int energyAmount, int mineralAmount)
        {
            return _data.Energy.Value >= energyAmount && _data.Minerals.Value >= mineralAmount;
        }

        public void AddEnergy(int amount)
        {
            _data.Energy.Value += amount;
        }
        
        public void AddMinerals(int amount)
        {
            _data.Minerals.Value += amount;
        }
        
        public void UpdateResourceProduction()
        {
            // add resources based on production rates
            _data.Energy.Value += _data.EnergyProduction.Value;
            _data.Minerals.Value += _data.MineralProduction.Value;
            
            // update population based on growth rate and available housing
            var populationGrowth = CalculatePopulationGrowth();
            if (populationGrowth > 0 && _data.Population.Value < _data.MaxPopulation.Value)
            {
                _data.Population.Value = Mathf.Min(
                    _data.Population.Value + populationGrowth, 
                    _data.MaxPopulation.Value
                );
            }
        }
        
        private int CalculatePopulationGrowth()
        {
            // calculate growth based on current population and max population
            // higher growth when population is low relative to max population
            float growthRate = 0.05f; // base growth rate of 5%
            int availableSpace = _data.MaxPopulation.Value - _data.Population.Value;
            
            if (availableSpace <= 0) return 0;
            
            float spaceRatio = (float)availableSpace / _data.MaxPopulation.Value;
            float adjustedGrowthRate = growthRate * spaceRatio;
            
            return Mathf.Max(1, Mathf.FloorToInt(_data.Population.Value * adjustedGrowthRate));
        }
        
        private async UniTaskVoid UpdateResourcesLoop()
        {
            var token = CancellationToken.None;
            
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_resourceUpdateInterval), cancellationToken: token);
                    UpdateResourceProduction();
                }
            }
            catch (OperationCanceledException)
            {
                // expected when disposing
            }
        }

        public void Dispose() => _disposables.Dispose();
    }
}