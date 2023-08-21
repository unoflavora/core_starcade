using Agate.Starcade.Core.Runtime.Lobby.UserProfile;
using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.Fragment;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Main;
using Assets.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using IngameDebugConsole;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Main
{
    public class MainRequestController
    {
        public enum RequestType
        {
            Mailbox,
            Friend,
            Pet,
            Collectible,
            Store,
            Profile
        }

        private GameBackendController _gameBackend;
        private MainSceneController _main;

        public void Init()
        {
            _main = MainSceneController.Instance;
            _gameBackend = _main.GameBackend;
            InitCheat();
        }

        public async Task FetchMainData()
        {
            try
            {
                MainSceneController.Instance.SetupUser();
                await InitProfileData();
                await InitCollectible();
                await InitPetLibrary();
            }
            catch(Exception error)
            {
                _main.Info.ShowSomethingWrong(error.Message);
                throw error;
            }
        }

        public async Task FetchLobbyData()
        {
            try
            {
                await InitMailbox();
                await InitShop();
                await InitLootbox();
                await InitFriends();
                await InitFacebookModule();
                await InitAdventureBox();
                //await InitUserPet(); -> NOT USED? NEED CONFIRMATION
            }
            catch (Exception error)
            {
                _main.Info.ShowSomethingWrong(error.Message);
            }
        }

        #region Fetch and Init
        //private async Task InitUserPet()
        //{
        //    var userPetResponse = await RequestHandler.Request(async () => await _gameBackend.GetUserPets());
        //    if (userPetResponse.Error != null)
        //    {
        //        throw new Exception(userPetResponse.Error.Code);
        //    }
        //    _main.Data.UserPets = userPetResponse.Data.Select(p => p.PetId).ToList();
        //}

        private async Task InitPetLibrary()
        {
            var petsResponse = await RequestHandler.Request(async () => await _gameBackend.GetPetsLibraryData());
            if (petsResponse.Error != null)
            {
                throw new Exception(petsResponse.Error.Code);
            }

            _main.Data.PetFragment = new PetFragment();
            _main.Data.PetFragment.Inventory = petsResponse.Data.PetFragmentInventory;

            _main.Data.PetInventory = petsResponse.Data.PetInventory;
            _main.Data.PetAlbum = petsResponse.Data.Pets;

            _main.Data.PetConfigs = petsResponse.Data.PetConfigs;
            _main.Data.PetAdventureData = petsResponse.Data.AdventureData;
        }

        private async Task InitFacebookModule()
        {
            var fbToken = await MainSceneController.Instance.Auth.FetchFacebookToken();
            var recommendedFriendReponse = await RequestHandler.Request(async () => await _gameBackend.GetFriendRecommendations(fbToken));
            if (recommendedFriendReponse.Error != null)
            {
                throw new Exception(recommendedFriendReponse.Error.Code);
            }
            _main.Data.FriendRecommendations = recommendedFriendReponse.Data;
        }

        private async Task InitFriends()
        {
            var friendReponse = await RequestHandler.Request(async () => await _gameBackend.GetFriendList());
            if (friendReponse.Error != null)
            {
                throw new Exception(friendReponse.Error.Code);
            }
            _main.Data.FriendsData = friendReponse.Data;
        }

        private async Task InitCollectible()
        {
            var clbResponse = await RequestHandler.Request(async () => await _gameBackend.GetCollectibles());
            if (clbResponse.Error != null)
            {
                throw new Exception(clbResponse.Error.Code);
            }
            _main.Data.CollectiblesData = clbResponse.Data;
        }

        private async Task InitProfileData()
        {
            var userProfileResponse = await RequestHandler.Request(async () => await _gameBackend.GetUserProfileData());
            if (userProfileResponse.Error != null)
            {
                throw new Exception(userProfileResponse.Error.Code);
            }
            _main.Data.UserProfileData = userProfileResponse.Data;
            _main.Data.UserProfileAction = new UserProfileAction();
        }

        private async Task InitLootbox()
        {
            var lbxResponse = await RequestHandler.Request(async () => await _gameBackend.GetLootboxItems());
            if (lbxResponse.Error != null)
            {
                throw new Exception(lbxResponse.Error.Code);
            }
            _main.Data.LootboxData = lbxResponse.Data;
        }

        private async Task InitShop()
        {
            var shopResponse = await RequestHandler.Request(async () => await _gameBackend.GetShopItems());
            if (shopResponse.Error != null)
            {
                throw new Exception(shopResponse.Error.Code);
            }
            _main.Data.ShopData = shopResponse.Data.ToList();
        }

        private async Task InitMailbox()
        {
            var mailResponse = await RequestHandler.Request(async () => await _gameBackend.GetMails());
            if (mailResponse.Error != null)
            {
                throw new Exception(mailResponse.Error.Code);
            }
            _main.Data.MailData = mailResponse.Data;
        }

        private async Task InitAdventureBox()
        {
            var fetchDataResponse = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.GetUserAdventureBox());
            if (fetchDataResponse.Error != null)
            {
                _main.Info.ShowSomethingWrong(fetchDataResponse.Error.Code);
                return;
            }
            _main.Data.AdventureBoxInventory = new AdventureBoxInventory();
            _main.Data.AdventureBoxInventory.Inventory = fetchDataResponse.Data;
        }

        #endregion

        #region Cheat

        private void InitCheat()
        {
            DebugLogConsole.AddCommand<RequestType>("Refresh", "Refresh data", async (RequestType type) => { await Refresh(type); }); ;
        }

        public async Task Refresh(RequestType type)
        {
            try
            {
                switch (type)
                {
                    case RequestType.Mailbox:
                        await InitMailbox();
                        break;
                    case RequestType.Friend:
                        await InitFriends();
                        await InitFacebookModule();
                        break;
                    case RequestType.Pet:
                        await InitPetLibrary();
                        await InitAdventureBox();
                        break;
                    case RequestType.Collectible:
                        await InitCollectible();
                        break;
                    case RequestType.Store:
                        await InitShop();
                        await InitLootbox();
                        break;
                    case RequestType.Profile:
                        await InitProfileData();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Failed to refresh - " + type.ToString() + " - " + ex.Message);
                return;
            }

            Debug.Log("Success refresh data - " + type.ToString());
            MainSceneController.Instance.Loading.DoneLoading();
        }

        #endregion
    }
}