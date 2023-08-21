using System;
using UnityEngine;

namespace Agate.Starcade.Runtime.Game
{
    public static class ExperienceController
    {
        public static PlayerExpData CalculateExp(float totalExp)
        {
            for (int level = 1; level < 100; level++)
            {
                var exp = CalculateExpNeed(level);
                if (exp >= totalExp)
                {
                    return new PlayerExpData()
                    {
                        Level = level,
                        Deviation = CalculateExpNeed(level) - CalculateExpNeed(level-1),
                        TotalExp = totalExp,
                        TotalExpNeeded = exp
                    };
                }
            }
            Debug.LogError("ERROR CALCULATE EXP");
            return null;
        }

        private static float CalculateExpNeed(int level)
        {
            return MathF.Floor((level * (level - 1) / 15) * 10000) + 10000;
        }
    }

    public class PlayerExpData
    {
        public float Level;
        public float TotalExp;
        public float Deviation;
        public float TotalExpNeeded;
    }
}
