using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace ColonyDefender.Core
{
    public interface ISaveService
    {
        /// <summary>
        /// Save colony data async
        /// </summary>
        UniTask SaveColonyDataAsync(ColonyData data, CancellationToken token);
        
        /// <summary>
        /// Loads colony data with a reactive result
        /// </summary>
        IUniTaskAsyncEnumerable<ColonyData> LoadColonyDataAsync(CancellationToken token);
        
        /// <summary>
        /// Reactive property with last save time
        /// </summary>
        IReadOnlyReactiveProperty<System.DateTime?> LastSaveTime { get; }
    }
}