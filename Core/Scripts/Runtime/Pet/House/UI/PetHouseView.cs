using System;
using System.Collections.Generic;
using System.Linq;
using Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using TMPro;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.House.UI
{
    public class PetHouseView : MonoBehaviour
    {
        [SerializeField] private Transform _petListContainer;
        [SerializeField] private TMP_Dropdown _petDropdown;
        [SerializeField] private Transform _petListContentPrefab;
        [SerializeField] private GameObject _unavailableText;

        private List<PetStatusView> _petHouseItemViews;
        public Action<PetInventoryData> OnPetItemClicked;
        private List<PetInventoryData> _pets;

        private PetHouseAnalyticEventHandler _analytic;

        private int _currentSort;

        public void Init()
        {
            _petDropdown.onValueChanged.AddListener(OnDropdownChanged);
            
            _petHouseItemViews = new List<PetStatusView>();
            
            foreach (Transform pet in _petListContainer)
            {
                var itemView = pet.GetComponent<PetStatusView>();
                _petHouseItemViews.Add(itemView);
            }

            _currentSort = 0;
            
            _petDropdown.SetValueWithoutNotify(_currentSort);
        }

        public void SetupAnalytic(PetHouseAnalyticEventHandler analyticEventHandler)
        {
            _analytic = analyticEventHandler;
        }

        public void DisplayPets(List<PetInventoryData> pets)
        {
            _pets = SortPetList(pets, _currentSort);
            
            for (int i = 0; i < _pets.Count; i++)
            {
                var data = _pets[i];
                
                if (i < _petHouseItemViews.Count)
                {
                    _petHouseItemViews[i].SetupView(data, () => OnPetViewClick(data));
                    _petHouseItemViews[i].gameObject.SetActive(true);
                }
                else
                {
                    var petItem = Instantiate(_petListContentPrefab, _petListContainer);
                    var itemView = petItem.GetComponent<PetStatusView>();
                    itemView.SetupView(data, () => OnPetViewClick(data));
                    _petHouseItemViews.Add(itemView);
                }
            }
            
            for(int i = _pets.Count; i < _petHouseItemViews.Count; i++)
            {
                _petHouseItemViews[i].gameObject.SetActive(false);
            }
            
            _unavailableText.SetActive(_pets.Count < 1);
        }

        private void OnPetViewClick(PetInventoryData pet)
        {
            OnPetItemClicked?.Invoke(pet);
        }

        private void OnDropdownChanged(int val)
        {
            if (_pets == null) return;
            
            _currentSort = val;
            
            DisplayPets(_pets);
        }

        private List<PetInventoryData> SortPetList(List<PetInventoryData> petListToSort, int val)
        {
            _currentSort = val;

            List<PetInventoryData> pets;

            switch (val)
            {
                case 1:
                    pets = petListToSort.OrderByDescending(pet => pet.ExperienceData.Level).ToList();
                    _analytic.SelectPetHouseFilterEvent("Level");

                    break;
                case 2:
                    pets = petListToSort.OrderByDescending(pet => pet, new DateComparer()).ToList();
                    _analytic.SelectPetHouseFilterEvent("Obtained Date");
                    break;
                default:
                    pets = petListToSort.OrderByDescending(pet => pet.Name)
                        .ThenByDescending(pet => pet.IsActive)
                        .ThenByDescending(pet => pet, new DateComparer())
                        .ToList();
                    _analytic.SelectPetHouseFilterEvent("Default");
                    break;
            }

            return pets;
        }
        
    }

    public class DateComparer : IComparer<PetInventoryData>
    {
        public int Compare(PetInventoryData x, PetInventoryData y)
        {
            return DateTime.Compare(DateTime.Parse(x.ObtainedDate), DateTime.Parse(y.ObtainedDate));
        }
    }
}