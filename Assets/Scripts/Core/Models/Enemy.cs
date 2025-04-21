using System;
using UnityEngine;

namespace ColonyDefender.Core
{
    [Serializable]
    public class Enemy
    {
        public EnemyType Type { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public int Damage { get; private set; }
        public float Speed { get; private set; }
        public int ResourceReward { get; private set; }
        
        public Enemy(EnemyType type, int health, int damage, float speed, int resourceReward)
        {
            Type = type;
            Health = health;
            MaxHealth = health;
            Damage = damage;
            Speed = speed;
            ResourceReward = resourceReward;
        }
        
        public void TakeDamage(int amount)
        {
            Health = Mathf.Max(0, Health - amount);
        }
        
        public bool IsDefeated => Health <= 0;
    }
    
    public enum EnemyType
    {
        Scout,
        Soldier,
        Tank,
        Bomber,
        Boss
    }
} 