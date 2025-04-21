using System;

namespace ColonyDefender.Core
{
    public interface IResourceService
    {
        /// <summary>
        /// Tries to spend energy if there's enough
        /// </summary>
        bool TrySpendEnergy(int amount);
        
        /// <summary>
        /// Tries to spend minerals if there's enough
        /// </summary>
        bool TrySpendMinerals(int amount);
        
        /// <summary>
        /// Tries to spend both energy and minerals if there's enough of both
        /// </summary>
        bool TrySpendResources(int energyAmount, int mineralAmount);
        
        /// <summary>
        /// Checks if there are enough resources
        /// </summary>
        bool HasEnoughResources(int energyAmount, int mineralAmount);
        
        /// <summary>
        /// Adds energy to the colony
        /// </summary>
        void AddEnergy(int amount);
        
        /// <summary>
        /// Adds minerals to the colony
        /// </summary>
        void AddMinerals(int amount);
        
        /// <summary>
        /// Updates resources based on production rates
        /// </summary>
        void UpdateResourceProduction();
    }
}