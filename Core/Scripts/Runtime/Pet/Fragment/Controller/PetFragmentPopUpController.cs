using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Runtime.Main;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.Fragment.Controller
{
	public class PetFragmentPopUpController : MonoBehaviour
    {
        [SerializeField] private GameObject _petFragmentPopUp;
        [SerializeField] private TMP_Text _petFragmentPopUpTitle;
        [SerializeField] private TMP_Text _petFragmentPopUpCaption;

        [SerializeField] private Image _petFragmentPopUpImage;

        [SerializeField] private Transform _petFragmentPopUpProgressContainer;
        [SerializeField] private Slider _petFragmentPopUpProgressSlider;
        [SerializeField] private TMP_Text _petFragmentPopUpProgressText;

        [SerializeField] private InputNumber _petFragmentInputNumber;

        [SerializeField] private TMP_Text _petFragmentPopUpMessage;

        [SerializeField] private Button _petFragmentSecondaryButton;
        [SerializeField] private TMP_Text _petFragmentSecondaryButtonText;

        [SerializeField] private Button _petFragmentPrimaryButton;
        [SerializeField] private TMP_Text _petFragmentPrimaryButtonText;

        [SerializeField] private Button _petFragmentPopUpCloseButton;

        public InputNumber PetFragmentInputNumber => _petFragmentInputNumber;
        private int _requirmentCost;

        public void Open()
        {
            _petFragmentPopUpCloseButton.onClick.AddListener(Close);
        }

        public async void Close()
        {
            this.gameObject.GetComponent<CanvasTransition>().TriggerFadeOut();
            this.gameObject.GetComponent<CanvasTransition>().TriggerSlideOut();

            _petFragmentSecondaryButton?.onClick.RemoveAllListeners();
            _petFragmentPrimaryButton?.onClick.RemoveAllListeners();
            await Task.Delay(500);

            this.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _petFragmentInputNumber?.ResetInput();
        }

        public void SetupImage(PetFragmentInventory petFragmentInvetory)
        {
            _petFragmentPopUpImage.sprite = MainSceneController.Instance.AssetLibrary.GetPetObject(petFragmentInvetory.PetId).FragmentSpriteAsset;
        }

        public void SetupAction(UnityAction PrimaryAction, UnityAction SecondaryAction)
        {
            Debug.Log("setup action");
            _petFragmentSecondaryButton?.onClick.RemoveAllListeners();
            _petFragmentPrimaryButton?.onClick.RemoveAllListeners();
            _petFragmentPrimaryButton?.onClick.AddListener(PrimaryAction);
            _petFragmentSecondaryButton?.onClick.AddListener(SecondaryAction);
        }

        public void SetupMessage(string message)
        {
            if (_petFragmentPopUpMessage != null) _petFragmentPopUpMessage.text = message;
        }

        public void SetupInputNumber(PetFragmentInventory petFragmentInvetory, int requirmentCost)
        {
            if (_petFragmentInputNumber != null)
            {
                _petFragmentInputNumber.OnIncrease.AddListener(CombineButton);
                _petFragmentInputNumber.OnDecrease.AddListener(CombineButton);

                int maxCombine = (int)Mathf.Floor(petFragmentInvetory.Owned / petFragmentInvetory.RequirementAmount);
                _petFragmentInputNumber.MaxValue = maxCombine > 10 ? 10 : maxCombine;

                _requirmentCost = requirmentCost;
                CombineButton(1);
            }
        }

        public void SetupSlider(PetFragmentInventory petFragmentInvetory)
        {
            if (_petFragmentPopUpProgressContainer != null)
            {
                _petFragmentPopUpProgressSlider.maxValue = petFragmentInvetory.RequirementAmount;
                if (petFragmentInvetory.RequirementAmount < petFragmentInvetory.Owned) _petFragmentPopUpProgressSlider.value = petFragmentInvetory.RequirementAmount;
                else _petFragmentPopUpProgressSlider.value = petFragmentInvetory.Owned;

                _petFragmentPopUpProgressText.text = petFragmentInvetory.Owned + "/" + petFragmentInvetory.RequirementAmount;
            }
        }

        public void SetupTitle(string title, string caption)
        {
            if (_petFragmentPopUpTitle != null) _petFragmentPopUpTitle.text = title;
            if (_petFragmentPopUpCaption != null) _petFragmentPopUpCaption.text = caption;
        }

        private void CombineButton(int val)
        {
            int cost = val * _requirmentCost;
            _petFragmentPrimaryButtonText.text = "Combine (<sprite=0> " + cost + ")";
        }
    }
}