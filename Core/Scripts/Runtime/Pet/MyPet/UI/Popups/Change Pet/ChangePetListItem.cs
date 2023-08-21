using System;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.House.UI;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.UI.Popups.Change_Pet
{
    public class ChangePetListItem : PetStatusView
    {
        [Header("Change Pet Items")]
        [SerializeField] private GameObject _selected;
        private string _id;
        public string Id => _id;

        private Action _onClicked;
        private bool isInteractable;

        public void SetupView(PetInventoryData pet, Action onClick)
        {
            _id = pet.UniqueId;
            
            _onClicked = () =>
            {
                if(isInteractable)
                    onClick();
            };
            
            base.SetupView(pet, _onClicked);
        }
        
        public void SetSelected(bool selected)
        {
            isInteractable = !selected;

            _selected.SetActive(selected);
        }
    }
}