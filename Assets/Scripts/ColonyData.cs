using UniRx;
using UnityEngine;

namespace ColonyDefender
{
    public class ColonyData
    {
        public ReactiveProperty<int> Energy { get; } = new(1000);
        public ReactiveProperty<int> Minerals { get; } = new(500);
        public ReactiveProperty<int> Population { get; } = new(10);
    }
}