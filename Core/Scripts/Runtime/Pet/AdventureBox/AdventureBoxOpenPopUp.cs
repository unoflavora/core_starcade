using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Runtime.Main;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.AdventureBox
{
	public class AdventureBoxOpenPopUp : MonoBehaviour
    {
        [SerializeField] private Transform _popUpContainer;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _boxAmount;
        [SerializeField] private InputNumber _inputNumber;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _openBoxButton;

        private AdventureBoxData _data;
        private bool _isActionSet;

        public void Start()
        {
            _closeButton.onClick.AddListener(Close);
        }

        public void Setup(UnityAction<AdventureBoxData, int> openBoxAction)
        {
            SetupAction(openBoxAction);
        }

        public void Show(AdventureBoxData adventureBoxData)
        {
            _popUpContainer.gameObject.SetActive(true);
            _label.text = adventureBoxData.Name;
            _boxAmount.text = adventureBoxData.Amount.ToString();

            _inputNumber.MinValue = 1;
            _inputNumber.MaxValue = adventureBoxData.Amount > 3 ? 3 : adventureBoxData.Amount;

            _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(adventureBoxData.AdventureBoxId);
            _data = adventureBoxData;
        }

        private void SetupAction(UnityAction<AdventureBoxData, int> openBoxAction)
        {
            if (_isActionSet) return;
            _openBoxButton.onClick.AddListener(() => 
            {
                openBoxAction.Invoke(_data, _inputNumber.Value);
                _inputNumber.ResetInput();
            });
            _isActionSet = true;
        }

        private void Close()
        {
            _inputNumber.ResetInput();
            _popUpContainer.gameObject.SetActive(false);
        }

    }
}