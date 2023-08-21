using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate
{
    public class ArcadeLevelUpController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _level;
        [SerializeField] Image _rewardIcon;
        [SerializeField] TextMeshProUGUI _rewardText;

        public void SetData(int level, RewardEnum type, long amount)
        {
            _level.text = $"LV. {level}";
            _rewardIcon.sprite = GetRewardSprite(type);
            _rewardText.text = CurrencyHandler.Convert(amount);
        }

        public async void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
            if (visible)
            {
                await Task.Delay(3000);
                SetVisible(false);
            }
        }

        public Sprite GetRewardSprite(RewardEnum type)
        {
            Sprite sprite = null;
            if (type == RewardEnum.GoldCoin) sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset("currency_default_goldcoin");
            else if (type == RewardEnum.StarCoin) sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset("currency_default_starcoin");
            else if (type == RewardEnum.StarTicket) sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset("currency_default_starticket");
            return sprite;
        }
    }
}
