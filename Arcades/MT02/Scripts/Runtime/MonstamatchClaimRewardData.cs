using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;
using Agate.Starcade.Scripts.Runtime.Data;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend.Data
{
    public class MonstamatchClaimRewardData
    {
        public MonstamatchGameData Game { get; set; }
        public PlayerBalance Balance { get; set; }
        public ClaimedReward ClaimedReward { get; set; }
    }
}