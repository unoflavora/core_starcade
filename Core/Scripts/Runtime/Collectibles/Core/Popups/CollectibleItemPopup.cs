using System;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using TMPro;
using Agate.Starcade.Runtime.Main;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Core.Popups
{
    public class CollectibleItemPopup : MonoBehaviour
    {
        [Header("Collectible main prefab")]
        [SerializeField] private CollectibleSlot _itemSlot;
        
        [Header("Popup UI")]
        [SerializeField] private FadeTween _unableToSendPopupText;
        [SerializeField] private GameObject _newLabel;
        
        [Header("Item UI")]
        [SerializeField] private Sprite _defaultImage;
        [SerializeField] private Image _pinBackground;
        [SerializeField] private Animator _animator;

        [Header("Popup interactables")]
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _actionButton;

        public UnityAction OnClosePopupClicked;
        public Action<string> GoToStoreLootbox;
        private static readonly int StartAnim = Animator.StringToHash("Start");

        public void DisplayItem(CollectibleItem item, bool displayNewLabel = false, bool displayActionButton = true)
        {
            if (displayNewLabel)
            {
                _animator.SetTrigger(StartAnim);
            }
            
            _newLabel.SetActive(displayNewLabel);
            _actionButton.gameObject.SetActive(displayActionButton);

            _itemSlot.SetupData(item, 0, new GridUIOptions { ShowItemCount = displayNewLabel == false && item.Amount > 0 });
            
            _pinBackground.sprite = item.Amount > 0 
                ? MainSceneController.Instance.AssetLibrary.GetSpriteAsset($"background_tier_{item.GetStarCount()}") 
                : _defaultImage;
            
            SetButton();
        }
        
        private void Start()
        {
            _closeButton.onClick.AddListener(OnClosePopup);
        }

        private void OnClosePopup()
        {
            //MainSceneLauncher.Instance.Audio.PlaySfx(MainSceneLauncher.AUDIO_KEY.BUTTON_GENERAL);

            OnClosePopupClicked.Invoke();
            
            _unableToSendPopupText.FadeOut();
            
            _pinBackground.sprite = _defaultImage;
            
            gameObject.SetActive(false);
        }
        
        private void SetButton()
        {
            bool isItemOwned = _itemSlot.ItemData.Amount > 0;
            
            bool isItemSpecial = _itemSlot.ItemData.GetStarCount() == 4;
            
            TextMeshProUGUI actionText = _actionButton.GetComponentInChildren<TextMeshProUGUI>();

            if (isItemOwned)
            {
                actionText.text = "Send";
            }
            else 
            {
                if (isItemSpecial)
                {
                    actionText.text = "Convert Pin";
                }
                else
                {
                    actionText.text = "Buy Lootbox";
                }
            }
            
            _actionButton.onClick.RemoveAllListeners();
            
            _actionButton.onClick.AddListener(() =>
            {
                
                MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.BUTTON_GENERAL);
                
                if (isItemOwned)
                {
                    if (isItemSpecial || _itemSlot.ItemData.Rarity == 3)
                    {
                        _unableToSendPopupText.FadeIn();
                        _unableToSendPopupText.GetComponentInChildren<TextMeshProUGUI>().text = "You can only send 1 and 2 star pins.";
                    }
                    else if (_itemSlot.ItemData.Amount < 2)
                    {
                        _unableToSendPopupText.FadeIn();
                        _unableToSendPopupText.GetComponentInChildren<TextMeshProUGUI>().text = "You can only send pins with duplicates.";
                    }
                    else
                    {
                        OnClosePopup();
                        CollectibleActionController.OnSendPinClicked(_itemSlot.ItemData);
                    }
                }
                else
                {
                    if (isItemSpecial)
                    {
                        CollectibleActionController.OnConvertPinClicked();
                    }
                    else
                    {
                        GoToStoreLootbox(_itemSlot.ItemData.CollectibleItemId);
                    }
                }
            });
        }
        
        
    }
}
