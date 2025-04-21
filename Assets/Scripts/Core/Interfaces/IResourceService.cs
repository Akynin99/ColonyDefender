namespace ColonyDefender.Core
{
    public interface IResourceService
    {
        bool TrySpendEnergy(int amount);
        void AddMinerals(int amount);
    }
}