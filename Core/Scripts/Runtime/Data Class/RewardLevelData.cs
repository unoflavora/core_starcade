using System;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    [Serializable]
    public class RewardLevelData
    {
        public int Level;
        public bool IsClaim;
        public RewardDetailData[] Rewards;
    }
}