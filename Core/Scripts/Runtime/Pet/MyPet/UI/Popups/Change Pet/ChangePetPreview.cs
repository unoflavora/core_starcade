using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.UI.Popups.Change_Pet
{
    public class ChangePetPreview : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _petName;
        [SerializeField] private Image _transform;
        [SerializeField] private Button _saveButton;
        [SerializeField] private TMPro.TextMeshProUGUI _petLevel;
        public void DisplayPreview(string petName, int petLevel, Sprite petSprite)
        {
            _petName.text = petName;
            _transform.sprite = petSprite;
            _petLevel.text = "Lv. " + petLevel;
        }
        
        public void SetButtonActive(bool selected)
        {
            _saveButton.interactable = selected;
        }

        public void RegisterOnConfirmChangePet(UnityAction onConfirmPet)
        {
            _saveButton.onClick.RemoveAllListeners();
            _saveButton.onClick.AddListener(onConfirmPet);
        }
    }
}