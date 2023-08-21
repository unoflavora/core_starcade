using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.DailyLogin.Data;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles.PostData;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles.Response;
using Agate.Starcade.Scripts.Runtime.Data_Class.Lootbox;
using Agate.Starcade.Scripts.Runtime.OnBoarding;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Backend.Data;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Core;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using HutongGames.PlayMaker.Actions;
using Agate.Starcade.Scripts.Runtime.Game;
using Agate.Starcade.Core.Runtime.Lobby.UserProfile;
using Agate.Starcade.Core.Scripts.Runtime.Lobby.Script.User_Profile;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using Agate.Starcade.Core.Runtime.Pet.Fragment.Model;

namespace Agate.Starcade.Runtime.Backend
{
	public class GameBackendController
    {
        private WebRequestHelper _webRequestHelper;
        private string _baseUrl;

        public GameBackendController()
        {
            _webRequestHelper = new WebRequestHelper(MainSceneController.Instance.GameConfig.Timeout);
            _baseUrl = MainSceneController.Instance.EnvironmentConfig.GameBaseUrl;
        }
        
        #region INIT

        public async Task<GenericResponseData<GameInitData>> GameInit()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Debug.Log("START GAME INIT");
            Debug.Log(MainSceneController.Token);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
            };
            var result = await _webRequestHelper.PostRequest<GameInitData>(_baseUrl + "game/init", header,string.Empty);
            MainSceneController.Instance.Loading.DoneLoading();
            //Debug.Log($"Backend init accessories {result.Data.data.Accessories.UnlockAvatar}");
            return result;
        }

        public async Task<GenericResponseData<LobbyInitData>> LobbyInit()
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token },
            };
            var result = await _webRequestHelper.GetRequest<LobbyInitData>(_baseUrl + "lobby", header);
            //Debug.Log($"Backend init accessories {result.Data.Data.Accessories.UnlockAvatar}"); 
            return result;
        }

        #endregion

        #region TERMS AND CONDITION

        public async Task<GenericResponseData<object>> SignTermsAndCondition()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
            };
            var result = await _webRequestHelper.PostRequest<object>(_baseUrl + "terms-and-condition/sign", header,string.Empty);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        #endregion

        #region ON BOARDING

        public async Task<GenericResponseData<OnBoardingData>> OnBoarding(string Field)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token },
            };
            var result = await _webRequestHelper.GetRequest<OnBoardingData>(_baseUrl + "onboarding/" + Field, header);

            return result;
        }

        public async Task<GenericResponseData<OnBoardingData>> OnBoardingCompletedEvent(OnBoardingStateData onBoardingStateData)
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token },
                { "Content-Type", "application/json" }
            };
            string body = JsonConvert.SerializeObject(onBoardingStateData);
            var result = await _webRequestHelper.PostRequest<OnBoardingData>(_baseUrl + "onboarding/save-state", header, body);
            return result;
        }

        public async Task<GenericResponseData<OnBoardingData>> OnBoardingCompleted()
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token },
            };
            var result = await _webRequestHelper.GetRequest<OnBoardingData>(_baseUrl + "onboarding/completed-fields", header);
            return result;
        }

        #endregion

        #region PROFILE

        public async Task<GenericResponseData<UserProfileData>> GetUserProfileData()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.GetRequest<UserProfileData>(_baseUrl + "user-profile/get-all", header);
            if (result.Error != null)
            {
                Debug.Log("fetch user profile data failed!");
            }
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        public async Task<GenericResponseData<AccessoryItemData>> BuyAccessoryItem(ItemTypeEnum itemType, string itemId)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);

            string item = itemType.ToString().ToLower();

            AccessoryItemRequest buyItemRequest = new AccessoryItemRequest(itemType, itemId);

            var body = JsonConvert.SerializeObject(buyItemRequest);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };

            var result = await _webRequestHelper.PostRequest<AccessoryItemData>(_baseUrl + "user-profile/" + item + "/buy", header, body);
            if (result.Error != null)
            {
                Debug.Log("buy item failed!");
            }
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        public async Task<GenericResponseData<AccessoryItemData>> SetAccessoryItem(ItemTypeEnum itemType, string itemId)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);

            string item = itemType.ToString().ToLower();

            AccessoryItemRequest buyItemRequest = new AccessoryItemRequest(itemType, itemId);

            var body = JsonConvert.SerializeObject(buyItemRequest);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };

            var result = await _webRequestHelper.PutRequest<AccessoryItemData>(_baseUrl + "user-profile/" + item + "/set", header, body);
            if (result.Error != null)
            {
                Debug.Log("buy item failed!");
            }
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        public async Task<GenericResponseData<object>> SetUsername(string username) //TODO FIX
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            UsernameData usernameData = new UsernameData()
            {
                username = username
            };
            var body = JsonConvert.SerializeObject(usernameData);
            var result = await _webRequestHelper.PostRequest<object>(_baseUrl + "user/set-username", header,body);
            Debug.Log("Finish set username");
            if(result.Error != null) Debug.Log("set username error");
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }


        public async Task<GenericResponseData<object>> UnlockAccessory(ItemTypeEnum type, string id)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            AvatarSetData accessoryData = new AvatarSetData()
            {
                AvatarId = id
            };
            var body = JsonConvert.SerializeObject(accessoryData);
            var result = await _webRequestHelper.PostRequest<object>(_baseUrl + $"user-profile/{type.ToString().ToLower()}/unlock", header, body);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }
        
        public async Task<UserGiftDailyLimit> GetGiftDailyLimit()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            
            var result = await _webRequestHelper.GetRequest<UserGiftDailyLimit>(_baseUrl + "friend-gift/collectibles/config", header);
            
            MainSceneController.Instance.Loading.DoneLoading();
            
            if (result.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(result.Error.Code);
            }

            return result.Data;
        }
        
        public async Task<ResetDailyLimitData> ResetGiftLimit()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);

            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            
            var result = await _webRequestHelper.PostRequest<ResetDailyLimitData>(_baseUrl + "friend-gift/collectibles/reset-limit", header, "");
            
            if (result.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(result.Error.Code);
            }
            
            MainSceneController.Instance.Loading.DoneLoading();
            
            return result.Data;
        }

        
        public async Task<SendCollectibleToFriendData> SendCollectible(long friendCode, List<string> collectibleIds)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };

            var bodyData = new SendCollectibleToFriendData()
            {
                CollectibleIds = collectibleIds,
                FriendCode = friendCode
            };
            
            var body = JsonConvert.SerializeObject(bodyData);
            
            Debug.Log(body);

            var res = await _webRequestHelper.PostRequest<SendCollectibleToFriendData>(_baseUrl + "friend-gift/collectibles/send", header, body);
            
            MainSceneController.Instance.Loading.DoneLoading();
            
            if (res.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(res.Error.Code);
            }
            

            return res.Data;
        }


        #endregion

        #region LOBBY

        public async Task<GenericResponseData<CollectCoinData>> CollectCoin()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.PostRequest<CollectCoinData>(_baseUrl + "game/collect-coin", header,String.Empty);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        public async Task<GenericResponseData<ArcadeSessionsData>> GetArcadeSession(string slug)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.GetRequest<ArcadeSessionsData>(_baseUrl + "arcade/" + slug + "/sessions", header);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        public async Task<GenericResponseData<ShopItem[]>> BuyItems(ShopItem cost, ShopItem[] items, UnityAction<BuyItemsData> OnBuySuccess)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            BuyItemsData buyItemsData = new BuyItemsData()
            {
                cost = cost,
                items = items
            };
            var body = JsonConvert.SerializeObject(buyItemsData);
            var result = await _webRequestHelper.PostRequest<ShopItem[]>(_baseUrl + "store/buy-items", header, body);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        public async Task<GenericResponseData<ShopData[]>> GetShopItems()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.GetRequest<ShopData[]>(_baseUrl + "store", header);

            return result;
        }

        public async Task<GenericResponseData<ShopDataBuyResponse>> ShopBuy(string id)
        {
            Debug.Log($"Buy item id = {id}");
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            ShopDataBuyRequest request = new ShopDataBuyRequest()
            {
                itemId = id
            };
            var body = JsonConvert.SerializeObject(request);
            var result = await _webRequestHelper.PostRequest<ShopDataBuyResponse>(_baseUrl + "store/buy-special-items", header, body);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }
        
        public async Task<GenericResponseData<List<LootboxData>>> GetLootboxItems()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.GetRequest<List<LootboxData>>(_baseUrl + "lootbox/get-all", header);

            return result;
        }

        #endregion

        #region REWARD

        public async Task<GenericResponseData<List<RewardData>>> GetRewards()
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
            };
            var result = await _webRequestHelper.GetRequest<List<RewardData>>(_baseUrl + "user-reward", header);
            return result;
        }

        public async Task<GenericResponseData<List<RewardLevelData>>> GetLevelRewards()
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
            };
            var result = await _webRequestHelper.GetRequest<List<RewardLevelData>>(_baseUrl + "level-up", header);
            return result;
        }

        public async Task<GenericResponseData<NextExperienceRewardData>> GetNextMilestoneReward()
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
            };
            var result = await _webRequestHelper.GetRequest<NextExperienceRewardData>(_baseUrl + "level-up/next-milestone", header);
            return result;
        }

        public async Task<GenericResponseData<ClaimRewardData>> ClaimRewards(string rewardId)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Debug.Log("CLAIMING REWARD " + rewardId);
            ClaimRewardRequest request = new ClaimRewardRequest()
            {
                rewardId = rewardId
            };
            var body = JsonConvert.SerializeObject(request);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.PostRequest<ClaimRewardData>(_baseUrl + "user-reward/claim", header,body); 
            return result;
        }

        #endregion

        #region COLLECTIBLES
        public async Task<GenericResponseData<List<CollectibleSetData>>> GetCollectibles()
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
            };

            var result = await _webRequestHelper.GetRequest<List<CollectibleSetData>>(_baseUrl + "collectibles/set", header);
			Debug.Log(JsonConvert.SerializeObject(result));

            return result;
        }

        public async Task<GenericResponseData<CollectiblesCombineResponseData>> CombineCollectible(List<string> items, string setId)
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };

            var bodyData = new CombineCollectiblePostData()
            {
                ItemId = items,
                SetId = setId
            };

            var body = JsonConvert.SerializeObject(bodyData);

            Debug.Log(body);

            var result = await _webRequestHelper.PostRequest<CollectiblesCombineResponseData>(_baseUrl + "user-collectibles/combine", header, body);
            
            return result;
        }
        
        public async Task<GenericResponseData<CollectibleUserRewardData>> ClaimReward (string setId)
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };

            var bodyData = new Dictionary<string, string>();
                
            bodyData["setId"] = setId;

            var body = JsonConvert.SerializeObject(bodyData);
                
            var result = await _webRequestHelper.PostRequest<CollectibleUserRewardData>(_baseUrl + "user-collectibles/claim-reward", header, body);
                
            return result;
        }
        
        public async Task<GenericResponseData<List<CollectibleSetData>>> AddCollectibles()
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
            };
            
            await _webRequestHelper.PostRequest<GenericResponseData<bool>>(_baseUrl + "user-collectibles/add-dummy", header, "");
            var result = await _webRequestHelper.GetRequest<List<CollectibleSetData>>(_baseUrl + "collectibles/set", header);
            Debug.Log(JsonConvert.SerializeObject(result));
            return result;
        }

        #endregion

        #region MAILBOX

        public async Task<GenericResponseData<List<MailboxDataItem>>> GetMails()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.GetRequest<List<MailboxDataItem>>(_baseUrl + "mailbox/list", header);

            return result;
        }

        public async Task<GenericResponseData<MailClaimData>> ClaimMail(string id)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.PostRequest<MailClaimData>(_baseUrl + "mailbox/open-mailbox/" + id, header, String.Empty);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        public async Task<GenericResponseData<MailClaimData[]>> ClaimAllMail(MailboxMenuEnum category)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.PostRequest<MailClaimData[]>(_baseUrl + "mailbox/open-all-mailbox/" + category.ToString().ToLower(), header, String.Empty);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        #endregion

        #region Daily Login

        public async Task<GenericResponseData<DailyLoginData>> GetDailyLoginRewards()
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
            };
            var result = await _webRequestHelper.GetRequest<DailyLoginData>(_baseUrl + "daily-login/", header);

			return result;
        }

        public async Task<GenericResponseData<DailyLoginData>> LeapDayDailyLogin()
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
            };
            var result = await _webRequestHelper.PostRequest<DailyLoginData>(_baseUrl + "daily-login/leap-last-login", header,string.Empty);

            return result;
        }

        #endregion
        
        #region Pets
        public async Task<GenericResponseData<PetBackendData>> GetPetsLibraryData()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            
            var result = await _webRequestHelper.GetRequest<PetBackendData>(_baseUrl + "user-pet/get-all", header);

            return result;
        }
        
        public async Task<GenericResponseData<List<PetInventoryData>>> GetUserPets()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            
            var result = await _webRequestHelper.GetRequest<List<PetInventoryData>>(_baseUrl + "user-pet", header);

            return result;
        }

        public async Task<GenericResponseData<PetFragmentInventory>> AddFragment(PetFragmentAddRequest fragmentAddRequest)
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            var body = JsonConvert.SerializeObject(fragmentAddRequest);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.PostRequest<PetFragmentInventory>(_baseUrl + "pet-fragment/add", header, body);
            return result;
        }

        public async Task<GenericResponseData<CombinePetFragmentResponse>> CombineFragment(CombinePetFragmentRequest combinePetFragmentRequest)
        {
            await MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            var body = JsonConvert.SerializeObject(combinePetFragmentRequest);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.PostRequest<CombinePetFragmentResponse>(_baseUrl + "pet-fragment/combine", header,body);
            return result;
        }
        #endregion

        #region ADVENTURE BOX

        public async Task<GenericResponseData<List<AdventureBoxData>>> GetUserAdventureBox()
        {
            await MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.GetRequest<List<AdventureBoxData>>(_baseUrl + "user-adventure-box", header);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        public async Task<GenericResponseData<AdventureBoxResponse>> AddAdventureBox(AdventureBoxRequest adventureBox)
        {
            await MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            var body = JsonConvert.SerializeObject(adventureBox);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.PostRequest<AdventureBoxResponse>(_baseUrl + "user-adventure-box/add", header, body);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        public async Task<GenericResponseData<OpenAdventureBoxResponse>> OpenAdventureBox(AdventureBoxRequest adventureBox)
        {
            await MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            var body = JsonConvert.SerializeObject(adventureBox);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.PostRequest<OpenAdventureBoxResponse>(_baseUrl + "user-adventure-box/open", header, body);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        public async Task<GenericResponseData<SendAdventureBoxResponse>> SendAdventureBox(SendAdventureBoxRequest sendAdventureBox)
        {
            await MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            var body = JsonConvert.SerializeObject(sendAdventureBox);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await _webRequestHelper.PostRequest<SendAdventureBoxResponse>(_baseUrl + "friend-gift/adventure-box/send", header, body);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        #endregion

        public async Task DownloadPhotoProfile()
        {
            var photoUrl = MainSceneController.Instance.Data.AccessoryData.PhotoURL;

			if (photoUrl == null || photoUrl == "")
            {
                return;
            }
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(MainSceneController.Instance.Data.AccessoryData.PhotoURL);
            www.SendWebRequest();

            while (!www.isDone)
            {
                await Task.Yield();
            }
            
            if (www.result != UnityWebRequest.Result.Success) 
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("download success");
                DownloadHandlerTexture downloadHandlerTexture = (DownloadHandlerTexture)www.downloadHandler;
                Debug.Log("DOWNLOAD PHOTO PROFILE SUCCESS");
                Texture2D texture2D = downloadHandlerTexture.texture;
                Sprite newSprite = Sprite.Create(
                    texture2D, 
                    new Rect(0.0f, 0.0f, texture2D.width, 
                        texture2D.height), 
                    new Vector2(0.5f, 0.5f), 
                    100.0f);
                MainSceneController.Instance.Data.AccessoryData.PhotoUser = newSprite;

                MainSceneController.Instance.Data.UserProfileThirdPartyData.GoogleAvatar = newSprite;

                MainSceneController.Instance.Loading.DoneLoading();
            }
        }
        
       
        public IEnumerator GetPhotoProfile()
        {
            if (MainSceneController.Instance.Data.AccessoryData.PhotoURL == null)
            {
                yield return null;
            }
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(MainSceneController.Instance.Data.AccessoryData.PhotoURL);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) 
            {
                Debug.Log(www.error);
            }
            else
            {
                DownloadHandlerTexture downloadHandlerTexture = (DownloadHandlerTexture)www.downloadHandler;
                Debug.Log("DOWNLOAD PHOTO PROFILE SUCCESS");
                Texture2D texture2D = downloadHandlerTexture.texture;
                Sprite newSprite = Sprite.Create(
                    texture2D, 
                    new Rect(0.0f, 0.0f, texture2D.width, 
                        texture2D.height), 
                    new Vector2(0.5f, 0.5f), 
                    100.0f);
                MainSceneController.Instance.Data.AccessoryData.PhotoUser = newSprite;
            }
        }


        #region FriendList
        public Task<GenericResponseData<FriendsData>> GetFriendList()
        {
            Dictionary<string, string> header = new()
            {
                { "Authorization", "Bearer " + MainSceneController.Token },
                { "Content-Type", "application/json" }
            };

            return _webRequestHelper.GetRequest<FriendsData>(_baseUrl + "friend/list", header);
        }
        
        public Task<GenericResponseData<List<FriendProfile>>> GetFriendRecommendations(string fbToken)
        {
            var body = JsonConvert.SerializeObject(new Dictionary<string, string>()
            {
                {"fbToken", fbToken}
            });
            
            Dictionary<string, string> header = new()
            {
                { "Authorization", "Bearer " + MainSceneController.Token },
                { "Content-Type", "application/json" }
            };

            return _webRequestHelper.PostRequest<List<FriendProfile>>(_baseUrl + "friend/recommended", header, body);
        }
        #endregion
    }
    
    
    
    
}

public class UsernameData
{
    public string username;
}







