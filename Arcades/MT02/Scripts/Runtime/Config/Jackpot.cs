using System.Collections.Generic;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Jackpots;
using Starcade.Minigames.LightsRoulette.Scripts.Runtime;

namespace Agate.Starcade.Arcades.MT02.Scripts.Runtime.Config
{
    public class InstantJackpotConfig
    {
        public int Index { get; set; }
        public int Target { get; set; }
        public InstantJackpotType RewardType { get; set; }
        public CurrencyTypeEnum RewardCurrency { get; set; }
        public double RewardAmount { get; set; }
    }

    public class PuzzleJackpotConfig
    {
        public List<int> Puzzle { get; set; }
        public double RewardAmount { get; set; }
        public CurrencyTypeEnum RewardCurrency { get; set; }
    }

    public class JackpotConfig : LightsRouletteConfig
    {
        public int TotalProgress { get; set; }
        public int BonusExp { get; set; }
    }

}