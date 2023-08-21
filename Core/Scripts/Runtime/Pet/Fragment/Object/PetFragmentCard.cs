using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.MyPet.UI;
using Agate.Starcade.Runtime.Main;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.Fragment.Object
{
	public class PetFragmentCard : MonoBehaviour
    {
        [SerializeField] private Button _petCardButton;
        [SerializeField] private Image _petImage;
        [SerializeField] private TMP_Text _petName;
        [SerializeField] private TMP_Text _petFragmentProgressText;
        [SerializeField] private SliderTransition _petFragmentProgressSlider;

        [SerializeField] private GameObject _highlightBorder;
        [SerializeField] private GameObject _highlightVFX;

        private PetFragmentInventory _petFragmentInvetory;

        //public UnityEvent<PetFragmentData> OnClickCard = new UnityEvent<PetFragmentData>();

        public void Setup(PetFragmentInventory petFragmentInvetory, UnityAction<PetFragmentInventory> OnClickCard)
        {
            _petFragmentInvetory = petFragmentInvetory;

            _petImage.sprite = MainSceneController.Instance.AssetLibrary.GetPetObject(petFragmentInvetory.PetId)?.FragmentSpriteAsset;

            _petFragmentProgressSlider.maxValue = petFragmentInvetory.RequirementAmount;

            if (petFragmentInvetory.Owned > petFragmentInvetory.RequirementAmount) _petFragmentProgressSlider.ValueTransition(petFragmentInvetory.RequirementAmount);
            else _petFragmentProgressSlider.ValueTransition(petFragmentInvetory.Owned);
           
            _petFragmentProgressText.text = petFragmentInvetory.Owned + "/" + petFragmentInvetory.RequirementAmount;

            CheckState(petFragmentInvetory.Owned >= petFragmentInvetory.RequirementAmount);

            _petName.text = petFragmentInvetory.PetName + "'s Fragment";

            _petCardButton.onClick.RemoveAllListeners();
            _petCardButton.onClick.AddListener(() => OnClickCard.Invoke(_petFragmentInvetory));
        }

        private void CheckState(bool canClaim)
        {
            _highlightBorder.SetActive(canClaim);
            _highlightVFX.SetActive(canClaim);
        }
    }
}