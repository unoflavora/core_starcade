using Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet;
using Agate.Starcade.Core.Runtime.Pet.House.UI;
using Agate.Starcade.Core.Runtime.Pet.MyPet.Interaction;
using Agate.Starcade.Runtime.Main;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.Core.Model
{
    public class InteractablePetStatusView : PetStatusView
    {
        [Header("Interactable Pet Module")]
        [SerializeField] private PetSpineController _currentPet;
        [SerializeField] private PetInteractionController _interactionController;
  

        public void SetupView(PetInventoryData pet, PetHouseAnalyticEventHandler analytic)
        {
            UpdatePetData(pet);
            _currentPet.gameObject.SetActive(true);
            var spineData = MainSceneController.Instance.AssetLibrary.GetPetObject(pet.Id).SkeletonDataAsset;
            _currentPet.SetPetAsset(spineData);
            _interactionController.RegisterOnPetTouched(analytic.TrackTouchPetAtPetHouseEvent);
        }

        public void UpdatePetData(PetInventoryData pet)
        {
            base.SetupView(pet);
        }

        public void ActivatePet()
        {
            if (_currentPet != null)
            {
                _currentPet.OnPetInteract();
            }
        }

        private void OnDisable()
        {
            _currentPet.gameObject.SetActive(false);
        }
    
    }
}
