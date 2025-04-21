using UnityEngine;

namespace ColonyDefender.Core
{
    public class BuildingPlaced
    {
        public Building Building { get; set; }
        public Vector2Int Position { get; set; }
    }
    
    public class BuildingRemoved
    {
        public Vector2Int Position { get; set; }
    }
    
    public class BuildingDamaged
    {
        public Vector2Int Position { get; set; }
        public int Damage { get; set; }
    }
} 