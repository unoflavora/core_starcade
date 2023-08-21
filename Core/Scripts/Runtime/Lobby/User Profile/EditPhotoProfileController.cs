using Agate.Starcade.Core.Runtime.Lobby.UserProfile;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Game;
using Agate.Starcade.Scripts.Runtime.Info;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;
using Agate.Starcade.Core.Runtime.UI;

namespace Agate.Starcade.Runtime.Lobby.UserProfile
{
    public class EditPhotoProfileController : SceneAdditiveBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private EditItemData[] _data;

        [SerializeField] private Sprite _empty;

        [SerializeField] private Image _avatarImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private Button _submitButton;

        [SerializeField] private ScrollablePoolController _scroll;

        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private List<EditItemController> _items;

        [SerializeField] private bool _showLocked;

        private ItemTypeEnum _activeItemTypeTab = ItemTypeEnum.Avatar;

        private AudioController _audio;
        private AddressableController _addressableController;
        private GameBackendController _gameBackend;

        private EditItemData _currentData = null;

        private ItemTypeEnum _currentState = ItemTypeEnum.Avatar;
        private ItemAccessoryData[] _currentPool;
        private EditItemController _currentItem;

        private int _startIndex = 0;
        private int _endIndex = 0;

        [HideInInspector] public UnityEvent<string, Sprite, string, Sprite> OnSubmit;
        private UnityEvent _onProfileEdited;

		private EditUserProfileAnalyticEventHandler _editUserProfileAnalyticEvent { get; set; }

        private List<AccessoryItemData> _activeAvatar;
        private List<AccessoryItemData> _activeFrames;

        private Dictionary<ItemTypeEnum, string> _accessoryChanged = new Dictionary<ItemTypeEnum, string>();

        private EditItemController _selectedItem;

        private bool _isChangingTab;

        private async void Start()
        {
            //OnOpenPanel.AddListener(SetData);
            _submitButton.onClick.AddListener(Submit);
            _addressableController = MainSceneController.Instance.AddressableController;
            _gameBackend = MainSceneController.Instance.GameBackend;
            _audio = MainSceneController.Instance.Audio;
            _editUserProfileAnalyticEvent = new EditUserProfileAnalyticEventHandler(MainSceneController.Instance.Analytic);

            SetupPreview();
            await Init();
            //OnOpen();
        }

        private void SetData()
        {
            _onProfileEdited = LoadSceneHelper.PullEvent();
            LoadSceneHelper.ClearData(); //HOTFIX
        }

        public async Task Init()
        {
            MainSceneController.Instance.Loading.StartTitleLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            await MainSceneController.Instance.AssetLibrary.LoadAllAssets();

            InitItem();
            InitItemButton();
            
            MainSceneController.Instance.Loading.DoneLoading();
            _panel.SetActive(true);
            InitTransition();
        }

        private void InitItem()
        {
            _activeAvatar = MainSceneController.Instance.Data.UserProfileData.Avatars;
            _activeFrames = MainSceneController.Instance.Data.UserProfileData.Frames;
        }

        private void InitItemButton()
        {
            int activeCount = 0;
            List<AccessoryItemData> activeAccessory = new List<AccessoryItemData>();
            string activeId = string.Empty;

            //SET CURRENT ACTIVE TO EDIT ITEM TYPE
            switch (_activeItemTypeTab)
            {
                case ItemTypeEnum.Avatar:
                    activeCount = _activeAvatar.Count;
                    activeAccessory = _activeAvatar;
                    activeId = MainSceneController.Instance.Data.UserProfileData.UsedAvatar;

                    break;
                case ItemTypeEnum.Frame:
                    activeCount = _activeFrames.Count;
                    activeAccessory = _activeFrames;
                    activeId = MainSceneController.Instance.Data.UserProfileData.UsedFrame;
                    break;
            }

            //SIMPLE POOL ITEM BUTTON
            if(_itemContainer.childCount < activeCount)
            {
                int needed = activeCount - _itemContainer.childCount;
                for (int i = 0; i < needed; i++)
                {
                    var temp = Instantiate(_itemPrefab, _itemContainer);
                    temp.SetActive(true);
                    _items.Add(temp.GetComponent<EditItemController>());
                }
            }

            //ASSIGN DATA TO ITEM BUTTON
            for (int i = 0; i < _items.Count; i++)
            {
                if(activeAccessory.Count <= i)
                {
                    _items[i].gameObject.SetActive(false);
                }
                else
                {
                    //if (!_showLocked && !activeAccessory[i].IsOwned)
                    //{
                    //    _items[i].gameObject.SetActive(false);
                    //    continue;
                    //}
                    //else _items[i].gameObject.SetActive(true);

                    _items[i].gameObject.SetActive(true);
                    _items[i].Setup(activeAccessory[i],_activeItemTypeTab);
                    _items[i].OnBuyItem.AddListener(OnBuyItem);
                    _items[i].OnSelectItem.AddListener(OnSelectItem);

                    if (activeAccessory[i].Id == activeId) 
                    {
                        _items[i].SetHighlighted(true);
                        _selectedItem = _items[i];
                    };
                }
            }
        }

