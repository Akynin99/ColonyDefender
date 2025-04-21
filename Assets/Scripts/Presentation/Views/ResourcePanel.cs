using System;
using ColonyDefender.Core;
using TMPro;
using UniRx;
using UnityEngine;

namespace ColonyDefender.Presentation
{
    public class ResourcePanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private TMP_Text _mineralsText;
    
        private IDisposable _subscription;

        public void Bind(ColonyData data)
        {
            _subscription?.Dispose();
        
            var disposables = new CompositeDisposable();
            data.Energy
                .SubscribeWithState(_energyText, (v, t) => t.text = $"Energy: {v}")
                .AddTo(disposables);
        
            data.Minerals
                .SubscribeWithState(_mineralsText, (v, t) => t.text = $"Minerals: {v}")
                .AddTo(disposables);

            _subscription = disposables;
        }

        private void OnDestroy() => _subscription?.Dispose();
    }

}