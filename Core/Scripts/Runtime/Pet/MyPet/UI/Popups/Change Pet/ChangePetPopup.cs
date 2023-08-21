using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Runtime.Main;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.UI.Popups.Change_Pet
{
    public class ChangePetPopup : MonoBehaviour
    {
        [Header("Change Pet UI")]
        [SerializeField] private GameObject _listItemPrefab;
        [SerializeField] private Transform _listContainer;
        [SerializeField] private ChangePetPreview _changePetPreview;
        [SerializeField] private ScrollRect _scrollRect;
        
        [Header("Interactables")]
        [SerializeField] private Button _closeButton;

        [SerializeField] private Button _changePetButton;
        
        private PetInventoryData _selectedPet;
        private PetInventoryData _previousPet;
        private List<ChangePetListItem> _listItems;

        private Action _onClose;
        
        public void DisplayPopup(List<PetInventoryData> petToDisplay, UnityAction<PetInventoryData> onConfirmChange)
        {
            gameObject.SetActive(true);
            
            _previousPet = petToDisplay.Find(pet => pet.IsActive);
            
            _selectedPet = _previousPet;
            
            _changePetPreview.SetButtonActive(false);
            
            _changePetPreview.DisplayPreview(_selectedPet.Name, _selectedPet.ExperienceData.Level, _selectedPet.GetImage());
            
            _changePetPreview.RegisterOnConfirmChangePet(() =>
            {
                _closeButton.onClick.Invoke();
                
                onConfirmChange(_selectedPet);
            });

            petToDisplay.ForEach(pet => AddListItem(pet, OnPetClick));
            
            RegisterOnClose(() =>
            {
                _previousPet = _selectedPet;
             
                _onClose?.Invoke();
            });

            _scrollRect.verticalNormalizedPosition = 1;
        }
        
        private void OnPetClick(PetInventoryData petLibraryData)
        {
            if (_selectedPet.UniqueId == petLibraryData.UniqueId) return;
            
            _selectedPet = petLibraryData;
            
            // Set the previewed Pet
            _changePetPreview.SetButtonActive(_selectedPet.UniqueId != _previousPet.UniqueId);
            _changePetPreview.DisplayPreview(_selectedPet.Name, _selectedPet.ExperienceData.Level, _selectedPet.GetImage());

            // Set the selected pet in the view
            foreach (Transform listItem in _listContainer)
            {
                var changePetListItem = listItem.GetComponent<ChangePetListItem>();
                changePetListItem.SetSelected(changePetListItem.Id == _selectedPet.UniqueId);
            }
        }
        private void RegisterOnClose(UnityAction onClose)
        {
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(onClose);
            _closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }

        public void SetOnClosePopup(Action close)
        {
            _onClose = close;
        }
        
        private void AddListItem(PetInventoryData petLibraryData, UnityAction<PetInventoryData> onClick)
        {
            if (_listItems == null) _listItems = new List<ChangePetListItem>();
            
            ChangePetListItem listItem = Instantiate(_listItemPrefab, _listContainer).GetComponent<ChangePetListItem>();
            
            _listItems.Add(listItem);
            
            // Setup the list item with the pet data
            listItem.SetupView(petLibraryData, () => {onClick.Invoke(petLibraryData);});
            
            listItem.SetSelected(petLibraryData.IsActive);
        }

        private void OnDisable()
        {
            foreach (Transform container in _listContainer)
            {
                Destroy(container.gameObject);
            }
            
            _listItems.Clear();
        }
    }


}