        private async void InitTransition()
        {
            foreach(var item in _items)
            {
                item.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            }

            foreach(var item in _items)
            {
                item.gameObject.GetComponent<CanvasTransition>().TriggerTransition();
            }
        }
      
        private void OnSelectItem(string itemId, ItemTypeEnum itemType, EditItemController editItemController)
        {
            _selectedItem.SetHighlighted(false);
            _selectedItem = editItemController;
            _selectedItem.SetHighlighted(true);
            UpdatePreview(itemId, itemType);

			switch (itemType)
			{
				case ItemTypeEnum.Avatar:
					_editUserProfileAnalyticEvent.TrackEditEditProfileAvatarItemEvent(itemId);

					break;
				case ItemTypeEnum.Frame:
					_editUserProfileAnalyticEvent.TrackClickEditProfileFrameItemEvent(itemId);
					break;
			}
		}

        private async void OnBuyItem(string itemId,ItemTypeEnum itemType,EditItemController editItemController)
        {
            var icon = Instantiate(_itemPrefab);
            icon.GetComponent<EditItemController>().SetupConfirm(editItemController);
            icon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
            icon.GetComponent<CanvasTransition>().TriggerTransition();

            MainSceneController.Instance.Info.ShowConfirmation(new TitleLabel("Confirm Purchase", "Do you want to purchase this avatar?"), icon, () =>
            {
                ConfirmBuy(itemId,itemType,editItemController);
            });
        }

        private async void ConfirmBuy(string itemId, ItemTypeEnum itemType, EditItemController editItemController)
        {
            var buyItemResponse = await RequestHandler.Request(async () => await _gameBackend.BuyAccessoryItem(itemType, itemId));

            if (buyItemResponse.Error != null)
            {
                if(buyItemResponse.Error.Code == "10401")
                {
                    MainSceneController.Instance.Info.Show(InfoType.InsufficientBalance, new InfoAction("Close", null),null);
                }
                return;
            }

            var dataResponse = buyItemResponse.Data;

			switch (itemType)
            {
                case ItemTypeEnum.Avatar:
                    var avatar = _activeAvatar.Find(x => x.Id == itemId);
                    avatar.Type = AccessoryStatusEnum.Default;
                    avatar.IsOwned = true;

					_editUserProfileAnalyticEvent.TrackBuyAvatarEvent(new EditUserProfileAnalyticEventHandler.BuyAccessoriesParameters()
                    {
                        ItemId = itemId,
                        Currency = dataResponse.CurrencyType.ToString(),
                        Cost = dataResponse.Cost
                    });

					break;
                case ItemTypeEnum.Frame:
                    var frame = _activeFrames.Find(x => x.Id == itemId);
                    frame.Type = AccessoryStatusEnum.Default;
                    frame.IsOwned = true;

					_editUserProfileAnalyticEvent.TrackBuyFrameEvent(new EditUserProfileAnalyticEventHandler.BuyAccessoriesParameters()
					{
						ItemId = itemId,
						Currency = dataResponse.CurrencyType.ToString(),
						Cost = dataResponse.Cost
					});
					break;
            }

            var icon = Instantiate(_itemPrefab);
            icon.GetComponent<EditItemController>().SetupConfirm(editItemController,false);
            icon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
            icon.GetComponent<CanvasTransition>().TriggerTransition();

            MainSceneController.Instance.Info.ShowSuccessConfirmation(new TitleLabel("Purchase Successful!", "New avatar has been added to your collection."), icon);
            MainSceneController.Instance.Data.PlayerBalanceActions.ReduceBalance(buyItemResponse.Data.CurrencyType, buyItemResponse.Data.Cost);

            editItemController.OnSuccessBuy();
        }

