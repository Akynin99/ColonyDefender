using UnityEngine;

namespace ColonyDefender.Core
{
    [CreateAssetMenu(fileName = "BuildingConfig", menuName = "ColonyDefender/BuildingConfig", order = 1)]
    public class BuildingConfig : ScriptableObject
    {
        [SerializeField] private BuildingType type;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector2Int size = new Vector2Int(1, 1);
        [SerializeField] private int health = 100;
        [SerializeField] private int energyCost;
        [SerializeField] private int mineralCost;
        [SerializeField] private int energyProduction;
        [SerializeField] private int mineralProduction;
        [SerializeField] private int populationSupport;
        [SerializeField] private string description;
        [SerializeField] private float buildTime = 2f;
        
        public BuildingType Type => type;
        public GameObject Prefab => prefab;
        public Sprite Icon => icon;
        public Vector2Int Size => size;
        public int Health => health;
        public int EnergyCost => energyCost;
        public int MineralCost => mineralCost;
        public int EnergyProduction => energyProduction;
        public int MineralProduction => mineralProduction;
        public int PopulationSupport => populationSupport;
        public string Description => description;
        public float BuildTime => buildTime;
        
        public Building CreateBuilding(Vector2Int position)
        {
            return new Building(type, position, size, health, energyCost, mineralCost, 
                energyProduction, mineralProduction, populationSupport);
        }
    }
} 