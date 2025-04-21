using UnityEngine;

namespace ColonyDefender.Core
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "ColonyDefender/EnemyConfig", order = 2)]
    public class EnemyConfig : ScriptableObject
    {
        [SerializeField] private EnemyType type;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int health = 100;
        [SerializeField] private int damage = 10;
        [SerializeField] private float speed = 3f;
        [SerializeField] private int resourceReward = 25;
        
        public EnemyType Type => type;
        public GameObject Prefab => prefab;
        public int Health => health;
        public int Damage => damage;
        public float Speed => speed;
        public int ResourceReward => resourceReward;
        
        public Enemy CreateEnemy()
        {
            return new Enemy(type, health, damage, speed, resourceReward);
        }
    }
} 