        public async void ChangeTabState(ItemTypeEnum state)
        {
            _isChangingTab = true;
            //_currentState = state;
            //_currentItem = null;
            //SetCurrentContent();
            _selectedItem.SetHighlighted(false);

            switch (state)
            {
                case ItemTypeEnum.Avatar:
                    _activeItemTypeTab = ItemTypeEnum.Avatar;
                    break;
                case ItemTypeEnum.Frame:
                    _activeItemTypeTab = ItemTypeEnum.Frame;
                    break;
                default:
                    break;
            }

            await Init();

            //_scroll.ResetPosition(_currentData.Data.Length);
            //_scroll.ResetScroll(ScrollablePoolController.ScrollDirection.Down);
            //_startIndex = 0;
            //_endIndex = 0;
            //_scroll.OnResetScroll.Invoke(true);

            switch (state)
            {
                case ItemTypeEnum.Avatar:
                    _editUserProfileAnalyticEvent.TrackClickEditProfileAvatarTabEvent();
                    break;
				case ItemTypeEnum.Frame:
					_editUserProfileAnalyticEvent.TrackClickEditProfileFrameTabEvent();
					break;
			}
        }

        private void SetupPreview()
        {
            _avatarImage.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentAvatar();
            _frameImage.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentFrame();
        }

        private void UpdatePreview(string itemId, ItemTypeEnum itemType)
        {
            switch (_activeItemTypeTab)
            {
                case ItemTypeEnum.Avatar:
                    _avatarImage.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(itemId);
                    break;
                case ItemTypeEnum.Frame:
                    _frameImage.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(itemId);
                    break;
                default:
                    break;
            }

            if (_accessoryChanged.ContainsKey(itemType)) _accessoryChanged[itemType] = itemId;
            else _accessoryChanged.Add(itemType, itemId);
        }

        private async void Submit()
        {
            if (_accessoryChanged.ContainsKey(ItemTypeEnum.Avatar))
            {
				string avatarId = _accessoryChanged[ItemTypeEnum.Avatar];
				var setAvatarItemResponse = await RequestHandler.Request(async () => await _gameBackend.SetAccessoryItem(ItemTypeEnum.Avatar, avatarId));

                if (setAvatarItemResponse.Error != null)
                {
                    //ERROR BUY HERE
                    return;
                }
                _editUserProfileAnalyticEvent.TrackSetAvatarItemEvent(avatarId);

                MainSceneController.Instance.Data.UserProfileData.UsedAvatar = avatarId;
            }
            if (_accessoryChanged.ContainsKey(ItemTypeEnum.Frame))
            {
				string frameId = _accessoryChanged[ItemTypeEnum.Frame];
				var setFrameItemResponse = await RequestHandler.Request(async () => await _gameBackend.SetAccessoryItem(ItemTypeEnum.Frame, frameId));

                if (setFrameItemResponse.Error != null)
                {
                    //ERROR BUY HERE
                    return;
                }
				_editUserProfileAnalyticEvent.TrackSetFrameItemEvent(frameId);

				MainSceneController.Instance.Data.UserProfileData.UsedFrame = frameId;
            }

			_editUserProfileAnalyticEvent.TrackSaveEditProfileEvent(MainSceneController.Instance.Data.UserProfileData.UsedAvatar, MainSceneController.Instance.Data.UserProfileData.UsedFrame);
			MainSceneController.Instance.Data.UserProfileAction.OnAccessoryChanged?.Invoke(_accessoryChanged);
            ClosePanel();
        }

        //private async void OnEditProfileAccessory(string avatarId, Sprite avatarSprite, string frameId, Sprite frameSprite)
        //{
        //    if (avatarId != "") await SubmitAvatarAccessory(ItemTypeEnum.Avatar, avatarId, avatarSprite);
        //    if (frameId != "") await SubmitFrameAccessory(ItemTypeEnum.Frame, frameId, frameSprite);
        //    LoadSceneHelper.CloseSceneAdditive();
        //}

