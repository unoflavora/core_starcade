using System.Collections;
using System.Collections.Generic;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Game;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.DailyLogin;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Agate.Starcade
{
    public class LobbyUI : MonoBehaviour
    {
        [Header("PROFILE")]
        [SerializeField] private Image _photoImage;
        [SerializeField] private Image _frameImage;

        [Header("BALANCE")]
        [SerializeField] private TMP_Text _goldCoinLabel;
        [SerializeField] private TMP_Text _starCoinLabel;
        [SerializeField] private TMP_Text _starTicketLabel;

        [Header("EXP")]
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private Image _expSlider;


        public void UpdateUserExp(PlayerExpData playerExpData)
        {
            _levelLabel.text = playerExpData.Level.ToString();
            float currentExp = float.IsInfinity((playerExpData.TotalExpNeeded - playerExpData.TotalExp) / playerExpData.Deviation) ? 0 : (playerExpData.TotalExpNeeded - playerExpData.TotalExp) / playerExpData.Deviation;
            _expSlider.fillAmount = currentExp;
        }

        public void UpdateUserName(string name)
        {
            _nameLabel.text = name;
        }

        public void UpdateBalanceLabel(PlayerBalance playerBalance)
        {
            _goldCoinLabel.text = CurrencyHandler.Convert(playerBalance.GoldCoin);
            _starCoinLabel.text = CurrencyHandler.Convert(playerBalance.StarCoin);
            _starTicketLabel.text = CurrencyHandler.Convert(playerBalance.StarTicket);
        }

        public void UpdatePhotoProfile()
        {
            _photoImage.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentAvatar();
            if (MainSceneController.Instance.Data.UserProfileData.GetCurrentFrame() != null)
            {
                _frameImage.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentFrame();
                _frameImage.gameObject.SetActive(true);
            }
            else
            {
                _frameImage.gameObject.SetActive(false);
            }
        }

        public void UpdatePhotoProfile(string avatarId, string frameId)
        {
            Debug.Log("UPDATE AVATAR AND FRAME = " + avatarId + " & " + frameId);
            _photoImage.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentAvatar();
            _frameImage.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentFrame();
        }

        public void UpdatePhotoProfile(Dictionary<ItemTypeEnum, string> data)
        {
            Debug.Log("Updating profile..");
            if (data.ContainsKey(ItemTypeEnum.Avatar)) 
            {
                _photoImage.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentAvatar();
                //if (_photoImage != null) 
            } 
            if (data.ContainsKey(ItemTypeEnum.Frame)) _frameImage.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentFrame();
        }
    }
}
