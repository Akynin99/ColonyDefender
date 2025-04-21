using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ColonyDefender.Core
{
    public interface IWaveManager
    {
        /// <summary>
        /// Starts a wave of enemies
        /// </summary>
        UniTask StartWaveAsync(WaveConfig wave, CancellationToken token);
        
        /// <summary>
        /// The current active wave
        /// </summary>
        WaveConfig CurrentWave { get; }
        
        /// <summary>
        /// The number of active enemies
        /// </summary>
        int ActiveEnemyCount { get; }
        
        /// <summary>
        /// Observable for enemy spawn events
        /// </summary>
        IObservable<EnemySpawned> OnEnemySpawned { get; }
        
        /// <summary>
        /// Observable for enemy defeated events
        /// </summary>
        IObservable<EnemyDefeated> OnEnemyDefeated { get; }
        
        /// <summary>
        /// Observable for wave started events
        /// </summary>
        IObservable<WaveStarted> OnWaveStarted { get; }
        
        /// <summary>
        /// Observable for wave completed events
        /// </summary>
        IObservable<WaveCompleted> OnWaveCompleted { get; }
    }
}