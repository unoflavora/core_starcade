using System;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.DailyLogin.SO;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Newtonsoft.Json;
using Starcade.Core.Scripts.Runtime.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.UI.Reward
{
    public class RewardPopup : MonoBehaviour
    {
        [SerializeField] private Image _rewardImage;
        [SerializeField] private Button _closeButton;
        [SerializeField] private NumberCounter _rewardAmountText;
        
        public void InitEvent(UnityAction onClosePopup)
        {
            _closeButton.onClick.AddListener(onClosePopup);
        }

        public void Init(DailyLoginRewardData rewardData, DailyLoginSO dailyLoginSo)
        {
            _rewardAmountText.Text.text = "0";
            SetRewardSprite(rewardData);

		}

        private void SetRewardSprite(DailyLoginRewardData rewardData)
        {
			if (rewardData == null) return;

			var rewardBase = new Starcade.Runtime.Data.RewardBase()
			{
				Type = rewardData.RewardType,
				Amount = rewardData.Amount,
				Ref = rewardData.Ref,
				RefObject = rewardData.RefObject,
			};

			var spriteAsset = MainSceneController.Instance.AssetLibrary.GetSpriteRewardAsset(rewardBase);
			Debug.Log($"load sprite: {JsonConvert.SerializeObject(rewardBase)}");
			if (spriteAsset != null)
			{
				_rewardImage.sprite = spriteAsset;
			}
			else
			{
				Debug.LogError($"Failed to find sprite asset data: {JsonConvert.SerializeObject(rewardBase)}");
			}
		}

        public void ShowUI(bool show, DailyLoginRewardData rewardData = null)
        {
            gameObject.SetActive(show);
            if (rewardData != null)
                _rewardAmountText.Balance = (rewardData.Amount);
        }
    }
}
