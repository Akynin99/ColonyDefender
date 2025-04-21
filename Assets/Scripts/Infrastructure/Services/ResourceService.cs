using System;
using ColonyDefender.Core;
using UniRx;
using UnityEngine;

namespace ColonyDefender.Infrastructure.Services
{
    public class ResourceService : IResourceService, IDisposable
    {
        private readonly ColonyData _data;
        private readonly CompositeDisposable _disposables = new();

        public ResourceService(ColonyData data)
        {
            _data = data;
            
            _data.Population
                .Where(p => p >= 100)
                .Subscribe(_ => Debug.Log("Population milestone!"))
                .AddTo(_disposables);
        }

        public bool TrySpendEnergy(int amount)
        {
            if (_data.Energy.Value < amount) return false;
            _data.Energy.Value -= amount;
            return true;
        }

        public void AddMinerals(int amount)
        {
            // TODO: add minerals logic
        }

        public void Dispose() => _disposables.Dispose();
    }
}