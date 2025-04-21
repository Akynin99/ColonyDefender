using ColonyDefender.Core;
using UnityEngine;

namespace ColonyDefender
{
    public class BuildingClicked
    {
        public BuildingType BuildingType { get; set; }
    }
    
    public class CellClicked
    {
        public Vector2Int GridPosition { get; set; }
    }
    
    public class ShowNotification
    {
        public string Message { get; set; }
        public float Duration { get; set; } = 3f;
    }
} 