using Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet;
using Agate.Starcade.Core.Runtime.Lobby.Lootbox;
using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Enum;
using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.Core.Popups;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Assets.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using IngameDebugConsole;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet.AdventureBoxAnalyticEventHandler;

namespace Agate.Starcade.Core.Runtime.Pet.AdventureBox.Controller
{
	public class AdventureBoxController : MonoBehaviour
    {
        private static Dictionary<string, AdventureBoxTypeEnum> AdventureBoxTypeMap = new Dictionary<string, AdventureBoxTypeEnum>
        {
            { "abx_common", AdventureBoxTypeEnum.Common },
            { "abx_rare", AdventureBoxTypeEnum.Rare },
            { "abx_epic", AdventureBoxTypeEnum.Epic },
            { "abx_legendary", AdventureBoxTypeEnum.Legendary }
        };

        public static string ADVENTURE_BOX_OPEN_BOTTOM_TYPE = "adventure_box_bottom_type";
        public static string ADVENTURE_BOX_OPEN_UPPER_TYPE = "adventure_box_upper_type";

        public static string ADVENTURE_BOX_RESULT = "adventure_box_result";
        public static string ADVENTURE_BOX_GIFT = "adventure_box_gift";

        [SerializeField] private Canvas _adventureBoxCanvas;
        [SerializeField] private List<AdventureBoxCard> _adventureBoxCards;
        [SerializeField] private AdventureBoxVFX _vfx;

        [Header("POP UP")]
        [SerializeField] private AdventureBoxOpenPopUp _adventureBoxOpenPopUp;
        [SerializeField] private LootboxInfoUI _adventureBoxInfoPopUp;
        [SerializeField] private AdventureBoxConfirmPopUpController _adventureBoxConfirmPopUp;
        [SerializeField] private PinGiftFriendListController _giftFriendController;

        [Header("DEBUG MODE")]
        [SerializeField] private bool _isDebugging;
        [SerializeField] private bool _useCheat;
        [SerializeField] private SpecialBoxConfig _debugConfig;

        [SerializeField] private List<AdventureBoxData> _dummyAdventureBoxData;

        private AdventureBoxInventory _adventureBoxInventory; 

        private SpecialBoxConfig _config;
        private List<AdventureBoxData> _adventureBoxDatas = new List<AdventureBoxData>();

        private AdventureBoxData _selectedBox;

        private AdventureBoxAnalyticEventHandler _analytic;

        private async void Start()
        {
            _adventureBoxCanvas.worldCamera = Camera.main;

            _analytic = new AdventureBoxAnalyticEventHandler(MainSceneController.Instance.Analytic);

            //await MainSceneController.Instance.AssetLibrary.LoadAssets(ItemTypeEnum.LootBoxChest);
            //await MainSceneController.Instance.AssetLibrary.LoadAssets(ItemTypeEnum.Reward);
            //await MainSceneController.Instance.AssetLibrary.LoadAssets(ItemTypeEnum.Currency);
            //await MainSceneController.Instance.AssetLibrary.LoadAssets(ItemTypeEnum.Pets);
            //MainSceneController.Instance.Localizations.LoadTable(PetAdventureBoxLocalizations.Table);


            FetchData();

            AssignData();
            AssignCallback();
            SetupPopUp();

            if (_useCheat) ActivateCheat();

            await TriggerTransition();
            await MainSceneController.Instance.Audio.LoadAudioData("adventure_box_audio");
        }

        #region UI HANDLER
        private async Task TriggerTransition()
        {
            foreach (var card in _adventureBoxCards)
            {
                card.gameObject.GetComponent<CanvasTransition>().TriggerTransition();
                await Task.Delay(150);
            }
        }

        private void TriggerFadeOutTransition()
        {
            foreach (var card in _adventureBoxCards)
            {
                card.gameObject.GetComponent<CanvasTransition>().TriggerFadeOut();
            }
        } 
        #endregion

        #region VIEW HANDLER
        private void AssignData()
        {
            _adventureBoxDatas.Sort((x, y) => x.Tier.CompareTo(y.Tier));

            for (int i = 0; i < _adventureBoxCards.Count; i++)
            {
                _adventureBoxCards[i].Setup(_adventureBoxDatas[i]);
            }
        }

