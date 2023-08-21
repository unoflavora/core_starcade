using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Data;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend.Data
{
    public class MonstamatchUsePuzzleData
    {
        public MonstamatchGameData Game { get; set; }
        public PlayerBalance Balance { get; set; }
        public ClaimedReward ClaimedReward { get; set; }
    }

    public class ClaimedReward
    {
        public CurrencyTypeEnum Type { get; set; }
        public double Amount { get; set; }
    }
}