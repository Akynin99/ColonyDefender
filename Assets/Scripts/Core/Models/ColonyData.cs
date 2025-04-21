using UniRx;

namespace ColonyDefender.Core
{
    public class ColonyData
    {
        public ReactiveProperty<int> Energy { get; } = new(1000);
        public ReactiveProperty<int> Minerals { get; } = new(500);
        public ReactiveProperty<int> Population { get; } = new(10);
        
        // production rates
        public ReactiveProperty<int> EnergyProduction { get; } = new(0);
        public ReactiveProperty<int> MineralProduction { get; } = new(0);
        
        // limits
        public ReactiveProperty<int> MaxPopulation { get; } = new(50);
        
        // research
        public ReactiveProperty<int> ResearchPoints { get; } = new(0);
        public ReactiveProperty<int> TechLevel { get; } = new(1);
    }
}