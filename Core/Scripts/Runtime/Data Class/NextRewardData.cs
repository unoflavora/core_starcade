using System;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    [Serializable]
    public class NextRewardData
    {
        public int Level;
        public string UserId;
        public bool ClaimData;
        public RewardDetailData[] RewardGain;
    }
}