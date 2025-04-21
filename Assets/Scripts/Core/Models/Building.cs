using System;
using UnityEngine;

namespace ColonyDefender.Core
{
    [Serializable]
    public class Building
    {
        public BuildingType Type { get; private set; }
        public Vector2Int Position { get; private set; }
        public Vector2Int Size { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public int EnergyCost { get; private set; }
        public int MineralCost { get; private set; }
        public int EnergyProduction { get; private set; }
        public int MineralProduction { get; private set; }
        public int PopulationSupport { get; private set; }
        
        public Building(BuildingType type, Vector2Int position, Vector2Int size, 
            int health, int energyCost, int mineralCost, 
            int energyProduction = 0, int mineralProduction = 0, int populationSupport = 0)
        {
            Type = type;
            Position = position;
            Size = size;
            Health = health;
            MaxHealth = health;
            EnergyCost = energyCost;
            MineralCost = mineralCost;
            EnergyProduction = energyProduction;
            MineralProduction = mineralProduction;
            PopulationSupport = populationSupport;
        }
        
        public void TakeDamage(int amount)
        {
            Health = Mathf.Max(0, Health - amount);
        }
        
        public void Repair(int amount)
        {
            Health = Mathf.Min(MaxHealth, Health + amount);
        }
        
        public bool IsDestroyed => Health <= 0;
    }
    
    public enum BuildingType
    {
        PowerPlant,
        Mine,
        House,
        DefenseTurret,
        Wall,
        ResearchLab,
        CommandCenter
    }
} 