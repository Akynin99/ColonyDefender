using System;
using ColonyDefender.Core;
using UniRx;

namespace ColonyDefender.Presentation
{
    public class ColonyViewModel : IDisposable
    {
        private readonly ColonyData _data;
        private readonly IResourceService _resources;
        private readonly CompositeDisposable _disposables = new();

        public ColonyViewModel(ColonyData data, IResourceService resources)
        {
            _data = data;
            _resources = resources;
        
            MessageBroker.Default.Receive<BuildingClicked>()
                .Subscribe(OnBuildingClicked)
                .AddTo(_disposables);
        }

        private void OnBuildingClicked(BuildingClicked evt)
        {
            if (_resources.TrySpendEnergy(evt.EnergyCost))
            {
                //TODO: add building logic
            }
        }

        public void Dispose() => _disposables.Dispose();
    }

}