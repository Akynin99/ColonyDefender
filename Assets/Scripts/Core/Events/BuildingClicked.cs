namespace ColonyDefender.Core
{
    public readonly struct BuildingClicked
    {
        public readonly int EnergyCost;

        public BuildingClicked(int energyCost)
        {
            EnergyCost = energyCost;
        }
    }
}