using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Backend.Data;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data_Class.Account;
using Agate.Starcade.Scripts.Runtime.Info;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.Backend
{
    public class FriendsBackendController
    {
        private readonly WebRequestHelper _webRequestHelper;
        private readonly string _baseUrl;
        
        public FriendsBackendController()
        {
            _webRequestHelper = new WebRequestHelper(MainSceneController.Instance.GameConfig.Timeout);
            _baseUrl = MainSceneController.Instance.EnvironmentConfig.GameBaseUrl;
        }

        public async Task<(FriendsData, List<FriendProfile>)> GetFriends(string fbToken)
        {
            // Retrieve user friends and user's received request
            var friendResponse = await RequestHandler.Request(() => MainSceneController.Instance.GameBackend.GetFriendList());
            
            if (friendResponse.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(friendResponse.Error.Code);
            }
            
            
            // Retrieve friend recommendations
            var recommendationResponse = await RequestHandler.Request(() => MainSceneController.Instance.GameBackend.GetFriendRecommendations(fbToken));
            
            if (recommendationResponse.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(recommendationResponse.Error.Code);
            }

            return (friendResponse.Data, recommendationResponse.Data);
        }
        
       
        
        public async Task<UserGiftDailyLimit> GetGiftDailyLimit()
        {
            var friendResponse = await GetGiftLimit();
            
            if (friendResponse.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(friendResponse.Error.Code);
            }

            return friendResponse.Data;
        }

        public async Task<FriendServerStatus> SendFriendInteraction(FriendsInteractionType interactionType, long friendCode)
        {
            var type = Enum.GetName(typeof(FriendsInteractionType), interactionType);
            Debug.Log(type);
            
            var friendResponse = await SendFriendRequest(type, friendCode);
            
            if (friendResponse.Error != null)
            {
                MainSceneController.Instance.Info.Show(
                    title: friendResponse.Error.Message,
                    desc: "", 
                    iconType: InfoIconTypeEnum.Alert, 
                    positiveAction: new InfoAction("OK", null), 
                    negativeAction: null);
            }


            return friendResponse.Data;
        }
        
        public async Task<FriendsResponseData> GetFriendSearch(long friendCode)
        {
            var friendResponse = await SearchFriend(friendCode);
            
            if (friendResponse.Data == null) return null;
            
            return friendResponse.Data;
        }

        public async Task<SendCollectibleToFriendData> SendCollectible(long friendCode, List<string> collectibleIds)
        {
            var res = await SendGiftToFriend(friendCode, collectibleIds);
            
            if (res.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(res.Error.Code);
            }
            

            return res.Data;
        }
        
        public async Task<ResetDailyLimitData> ResetDailyLimit()
        {
            var res = await ResetGiftLimit();
            
            if (res.Error != null)
            {
                if (res.Error.Code == "10401")
                {
                    Debug.LogError("Insufficient Balance!");
                    MainSceneController.Instance.Info.Show(InfoType.InsufficientBalance,
                        new InfoAction("Go To Store", () => { MainSceneController.Instance.GoToLobby(LobbyMenuEnum.Store); }),
                        new InfoAction("Close", () => { }));
                }
                else
                {
                    MainSceneController.Instance.Info.ShowSomethingWrong(res.Error.Code);
                }

                return null;

            }
            

            return res.Data;
        }
        
        
        #region FriendList API Calls
       
        private async Task<GenericResponseData<FriendsResponseData>> SearchFriend(long friendCode)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            
            var header = GetHeader();

            var result = await _webRequestHelper.GetRequest<FriendsResponseData>(_baseUrl + $"friend/search/{friendCode}", header);
            
            MainSceneController.Instance.Loading.DoneLoading();
            
            return result;
        }
        private async Task<GenericResponseData<FriendServerStatus>> SendFriendRequest(string key, long friendCode)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            
            var header = GetHeader();
            
            var bodyData = new Dictionary<string, long>();
            
            bodyData["friendCode"] = friendCode;
            
            var body = JsonConvert.SerializeObject(bodyData);

            var result = await _webRequestHelper.PostRequest<FriendServerStatus>(_baseUrl + $"friend/{key.ToLower()}", header, body);
            
            MainSceneController.Instance.Loading.DoneLoading();
            
            return result;
        }
        
        
        #region Gift API Calls
        private async Task<GenericResponseData<SendCollectibleToFriendData>> SendGiftToFriend(long friendCode, List<string> collectibleIds)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            
            var header = GetHeader();

            var bodyData = new SendCollectibleToFriendData()
            {
                CollectibleIds = collectibleIds,
                FriendCode = friendCode
            };
            
            var body = JsonConvert.SerializeObject(bodyData);
            
            Debug.Log(body);

            var result = await _webRequestHelper.PostRequest<SendCollectibleToFriendData>(_baseUrl + "friend-gift/collectibles/send", header, body);
            
            MainSceneController.Instance.Loading.DoneLoading();
            
            return result;
        }
        private async Task<GenericResponseData<UserGiftDailyLimit>> GetGiftLimit()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            
            var header = GetHeader();

            var result = await _webRequestHelper.GetRequest<UserGiftDailyLimit>(_baseUrl + "friend-gift/collectibles/config", header);
            
            MainSceneController.Instance.Loading.DoneLoading();
            
            return result;
        }

        private async Task<GenericResponseData<ResetDailyLimitData>> ResetGiftLimit()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);

            var header = GetHeader();
            
            var result = await _webRequestHelper.PostRequest<ResetDailyLimitData>(_baseUrl + "friend-gift/collectibles/reset-limit", header, "");
            
            MainSceneController.Instance.Loading.DoneLoading();
            
            return result;
        }

        #endregion
        private static Dictionary<string, string> GetHeader()
        {
            Dictionary<string, string> header = new()
            {
                { "Authorization", "Bearer " + MainSceneController.Token },
                { "Content-Type", "application/json" }
            };
            return header;
        }

        #endregion
    }
}