        //private async Task SubmitAvatarAccessory(ItemTypeEnum type, string id, Sprite sprite)
        //{
        //    var response = await _gameBackend.SetAvatarAccessory(type, id);
        //    if (response.Error != null)
        //    {
        //        MainSceneController.Instance.Info.ShowSomethingWrong(response.Error.Code);
        //    }
        //    else
        //    {
        //        MainSceneController.Instance.Data.AccessoryData.UserAccessories[type].Data = sprite;
        //        MainSceneController.Instance.Data.AccessoryData.UserAccessories[type].Id = id;
        //        MainSceneController.Instance.Data.AccessoryData.UserAccessories[type].OnAccessoryChanged.Invoke();
        //    }
        //}

        //private async Task SubmitFrameAccessory(ItemTypeEnum type, string id, Sprite sprite)
        //{
        //    var response = await _gameBackend.SetFrameAccessory(type, id);
        //    if (response.Error != null)
        //    {
        //        MainSceneController.Instance.Info.ShowSomethingWrong(response.Error.Code);
        //    }
        //    else
        //    {
        //        MainSceneController.Instance.Data.AccessoryData.UserAccessories[type].Data = sprite;
        //        MainSceneController.Instance.Data.AccessoryData.UserAccessories[type].Id = id;
        //        MainSceneController.Instance.Data.AccessoryData.UserAccessories[type].OnAccessoryChanged.Invoke();
        //    }
        //}

        //private ItemAccessoryData CreateItem(string id, ItemTypeEnum type, bool isLocked, AssetReference reference)
        //{
        //    return new ItemAccessoryData()
        //    {
        //        Id = id,
        //        Type = type,
        //        IsLocked = isLocked,
        //        IsLoaded = false,
        //        UseNullData = false,
        //        Reference = reference
        //    };
        //}

        //private bool IsLocked(ItemTypeEnum state, string id)
        //{
        //    bool isLocked = true;
        //    string[] unlockedList = null;
        //    if (state == ItemTypeEnum.Avatar)
        //    {
        //        unlockedList = MainSceneController.Instance.Data.AccessoryData.AvatarLibrary.UnlockedItems.ToArray();
        //        //string[] defaultIds = MainSceneLauncher.Instance.MainModel.AccessoryData.AvatarLibrary.DefaultItemIds;
        //        //unlockedList = unlockedList.Concat(defaultIds).ToArray();
        //    }
        //    else if (state == ItemTypeEnum.Frame)
        //    {
        //        unlockedList = MainSceneController.Instance.Data.AccessoryData.FrameLibrary.UnlockedItems.ToArray();
        //        //string[] defaultIds = MainSceneLauncher.Instance.MainModel.AccessoryData.FrameLibrary.DefaultItemIds;
        //        //unlockedList = unlockedList.Concat(defaultIds).ToArray();
        //    }
        //    if (unlockedList == null) return isLocked;
        //    foreach (string unlockedId in unlockedList)
        //    {
        //        if (id == unlockedId) isLocked = false;
        //    }
        //    return isLocked;
        //}

        //private bool IsHighlighted(ItemTypeEnum type, string id)
        //{

        //    string setId = "";
        //    if (type == ItemTypeEnum.Avatar) setId = _data[0].Id;
        //    else if (type == ItemTypeEnum.Frame) setId = _data[1].Id;
        //    if (setId == "")
        //    {
        //        bool isActive = MainSceneController.Instance.Data.AccessoryData.UserAccessories[type].Id == id;
        //        return isActive;
        //    }
        //    bool isSet = setId == id;
        //    return isSet;
        //}

        //private ItemAccessoryData FindAccessory(string id)
        //{
        //    ItemAccessoryData data = null;
        //    for (int i = 0; i < _currentData.Data.Length; i++)
        //    {
        //        if (_currentData.Data[i].Id == id)
        //        {
        //            data = _currentData.Data[i];
        //            break;
        //        }
        //    }
        //    return data;
        //}

