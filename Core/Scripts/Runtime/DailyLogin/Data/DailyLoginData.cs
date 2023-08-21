using System;
using System.Collections.Generic;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using UnityEngine.Serialization;

namespace Agate.Starcade.Scripts.Runtime.DailyLogin.Data
{
    [System.Serializable]
    public class DailyLoginData : InitAdditiveBaseData
    {
        public int DayCount;
        public DateTime? LastClaimDate { get; set; }
        public DateTime? ResetDay { get; set; }
        public List<DailyLoginRewardData> RewardList;
        public DailyLoginRewardData Reward;
        public PlayerBalance Balance;
        public DailyLoginEnum DailyLoginState;
    }
    
    public enum DailyLoginEnum
    {
        Opened,
        Closed
    }
}
