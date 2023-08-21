using System;
using System.Collections.Generic;
using System.Linq;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Lobby.Script.UserProfile.FriendsManager.UI.Popups;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using Agate.Starcade.Scripts.Runtime.Info;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.Core.Popups
{
    public class SendGiftCollectiblePopupController : MonoBehaviour
    {
        [Header("Popup Modules")]
        [SerializeField] private List<CollectibleSlot> _giftToSelect;
        [FormerlySerializedAs("_collectibleGridUI")] [SerializeField] private PoolableGridUI _poolableGridUI;
        [SerializeField] private GameObject _emptyText;
        [SerializeField] private Dropdown _displayFilterDropdown;
        [SerializeField] private TextMeshProUGUI _friendName;
        
        [Header("Interactables")]
        [SerializeField] private Button _sendGiftButton;
        [SerializeField] private Button _closeButton;

        // Variables
        private List<CollectibleSlot> _selectedItem { get; set; }
        private int _maxAmount;

        // Event Listeners
        public UnityAction<List<CollectibleItem>> OnSendGiftConfirmed;
        public UnityAction OnLimitReached;
        public UnityAction OnClosePopup;
        public int _giftDailyLimit;
        public int GiftDailyLimit
        {
            set => _giftDailyLimit = value;
        }

        public async void DisplayPopup(int maxAmount, string friendName, CollectibleItem collectible)
        {
            await InitPopup(maxAmount, friendName);
            
            if(collectible == null) return;
            
            _poolableGridUI.ItemObjects.ForEach(gift =>
            {
                if (gift.GetComponent<CollectibleSlot>().ItemData.GetItemId() == collectible.GetItemId())
                {
                    HandleItemClicked(gift);
                }
            });
        }
        
        private async Task InitPopup(int maxAmount, string friendName)
        {
            var collectibles = await MainSceneController.Instance.GameBackend.GetCollectibles();
            
            _friendName.SetText("Send Pin to " + friendName);
            
            MainSceneController.Instance.Data.CollectiblesData = collectibles.Data;
            
            InitDropdowns();

            _maxAmount = maxAmount;
            
            _selectedItem = new List<CollectibleSlot>();
            
            RefreshPins();
            PreviewItems();
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
            
            _sendGiftButton.onClick.AddListener(OnSendGiftButtonClicked);

            _displayFilterDropdown.onValueChanged.AddListener(OnFilterDropdownChanged);
            
            _giftToSelect.ForEach(gift =>
            {
                gift.OnItemClicked = (t) => OnGiftItemClicked(gift);
            });
        }

        private void Update()
        {
            _sendGiftButton.interactable = _selectedItem != null && _selectedItem.Count > 0; 
        }

        private void OnGiftItemClicked(CollectibleSlot collectibleItem)
        {
            if (collectibleItem.ItemData == null) return;
            
            if (_selectedItem == null) return;
            
            var pin = _selectedItem.Find(item => item.ItemData.GetItemId() == collectibleItem.ItemData.GetItemId());
            
            if (pin != null)
            {
                _selectedItem.Remove(pin);
                pin.EnableOverlay(false);
                pin.EnableCheckmark(false);

                collectibleItem.SetupData(null, 0);
                PreviewItems();
            }
        }

        private void InitDropdowns()
        {
            _displayFilterDropdown.options.Clear();
            _displayFilterDropdown.options.Add(new Dropdown.OptionData("All Pins"));
            _displayFilterDropdown.options.AddRange(MainSceneController.Instance.Data.CollectiblesData
                .Select(t => new Dropdown.OptionData(t.CollectibleSetName))
                .ToArray());
        }

        private void OnFilterDropdownChanged(int arg0)
        {
            var selectedFilter = _displayFilterDropdown.options[arg0].text;
            if (selectedFilter == "All Pins") selectedFilter = "";
            
            Debug.Log(selectedFilter);
            
            RefreshPins(selectedFilter);
        }
        

        private void RefreshPins(string setName = "")
        {
            var availableSlots = MainSceneController.Instance.Data.CollectiblesData
                .Where(t => String.IsNullOrEmpty(setName) || t.CollectibleSetName == setName) 
                .SelectMany(t => t.CollectibleItems)
                .Where(t => t.Amount >= 2 && t.Rarity < 3)
                .ToList();
            
            OnGiftablePinExist(availableSlots.Count > 0);

            _poolableGridUI.Draw(availableSlots, HandleItemClicked);
        }

        private void OnGiftablePinExist(bool exist)
        {
            _poolableGridUI.gameObject.SetActive(exist);
            _emptyText.SetActive(!exist);
        }

        private void HandleItemClicked(Transform itemClicked)
        {
            var pin = itemClicked.GetComponent<CollectibleSlot>();
            
            if (_selectedItem.Contains(pin))
            {
                pin.EnableOverlay(false);
                pin.EnableCheckmark(false);
                _selectedItem.Remove(pin);
            }
            else
            {
                if (_selectedItem.Count < _giftToSelect.Count)
                {
                    pin.EnableOverlay(true);
                    pin.EnableCheckmark(true);
                    _selectedItem.Add(pin);
                }
                else
                {
                    MainSceneController.Instance.Info.Show("You can only send 5 gifts at a time.", "Limit Reached", InfoIconTypeEnum.Alert,
                        new InfoAction("OK", () => {}), null);
                }
            }

            PreviewItems();
        }

        private void OnCloseButtonClicked()
        {
            RefreshLayout();

            RefreshPins();
            
            OnClosePopup?.Invoke();
            
            gameObject.SetActive(false);
        }

        private void RefreshLayout()
        {
            _selectedItem.ForEach(pin =>
            {
                pin.EnableOverlay(false);
                pin.EnableCheckmark(false);
            });

            _selectedItem.Clear();

            _giftToSelect.ForEach(pin => pin.SetupData(null, 0));

            _displayFilterDropdown.SetValueWithoutNotify(0);
        }

        private void OnSendGiftButtonClicked()
        {
            if (_selectedItem.Count > _giftDailyLimit)
            {
                OnLimitReached?.Invoke();
                return;
            }

            _giftDailyLimit -= _selectedItem.Count;
            
            OnSendGiftConfirmed?.Invoke(_selectedItem.Select(t => t.ItemData).ToList());
        }

        public void OnBuySuccess()
        {
            RefreshPins();

            RefreshLayout();
        }
        
        private void PreviewItems()
        {
            for (var i = 0; i < _giftToSelect.Count; i++)
            {
                if (i >= _selectedItem.Count)
                    _giftToSelect[i].SetupData(null, i);
                else
                {
                    _giftToSelect[i].gameObject.SetActive(true);
                    _giftToSelect[i].SetupData(_selectedItem[i].ItemData, i);
                }
            }
        }
    }
}
