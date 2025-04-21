using System;
using UnityEngine;

namespace ColonyDefender.Core
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "ColonyDefender/WaveConfig", order = 0)]
    public class WaveConfig : ScriptableObject
    {
        [SerializeField] private string waveName = "Wave 1";
        [SerializeField] private EnemySpawnInfo[] enemySpawns;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private float waveDuration = 60f;
        [SerializeField] private int bossActivationThreshold = 20;
        [SerializeField] private EnemyConfig bossConfig;
        [SerializeField] private int energyReward = 200;
        [SerializeField] private int mineralReward = 100;
        [SerializeField] private int waveNumber = 1;
        [SerializeField] private WaveConfig nextWave;
        
        public string WaveName => waveName;
        public EnemySpawnInfo[] EnemySpawns => enemySpawns;
        public float SpawnInterval => spawnInterval;
        public float WaveDuration => waveDuration;
        public int BossActivationThreshold => bossActivationThreshold;
        public EnemyConfig BossConfig => bossConfig;
        public int EnergyReward => energyReward;
        public int MineralReward => mineralReward;
        public int WaveNumber => waveNumber;
        public WaveConfig NextWave => nextWave;
    }
    
    [Serializable]
    public class EnemySpawnInfo
    {
        [SerializeField] private EnemyConfig enemyConfig;
        [SerializeField] private int count;
        [SerializeField] private SpawnPoint spawnPoint;
        
        public EnemyConfig EnemyConfig => enemyConfig;
        public int Count => count;
        public SpawnPoint SpawnPoint => spawnPoint;
    }
    
    public enum SpawnPoint
    {
        North,
        South,
        East,
        West,
        Random
    }
}