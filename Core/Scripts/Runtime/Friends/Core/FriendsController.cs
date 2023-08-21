using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Backend;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Backend.Data;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Core.Popups;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Lobby.Friends.UI.Navbar;
using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.Lobby.Friends_Manager.Core;
using UnityEngine;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.Core
{
    public class FriendsController : MonoBehaviour
    {
        [Header("Controllers")]
        [SerializeField] private NavbarFriendsController _navbarFriendsController;
        [SerializeField] private FriendsUIController _friendsUIController;
        [SerializeField] private FriendsPopupController _friendsPopupController;
        [SerializeField] private GameObject _friendsNotification;
        private FriendsBackendController _backendController;
        
        // Modules
        private IAnalyticController _analytic;

        // Data
        private Dictionary<string, Sprite> _friendsPhotosCache;
        
        // State
        private bool _isInitiated;

        private List<FriendProfile> _latestFriendData;
        private int _currentFriend;
        private int _maxFriend;


		private void OnEnable()
        {
            Init();

            _friendsNotification.SetActive(true);

            _friendsUIController.Init(MainSceneController.Instance.Data != null ? MainSceneController.Instance.Data.UserData.FriendCode : 999999);
            
            _navbarFriendsController.OnNavbarValueChangedListener(OnNavbarValueChanged);

            _navbarFriendsController.SetActiveToggle("Friend List");
        }
        private void OnDisable()
        {
            _friendsNotification.SetActive(false);
            
            _friendsUIController.SetFriendListData(_latestFriendData);

            _navbarFriendsController.SetActiveToggle("Friend List", false);
        }
        private void OnDestroy()
        {
            UnsubscribeEvents();
        }
        private static void UnsubscribeEvents()
        {
            FriendsEventBus.Instance.UnsubscribeAll(FriendsEvent.OnSendFriendRequest);
            FriendsEventBus.Instance.UnsubscribeAll(FriendsEvent.OnFriendRequestAction);
            FriendsEventBus.Instance.UnsubscribeAll(FriendsEvent.OnUnfriendFriend);
            FriendsEventBus.Instance.UnsubscribeAll(FriendsEvent.OnSendGiftToFriend);
            FriendsEventBus.Instance.UnsubscribeAll(FriendsEvent.OnFriendStatusChanged);
        }
        private void Init()
        {
            if (_isInitiated) return;

            _isInitiated = true;
            
            _backendController = new FriendsBackendController();

            _analytic = MainSceneController.Instance.Analytic;


			InitListeners();
        }
        private void InitListeners()
        {
            FriendsEventBus.Instance.Subscribe(FriendsEvent.OnSendFriendRequest, OnSendFriendRequest);
            FriendsEventBus.Instance.Subscribe(FriendsEvent.OnFriendRequestAction, OnFriendRequestAction);
            FriendsEventBus.Instance.Subscribe(FriendsEvent.OnUnfriendFriend, OnUnfriendFriend);
            FriendsEventBus.Instance.Subscribe(FriendsEvent.OnSendGiftToFriend, (o) => SendGiftToFriend(o));
            
            _friendsUIController.OnSearchFriend = OnSearchFriend;
            _friendsUIController.OnRefreshClicked = OnRefreshClicked;
        }

        #region Event Handlers
        private async void OnNavbarValueChanged(string panelName)
        {
            await FetchFriendData();
            
            switch (panelName)
            {
                case "Friend List":
                    _analytic.LogEvent(FriendAnalyticEventHandler.ANALYTIC_KEY.CLICK_FRIENDLIST_TAB_EVENT);
                    break;
                case "Request":
                    _analytic.LogEvent(FriendAnalyticEventHandler.ANALYTIC_KEY.CLICK_FRIENDLIST_REQUEST_TAB_EVENT);
                    break;
                default:
                    _analytic.LogEvent(FriendAnalyticEventHandler.ANALYTIC_KEY.CLICK_FRIENDLIST_SEARCH_TAB_EVENT);
                    break;
            }

            
            _currentFriend = MainSceneController.Instance.Data.FriendsData.FriendConfig.TotalFriend;
            
            _maxFriend = MainSceneController.Instance.Data.FriendsData.FriendConfig.MaxFriend;
            
            _friendsUIController.SetFriendCount(_currentFriend, _maxFriend);

            _friendsUIController.SetFriendListData(_latestFriendData);
            
            _friendsUIController.SetActivePanel(panelName);
        }
        private async void OnRefreshClicked()
        {
            await FetchFriendData(true);
            
            _friendsUIController.SetFriendListData(_latestFriendData);
        }
        private void OnUnfriendFriend(object friendData)
        {
            var friend = (CurrentFriendInteractionData) friendData;
            
            var friendID = friend.FriendCode;
            
            var friendName = friend.FriendName;
            
            // Handle unfriend event with friendID
            Debug.Log("Unfriend friendID: " + friendID);
            
            MainSceneController.Instance.Info.Show($"Are you sure you want to remove <b>{friendName}</b> from your friend list?",
                "Remove Friend?", InfoIconTypeEnum.Alert,
                new InfoAction("Confirm", () => RemoveFriend(friendID)),
                new InfoAction("Cancel", null));
        }
        private async void OnSendFriendRequest(object friendId)
        {
            var friendID = (long) friendId;
            
            if (MainSceneController.Instance.Data.FriendsData.FriendConfig.TotalFriend >= MainSceneController.Instance.Data.FriendsData.FriendConfig.MaxFriend)
            {
                MainSceneController.Instance.Info.Show("You have reached the maximum number of friends.", "Max Friend",
                    InfoIconTypeEnum.Alert, new InfoAction("Close", null), null);
                
                return;
            }
            
            var response = await _backendController.SendFriendInteraction(FriendsInteractionType.Add, friendID);
            
            _analytic.LogEvent(FriendAnalyticEventHandler.ANALYTIC_KEY.ADD_FRIEND_EVENT);
            
            FriendsEventBus.Instance.Publish(FriendsEvent.OnFriendStatusChanged, new FriendStatusChangedEventData(friendID, response.status));
        }
        private async void OnSearchFriend(long friendId)
        {
            _analytic.LogEvent(FriendAnalyticEventHandler.ANALYTIC_KEY.SEARCH_FRIEND_EVENT);

            // Handle search friend event with friendID
            var friendData = await _backendController.GetFriendSearch(friendId);

            _friendsPopupController.DisplaySearchFriendResultPopup(friendData);
        }
        private async void OnFriendRequestAction(object friendRequestData)
        {
            var friendData = (FriendRequestEventData) friendRequestData;
            var friendID = friendData.FriendCode;
            var isAccepted = friendData.IsAccept;
            
            // Handle friend request event with friendID and isAccepted
            Debug.Log("Friend request action with friendID: " + friendID + " isAccepted: " + isAccepted);
            
            FriendsPrefManager.RemoveFriendRequestFromPlayerPrefs(friendID);

            FriendServerStatus response;

            var indexAtMain =
                MainSceneController.Instance.Data.FriendsData.Pendings.FindIndex(t =>
                    t.Profile.FriendCode == friendID);
            
            if (isAccepted)
            {
                MainSceneController.Instance.Analytic.LogEvent(FriendAnalyticEventHandler.ANALYTIC_KEY.ACCEPT_FRIEND_REQUEST_EVENT);

                if (MainSceneController.Instance.Data.FriendsData.FriendConfig.TotalFriend >= MainSceneController.Instance.Data.FriendsData.FriendConfig.MaxFriend)
                {
                    MainSceneController.Instance.Info.Show("You have reached the maximum number of friends.", "Max Friend",
                        InfoIconTypeEnum.Alert, new InfoAction("Close", null), null);
                    
                    return;
                }
                
                response = await _backendController.SendFriendInteraction(FriendsInteractionType.Accept, friendID);
                
                MainSceneController.Instance.Data.FriendsData.Pendings[indexAtMain].Profile.Category = FriendCategory.Friend;
                _latestFriendData[_latestFriendData.FindIndex(t => t.FriendCode == friendID)].Category = FriendCategory.Friend;
            }
            else
            { 
                MainSceneController.Instance.Analytic.LogEvent(FriendAnalyticEventHandler.ANALYTIC_KEY.DECLINE_FRIEND_REQUEST_EVENT);

                response = await _backendController.SendFriendInteraction(FriendsInteractionType.Decline, friendID);

                MainSceneController.Instance.Data.FriendsData.Pendings.RemoveAt(indexAtMain);
                _latestFriendData.RemoveAt(_latestFriendData.FindIndex(t => t.FriendCode == friendID));

            }
            
            FriendsEventBus.Instance.Publish(FriendsEvent.OnFriendStatusChanged, new FriendStatusChangedEventData(friendID, response.status));
        }
        private async void RemoveFriend(long friendID)
        {
            try
            {
                var response = await _backendController.SendFriendInteraction(FriendsInteractionType.Unfriend, friendID);
                
                MainSceneController.Instance.Analytic.LogEvent(FriendAnalyticEventHandler.ANALYTIC_KEY.UNFRIEND_EVENT);

                FriendsEventBus.Instance.Publish(FriendsEvent.OnFriendStatusChanged, new FriendStatusChangedEventData(friendID, response.status));

                FriendsPrefManager.RemoveFriendFromPlayerPrefs(friendID);
                
                var indexAtMain = MainSceneController.Instance.Data.FriendsData.Friends.FindIndex(t => t.Profile.FriendCode == friendID);
                
                MainSceneController.Instance.Data.FriendsData.Friends.RemoveAt(indexAtMain);
                
                _latestFriendData.RemoveAt(_latestFriendData.FindIndex(t => t.FriendCode == friendID));

                _currentFriend--;
                
                _friendsUIController.SetFriendCount(_currentFriend, _maxFriend);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            _friendsUIController.SetFriendListData(_latestFriendData);
        }
        #endregion

        #region Data Fetching Methods
        private async Task FetchFriendData(bool useFb = false)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);

            if (useFb)
            {
                var fbToken = await MainSceneController.Instance.Auth.FetchFacebookToken();
                var (friendsData, recommendations) = await _backendController.GetFriends(fbToken);
                MainSceneController.Instance.Data.FriendsData = friendsData;
                MainSceneController.Instance.Data.FriendRecommendations = recommendations;
            }
            else
            {
                var (friendsData, recommendations) = await _backendController.GetFriends("");
                MainSceneController.Instance.Data.FriendsData = friendsData;
                MainSceneController.Instance.Data.FriendRecommendations = recommendations;
            }
            
            MainSceneController.Instance.Loading.DoneLoading();
            
            AggregateData();
        }
        private void AggregateData()
        {
            var friendsData = MainSceneController.Instance.Data.FriendsData;
            
            var recommendations = MainSceneController.Instance.Data.FriendRecommendations;

            // Combine the two sets of data into one list
            List<FriendProfile> combinedList = new List<FriendProfile>();

            foreach (var friend in friendsData.Friends)
            {
                var profile = friend.Profile;
                profile.Category = FriendCategory.Friend;
                combinedList.Add(profile);

                if (FriendsPrefManager.FriendExistsInPlayerPrefs(friend.Profile.FriendCode) == false)
                    profile.IsNew = true;
            }

            foreach (var friend in friendsData.Pendings)
            {
                var profile = friend.Profile;
                profile.Category = FriendCategory.FriendRequest;
                combinedList.Add(profile);

                if (FriendsPrefManager.FriendRequestExistsInPlayerPrefs(friend.Profile.FriendCode) == false)
                    profile.IsNew = true;
            }

            foreach (var recommendation in recommendations)
            {
                recommendation.Category = FriendCategory.Recommendation;
                combinedList.Add(recommendation);
            }

            _latestFriendData = combinedList;
        }
        #endregion
        
        #region Static Methods
        
        // ReSharper disable Unity.PerformanceAnalysis
        private static async Task<UserGiftDailyLimit> OnUserPurchaseLimit(long price, CurrencyTypeEnum currency)
        {
            var playerBalance = currency == CurrencyTypeEnum.GoldCoin
                ? MainSceneController.Instance.Data.UserBalance.GoldCoin
                : currency == CurrencyTypeEnum.StarCoin
                    ? MainSceneController.Instance.Data.UserBalance.StarCoin
                    : MainSceneController.Instance.Data.UserBalance.StarTicket;
            
            if(playerBalance < price)
            {
                MainSceneController.Instance.Info.Show(InfoType.InsufficientBalance,
                    new InfoAction("Go To Store", () => { MainSceneController.Instance.GoToLobby(LobbyMenuEnum.Store); }),
                    new InfoAction("Close", null));
                return null;
            }

            await MainSceneController.Instance.GameBackend.ResetGiftLimit();
            
            var data = await MainSceneController.Instance.GameBackend.GetGiftDailyLimit();

            if (data == null) return null;
            
            MainSceneController.Instance.Analytic.LogEvent(FriendGiftAnalyticEventHandler.ANALYTIC_KEY.RESET_GIVE_COLLECTIBLE_LIMIT);
            
            MainSceneController.Instance.Data.PlayerBalanceActions?.ReduceBalance(currency, price);
            
            MainSceneController.Instance.Info.Show("Your daily limit has been extended by 5", "Limit Extended!",
                InfoIconTypeEnum.Success, new InfoAction("Close", null), null);

            return data;
        }
        public static async void SendGiftToFriend(object friendData, CollectibleItem item = null, Action onBuySuccess = null)
        {
            var friend = (CurrentFriendInteractionData) friendData;
            
            var popupController = FindObjectOfType<FriendsPopupController>();
            popupController.DisablePopup();
            popupController.DisplaySendGiftCollectiblePopup(friend.FriendName, SendGift, item);
            popupController.OnUserPurchaseLimit = async (price, currency) =>
            {
                var currentLimit = await OnUserPurchaseLimit(price, currency);
                if(currentLimit != null)
                    popupController.SetGiftDailyLimit(currentLimit);
            };

            var dailyLimit = await MainSceneController.Instance.GameBackend.GetGiftDailyLimit();
            popupController.SetGiftDailyLimit(dailyLimit);
            
            async void SendGift(List<CollectibleItem> itemsToSend)
            {
                var collectibleIds = itemsToSend.Select(t => t.CollectibleItemId).ToList();
                
                var res = await MainSceneController.Instance.GameBackend.SendCollectible(friend.FriendCode, collectibleIds);
                
                if (res != null)
                {
                    MainSceneController.Instance.Info.Show("Pin has been successfully sent to your friend.", "Pin Sent", InfoIconTypeEnum.Success, new InfoAction("Close", null), null);

                    dailyLimit = await MainSceneController.Instance.GameBackend.GetGiftDailyLimit();
                    
                    popupController.SetGiftDailyLimit(dailyLimit);

                    var collectibleData = await MainSceneController.Instance.GameBackend.GetCollectibles();

                    MainSceneController.Instance.Data.CollectiblesData = collectibleData.Data;

                    popupController.OnBuySuccess();
                    
                    onBuySuccess?.Invoke();

                    try
                    {
						var parameters = new Dictionary<string, object>()
					    {
						    {"total_pin", itemsToSend.Count },
						    {"pins", itemsToSend?.Select(d => new
							    {
								    Id = d.CollectibleItemId,
								    Rarity = d.Rarity
							    }).ToList()
						    },
					    };
					    MainSceneController.Instance.Analytic.LogEvent(FriendGiftAnalyticEventHandler.ANALYTIC_KEY.GIVE_COLLECTIBLE_PIN, parameters);
					}
                    catch { }
					

				}
            }
        }
        #endregion
        
    }
}
