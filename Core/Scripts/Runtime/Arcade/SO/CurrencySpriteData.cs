using System;
using Agate.Starcade.Runtime.Enums;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.SO
{
    [Serializable]
    [CreateAssetMenu(menuName = "Starcade/Arcade/CurrencySpriteData")]
    public class CurrencySpriteData : ScriptableObject
    {
        [Header("CurrencyType")] 
        [Header("Gold")] 
        public Sprite IconGold;
        [Header("Star")] 
        public Sprite IconStar;
        [Header("StarTicket")] 
        public Sprite IconTicket;
        
        public Sprite GetCurrencyIcon(CurrencyTypeEnum currencyType)
        {
            Sprite sprite;
            switch (currencyType)
            {
                case CurrencyTypeEnum.GoldCoin:
                    sprite = IconGold;
                    break;
                case CurrencyTypeEnum.StarCoin:
                    sprite = IconStar;
                    break;
                case CurrencyTypeEnum.StarTicket:
                    sprite = IconTicket;
                    break;
                default:
                    return null;
            }
            return sprite;
        }
    }
}
