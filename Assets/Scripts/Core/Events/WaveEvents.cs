using ColonyDefender.Core;

namespace ColonyDefender
{
    public class WaveStarted
    {
        public WaveConfig Wave { get; set; }
    }
    
    public class WaveCompleted
    {
        public WaveConfig Wave { get; set; }
    }
} 