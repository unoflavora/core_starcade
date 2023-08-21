using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Arcade.Modules.UI;
using TMPro;
using UnityEngine.Events;

namespace Starcade.Core.Scripts.Runtime.Arcade.Modules.UI
{
    
    // TODO: Pindahin ini ke Plinko Module
    // CUMA DIPAKE DI PLINKO
    public class ErrorPopupHandler : PopupController
    {
        private UnityEvent onButtonTriggered { get; set; } = new UnityEvent();
        private AudioController _audioController;

        private void Awake()
        {
            _audioController = MainSceneController.Instance.Audio;
        }

        private void AcceptAction()
        {
            gameObject.SetActive(false);
            onButtonTriggered?.Invoke();
            onButtonTriggered?.RemoveAllListeners();
        }

        public void ActivatePopup(string description, string buttonAction, UnityAction action)
        {
            AcceptButton.onClick.AddListener(AcceptAction);
            AcceptButton.onClick.AddListener(() => AcceptButton.onClick.RemoveListener(AcceptAction));

            DescriptionText.text = description;
            if (action != null)
            {
                onButtonTriggered.AddListener(action);  
            }
            AcceptButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonAction;
            gameObject.SetActive(true);
        }

    }
}
