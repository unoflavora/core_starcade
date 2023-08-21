using Agate.Starcade.Core.Runtime.Lobby.UserProfile;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Backend.Data;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.DailyLogin.Data;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Data_Class.Account;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles;
using Agate.Starcade.Scripts.Runtime.Data_Class.Lootbox;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;
using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using UnityEngine;
using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using Assets.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;
using Agate.Starcade.Core.Runtime.Pet.Fragment;
using Newtonsoft.Json;
using Starcade.Core.Runtime.Lobby.Script.Lootbox.VFX;
using Agate.Starcade.Runtime.Backend;
using UnityEngine.Events;
using System.Threading.Tasks;

namespace Agate.Starcade.Runtime.Main
{
    public class MainModel : MonoBehaviour
    {
        [Header("User Data")]
        public UserData UserData;
        public PlayerBalance UserBalance;
        public UserExperienceData ExperienceData;
        public UserAccessoryData AccessoryData;
        public UserProfileData UserProfileData;
        public UserProfileThirdPartyData UserProfileThirdPartyData;
        public UserProfileAction UserProfileAction;

        [Header("Lobby")]
        public LobbyData LobbyData;
        public LobbyConfigData lobbyConfig;
        public DailyLoginData dailyLoginData;

        [Header("Game")]
        public GameVersionData GameVersionData;
        public TermsAndCondition TermsAndConditionData;

        // Collectibles
        public List<CollectibleSetData> CollectiblesData { get; set; }

        [Header("Friends")]
        public FriendsData FriendsData;
        public List<FriendProfile> FriendRecommendations;
        public List<PetInventoryData> PetInventory { get; set; }
        public PetAdventureData PetAdventureData { get; set; }
        public List<PetAlbumData> PetAlbum { get; set; }
        public PetFragment PetFragment { get; set; }
        public PetConfigs PetConfigs { get; set; }
        public AdventureBoxInventory AdventureBoxInventory { get; set; }

        [Header("Mails")]
        public List<MailboxDataItem> MailData;

        [Header("Store")]
        public List<ShopData> ShopData;
        public List<LootboxData> LootboxData;

        [Header("Poster")]
        public List<PosterData> PosterData;

        [Header("Authentication")]
        public Dictionary<AccountTypesEnum, UserAccountData> UserAccounts;

        [Header("Balance")]
        public PlayerBalanceActions PlayerBalanceActions;
        public Action<List<CollectibleItem>, bool, LootboxRarityEnum, Action> OnLootboxObtained;

        public Action<List<RewardBase>> OnPetBoxObtained;

		public void SetGameInitData(GameInitData gameinitData)
        {
            UserData = gameinitData.data;
            UserBalance = gameinitData.balance;
            LobbyData = gameinitData.lobby;
            lobbyConfig = gameinitData.lobbyConfig;
            GameVersionData = gameinitData.GameVersion;
            TermsAndConditionData = gameinitData.TermsAndCondition;
        }

        public void ProcessReward(RewardBase reward)
        {
            switch (reward.Type)
            {
                case RewardEnum.GoldCoin:
                    PlayerBalanceActions.AddBalance(CurrencyTypeEnum.GoldCoin, reward.Amount);
                    break;
                case RewardEnum.StarCoin:
                    PlayerBalanceActions.AddBalance(CurrencyTypeEnum.StarCoin, reward.Amount);
                    break;
                case RewardEnum.StarTicket:
                    PlayerBalanceActions.AddBalance(CurrencyTypeEnum.StarTicket, reward.Amount);
                    break;
                case RewardEnum.Avatar:
                    Debug.Log("This reward process reward not implemented yet - " + reward.Type.ToString());
                    break;
                case RewardEnum.Frame:
                    Debug.Log("This reward process reward not implemented yet - " + reward.Type.ToString());
                    break;
                case RewardEnum.Lootbox:
                    Debug.Log("This reward process reward not implemented yet - " + reward.Type.ToString());
                    break;
                case RewardEnum.Collectible:
                    CollectiblesController.UpdateCollectibleAmountInModel(reward.Ref.ToString(), (int)reward.Amount);
                    break;
                case RewardEnum.Pet:
                    PetInventory.AddPet(JsonConvert.DeserializeObject<PetInventoryData>(reward.RefObject.ToString()));
                    break;
                case RewardEnum.PetFragment:
                    Debug.Log("disini bro = " + JsonConvert.SerializeObject(reward));
                    PetFragment.AddPetFragment(reward.Ref.ToString(), (int)reward.Amount, DateTime.UtcNow);
                    break;
                case RewardEnum.SpecialBox:
                    AdventureBoxInventory.AddAdventureBox(reward.Ref.ToString(), (int)reward.Amount);
                    break;
                case RewardEnum.PetBox:
                    Debug.Log("This reward process reward not implemented yet - " + reward.Type.ToString());
                    break;
            }
        }

        public void ProcessRewards(RewardBase[] rewards)
        {
            foreach (RewardBase reward in rewards)
            {
                ProcessReward(reward);
            }
        }

        public async Task<ClaimRewardData> ClaimCustomReward(string id, UnityAction onComplete, string title, string desc)
        {
            await MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            var response = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.ClaimRewards(id));
            if (response.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(response.Error.Code);
                return null;
            }
            Debug.Log("CLAIM SUCCESS - " + response.Data.RewardGain[0].Amount);
            var rewards = response.Data.RewardGain;
            MainSceneController.Instance.Data.UserBalance = response.Data.UserBalance;
            MainSceneController.Instance.Data.PlayerBalanceActions.OnBalanceChanged.Invoke(response.Data.UserBalance);
            //_lobbyUI.UpdateBalanceLabel(response.Data.UserBalance);

            //var title = string.Empty;
            //if (response.Data.RewardGain[0] != null) title += "<sprite=0>" + response.Data.RewardGain[0].Amount + "\n";
            //if (response.Data.RewardGain.Count >= 2) title += "<sprite=1>" + response.Data.RewardGain[1].Amount;
            //var desc = "You’re all set to go!\n Please enjoy your rewards";
            //_main.Info.Show(desc, title, InfoIconTypeEnum.Success, new InfoAction("Close", onComplete), null);

            List<RewardBase> items = new List<RewardBase>();
            foreach (var reward in rewards)
            {
                RewardBase item = new RewardBase()
                {
                    Amount = (long)reward.Amount,
                    Ref = null,
                    Type = Enum.Parse<RewardEnum>(reward.Type.ToString()),
                };
                items.Add(item);
            }

            MainSceneController.Instance.Info.ShowReward(title, desc, items.ToArray(), new Scripts.Runtime.Info.InfoAction("Close", onComplete), null);
            MainSceneController.Instance.Loading.DoneLoading();

            return response.Data;
        }
    }
}