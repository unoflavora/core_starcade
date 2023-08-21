using System;
using Agate.Starcade.Core.Scripts.Runtime.UI.Reward;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.DailyLogin.SO;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.DailyLogin.UI
{
    public class RewardItemUi : MonoBehaviour
    {
        private const string LOOTBOX = "Loot Box";
        private const string PET = "Pet";
        private const string DEFAULT_TEXT = "";
        private const string DAY = "DAY ";

        [SerializeField] private Image _rewardBackgroundImage;
        [SerializeField] private Image _rewardImage;
        [SerializeField] private GameObject _obtainedVFX;
        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private TextMeshProUGUI _dayText;
        [SerializeField] private TextMeshProUGUI _captionText;
        [SerializeField] private GameObject _rewardOverlayImage;
        [SerializeField] private Animator _rewardOverlayAnimator;
        [SerializeField] private GameObject _priceTextRelocation;
        [SerializeField] private Image _dayHeader;

        [SerializeField] private Color _hiddenRewardColor;

		private DailyLoginRewardData _rewardData { get; set; }

        private bool _doneClaiming;
        private bool _currentReward;

        private DailyLoginSO _dailyLoginSO;

        public void SetupReward(DailyLoginRewardData dailyRewardData, DailyLoginSO dailyLoginSO)
        {
            SetRewardUI(dailyRewardData, dailyLoginSO);
            SetRewardStatusUI(dailyRewardData.IsClaim);
        }

        public void SetupButtonEvent(DailyLoginRewardData dailyRewardData, UnityAction<bool,Sprite> rewardAction)
        {
            bool statusReward = dailyRewardData.IsClaim == RewardStatusEnum.Granted;
            _currentReward = statusReward;

            this.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                if(_currentReward)
                {
                    if (!_doneClaiming)
                    {
                        Sprite iconSprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(dailyRewardData.Ref);
                        rewardAction.Invoke(true, iconSprite);
                        _doneClaiming = true;
                        if (statusReward)
                        {
                            SetRewardStatusUI(RewardStatusEnum.Claimed);
                        }
                    }
                    else
                    {
                        rewardAction.Invoke(false,null);
                    }  
                }
                else
                {
                    rewardAction.Invoke(statusReward,null);
                }
            });
        }

		private void SetRewardUI(DailyLoginRewardData dailyRewardData, DailyLoginSO dailyLoginSO)
        {
            _rewardData = dailyRewardData;
            _dailyLoginSO = dailyLoginSO;

            var tier = _rewardData.Tier;
            if (_rewardData.Tier > 4) tier = 4; //TOO HARDCODED, NEED LIMITING BY BACKEND

            SetupSprite(dailyRewardData.Ref,dailyRewardData.RewardType, tier, dailyRewardData.Day, dailyLoginSO);
            SetupText(dailyRewardData.RewardType,dailyRewardData.Day, dailyRewardData.Caption);
        }

        private void SetupSprite(string assetRef, RewardEnum rewardEnum,int tier, int day, DailyLoginSO dailyLoginSO)
        {
            Sprite sprite;
            if (rewardEnum == RewardEnum.Pet)
            {
                sprite = MainSceneController.Instance.AssetLibrary.GetPetObject(assetRef).PetSpriteAsset;
				_rewardImage.sprite = sprite;
				_rewardImage.color = Color.white;
				Debug.Log("image sprite name = " + sprite.name);
            }
            else if(rewardEnum == RewardEnum.Lootbox)
            {
                _rewardImage.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(assetRef);
                _rewardImage.color = Color.white;
                Debug.Log("image sprite name = " + _rewardImage.sprite.name);
            }
            else
            {
                _rewardImage.sprite = dailyLoginSO.GetRewardSprite(rewardEnum, tier, true);
                _rewardImage.color = Color.white;
                Debug.Log("image sprite name = " + _rewardImage.sprite.name);
            }

            if(day < 7)
            {
                _dayHeader.sprite = dailyLoginSO.GetRewardHeader(rewardEnum);
                _rewardBackgroundImage.sprite = dailyLoginSO.GetRewardBackground(rewardEnum);
            }
        }

        private void SetupText(RewardEnum rewardEnum, int day ,string caption)
        {
            _dayText.text = DAY + day;

            switch (rewardEnum)
            {
                case RewardEnum.GoldCoin:
                    _rewardText.SetText("<sprite=0> " + CurrencyHandler.Convert(_rewardData.Amount));
                    break;
                case RewardEnum.StarCoin:
                    _rewardText.SetText("<sprite=1> " + CurrencyHandler.Convert(_rewardData.Amount));
                    break;
                case RewardEnum.StarTicket:
                    _rewardText.SetText("<sprite=2> " + CurrencyHandler.Convert(_rewardData.Amount));
                    break;
                case RewardEnum.Lootbox:
                    _rewardText.text = LOOTBOX;
                    _rewardText.gameObject.transform.localPosition = _priceTextRelocation.gameObject.transform.localPosition;
                    break;
                case RewardEnum.Pet:
                    _rewardText.text = PET;
                    _rewardText.gameObject.transform.localPosition = _priceTextRelocation.gameObject.transform.localPosition;
                    break;
                default:
                    _rewardText.text = DEFAULT_TEXT;
                    break;
            }

            _captionText.gameObject.SetActive(!string.IsNullOrEmpty(caption));
			_captionText.text = caption;
		}

        private void SetRewardStatusUI(RewardStatusEnum rewardStatus)
        {
            switch (rewardStatus)
            {
                case RewardStatusEnum.Claimed:
                    _rewardOverlayImage.SetActive(true);
                    break;
                case RewardStatusEnum.Granted:
                    _obtainedVFX.SetActive(true);
                    _rewardOverlayImage.SetActive(false);
                    //_rewardOverlayAnimator.SetTrigger("granted");
                    break;
                case RewardStatusEnum.Unclaimed:
                    _rewardOverlayImage.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rewardStatus), rewardStatus, null);
            }
        }
		
    }
}