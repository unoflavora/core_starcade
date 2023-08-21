using Agate.Starcade.Core.Runtime.Lobby;
using Agate.Starcade.Scripts.Runtime.Utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Lobby.PetBox
{
    public class PetBoxConfirmationPopUp : ConfirmationPopup
    {
        [SerializeField] private Image _iconImage;

        [SerializeField] private TextMeshProUGUI _iconText;

        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private TextMeshProUGUI _priceCurrencyText;
        public void ShowConfirmation(Sprite icon, string iconTitle, float price, string currency ,Action onConfirmAction, Action onCancelAction = null)
        {
            gameObject.SetActive(true);

            onConfirmAction += () => 
            { 
                gameObject.SetActive(false);
                onConfirmAction = null;
            };
            _onConfirm = onConfirmAction;

            onCancelAction += () => 
            { 
                gameObject.SetActive(false);
                onCancelAction = null;
            };
            _onCancel = onCancelAction;

            _iconImage.sprite = icon;
            _iconText.text = iconTitle;
            _priceText.text = CurrencyHandler.Convert(price);
            _priceCurrencyText.text = currency;
        }
    }
}