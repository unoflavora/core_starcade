using System;
using Agate.Starcade.Runtime.Enums;
using TMPro;
using UnityEngine;
using Agate.Starcade.Core.Runtime.Lobby;
using Agate.Starcade.Scripts.Runtime.Utilities;

namespace Agate.Starcade.Lobby.Script.UserProfile.FriendsManager.UI.Popups
{
    public class LimitReachedPopup : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _priceText;
        
        private ConfirmationPopup _confirmationPopup;
        
        public Action<bool> IsUserApprove;
        
        private long _price;
        private CurrencyTypeEnum _currencyType;
        public long Price
        {
            get => _price;
        }

        public CurrencyTypeEnum CurrencyType => _currencyType;

        private void Start()
        {
            _confirmationPopup = GetComponent<ConfirmationPopup>();
            
            _confirmationPopup.AddListenerOnCancel(() => OnConfirmation(false), false);

            _confirmationPopup.AddListenerOnConfirmation(() => OnConfirmation(true), false);
        }

        public void SetPriceForAddMoreLimit(CurrencyTypeEnum currencyType, long price)
        {
            _price = price;
            
            _currencyType = currencyType;
            
            var currencyIcon = currencyType == CurrencyTypeEnum.GoldCoin 
                                        ? "<sprite=0>" 
                                        : currencyType == CurrencyTypeEnum.StarCoin ? "<sprite= 1>" 
                                            : "<sprite= 2>" ;
            
            _priceText.SetText($"{currencyIcon} {CurrencyHandler.Convert(price)}");
        }
        
        public void DisplayPopup()
        {
            gameObject.SetActive(true);
        }

        private void OnConfirmation(bool isConfirmed)
        {
            IsUserApprove?.Invoke(isConfirmed);
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
