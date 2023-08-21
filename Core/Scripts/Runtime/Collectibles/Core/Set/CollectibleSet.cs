using System;
using System.Collections.Generic;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Popups;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Reward;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles;
using Agate.Starcade.Scripts.Runtime.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set
{
    public class CollectibleSet : MonoBehaviour
    {
        [FormerlySerializedAs("_collectibleSetGrid")]
        [Header("Set Modules")]
        [SerializeField] private PoolableGridUI _poolableSetGrid;
        [SerializeField] private CollectibleReward _rewardController;
        [SerializeField] private TextMeshProUGUI _setTitle;
        [SerializeField] private ScrollRect _scrollUi;
        
        [Header("Set Intractable")]
        [SerializeField] private Button _convertButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _shareButton;

        private List<CollectibleItem> _slots;
        
        private List<CollectibleItem> _toBeConvertedItems;
        public UnityAction OnBackClicked;

        private const int MIN_STAR_FOR_CONVERT = 3;
        
        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="item">The item type to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public void AddItemToSlot(CollectibleItem item)
        {
            if (_slots == null)
            {
                _slots = new List<CollectibleItem>();
            }
            
            AddItemToFirstEmptySlot(item);
        }
        
        public void RemoveItem(CollectibleItem item)
        {
            try
            {
                _slots.Find(i => i == item).Amount--;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            UpdateInventory();
        }

        private void AddItemToFirstEmptySlot(CollectibleItem item)
        {
            var slot = _slots.Find(slotItem => slotItem.GetItemId() == item.GetItemId());

            if (slot == null)
            {
                _slots.Add(item);
            }
            else
            {
                item.Amount++;
            }
            
            UpdateInventory();
        }
        
        public void UpdateInventory()
        {
            _poolableSetGrid.Draw(_slots, (reward) => CollectibleActionController.OnPinClicked(reward));
            
            _rewardController.SetPinCount(_slots.FindAll(slot => slot.Amount > 0).Count, _slots.Count);
        }
        
        private void Start()
        {
            CollectibleActionController.OnConvertPinClicked = OnConvertPopupClicked;
            
            _convertButton.onClick.AddListener(() =>
            {
                OnConvertPopupClicked();
            });
            _backButton.onClick.AddListener(() =>
            {
                OnBackClicked();
            });
            _shareButton.onClick.AddListener(async () =>
            { 

                CollectibleActionController.OnShareClicked(_slots);
            });
        }

        public void ResetSlot()
        {
            _slots?.Clear();
        }
        
        public void SetCollectionSetReward(CollectibleRewardData userRewardData)
        {
            _rewardController.SetReward(userRewardData);
        }

        public void SetCollectionSetTitle(string title)
        {
            _setTitle.SetText(title);
        }
        
        private void OnConvertPopupClicked()
        {
            List<CollectibleItem> eligibleItem = _slots.FindAll(item => item.GetStarCount() == MIN_STAR_FOR_CONVERT && item.Amount > 1);
            
            _toBeConvertedItems = eligibleItem;
            
            CollectibleActionController.OnConvertPinPopupClicked.Invoke(eligibleItem);
        }

        private void OnEnable()
        {
            _scrollUi.verticalNormalizedPosition = 1;
        }
    }
}
