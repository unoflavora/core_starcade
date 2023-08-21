using System;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot.Item;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot
{
    /// <summary>
    /// The UI of collectible item;
    /// </summary>
    public class CollectibleSlot : MonoBehaviour, IPoolableGridItem<CollectibleItem>
    {
        [SerializeField] private Image _overlayImage;
        [SerializeField] private Image _checkMark;
        [SerializeField] private TextMeshProUGUI _itemName;
        // Data
        public CollectibleItem ItemData { get; private set; }

        // Listeners
        public Action<CollectibleItem> OnItemClicked { get; set; }

        // UI
        private Button _clickable;

        /// <summary>
        /// Set this slot's item.
        /// </summary>
        /// <param name="collectibleItem"> Item of which the slot will be assigned to</param>
        /// <param name="index"> Index of this slot in the parent's grid </param>
        /// <param name="uiOptions"> Class object for the adjusting the UI of this item, feel free to add something to it but please don't remove anything</param>
        public void SetupData(CollectibleItem collectibleItem, int index = 0, GridUIOptions uiOptions = null)
        {
            if (collectibleItem != null)
            {
                _itemName.gameObject.SetActive(true);

                _itemName.text = collectibleItem.Amount > 0 ? collectibleItem.GetDisplayName() : "Unknown";
            
                _itemName.alpha = collectibleItem.Amount > 0 ? 1 : .3f;
            }
            else
            {
                _itemName.gameObject.SetActive(false);
            }

            ItemData = collectibleItem;

            foreach (Transform child in transform)
            {
                var uiObject = child.GetComponent<ICollectibleItemUIObjects>();
                
                uiObject?.SetData(ItemData, uiOptions);
            }

            if (uiOptions != null)
            {
                _itemName.gameObject.SetActive(uiOptions.ShowItemName);
            }
        }
        
        public void SetupData(string refId, long amount)
        {
            ItemData = CollectibleItem.FindCollectibleItemById(refId);
            
            CollectibleItem item = new CollectibleItem()
            {
                Amount = (int)amount,
                CollectibleItemId = ItemData.CollectibleItemId,
                CollectibleItemName = ItemData.CollectibleItemName,
                Rarity = ItemData.Rarity
            };
            
            SetupData(item, 0);
        }

        public void EnableOverlay(bool enable, Color overlayColor = default)
        {
            overlayColor.a = .65f;
            _overlayImage.enabled = enable;
            
            if(enable) _overlayImage.color = overlayColor;
        }
        
        public void EnableCheckmark(bool enable)
        {
            _checkMark.enabled = enable;
        }
        
        private void Start()
        {
            _clickable = GetComponent<Button>();
            _clickable.onClick.AddListener(delegate
            {
                if (OnItemClicked == null) return;
                
                OnItemClicked.Invoke(ItemData);
            });
        }
    }
}
