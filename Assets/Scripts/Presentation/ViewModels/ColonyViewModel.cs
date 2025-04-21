using System;
using System.Collections.Generic;
using System.Threading;
using ColonyDefender;
using ColonyDefender.Core;
using ColonyDefender.Infrastructure.Services;
using UniRx;
using UnityEngine;

namespace ColonyDefender.Presentation
{
    public class ColonyViewModel : IDisposable
    {
        private readonly ColonyData _data;
        private readonly IResourceService _resources;
        private readonly IBuildingService _buildings;
        private readonly CompositeDisposable _disposables = new();

        // resource properties
        public IReadOnlyReactiveProperty<int> Energy => _data.Energy;
        public IReadOnlyReactiveProperty<int> Minerals => _data.Minerals;
        public IReadOnlyReactiveProperty<int> Population => _data.Population;
        public IReadOnlyReactiveProperty<int> MaxPopulation => _data.MaxPopulation;
        public IReadOnlyReactiveProperty<int> EnergyProduction => _data.EnergyProduction;
        public IReadOnlyReactiveProperty<int> MineralProduction => _data.MineralProduction;
        
        // building selection
        public ReactiveProperty<BuildingType?> SelectedBuildingType { get; } = new();
        
        // wave information
        public ReactiveProperty<string> CurrentWaveName { get; } = new("No wave");
        public ReactiveProperty<int> EnemiesRemaining { get; } = new(0);
        public ReactiveProperty<bool> IsBossActive { get; } = new(false);
        
        public ColonyViewModel(ColonyData data, IResourceService resources, IBuildingService buildings = null)
        {
            _data = data;
            _resources = resources;
            _buildings = buildings;
        
            MessageBroker.Default.Receive<BuildingClicked>()
                .Subscribe(OnBuildingClicked)
                .AddTo(_disposables);
                
            MessageBroker.Default.Receive<CellClicked>()
                .Subscribe(OnCellClicked)
                .AddTo(_disposables);
                
            MessageBroker.Default.Receive<WaveStarted>()
                .Subscribe(OnWaveStarted)
                .AddTo(_disposables);
                
            MessageBroker.Default.Receive<WaveCompleted>()
                .Subscribe(OnWaveCompleted)
                .AddTo(_disposables);
                
            MessageBroker.Default.Receive<EnemySpawned>()
                .Subscribe(OnEnemySpawned)
                .AddTo(_disposables);
                
            MessageBroker.Default.Receive<EnemyDefeated>()
                .Subscribe(OnEnemyDefeated)
                .AddTo(_disposables);
        }

        private void OnBuildingClicked(BuildingClicked evt)
        {
            SelectedBuildingType.Value = evt.BuildingType;
        }
        
        private void OnCellClicked(CellClicked evt)
        {
            if (!SelectedBuildingType.HasValue || _buildings == null)
                return;
            
            var buildingType = SelectedBuildingType.Value.Value;
            var token = CancellationToken.None;
            
            _buildings.PlaceBuildingAsync(buildingType, evt.GridPosition, token);
        }
        
        private void OnWaveStarted(WaveStarted evt)
        {
            CurrentWaveName.Value = $"Wave {evt.Wave.WaveNumber}: {evt.Wave.WaveName}";
            IsBossActive.Value = false;
        }
        
        private void OnWaveCompleted(WaveCompleted evt)
        {
            CurrentWaveName.Value = "Wave Complete!";
            EnemiesRemaining.Value = 0;
            IsBossActive.Value = false;
        }
        
        private void OnEnemySpawned(EnemySpawned evt)
        {
            EnemiesRemaining.Value++;
            
            if (evt.IsBoss)
            {
                IsBossActive.Value = true;
                MessageBroker.Default.Publish(new ShowNotification { Message = "BOSS HAS SPAWNED!" });
            }
        }
        
        private void OnEnemyDefeated(EnemyDefeated evt)
        {
            EnemiesRemaining.Value = Math.Max(0, EnemiesRemaining.Value - 1);
        }

        public void Dispose() => _disposables.Dispose();
    }
}