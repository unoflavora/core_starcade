using System;
using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.Data;

namespace Agate.Starcade.Scripts.Runtime.Data_Class
{
    public class ClaimRewardData
    {
        public string RewardId;
        public string UserId;
        public DateTime ClaimDate;
        public List<RewardGainData> RewardGain;
        public PlayerBalance UserBalance;
    }
}