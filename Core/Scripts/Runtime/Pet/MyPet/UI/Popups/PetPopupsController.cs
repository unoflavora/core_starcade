using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.MyPet.Modules;
using Agate.Starcade.Core.Runtime.Pet.MyPet.UI.Popups.Change_Pet;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.UI.Popups
{
    public class PetPopupsController : MonoBehaviour
    {
        [SerializeField] GameObject _popupBackground;
        [SerializeField] ChangePetPopup _changePetPopup;
        [SerializeField] PetAdventureController _petAdventurePopup;

        public void OpenChangePetPopup(List<PetInventoryData> pets, UnityAction<PetInventoryData> onChangePet)
        {
            _popupBackground.SetActive(true);
            _changePetPopup.DisplayPopup(pets, onChangePet);
            _changePetPopup.SetOnClosePopup(TurnOffBackground);
        }

        public void OpenChooseAdventurePopup(PetInventoryData currentPet, List<AdventureConfig> petConfigsAdventureConfigs, Action<int> onPetDispatch)
        {
            _popupBackground.SetActive(true);
            _petAdventurePopup.SetupDisplay(currentPet, petConfigsAdventureConfigs, onPetDispatch);
            _petAdventurePopup.gameObject.SetActive(true);

            _petAdventurePopup.SetOnClosePopup(TurnOffBackground);
        }
        
        private void TurnOffBackground()
        {
            _popupBackground.SetActive(false);
        }

    }
}