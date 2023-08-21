using System;
using System.Collections.Generic;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;
using Agate.Starcade.Scripts.Runtime.Data;

namespace Starcade.Arcades.MT02.Scripts.Runtime
{
    public class MonstamatchMatchData
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool isStarted { get; set; }
        public Pay pay { get; set; }
        public MatchResult matchResult { get; set; }
        public MonstamatchGameData Game { get; set; }
        public PlayerBalance balance { get; set; }
    }
    
    public class MatchResult
    {
        public int MatchCount { get; set; }
        public int RemainingMatchChance { get; set; }
        public int SpecialCoinCollected { get; set; }
        public double TotalReward { get; set; }
        public double TotalJackpotProgressReward { get; set; }
        public List<int> PuzzleJackpotCollected { get; set; }
    }
    
   
}