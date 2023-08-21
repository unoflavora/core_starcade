using System;
using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Utility;
using Starcade.Arcades.MT02.Scripts.Runtime;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Symbols
{
    public class MonstaMatchSymbolPicker
    {
        public static MonstamatchSymbolData PickNextSymbol(List<MonstamatchSymbolData> symbols, LinearCongruentialGenerator rand)
        {
            double totalPercentage = 0;
            var itemPercentages = new double[symbols.Count];
            var index = 0;
            foreach (var item in symbols)
            {
                totalPercentage += item.Percentage;
                itemPercentages[index] = item.Percentage;
                index++;
            }

            var targetPercentage = rand.Next(Convert.ToInt64(totalPercentage));

            var targetIndex = -1;
            double accumulatedPercentage = 0;
            for (int i = 0; i < itemPercentages.Length; i++)
            {
                var p = itemPercentages[i];
                accumulatedPercentage += p;
                if (targetPercentage <= accumulatedPercentage)
                {
                    targetIndex = i;
                    break;
                }
            }

            MonstamatchSymbolData data = new(symbols[targetIndex]);

            return data;
        }
    }
}
