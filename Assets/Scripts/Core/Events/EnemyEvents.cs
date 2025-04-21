using ColonyDefender.Core;
using UnityEngine;

namespace ColonyDefender
{
    public class EnemySpawned
    {
        public Enemy Enemy { get; set; }
        public Vector3 Position { get; set; }
        public bool IsBoss { get; set; }
    }
    
    public class EnemyDefeated
    {
        public Enemy Enemy { get; set; }
    }
    
    public class EnemyDamaged
    {
        public Enemy Enemy { get; set; }
        public int Damage { get; set; }
    }
} 