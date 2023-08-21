using System;
using System.Collections.Generic;
using Agate.Modules.Hexa.Pathfinding;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Symbols;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Jackpots;
using Starcade.Arcades.MT02.Scripts.Runtime;
using Starcade.Minigames.KillTheMonster.Scripts.Runtime;

namespace Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data
{ 
    public class MonstamatchGameData
    {
        public double JackpotProgress { get; set; }
        public int JackpotCount { get; set; }
        public int RemainingSpinChance { get; set; }
        public List<MonstamatchJackpotEnums> JackpotsQueue { get; set; }
        public List<int> CurrentPuzzleJackpotProgress { get; set; }
        public KillMonsterProgress KillMonsterProgress { get; set; }
        public SpinSession SpinSession { get; set; }
    }
    
    public class SpinSession
    {
        public long lcgKey { get; set; }
        public int SpinCount { get; set; }
        public int MatchCount { get; set; }
        public int RemainingMatchChance { get; set; }
        public int SpecialCollectedCount { get; set; }
        public InstantJackpotProgress InstantJackpotProgress { get; set; }
        public List<MonstamatchSymbolData> BoardState { get; set; }
    }
    
    public class Pay
    {
        public string type { get; set; }
        public double amount { get; set; }
    }
    
    public class InstantJackpotProgress
    {
        public int TotalCollected { get; set; }
        public int ClaimedRewardIndex { get; set; }
        public List<InstantJackpotType> Rewards { get; set; }
    }

    [Serializable]
    public class CoordinatePath
    {
        public int x { get; set; }
        public int y { get; set; }
        public CoordinatePath(Coordinate coordinate)
        {
            x = coordinate.x;
            y = coordinate.y;
        }
        
    }
    
    

}