        //private void SetCurrentContent()
        //{
        //    foreach (EditItemData item in _data)
        //    {
        //        if (item.Type == _currentState)
        //        {
        //            _currentData = item;
        //        }
        //    }
        //    foreach (var item in _currentData.Data)
        //    {
        //        item.IsHighlighted = IsHighlighted(_currentState, item.Id);
        //    }
        //}

        //private int LimitIndex(int index, int limit)
        //{
        //    if (index > limit) return limit;
        //    else if (index < 0) return 0;
        //    else return index;
        //}


        //private void InitScrollable()
        //{
        //    _scroll.Init(0);
        //    _scroll.OnResetScroll.AddListener(OnResetScroll);
        //    _scroll.OnItemActivated.AddListener((index, gameObject) =>
        //    {
        //        EditItemController editItemController = gameObject.GetComponent<EditItemController>();
        //        if (editItemController == null || _currentPool[index] == null) return;

        //        editItemController.RemoveAllListeners();
        //        editItemController.SetImage((Sprite)_currentPool[index].Data);
        //        string id = _currentPool[index].Id;
        //        editItemController.AddButtonListener(() => OnItemClicked(id, editItemController));
        //        editItemController.SetLock(_currentPool[index].IsLocked);
        //        editItemController.SetHighlight(_currentPool[index].IsHighlighted);
        //        if (_currentPool[index].IsHighlighted) _currentItem = editItemController;
        //        gameObject.SetActive(true);
        //    });
        //    _scroll.OnItemDeactivated.AddListener((index, gameObject) =>
        //    {
        //        EditItemController editItemController = gameObject.GetComponent<EditItemController>();
        //        if (editItemController == null) return;

        //        editItemController.RemoveAllListeners();
        //    });

        //    _scroll.ResetPosition(_currentData.Data.Length);
        //    _scroll.ResetScroll(ScrollablePoolController.ScrollDirection.Down);
        //}

        //private void InitScrollablee()
        //{
        //    _scroll.Init(0);
        //    _scroll.OnResetScroll.AddListener(OnResetScroll);
        //    _scroll.OnItemActivated.AddListener((index, gameObject) =>
        //    {
        //        EditItemController editItemController = gameObject.GetComponent<EditItemController>();
        //        if (editItemController == null || _currentPool[index] == null) return;

        //        editItemController.RemoveAllListeners();

        //        editItemController.SetImage((Sprite)_currentPool[index].Data); //SET IMAGE
        //        string id = _currentPool[index].Id; //SET ID
        //        editItemController.AddButtonListener(() => OnItemClicked(id, editItemController)); //SET EVENT WHEN CLICKED
        //        editItemController.SetLock(_currentPool[index].IsLocked); //SET LOCKED STATUS
        //        editItemController.SetHighlight(_currentPool[index].IsHighlighted);//SET HIGHLIGHT

        //        if (_currentPool[index].IsHighlighted) _currentItem = editItemController;
        //        gameObject.SetActive(true);
        //    });
        //}

        //      private void OnItemClicked(string id, EditItemController itemController)
        //      {
        //          MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.BUTTON_GENERAL);
        //          _currentData.Id = id;
        //          ItemAccessoryData item = FindAccessory(_currentData.Id);
        //          Sprite sprite = item.UseNullData ? null : (Sprite)FindAccessory((string)id).Data;
        //          _currentData.PreviewImage.sprite = sprite;
        //          if (_currentItem != null) _currentItem.SetHighlight(false);
        //          _currentItem = itemController;
        //          _currentItem.SetHighlight(true);
        //          if (_currentData.Type == ItemTypeEnum.Frame && sprite == null)
        //          {
        //		_currentData.PreviewImage.gameObject.SetActive(false);
        //	} else
        //          {
        //		_currentData.PreviewImage.gameObject.SetActive(true);
        //	}

        //	switch (_currentState)
        //	{
        //		case ItemTypeEnum.Avatar:
        //			_editUserProfileAnalyticEvent.TrackSelectEditProfileAvatarItemEvent(id);
        //			break;
        //		case ItemTypeEnum.Frame:
        //			_editUserProfileAnalyticEvent.TrackSelectEditProfileFrameItemEvent(id);
        //			break;
        //	}
        //}

