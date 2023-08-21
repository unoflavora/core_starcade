using Agate.Starcade.Runtime.Enums;
using System;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    [Serializable]
    public class RewardDetailData
    {
        public string Type;
        public long Amount;
        public string ItemId;
    }
}