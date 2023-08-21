using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Arcade.SO;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.DailyLogin.SO
{
    [CreateAssetMenu(menuName = "Starcade/DailyLogin/DailyLoginSO")]
    public class DailyLoginSO : ScriptableObject
    {
        [SerializeField]
        public RewardSpriteData RewardSpriteData;
        
        [SerializeField] private LootBoxChestSO _lootBox;

        [SerializeField] private Sprite _lootBoxBackground;
        [SerializeField] private Sprite _lootBoxDayHeader;

        [SerializeField] private Sprite _petBackground;
        [SerializeField] private Sprite _petDayHeader;
        
        [SerializeField] private Sprite _currencyBackground;
        [SerializeField] private Sprite _currencyDayHeader;

        public Sprite GetRewardSprite(RewardEnum rewardEnum, int tier, bool isHidden)
        {
            return RewardSpriteData.GetDailyRewardIcon(rewardEnum, tier, isHidden);
        }
        

        public Sprite GetRewardBackground(RewardEnum rewardEnum)
        {
            switch (rewardEnum)
            {
                case RewardEnum.GoldCoin:
                    return _currencyBackground;
                case RewardEnum.StarCoin:
                    return _currencyBackground;
                case RewardEnum.StarTicket:
                    return _currencyBackground;
                case RewardEnum.Lootbox:
                    return _lootBoxBackground;
                case RewardEnum.Pet:
                    return _petBackground;
                default:
                    return _currencyBackground;
            }
        }
        
        public Sprite GetRewardHeader(RewardEnum rewardEnum)
        {
            switch (rewardEnum)
            {
                case RewardEnum.GoldCoin:
                    return _currencyDayHeader; 
                case RewardEnum.StarCoin:
                    return _currencyDayHeader;
                case RewardEnum.StarTicket:
                    return _currencyDayHeader;
                case RewardEnum.Lootbox:
                    return _lootBoxDayHeader;
                case RewardEnum.Pet:
                    return _petDayHeader;
                default:
                    return _currencyDayHeader;
            }
        }
    }
}