        private void AssignCallback()
        {
            for (int i = 0; i < _adventureBoxCards.Count; i++)
            {
                _adventureBoxCards[i].SetupCallback(ShowOpenAdventureBoxPopUp, ShowSendAdventureBoxPopUp, ShowInfoAdventureBoxPopUp);
            }
        } 
        #endregion

        #region POP UP HANDLER
        private void SetupPopUp()
        {
            _adventureBoxOpenPopUp.Setup(OpenBox);
            _adventureBoxConfirmPopUp.SetupCallback(SendBox, () => ShowSendAdventureBoxPopUp(_selectedBox));
        }

        private void ShowOpenAdventureBoxPopUp(AdventureBoxData adventureBoxData)
        {
            _adventureBoxOpenPopUp.Show(adventureBoxData);
            OpenAdventureBoxSfx(GetAdventureBoxTypeEnum(adventureBoxData.AdventureBoxId));
            _analytic.TrackClickOpenAdventureBoxButtonEvent(adventureBoxData.AdventureBoxId);
        }

        private void ShowSendAdventureBoxPopUp(AdventureBoxData adventureBoxData)
        {
            _selectedBox = adventureBoxData;
            _giftFriendController.gameObject.SetActive(true);
            _giftFriendController.DisplayFriends(ShowConfirmPopUp, null);
            _analytic.TrackClickGiveAdventureBoxButtonEvent(adventureBoxData.AdventureBoxId);
        }

        private void ShowConfirmPopUp(FriendProfile targetFriend)
        {
            _adventureBoxConfirmPopUp.OpenConfirm(targetFriend, _selectedBox);
        }

        private void ShowInfoAdventureBoxPopUp(AdventureBoxData adventureBoxData)
        {
            OpenAdventureBoxSfx(GetAdventureBoxTypeEnum(adventureBoxData.AdventureBoxId));
            _adventureBoxInfoPopUp.ShowSpecialBoxInfo(adventureBoxData);
        }
        #endregion

        private async Task UpdateAdventureBox(List<AdventureBoxData> addBoxResponse, bool transition = true)
        {
            _adventureBoxDatas = addBoxResponse;
            AssignData();
            AssignCallback();
            SetupPopUp();
            if(transition) await TriggerTransition();
        }

        private void ActivateCheat()
        {
            DebugLogConsole.AddCommand<string, int>("AddAdvBox", "Add Adventure Box", AddBox);
        }

        private void FetchData()
        {
            _adventureBoxInventory = MainSceneController.Instance.Data.AdventureBoxInventory;
            if (_isDebugging)
            {
                _config = _debugConfig;
                _adventureBoxDatas = _dummyAdventureBoxData;
                return;
            }
            else
            {
                _config = _debugConfig;
                _adventureBoxDatas = _adventureBoxInventory.Inventory;
                return;
            }
        }

