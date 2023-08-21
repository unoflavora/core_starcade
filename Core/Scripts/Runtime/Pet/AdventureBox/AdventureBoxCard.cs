using Agate.Starcade.Core.Runtime.Game;
using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using Agate.Starcade.Runtime.Main;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.AdventureBox
{
	public class AdventureBoxCard : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private TextMeshProUGUI _labelText;
        [SerializeField] private Image _background;

        [SerializeField] private Button _openButton;
        [SerializeField] private Button _sendAsGiftButton;
        [SerializeField] private Button _infoButton;
        [SerializeField] private GameObject[] _vfx;
        [SerializeField] private GameObject _disableScreen;
        [SerializeField] private NotificationBadge _badge;

        [SerializeField] private AdventureBoxData _data;

        public AdventureBoxData Data => _data;

        private bool _isDoneSetupCallback = false;

        public void Start()
        {
            //Setup(_data);
        }

        public void Setup(AdventureBoxData adventureBox)
        {
            _data = adventureBox;

            _disableScreen.gameObject.SetActive(adventureBox.Amount <= 0);

            foreach (var vfx in _vfx)
            {
                vfx.SetActive(adventureBox.Amount > 0);
            }

            if (adventureBox.Amount > 1) { _badge.EnableBadgeWithDuplicated(adventureBox.Amount); }
            else _badge.DisableBadge();

            _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(adventureBox.AdventureBoxId);

            //_labelText.SetTextFromLocalizedId(PetAdventureBoxLocalizations.Table, adventureBox.AdventureBoxId);

            _label.text = adventureBox.Name;
        }

        public void SetupCallback(UnityAction<AdventureBoxData> onOpenCard, UnityAction<AdventureBoxData> onSendGift ,UnityAction<AdventureBoxData> onClickInfo)
        {
            if (_isDoneSetupCallback) return;
            _openButton.onClick.AddListener(() => onOpenCard.Invoke(_data));
            _sendAsGiftButton.onClick.AddListener(() => onSendGift.Invoke(_data));
            _infoButton.onClick.AddListener(() => onClickInfo.Invoke(_data));
            _isDoneSetupCallback = true;
        }
    }
}