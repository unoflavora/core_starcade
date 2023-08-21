using Agate.Starcade.Core.Runtime.DailyLogin;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.DailyLogin.Data;
using Agate.Starcade.Scripts.Runtime.DailyLogin.SO;
using Agate.Starcade.Scripts.Runtime.DailyLogin.UI;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.UI.Reward;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Scripts.Runtime.DailyLogin
{
    public class DailyLoginController : MonoBehaviour
    {
        [SerializeField] private Canvas _dailyLoginCanvas;
        [SerializeField] private DailyLoginUI _dailyLoginUi;
        [SerializeField] private RewardPopup rewardPopup;
        [SerializeField] private VFXManager _vfxManager;

        [Header("Config")][SerializeField] private float _dailyRewardVfxDuration = 3f;
        [SerializeField] private DailyLoginSO _dailyLoginSo;

        [Header("For Debug")][SerializeField] private bool _debugMode;
        [SerializeField] private DailyLoginDataDummySO _dummyData;

        public UnityAction OnGetReward;
        public UnityAction OnCloseMenu;
        private AudioController _audio;

        private DailyLoginAnalyticEventHandler _dailyLoginAnalyticEvent { get; set; }

        [SerializeField, SceneReference] private string _userProfileScene;

        private async void Start()
        {
            _dailyLoginCanvas.worldCamera = Camera.main;
            DailyLoginData loginData;

            //await MainSceneController.Instance.AssetLibrary.LoadAssets(ItemTypeEnum.Reward);
            //await MainSceneController.Instance.AssetLibrary.LoadAssets(ItemTypeEnum.Pets);

            _dailyLoginAnalyticEvent = new DailyLoginAnalyticEventHandler(MainSceneController.Instance.Analytic);
            _audio = MainSceneController.Instance.Audio;
            await _audio.LoadAudioData(AudioKey.AUDIO_LABEL);


            if (_debugMode)
            {
                loginData = _dummyData.dailyLoginData;
                InitData(loginData);
                _audio.PlaySfx(AudioKey.CHECKLIST_ANIMATION);
                return;
            }
            else
            {
                loginData = MainSceneController.Instance.Data.dailyLoginData;
                //HARDCODED : FIX VIA BACKEND MUCH BETTER 
                if (loginData.Reward != null) loginData.Reward.Day = loginData.DayCount;
            }


            if (loginData.Reward != null && loginData.DailyLoginState == DailyLoginEnum.Closed)
            {
                InitData(loginData);
                _audio.PlaySfx(AudioKey.CHECKLIST_ANIMATION);
            }
            else
            {
                InitAfterClaim(loginData);
                _audio.PlaySfx(AudioKey.CHECKLIST_ANIMATION);
            }
        }


        private void InitData(DailyLoginData dailyLoginData)
        {
            rewardPopup.InitEvent(OnCloseRewardPopup);
            _dailyLoginUi.OnClaim.AddListener(ShowClaim);
            _dailyLoginUi.SetUI(dailyLoginData, _dailyLoginSo);
            _dailyLoginUi.InitEvent(UnloadScene);

            var dailyLoginDataReward = dailyLoginData.Reward;

            if (dailyLoginDataReward == null) return;
            rewardPopup.Init(dailyLoginDataReward, _dailyLoginSo);

            _dailyLoginAnalyticEvent.TrackReceiveDailyLoginReward(dailyLoginData.DayCount, dailyLoginDataReward.RewardType.ToString(), dailyLoginDataReward.Amount, dailyLoginDataReward.Ref, dailyLoginDataReward.RefObject);
        }

        private void InitAfterClaim(DailyLoginData dailyLoginData)
        {
            _dailyLoginUi.OnClaim.AddListener(ShowClaim);
            _dailyLoginUi.SetUIAfterClaim(dailyLoginData, _dailyLoginSo);
            _dailyLoginUi.InitEvent(UnloadScene);
            rewardPopup.Init(dailyLoginData.Reward, _dailyLoginSo);
        }

        private void ShowClaim(DailyLoginRewardData rewardData, Sprite rewardSprite, int DayReward)
        {
            _vfxManager.ShowRewardVFX(rewardData, _dailyLoginSo);

            IOnClaimDailyLogin claimCallback = null;

            switch (rewardData.RewardType)
            {
                case RewardEnum.GoldCoin:
                    claimCallback = new OnClaimCurrency();
                    break;
                case RewardEnum.StarCoin:
                    claimCallback = new OnClaimCurrency();
                    break;
                case RewardEnum.StarTicket:
                    claimCallback = new OnClaimCurrency();
                    break;
                case RewardEnum.Lootbox:
                    claimCallback = new OnClaimLootbox();
                    claimCallback.Meta = _userProfileScene;
                    break;
                case RewardEnum.Pet:
                    claimCallback = new OnClaimPet();
                    break;
                case RewardEnum.PetBox:
                    claimCallback = new OnClaimPetBox();
                    break;
            }

            if (_debugMode)
            {
                if (rewardData.RewardType == RewardEnum.Pet)
                {
                    PetRef petRef = new PetRef();
                    petRef.PetId = "pet_default";
                    petRef.PetName = "Pokemen";
                    rewardData.RefObject = petRef;
                }

                if (rewardData.RewardType == RewardEnum.Lootbox)
                {
                    string dummyLootbox = "{\"lootboxData\":{\"collectibleSetId\":\"CLB_TheCarnivalSet\",\"lootboxItemId\":\"LBX_Set_1_Gold\",\"lootboxItemName\":null,\"index\":0,\"rarityType\":\"gold\",\"rarityConfig\":null},\"lootboxGachaResults\":[{\"collectibleItemId\":\"CLB_TheCarnivalSet_03\",\"collectibleItemName\":\"Surprise!\",\"rarity\":1},{\"collectibleItemId\":\"CLB_TheCarnivalSet_03\",\"collectibleItemName\":\"Surprise!\",\"rarity\":1},{\"collectibleItemId\":\"CLB_TheCarnivalSet_03\",\"collectibleItemName\":\"Surprise!\",\"rarity\":1},{\"collectibleItemId\":\"CLB_TheCarnivalSet_04\",\"collectibleItemName\":\"From Me, To You\",\"rarity\":1},{\"collectibleItemId\":\"CLB_TheCarnivalSet_08\",\"collectibleItemName\":\"Peek-A-Boo!\",\"rarity\":2}]}";
                    LootboxClaimRef lootboxClaimRef = JsonConvert.DeserializeObject<LootboxClaimRef>(dummyLootbox);
                    rewardData.RefObject = lootboxClaimRef;
                }

            }

            claimCallback.ShowClaim(rewardData, rewardSprite, DayReward, _dailyRewardVfxDuration, ClaimReward, OnCloseRewardPopup);

            LeanTween.delayedCall(_dailyRewardVfxDuration, () =>
            {
                _vfxManager.HideRewardVFX();
            });

            _audio.PlaySfx(AudioKey.REWARD_NOTIFICATION);
        }

        private void ClaimReward(DailyLoginRewardData dailyLoginRewardData)
        {
            PlayerBalance currentPlayerBalance = MainSceneController.Instance.Data.UserBalance;

            IOnClaimDailyLogin claimCallback = null;

            switch (dailyLoginRewardData.RewardType)
            {
                case RewardEnum.GoldCoin:
                    claimCallback = new OnClaimCurrency();
                    break;
                case RewardEnum.StarCoin:
                    claimCallback = new OnClaimCurrency();
                    break;
                case RewardEnum.StarTicket:
                    claimCallback = new OnClaimCurrency();
                    break;
                case RewardEnum.Lootbox:
                    claimCallback = new OnClaimLootbox();
                    break;
                case RewardEnum.Pet:
                    claimCallback = new OnClaimPet();
                    break;
                case RewardEnum.Avatar:
                    break;
                case RewardEnum.Frame:
                    break;
                case RewardEnum.Collectible:
                    break;
                case RewardEnum.PetFragment:
                    break;
                case RewardEnum.SpecialBox:
                    break;
                case RewardEnum.PetBox:
                    claimCallback = new OnClaimPetBox();
                    break;
            }

            claimCallback.ClaimReward(dailyLoginRewardData);

            MainSceneController.Instance.Data.dailyLoginData.DailyLoginState = DailyLoginEnum.Opened;
        }


        private void UnloadScene()
        {
            _audio.UnloadAudioData(AudioKey.AUDIO_LABEL);
            //OnCloseMenu.Invoke();
            LoadSceneHelper.CloseSceneAdditive();
        }

        private void OnCloseRewardPopup()
        {
            _vfxManager.HideRewardVFX();
            _dailyLoginUi.ShowAfter();
        }
    }
}