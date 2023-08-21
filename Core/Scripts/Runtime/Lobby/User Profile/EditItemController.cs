using Agate.Starcade.Core.Runtime.Lobby.UserProfile;
using Agate.Starcade;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data_Class.Account;
using Agate.Starcade.Scripts.Runtime.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Agate.Starcade.Runtime.Lobby.UserProfile
{
    public class EditItemController : MonoBehaviour
    {
        public const string DefaultAvatar = "AVA_DEFAULT_00";
        public const string DefaultFrame = "FRM_DEFAULT_00";

        [SerializeField] private Image _image;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _lock;
        [SerializeField] private TMP_Text _descLockText;
        [SerializeField] private GameObject _highlight;
        [SerializeField] private GameObject _priceContainer;
        [SerializeField] private TMP_Text _priceTag;
        [SerializeField] private TMP_Text _currencyTag;
        [SerializeField] private TMP_Text _priceConfirmationTag;
        //[SerializeField] private TMP_Text _c
        [SerializeField] private Sprite _emptyDefaultSprite;

        public UnityEvent<string,ItemTypeEnum,EditItemController> OnBuyItem = new UnityEvent<string,ItemTypeEnum,EditItemController>();
        public UnityEvent<string, ItemTypeEnum, EditItemController> OnSelectItem = new UnityEvent<string, ItemTypeEnum, EditItemController>();

        private AccessoryItemData _accessoryItemData;
        private string _itemId;
        private ItemTypeEnum _itemType;
        private AccessoryStatusEnum _accessoryStatusEnum;
        private bool _isOwned;

        private bool _isSetup;

        public AccessoryItemData AccessoryItemData => _accessoryItemData;
        public ItemTypeEnum ItemType => _itemType;

        private bool _isChangingTab;

        private void Start()
        {
            _button.onClick.AddListener(OnItemClick);
        }

        public void Setup(AccessoryItemData accessoryItemData, ItemTypeEnum itemType)
        {
            if (_isSetup) Reset();
            _accessoryItemData = accessoryItemData;
            _itemId = accessoryItemData.Id;
            _itemType = itemType;
            _isOwned = accessoryItemData.IsOwned;
            _accessoryStatusEnum = accessoryItemData.Type;
            _image.sprite = GetSprite(accessoryItemData, itemType);
            SetupLocked(accessoryItemData);
            _isSetup = true;
        }

        private void Reset()
        {
            OnBuyItem.RemoveAllListeners();
            OnSelectItem.RemoveAllListeners();
        }

        public void SetupConfirm(EditItemController editItemController, bool usePriceTag = true)
        {
            _image.sprite = GetSprite(editItemController.AccessoryItemData, editItemController.ItemType);
            _priceContainer.gameObject.SetActive(false);

            if (usePriceTag)
            {
                _priceConfirmationTag.gameObject.SetActive(true);
                switch (editItemController.AccessoryItemData.CurrencyType)
                {
                    case CurrencyTypeEnum.GoldCoin:
                        _priceConfirmationTag.text = "<sprite=0> " + CurrencyHandler.Convert(editItemController.AccessoryItemData.Cost);
                        break;
                    case CurrencyTypeEnum.StarCoin:
                        _priceConfirmationTag.text = "<sprite=1> " + CurrencyHandler.Convert(editItemController.AccessoryItemData.Cost);
                        break;
                    case CurrencyTypeEnum.StarTicket:
                        _priceConfirmationTag.text = "<sprite=2> " + CurrencyHandler.Convert(editItemController.AccessoryItemData.Cost);
                        break;
                }
            }
        }

        public void SetHighlighted(bool active)
        {
            _highlight.gameObject.SetActive(active);
        }


        private Sprite GetSprite(AccessoryItemData accessoryItemData, ItemTypeEnum itemType) 
        {
            if(accessoryItemData.Id == DefaultFrame && itemType == ItemTypeEnum.Frame)
            {
                return _emptyDefaultSprite; //FORCE RETURN EMPTY MARK
            }

            if (accessoryItemData.Id == DefaultAvatar && itemType == ItemTypeEnum.Avatar && MainSceneController.Instance.Data.UserAccounts.ContainsKey(AccountTypesEnum.Google))
            {
                return MainSceneController.Instance.Data.UserProfileThirdPartyData.GoogleAvatar; //FORCE RETURN GOOGLE AVATAR
            }

             return MainSceneController.Instance.AssetLibrary.GetSpriteAsset(accessoryItemData.Id);
        }

        private void SetupLocked(AccessoryItemData accessoryItemData)
        {
            if (accessoryItemData.IsOwned)
            {
                _lock.SetActive(false);
                _priceContainer.SetActive(false);
                return;
            }

            switch (accessoryItemData.Type)
            {
                case AccessoryStatusEnum.Locked:
                    SetLockedItem(accessoryItemData.Caption);
                    break;
                case AccessoryStatusEnum.Premium:
                    SetLockedItem(accessoryItemData.Caption);
                    break;
                case AccessoryStatusEnum.Default:
                    _lock.SetActive(false);
                    _priceContainer.SetActive(false);
                    break;
                case AccessoryStatusEnum.Paid:
                    SetPaidItem(accessoryItemData.CurrencyType, accessoryItemData.Cost);
                    break;
                default:
                    SetLockedItem("error");
                    break;
            }
        }

        private void SetLockedItem(string desc)
        {
            _lock.SetActive(true);
            _descLockText.gameObject.SetActive(true);
            _priceContainer.SetActive(false);
            if (desc == string.Empty || desc == null) _descLockText.gameObject.SetActive(false);
            else _descLockText.text = desc;

        }

        private void SetPaidItem(CurrencyTypeEnum currencyType,float price)
        {
            _lock.SetActive(true);
            _descLockText.gameObject.SetActive(false);
            _priceContainer.SetActive(true);

            switch (currencyType)
            {
                case CurrencyTypeEnum.GoldCoin:
                    _currencyTag.text = "<sprite=0>";
                    break;
                case CurrencyTypeEnum.StarCoin:
                    _currencyTag.text = "<sprite=1>";
                    break;
                case CurrencyTypeEnum.StarTicket:
                    _currencyTag.text = "<sprite=2>";
                    break;
            }

            _priceTag.text = CurrencyHandler.Convert(price);
        }


        #region EVENT

        private void OnItemClick()
        {
            switch (_accessoryStatusEnum)
            {
                case AccessoryStatusEnum.Premium:
                case AccessoryStatusEnum.Locked:
                    if (_isOwned)
                    {
                        OnSelect();
                    }
                    break;
                case AccessoryStatusEnum.Paid:
                    if (_isOwned)
                    {
                        OnSelect();
                    }
                    else
                    {
                        OnBuy();
                    }
                    break;
                case AccessoryStatusEnum.Default:
                    OnSelect();
                    break;
            }
        }

        private void OnSelect()
        {
            OnSelectItem.Invoke(_itemId, _itemType, this);
        }

        private void OnBuy()
        {
            OnBuyItem.Invoke(_itemId,_itemType,this);
        }

        public void OnSuccessBuy()
        {
            _lock.SetActive(false);
            _priceContainer.SetActive(false);
            _accessoryStatusEnum = AccessoryStatusEnum.Default;
        }

        #endregion

        //public void AddButtonListener(UnityAction call)
        //{
        //    _button.onClick.AddListener(call);
        //    _button.onClick.AddListener(() => _button.onClick.RemoveAllListeners());
        //}

        //public void SetImage(Sprite sprite)
        //{
        //    _image.sprite = sprite;
        //}

        //public void RemoveAllListeners()
        //{
        //    _button.onClick.RemoveAllListeners();
        //}

        //public void SetLock(bool IsLocked)
        //{
        //    _lock.SetActive(IsLocked);
        //    _button.interactable = !IsLocked;
        //}

        //public void SetHighlight(bool isHighlight)
        //{
        //    _highlight.SetActive(isHighlight);
        //}
    }
}
