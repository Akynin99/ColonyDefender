using System;
using System.Collections.Generic;
using System.Threading;
using ColonyDefender.Core;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ColonyDefender.Infrastructure.Services
{
    public class WaveSpawner : IWaveManager, IDisposable
    {
        private readonly IResourceService _resourceService;
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<Enemy, GameObject> _enemyObjects = new();
        private readonly List<Enemy> _activeEnemies = new();
        
        private readonly Subject<EnemySpawned> _enemySpawned = new();
        private readonly Subject<EnemyDefeated> _enemyDefeated = new();
        private readonly Subject<WaveStarted> _waveStarted = new();
        private readonly Subject<WaveCompleted> _waveCompleted = new();
        
        private WaveConfig _currentWave;
        private int _enemiesDefeatedInWave;
        private bool _isBossSpawned;
        
        public IObservable<EnemySpawned> OnEnemySpawned => _enemySpawned;
        public IObservable<EnemyDefeated> OnEnemyDefeated => _enemyDefeated;
        public IObservable<WaveStarted> OnWaveStarted => _waveStarted;
        public IObservable<WaveCompleted> OnWaveCompleted => _waveCompleted;
        
        public WaveConfig CurrentWave => _currentWave;
        public int ActiveEnemyCount => _activeEnemies.Count;
        
        public WaveSpawner(IResourceService resourceService)
        {
            _resourceService = resourceService;
            
            MessageBroker.Default.Receive<EnemyDamaged>()
                .Subscribe(OnEnemyDamaged)
                .AddTo(_disposables);
        }
        
        public async UniTask StartWaveAsync(WaveConfig wave, CancellationToken token)
        {
            _currentWave = wave;
            _enemiesDefeatedInWave = 0;
            _isBossSpawned = false;
            
            _waveStarted.OnNext(new WaveStarted { Wave = wave });
            
            Debug.Log($"Starting wave: {wave.WaveName}");
            
            // set up cancellation for wave duration
            using var waveCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            waveCts.CancelAfter(TimeSpan.FromSeconds(wave.WaveDuration));
            
            try
            {
                await SpawnEnemiesAsync(wave, waveCts.Token);
                
                // wait until all enemies are defeated or wave is cancelled
                await UniTask.WaitUntil(() => _activeEnemies.Count == 0 || token.IsCancellationRequested);
                
                if (!token.IsCancellationRequested)
                {
                    // award resources for completing the wave
                    _resourceService.AddEnergy(wave.EnergyReward);
                    _resourceService.AddMinerals(wave.MineralReward);
                    
                    _waveCompleted.OnNext(new WaveCompleted { Wave = wave });
                    
                    Debug.Log($"Wave completed: {wave.WaveName}");
                    
                    // start next wave if available
                    if (wave.NextWave != null)
                    {
                        // wait before starting next wave
                        await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: token);
                        
                        if (!token.IsCancellationRequested)
                        {
                            await StartWaveAsync(wave.NextWave, token);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Wave was cancelled");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in wave spawning: {ex.Message}");
            }
        }
        
        private async UniTask SpawnEnemiesAsync(WaveConfig wave, CancellationToken token)
        {
            foreach (var spawnInfo in wave.EnemySpawns)
            {
                for (int i = 0; i < spawnInfo.Count; i++)
                {
                    if (token.IsCancellationRequested) break;
                    
                    var enemy = spawnInfo.EnemyConfig.CreateEnemy();
                    var spawnPosition = GetSpawnPosition(spawnInfo.SpawnPoint);
                    
                    var enemyObj = UnityEngine.Object.Instantiate(
                        spawnInfo.EnemyConfig.Prefab,
                        spawnPosition,
                        Quaternion.identity
                    );
                    
                    _enemyObjects[enemy] = enemyObj;
                    _activeEnemies.Add(enemy);
                    
                    _enemySpawned.OnNext(new EnemySpawned { Enemy = enemy, Position = spawnPosition });
                    
                    // check if we should spawn a boss
                    if (!_isBossSpawned && wave.BossConfig != null && _enemiesDefeatedInWave >= wave.BossActivationThreshold)
                    {
                        await SpawnBossAsync(wave.BossConfig, token);
                        _isBossSpawned = true;
                    }
                    
                    await UniTask.Delay(TimeSpan.FromSeconds(wave.SpawnInterval), cancellationToken: token);
                }
            }
        }
        
        private async UniTask SpawnBossAsync(EnemyConfig bossConfig, CancellationToken token)
        {
            Debug.Log("Boss is spawning!");
            
            // dramatic pause before boss spawn
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);
            
            if (token.IsCancellationRequested) return;
            
            var boss = bossConfig.CreateEnemy();
            var spawnPosition = GetSpawnPosition(SpawnPoint.Random);
            
            var bossObj = UnityEngine.Object.Instantiate(
                bossConfig.Prefab,
                spawnPosition,
                Quaternion.identity
            );
            
            // make the boss bigger
            bossObj.transform.localScale *= 2f;
            
            _enemyObjects[boss] = bossObj;
            _activeEnemies.Add(boss);
            
            _enemySpawned.OnNext(new EnemySpawned { Enemy = boss, Position = spawnPosition, IsBoss = true });
        }
        
        private void OnEnemyDamaged(EnemyDamaged evt)
        {
            if (evt.Enemy == null) return;
            
            evt.Enemy.TakeDamage(evt.Damage);
            
            if (evt.Enemy.IsDefeated)
            {
                DefeatEnemy(evt.Enemy);
            }
        }
        
        private void DefeatEnemy(Enemy enemy)
        {
            if (_enemyObjects.TryGetValue(enemy, out var enemyObj))
            {
                UnityEngine.Object.Destroy(enemyObj);
                _enemyObjects.Remove(enemy);
            }
            
            _activeEnemies.Remove(enemy);
            _enemiesDefeatedInWave++;
            
            // award resources
            _resourceService.AddMinerals(enemy.ResourceReward);
            
            _enemyDefeated.OnNext(new EnemyDefeated { Enemy = enemy });
        }
        
        private Vector3 GetSpawnPosition(SpawnPoint spawnPoint)
        {
            // for simplicity, using fixed positions based on spawn point
            // in a real game, you'd use information about map boundaries
            const float mapSize = 40f;
            const float halfMap = mapSize / 2f;
            
            switch (spawnPoint)
            {
                case SpawnPoint.North:
                    return new Vector3(Random.Range(-halfMap, halfMap), 0, halfMap);
                case SpawnPoint.South:
                    return new Vector3(Random.Range(-halfMap, halfMap), 0, -halfMap);
                case SpawnPoint.East:
                    return new Vector3(halfMap, 0, Random.Range(-halfMap, halfMap));
                case SpawnPoint.West:
                    return new Vector3(-halfMap, 0, Random.Range(-halfMap, halfMap));
                case SpawnPoint.Random:
                default:
                    var randomSpawnPoint = (SpawnPoint)Random.Range(0, 4); // excluding Random itself
                    return GetSpawnPosition(randomSpawnPoint);
            }
        }

        public void Dispose() => _disposables.Dispose();
    }
}