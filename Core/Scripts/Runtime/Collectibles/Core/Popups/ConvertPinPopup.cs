using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Core.Popups
{
    public class ConvertPinPopup : MonoBehaviour
    {
        [SerializeField] private PoolableGridUI _itemSlot;
        [SerializeField] private ConfirmationPopup _confirmationPopup;
        
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _convertButton;
       
        [SerializeField] private ErrorMessage _errorText;

        [SerializeField] private GameObject _noPinText;
        
        public UnityAction OnClosePopupClicked;
        public Action<List<CollectibleItem>> OnConvertPin;

        private List<CollectibleSlot> _selectedPins;
        private List<Transform> _displayedPins;

        private const int PIN_REQUIRED_TO_CONVERT = 2;
        
        public void DisplayItem(List<CollectibleItem> items)
        {
            _selectedPins = new List<CollectibleSlot>();
            
            _noPinText.SetActive(items.Count < 1);

            _itemSlot.Draw(FlatItems(items), AddSelectedItem, new GridUIOptions { ShowItemCount = false });
            
            _displayedPins = _itemSlot.ItemObjects;
            DisableAllUnselectedItem(false);

        }

        private List<CollectibleItem> FlatItems(List<CollectibleItem> items)
        {
            List<CollectibleItem> flattenedItem = new List<CollectibleItem>();

            foreach (var item in items)
            {
                for (int i = 0; i < item.Amount - 1; i++)
                {
                    flattenedItem.Add(item);
                }
            }
            
            return flattenedItem;
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(OnClosePopup);
            _convertButton.onClick.AddListener(OnConvertPinClicked);
        }

        private void OnEnable()
        {
            _convertButton.interactable = false;
        }


        private void Update()
        {
            if (_selectedPins != null && _selectedPins.Count >= PIN_REQUIRED_TO_CONVERT)
            {
                if(PinAmountSufficient())
                {
                    _convertButton.interactable = true;
                    _errorText.gameObject.SetActive(false);
                }
                else
                {
                    ShowErrorMessage("Each pin must still be available after converted");
                }
            }
            else
            {
                ShowErrorMessage("You need at least 2 pin to convert");
            }
        }

        private bool PinAmountSufficient()
        {
            var distinctPins = _selectedPins.Select(item => item.ItemData.CollectibleItemId).Distinct();
            
            if (distinctPins.Count() == 1)
            {
                return _selectedPins[0].ItemData.Amount > 2;
            }
            
            return _selectedPins.TrueForAll(pin => pin.ItemData.Amount > 1);

        }

        private void ShowErrorMessage(string message)
        {
            _errorText.SetErrorText(message);
            _errorText.gameObject.SetActive(true);
            _convertButton.interactable = false;
        }
        
        
        private void AddSelectedItem(Transform item)
        {
            CollectibleSlot itemSlot = item.GetComponent<CollectibleSlot>();
            
            AddToSelectedItems(itemSlot);

            if (_selectedPins.Count >= PIN_REQUIRED_TO_CONVERT)
            {
                OnSelectedItemIsSufficient();
            }
        }

        private void AddToSelectedItems(CollectibleSlot item)
        {
            if (_selectedPins.Contains(item))
            {
                if (_selectedPins.Count == PIN_REQUIRED_TO_CONVERT)
                {
                    DisableAllUnselectedItem(false);
                }

                _selectedPins.Remove(item);
                SelectItem(item, false);
                return;
            };

            _selectedPins.Add(item);
            SelectItem(item, true);
        }

        private static void SelectItem(CollectibleSlot item, bool selected)
        {
            item.EnableCheckmark(selected);
            item.EnableOverlay(selected, Color.black);
        }

        private void OnSelectedItemIsSufficient()
        {
            DisableAllUnselectedItem(true);
        }

        private void DisableAllUnselectedItem(bool disable)
        {
            if(_displayedPins == null) return;
            
            foreach (var pin in _displayedPins)
            {
                var pinUi = pin.GetComponent<CollectibleSlot>();

                if (!_selectedPins.Contains(pinUi))
                {
                    pinUi.EnableOverlay(disable, Color.gray);
                    pinUi.EnableCheckmark(false);

                    pinUi.GetComponent<Button>().interactable = !disable;
                }
            }
        }

        private void OnClosePopup()
        {
            OnClosePopupClicked.Invoke();
            
            gameObject.SetActive(false);
        }

        private void OnConvertPinClicked()
        {
            if(_selectedPins.TrueForAll(pin => pin.ItemData.Amount > 1))
            OnConvertPin(_selectedPins.ConvertAll(i => i.ItemData));
        }
        
    }
}
