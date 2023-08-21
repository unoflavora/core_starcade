using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Data_Class.Reward;

namespace Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles
{
    public class CollectibleRewardData
    {
        public ItemTypeEnum TypeReward { get; set; }
        public string @ref { get; set; }
        public int Amount { get; set; }
        public UserRewardEnum Status { get; set; }
    }
    
}