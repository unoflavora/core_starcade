using System;
using System.Collections.Generic;
using System.Linq;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Core.Popups
{
    public class ConfirmationPopup : MonoBehaviour
    {
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        
        [SerializeField] private List<CollectibleSlot> _slots;
        public Action OnCancelClicked;
        public Action<List<CollectibleItem>> OnConfirmClicked;

        private HorizontalLayoutGroup _layout;

        private void Start()
        {
            _cancelButton.onClick.AddListener(() =>
            {
                OnCancelClicked.Invoke();
            });
            _confirmButton.onClick.AddListener(() =>
            {
                OnConfirmClicked.Invoke(_slots.ConvertAll(slot => slot.ItemData));
                EnableButtons(false);
            });

        }

        public void DisplayPopup(List<CollectibleItem> itemsToShow)
        {
            EnableButtons();
            
            if (_layout == null) _layout = GetComponentInChildren<HorizontalLayoutGroup>();

            _layout.enabled = true;

            for (int i = 0; i < itemsToShow.Count; i++)
            {
                _slots[i].SetupData(itemsToShow[i], i, new GridUIOptions { ShowItemCount = false });
            }
        }

        private void EnableButtons(bool enable = true)
        {
            _confirmButton.interactable = enable;

            _cancelButton.interactable = enable;
        }
       
    }
}
