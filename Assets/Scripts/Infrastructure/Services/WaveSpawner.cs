using System;
using System.Threading;
using ColonyDefender.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ColonyDefender.Infrastructure.Services
{
    public class WaveSpawner : IWaveManager
    {
        public async UniTaskVoid StartWaveAsync(WaveConfig wave, CancellationToken token)
        {
            UniTask.Void(async () =>
            {
                foreach (var enemy in wave.Enemies)
                {
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(wave.SpawnInterval),
                        cancellationToken: token
                    );

                    if (token.IsCancellationRequested)
                        return;

                    InstantiateEnemy(enemy);
                }
            });
        }

        private void InstantiateEnemy(GameObject prefab)
        {
            // TODO: object pooling

        }
    }
}