using System.Threading;
using Cysharp.Threading.Tasks;

namespace ColonyDefender
{
    public interface IWaveManager
    {
        UniTaskVoid StartWaveAsync(WaveConfig wave, CancellationToken token);
    }
}