        private async void AddBox(string boxId, int total)
        {
            AdventureBoxRequest addBoxRequest = new AdventureBoxRequest(boxId, total);
            var addBoxResponse = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.AddAdventureBox(addBoxRequest));
            if (addBoxResponse.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(addBoxResponse.Error.Code);
                return;
            }
            MainSceneController.Instance.Data.AdventureBoxInventory.Inventory = addBoxResponse.Data.BoxInventory;
            await UpdateAdventureBox(addBoxResponse.Data.BoxInventory);
            MainSceneController.Instance.Loading.DoneLoading();
            return;
        }

        
        private async void OpenBox(AdventureBoxData box, int total)
        {
            if (_isDebugging)
            {
                return;
            }
            else
            {
                AdventureBoxRequest adventureBoxRequest = new AdventureBoxRequest(box.AdventureBoxId, total);
                var openBoxResponse = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.OpenAdventureBox(adventureBoxRequest));
                if (openBoxResponse.Error != null)
                {
                    if(openBoxResponse.Error.Code == "10401")
                    {
                        MainSceneController.Instance.Info.Show(InfoType.InsufficientBalance,new InfoAction("Go To Store", () => MainSceneController.Instance.GoToLobby(LobbyMenuEnum.Store)), new InfoAction("Close", null));
                    }
                    else
                    {
                        MainSceneController.Instance.Info.ShowSomethingWrong(openBoxResponse.Error.Code);
                    }
                    
                    return;
                }

                MainSceneController.Instance.Loading.DoneLoading();

                _vfx.OnFinishVFX.AddListener(() =>
                {
                    ShowResult(box, openBoxResponse);
                    _vfx.OnFinishVFX.RemoveAllListeners();
                });

                List<RewardParameterData> rewardDatas = openBoxResponse.Data.RewardGain.Select(data => new RewardParameterData(data.Type.ToString(), data.Amount)).ToList();

                _analytic.TrackOpenAdventureBoxEvent(box.AdventureBoxId, total, rewardDatas);

                _vfx.StartVFX(GetAdventureBoxTypeEnum(box.AdventureBoxId));

                MainSceneController.Instance.Data.AdventureBoxInventory.Inventory = openBoxResponse.Data.BoxInventory;

                _adventureBoxOpenPopUp.gameObject.SetActive(false);

                await UpdateAdventureBox(openBoxResponse.Data.BoxInventory, false);
                ProcessReward(openBoxResponse.Data.RewardGain);

                return;
            }
        }

        private void ShowResult(AdventureBoxData box, GenericResponseData<OpenAdventureBoxResponse> openBoxResponse)
        {
            string title = "Your " + box.Name;
            List<Sprite> sprites = PrepareSpriteReward(openBoxResponse.Data.RewardGain);
            List<string> label = PrepareLabelReward(openBoxResponse.Data.RewardGain);
            MainSceneController.Instance.Audio.PlaySfx(ADVENTURE_BOX_RESULT);
            MainSceneController.Instance.Info.ShowReward(title, string.Empty, openBoxResponse.Data.RewardGain.ToArray(), new InfoAction("Close", null), null);
        }

        private List<Sprite> PrepareSpriteReward(List<RewardBase> rewardGains)
        {
            List<Sprite> sprites = new List<Sprite>();
            foreach (var reward in rewardGains)
            {
                switch (reward.Type)
                {
                    case RewardEnum.GoldCoin:
                        sprites.Add(MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Type.ToString()));
                        break;
                    case RewardEnum.StarCoin:
                        sprites.Add(MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Type.ToString()));
                        break;
                    case RewardEnum.StarTicket:
                        sprites.Add(MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Type.ToString()));
                        break;
                    case RewardEnum.PetFragment:
                        sprites.Add(MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Ref + "_fragment")); //-> IF ASSET READY USE THIS
                        //sprites.Add(MainSceneController.Instance.AssetLibrary.GetAsset("PetFragment"));
                        break;
                    default:
                        Debug.LogWarning("this reward = " + reward.Type + " is currently not supported for adventure box");
                        sprites.Add(MainSceneController.Instance.AssetLibrary.GetSpriteAsset(string.Empty));
                        break;
                }
            }
            return sprites;
        }

        private List<string> PrepareLabelReward(List<RewardBase> rewardGains)
        {
            List<string> label = new List<string>();
            foreach (var reward in rewardGains)
            {
                switch (reward.Type)
                {
                    case RewardEnum.GoldCoin:
                        label.Add(CurrencyHandler.Convert(reward.Amount));
                        break;
                    case RewardEnum.StarCoin:
                        label.Add(CurrencyHandler.Convert(reward.Amount));
                        break;
                    case RewardEnum.StarTicket:
                        label.Add(CurrencyHandler.Convert(reward.Amount));
                        break;
                    case RewardEnum.PetFragment:
                        string data = JsonConvert.SerializeObject(reward.RefObject);
                        var petFragmentData = JsonConvert.DeserializeObject<PetFragmentInventory>(data);
                        string petName = petFragmentData.PetName + "'s Fragment";
                        if (reward.Amount > 1) petName += "s";
                        label.Add(reward.Amount + "x " + petName);
                        break;
                    default:
                        Debug.LogWarning("this reward = " + reward.Type + " is currently not supported for adventure box");
                        label.Add("Reward");
                        break;
                }
            }
            return label;
        }

        private void ProcessReward(List<RewardBase> rewardGains)
        {

            foreach (var reward in rewardGains)
            {
                switch (reward.Type)
                {
                    case RewardEnum.GoldCoin:
                        var balanceGold = MainSceneController.Instance.Data.UserBalance;
                        balanceGold.GoldCoin += reward.Amount;
                        MainSceneController.Instance.Data.PlayerBalanceActions.OnBalanceChanged.Invoke(balanceGold);
                        break;
                    case RewardEnum.StarCoin:
                        var balanceStar = MainSceneController.Instance.Data.UserBalance;
                        balanceStar.StarCoin += reward.Amount;
                        MainSceneController.Instance.Data.PlayerBalanceActions.OnBalanceChanged.Invoke(balanceStar);
                        break;
                    case RewardEnum.StarTicket:
                        var balanceStarTicket = MainSceneController.Instance.Data.UserBalance;
                        balanceStarTicket.StarTicket += reward.Amount;
                        MainSceneController.Instance.Data.PlayerBalanceActions.OnBalanceChanged.Invoke(balanceStarTicket);
                        break;
                    case RewardEnum.PetFragment:
                        MainSceneController.Instance.Data.PetFragment.AddPetFragment(reward.Ref.ToString(), (int)reward.Amount, DateTime.UtcNow);
                        break;
                }
            }
        }

        private async void SendBox(FriendProfile targetFriend, AdventureBoxData box)
        {
            Debug.Log("sending box...");
            if (_isDebugging)
            {
                _adventureBoxConfirmPopUp.CloseConfirm();
                return;
            }
            else
            {
                SendAdventureBoxRequest sendAdventureBoxRequest = new SendAdventureBoxRequest(targetFriend.FriendCode.ToString(), box.AdventureBoxId);
                var sendAdventureBoxResponse = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.SendAdventureBox(sendAdventureBoxRequest));
                if (sendAdventureBoxResponse.Error != null)
                {
                    if (sendAdventureBoxResponse.Error.Code == "30005")
                    {
                        MainSceneController.Instance.Info.Show("Cannot sent more adventure box", "Daily limit reach", InfoIconTypeEnum.Alert, new InfoAction("Close", null), null);
                        _adventureBoxConfirmPopUp.CloseConfirm();
                        return;
                    }
                    else
                    {
                        MainSceneController.Instance.Info.ShowSomethingWrong(sendAdventureBoxResponse.Error.Code);
                        return;
                    }
                }

                _adventureBoxConfirmPopUp.CloseConfirm();
                _adventureBoxInventory.RemoveAdventureBox(box.AdventureBoxId);

                MainSceneController.Instance.Audio.PlaySfx(ADVENTURE_BOX_GIFT);
                MainSceneController.Instance.Info.Show($"You have successfully gifted your {box.Name} to your friend.", $"{box.Name} Gifted", InfoIconTypeEnum.Success, new InfoAction("Close", null), null);

                await UpdateAdventureBox(_adventureBoxInventory.Inventory);
                return;
            }
        }

        public AdventureBoxTypeEnum GetAdventureBoxTypeEnum(string id)
        {
            if (AdventureBoxTypeMap.TryGetValue(id, out var adventureBoxType))
            {
                return adventureBoxType;
            }
            return AdventureBoxTypeEnum.Common;
        }

        public void OpenAdventureBoxSfx(AdventureBoxTypeEnum adventureBoxTypeEnum)
        {
            switch (adventureBoxTypeEnum)
            {
                case AdventureBoxTypeEnum.Common:
                    MainSceneController.Instance.Audio.PlaySfx(ADVENTURE_BOX_OPEN_BOTTOM_TYPE);
                    break;
                case AdventureBoxTypeEnum.Rare:
                    MainSceneController.Instance.Audio.PlaySfx(ADVENTURE_BOX_OPEN_BOTTOM_TYPE);
                    break;
                case AdventureBoxTypeEnum.Epic:
                    MainSceneController.Instance.Audio.PlaySfx(ADVENTURE_BOX_OPEN_UPPER_TYPE);
                    break;
                case AdventureBoxTypeEnum.Legendary:
                    MainSceneController.Instance.Audio.PlaySfx(ADVENTURE_BOX_OPEN_UPPER_TYPE);
                    break;
                default:
                    MainSceneController.Instance.Audio.PlaySfx(ADVENTURE_BOX_OPEN_BOTTOM_TYPE);
                    break;
            }
        }
    }

    [Serializable]
    public class SpecialBoxConfig
    {
        public int MaxCapacity;
    }

    [Serializable]
    public class AdventureBoxReward
    {
        public ItemTypeEnum Type;
        public string Id;
        public float MaxAmount;
        public float MinAmount;

    }
}