        //private void OnResetScroll(ScrollablePoolController.ScrollDirection direction, int startIndex, int endIndex)
        //{
        //    ItemAccessoryData[] pool = new ItemAccessoryData[endIndex - startIndex];
        //    for (int i = startIndex; i < endIndex; i++)
        //    {
        //        pool[i - startIndex] = _currentData.Data[i];
        //    }
        //    _currentPool = pool;

        //}

        //private void OnEdit()
        //{
        //    gameObject.SetActive(false);
        //    //OnSubmit.Invoke(_data[0].Id, _data[0].PreviewImage.sprite, _data[1].Id, _data[1].PreviewImage.sprite);
        //    OnEditProfileAccessory(_data[0].Id, _data[0].PreviewImage.sprite, _data[1].Id, _data[1].PreviewImage.sprite);
        //    _editUserProfileAnalyticEvent.TrackSaveEditProfileEvent(_data[0].Id, _data[1].Id);
        //}

        //public void OnOpen()
        //{
        //    _data[0].PreviewImage.sprite = (Sprite) MainSceneController.Instance.Data.AccessoryData.UserAccessories[ItemTypeEnum.Avatar].Data;
        //    if (MainSceneController.Instance.Data.AccessoryData.UserAccessories[ItemTypeEnum.Frame].Data != null)
        //    {
        //        _data[1].PreviewImage.sprite = (Sprite)MainSceneController.Instance.Data.AccessoryData.UserAccessories[ItemTypeEnum.Frame].Data;
        //        _data[1].PreviewImage.gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        _data[1].PreviewImage.gameObject.SetActive(false);
        //    }
        //    gameObject.SetActive(true);
        //}

        //private void InitAvatar()
        //{
        //    InitItem(ItemTypeEnum.Avatar, _data[0]);
        //    if (MainSceneController.Instance.Data.AccessoryData.PhotoUser != null)
        //        _data[0].Data[0].Data = MainSceneController.Instance.Data.AccessoryData.PhotoUser;
        //}

        //private void InitFrame()
        //{
        //    InitItem(ItemTypeEnum.Frame, _data[1]);
        //    _data[1].Data[0].UseNullData = true;
        //}

        //INIT LIST ITEM NEED TO CREATED OLD WAY
        //private void InitItem(ItemTypeEnum type, EditItemData data)
        //{
        //    ItemSO accessoryLibrary = MainSceneController.Instance.AssetLibrary.FindItemSO(type);
        //    data.Data = new ItemAccessoryData[accessoryLibrary.Data.Length];
        //    List<ItemAccessoryData> unlockedList = new List<ItemAccessoryData>();
        //    List<ItemAccessoryData> lockedList = new List<ItemAccessoryData>();
        //    for (int i = 0; i < accessoryLibrary.Data.Length; i++)
        //    {
        //        ItemData item = accessoryLibrary.Data[i];
        //        ItemAccessoryData accessory = CreateItem(item.Id, data.Type, false, item.Reference);
        //        accessory.Data = MainSceneController.Instance.AssetLibrary.GetAsset(item.Id);
        //        //data.Data[i].IsLocked = false;
        //        accessory.IsLocked = IsLocked(data.Type, item.Id);
        //        accessory.IsHighlighted = IsHighlighted(data.Type, item.Id);
        //        if (accessory.IsLocked) lockedList.Add(accessory);
        //        else unlockedList.Add(accessory);
        //    }
        //    unlockedList.AddRange(lockedList);
        //    data.Data = unlockedList.ToArray();
        //}


        //public async Task Init()
        //{
        //    _addressableController = MainSceneController.Instance.AddressableController;
        //    _gameBackend = MainSceneController.Instance.GameBackend;
        //    _audio = MainSceneController.Instance.Audio;

        //    _editUserProfileAnalyticEvent = new EditUserProfileAnalyticEventHandler(MainSceneController.Instance.Analytic);

        //    await MainSceneController.Instance.AssetLibrary.LoadAllAssets();

        //    _submitButton.onClick.AddListener(OnEdit);
        //    MainSceneController.Instance.Loading.StartTitleLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
        //    InitAvatar();
        //    InitFrame();
        //    SetCurrentContent();
        //    InitScrollable();
        //    MainSceneController.Instance.Loading.DoneLoading();
        //    _panel.SetActive(true);
        //}
    }
}
