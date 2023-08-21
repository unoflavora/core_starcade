using System;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Newtonsoft.Json;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.SO
{
    [Serializable]   
    [CreateAssetMenu(menuName = "Starcade/Arcade/RewardSpriteData")]
    public class RewardSpriteData : UnityEngine.ScriptableObject
    {
        [Header("RewardType")] 
        [Header("Gold")] 
        public Sprite[] IconGold;
        [Header("Star")] 
        public Sprite[] IconStar;
        [Header("StarTicket")]
        public Sprite[] IconTicket;
        [Header("LootBox")]
        public Sprite[] IconLootBox;
        [Header("Pet")]
        public Sprite[] IconPet;

        public Sprite GetRewardIcon(CurrencyTypeEnum currencyType, int tier)
        {
            var targetIndex = Math.Clamp(tier-1, 0, int.MaxValue);
            Sprite sprite;
            switch (currencyType)
                {
                    case CurrencyTypeEnum.GoldCoin:
                        sprite = IconGold[targetIndex];
                        break;
                    case CurrencyTypeEnum.StarCoin:
                        sprite = IconStar[targetIndex];
                        break;
                    case CurrencyTypeEnum.StarTicket:
                        sprite = IconTicket[targetIndex];
                        break;
                    default:
                        return null;
                }
            return sprite;
        }

        public Sprite GetDailyRewardIcon(RewardEnum rewardType, int tier, bool hidden)
        {
            Sprite sprite = null;

            var targetIndex = Math.Clamp(tier - 1, 0, int.MaxValue);

            switch (rewardType)
            {
                case RewardEnum.GoldCoin:
                    sprite = IconGold[targetIndex];
                    break;
                case RewardEnum.StarCoin:
                    sprite = IconStar[targetIndex];
                    break;
                case RewardEnum.StarTicket:
                    sprite = IconTicket[targetIndex];
                    break;
                case RewardEnum.Lootbox:
                    sprite = IconLootBox[targetIndex];
                    break;
                default:
                    sprite = null;
                    break;
            }

            return sprite;
        }

    }
}
