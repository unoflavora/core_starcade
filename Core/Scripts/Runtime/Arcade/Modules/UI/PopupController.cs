using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules.UI
{
    public class PopupController : MonoBehaviour
    {
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _declineButton;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _popupImages;

        public Button AcceptButton
        {
            get => _acceptButton;
            set => _acceptButton = value;
        }

        public Button DeclineButton
        {
            get => _declineButton;
            set => _declineButton = value;
        }

        public TextMeshProUGUI DescriptionText
        {
            get => _descriptionText;
            set => _descriptionText = value;
        }

        public Image PopupImages
        {
            get => _popupImages;
            set => _popupImages = value;
        }

      
    }
}
