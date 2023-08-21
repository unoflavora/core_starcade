using System;

namespace Agate.Starcade.Scripts.Runtime.Data_Class
{
    [Serializable]
    public class RewardData
    {
        public string RewardId;
        public bool IsClaim;
        public int ClaimType;
        public int ClaimLimit;
    }
}