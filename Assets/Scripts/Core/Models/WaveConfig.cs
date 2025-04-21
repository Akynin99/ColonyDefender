using UnityEngine;

namespace ColonyDefender.Core
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "ColonyDefender/WaveConfig", order = 0)]
    public class WaveConfig : ScriptableObject
    {
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private float spawnInterval;
        [SerializeField] private int bossActivationThreshold = 20;
        
        public GameObject[] Enemies => enemies;
        public float SpawnInterval => spawnInterval;
        public int BossActivationThreshold => bossActivationThreshold;
    }
}