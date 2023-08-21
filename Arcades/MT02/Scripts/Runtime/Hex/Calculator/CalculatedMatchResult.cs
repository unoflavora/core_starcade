using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Symbols;
using UnityEngine;

namespace Starcade.Arcades.MT02.Scripts.Runtime.Hex.Calculator
{
    public class CalculatedMatchResult
    {
        public double GeneralReward { get; set; }
        public double JackpotExpReward { get; set; }
        public long LcgKey { get; set; }
        public bool IsPuzzleCollected { get; set; }
        public int SpecialCoinCollected { get; set; }
        public List<Vector3> SpecialCoinPositions { get; set; }
        public List<MonstamatchSymbolData> SymbolsBoardState { get; set; }
    }
}