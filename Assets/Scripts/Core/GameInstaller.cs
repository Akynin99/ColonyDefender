﻿using ColonyDefender.Infrastructure.Services;
using ColonyDefender.Presentation;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ColonyDefender.Core
{
    public class GameInstaller : MonoBehaviour
    {
        [SerializeField] private WaveConfig initialWave;
    
        private ColonyData _data;
        private IResourceService _resources;
        private ColonyViewModel _viewModel;

        private void Start()
        {
            _data = new ColonyData();
            _resources = new ResourceService(_data);
            _viewModel = new ColonyViewModel(_data, _resources);
        
            // first wave launch
            var waveManager = new WaveSpawner(_resources);
            var cancellationToken = this.GetCancellationTokenOnDestroy();
            waveManager.StartWaveAsync(initialWave, cancellationToken).Forget();
        }

        private void OnDestroy()
        {
            _viewModel?.Dispose();
        }
    }
}