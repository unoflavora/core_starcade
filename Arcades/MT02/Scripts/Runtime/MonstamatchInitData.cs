using System.Collections.Generic;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Config;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Grid;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Symbols;
using Agate.Starcade.Scripts.Runtime.Data;

namespace Starcade.Arcades.MT02.Scripts.Runtime
{
    public class MonstamatchInitData
    {
        public string SessionId { get; set; }
        public GameModeEnum Mode { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsStarted { get; set; }
        public bool IsTutorial { get; set; }
        public MonstamatchGameData Game { get; set; }
        public PlayerBalance Balance { get; set; }
        public MonstamatchGameConfig Config { get; set; }
    }
    
    public class MonstamatchGameConfig
    {
        public double Cost { get; set; }
        public int SessionDuration { get; set; }
        public CurrencyTypeEnum CostCurrency { get; set; }
        public CurrencyTypeEnum RewardCurrency { get; set; }
        public PaytableConfig PaytableConfig { get; set; }
        public List<MonstamatchSymbolData> Symbols { get; set; }
        public List<MonstaMatchSymbolSpecialConfig> SpecialCoinConfig { get; set; }
        public List<MonstamatchSymbolRewardConfig> RewardSymbols { get; set; }
        public JackpotConfig JackpotConfig { get; set; }
        public List<InstantJackpotConfig> InstantJackpotConfig { get; set; }
        public PuzzleJackpotConfig PuzzleJackpotConfig { get; set; }
        public MonstaMatchKillMonsterConfig KillMonsterConfig { get; set; }
        public string Id { get; set; }
        public string Mode { get; set; }
    }


}