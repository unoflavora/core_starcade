using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Data_Class.Reward;

namespace Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles
{
    public class CollectibleUserRewardData : RewardModelData
    {
        public string SetId { get; set; }
        public UserRewardEnum Status { get; set; }
    }
}