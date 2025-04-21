using System.Threading;
using Cysharp.Threading.Tasks;

namespace ColonyDefender.Core
{
    public interface IWaveManager
    {
        UniTaskVoid StartWaveAsync(WaveConfig wave, CancellationToken token);
    }
}