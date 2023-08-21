using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Runtime.Main;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.AdventureBox.Controller
{
    public class AdventureBoxConfirmPopUpController : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _caption;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        private FriendProfile _selectedFriend;
        private AdventureBoxData _selectedBox;

        public void SetupCallback(UnityAction<FriendProfile, AdventureBoxData> ConfirmAction, UnityAction CancelAction)
        {
            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.AddListener(() => ConfirmAction.Invoke(_selectedFriend,_selectedBox));
            _cancelButton.onClick.AddListener(() =>
            {
                CancelAction.Invoke();
                this.gameObject.SetActive(false);
            });
        }

        public void OpenConfirm(FriendProfile targetFriend, AdventureBoxData box)
        {
            this.gameObject.SetActive(true);
            _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(box.AdventureBoxId);
            _caption.SetText("<b>" + box.Name + "</b> to <b>" + targetFriend.Username + "</b>");
            _selectedFriend = targetFriend;
            _selectedBox = box;
        }

        public void CloseConfirm()
        {
            this.gameObject.SetActive(false);
        }

    }
}