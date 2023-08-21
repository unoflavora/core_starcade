using System;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    [Serializable]
    public class NextExperienceRewardData
    {
        public int Level;
        public string UserId;
        public bool ClaimData;
        public RewardDetailData[] RewardGain;
    }
}