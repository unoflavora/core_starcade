using System;
using System.Collections.Generic;
using System.Linq;

namespace Agate.Starcade.Scripts.Runtime.Utilities
{
    public static class RandomPicker
    {
        public static int PickIndex(List<double> itemPercentages)
        {
            var rand = new Random();
            var totalPercentage = itemPercentages.Sum();
            var targetPercentage = rand.NextDouble() * totalPercentage;
            var targetIndex = -1;
            double accumulatedPercentage = 0;
            for (int i = 0; i < itemPercentages.Count; i++)
            {
                var p = itemPercentages[i];
                accumulatedPercentage += p;
                if (targetPercentage <= accumulatedPercentage)
                {
                    targetIndex = i;
                    break;
                }
            }
            return targetIndex;
        }
    }
}