using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.House.UI
{
    public class PetConfirmGiftView : MonoBehaviour
    {
        [SerializeField] private Image petImage;
        [SerializeField] private TextMeshProUGUI confirmationText;
        [SerializeField] private Button confirmationButton;
        [SerializeField] private Button cancelButton;
    
        public void DisplayConfirmation(string friendName, PetInventoryData pet, UnityAction onConfirm, UnityAction onCancel)
        {
            petImage.sprite = pet.GetImage();
        
            confirmationText.SetText($"<b>{pet.Name}</b> to <b>{friendName}</b>");
        
            confirmationButton.onClick.AddListener(() =>
            {
                onConfirm();
                OnClose();
            });
        
            cancelButton.onClick.AddListener(() =>
            {
                onCancel();
                OnClose();
            });
        }

        private void OnClose()
        {
            gameObject.SetActive(false);
            cancelButton.onClick.RemoveAllListeners();
            confirmationButton.onClick.RemoveAllListeners();
        }
    }
}
