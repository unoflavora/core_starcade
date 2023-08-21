using System;
using System.Collections.Generic;
using System.Linq;
using Agate.Modules.Hexa.Pathfinding;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Grid;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Symbols;
using Starcade.Arcades.MT02.Scripts.Runtime.Hex.Grid;
using UnityEngine;

namespace Starcade.Arcades.MT02.Scripts.Runtime.Hex.Calculator
{
    public class MatchCalculator
    {
        public static CalculatedMatchResult CalculateMatchResult(List<GridNode> paths, List<MonstamatchSymbolRewardConfig> configSymbolRewards)
        {
            double reward = 0;
            double jackpotProgress = 0;
            bool isPuzzleCollected = false;
            int specialCoinCollected = 0;
            List<Vector3> specialCoinPositions = new List<Vector3>();

            MonstamatchSymbolRewardConfig configReward = configSymbolRewards.Where(t => t.Type == paths[0].symbol.Data.Type).First();

            foreach (var item in paths)
            {
                //base reward * total match
                reward += configReward.BaseReward;
                jackpotProgress += configReward.BaseExpJackpot;

                if (item.symbol.Data.IsSpecial)
                {
                    specialCoinPositions.Add(item.symbol.transform.position);
                    specialCoinCollected++;
                }
            }

            if (paths[0].symbol.Data.Type == MonstaMatchSymbolTypesEnum.Rare) isPuzzleCollected = true;
            CalculatedMatchResult result = new()
            {
                GeneralReward = Math.Ceiling(reward),
                JackpotExpReward = Math.Ceiling(jackpotProgress),
                IsPuzzleCollected = isPuzzleCollected,
                SpecialCoinCollected = specialCoinCollected,
                SpecialCoinPositions = specialCoinPositions
            };

            return result;
        }
    }
}