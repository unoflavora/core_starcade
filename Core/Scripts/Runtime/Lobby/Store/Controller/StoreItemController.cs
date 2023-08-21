using System;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Data_Class.Lootbox;
using Agate.Starcade.Core.Runtime.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;
using UnityEngine.EventSystems;

namespace Agate.Starcade.Core.Runtime.Lobby.Store.Controller
{
    public class StoreItemController : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _background;
        [SerializeField] private Image _item;
        [SerializeField] private GameObject _lock;
        [SerializeField] private TextMeshProUGUI _goldText;
        [SerializeField] private TextMeshProUGUI _starText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private GameObject _premiumOrnament;

        [Header("Lootbox UI Items")] 
        [SerializeField] private Button _infoButton;
        
        private AssetLibrary _assetLibrary;
        public void AddListener(UnityAction call)
        {
            _button.onClick.AddListener(call);
        }

        public void AddInfoListener(UnityAction call)
        {
            _infoButton.onClick.RemoveAllListeners();
            _infoButton.onClick.AddListener(call);
        }

        public void SetContent(object content)
        {
            GeneralStoreModel item = (GeneralStoreModel)content;
            _goldText.SetText("<sprite=0> " + CurrencyHandler.Format(item.Gold));
            _starText.SetText("<sprite=1> " + CurrencyHandler.Format(item.Star));
            _costText.SetText($"<sprite=0> ${item.Cost}.00");
            _background.sprite = item.Background;
            _item.sprite = item.Item;
            SetLock(item.IsLocked);
        }
        
        public void SetContent(ShopData content, LootboxData lootboxData)
        {
            _button.onClick.RemoveAllListeners();
            
            if (_assetLibrary == null) _assetLibrary = MainSceneController.Instance.AssetLibrary;
            
            _lock.SetActive(false);
            
            String spriteIconId;
            switch (content.itemConfig.CostCurrency)
            {
                case CurrencyTypeEnum.GoldCoin:
                    spriteIconId = "<sprite=0> ";
                    break;
                case CurrencyTypeEnum.StarCoin:
                    spriteIconId = "<sprite=1> ";
                    break;
                case CurrencyTypeEnum.StarTicket:
                    spriteIconId = "<sprite=2> ";
                    break;
                default:
                    spriteIconId = "<sprite=0> ";
                    break;
            }
            
            _goldText.SetText(spriteIconId + CurrencyHandler.Format(content.itemConfig.Cost));

            _costText.SetText(content.itemName);

            string idBackground = "bg_lootbox_" + Enum.GetName(typeof(LootboxRarityEnum), lootboxData.RarityType)!.ToLower();
            if (lootboxData.IsPremium) idBackground += "_premium";
            _background.sprite = _assetLibrary.GetSpriteAsset(idBackground);

            string idItem = lootboxData.LootboxItemId;
            _item.sprite = _assetLibrary.GetSpriteAsset(idItem);
            
            _premiumOrnament.SetActive(lootboxData.IsPremium);

        }

        public void SetLock(bool isLocked)
        {
            _lock.SetActive(isLocked);
            _button.interactable = !isLocked;
        }
    }
}
