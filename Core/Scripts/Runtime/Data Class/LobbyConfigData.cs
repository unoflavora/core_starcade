using Agate.Starcade.Runtime.Enums;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    public class LobbyConfigData
    {
        public int TimeBaseCoinRewardAmount { get; set; }
        public int TimeBaseCoinRewardInterval { get; set; }
        public CurrencyTypeEnum TimeBaseCoinRewardType {get;set;}
    }
}