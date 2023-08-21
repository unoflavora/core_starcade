using Agate;
using Agate.Starcade;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Core.Runtime.Game;
using Agate.Starcade.Core.Runtime.Lobby.EntryPoint;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Core.Scripts.Runtime.WelcomePopUp;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Lobby;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.DailyLogin;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;
using Agate.Starcade.Scripts.Runtime.OnBoarding;
using Newtonsoft.Json;
using Starcade.Core.Runtime.Lobby.Script.Lootbox.VFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Agate.Starcade.Runtime.Main.MainSceneController.AUDIO_KEY;

public class LobbySceneController : MonoBehaviour
{

    [SerializeField] private AutoClaimRewardController _autoClaimRewardController;
    [SerializeField] private OnBoardingController _onBoardingController;
    [SerializeField] private LobbyUI _lobbyUI;
    [SerializeField] private LobbyAudio _lobbyAudio;

    [Header("MANAGER MENU")] 
    [SerializeField] private LobbyMenuEnum _onStartMenu;
    [SerializeField] private ArcadeManager _arcadeManager;
    [SerializeField] private StoreManager _storeManager;
    [SerializeField] private PetsManager _petsManager;
    [SerializeField] private PosterManager _posterManager;
    [SerializeField] private SettingManager _settingManager;

    [Header("NOTIFICATION BADGE")] 
    [SerializeField] private NotificationBadge _arcadeNotificationBadge;
    [SerializeField] private NotificationBadge _storeNotificationBadge;
    [SerializeField] private NotificationBadge _collectionNotificationBadge;
    [SerializeField] private NotificationBadge _eventsNotificationBadge;

    [Header("REMOVE LATER")] 
    [SerializeField] private Button _goldCoinAdd;
    [SerializeField] private Button _starCoinAdd;
    [SerializeField] private Button _profileButton;
    [SerializeField] private Button _mailboxButton;
    [SerializeField] private Button _entryPointButton;
    //[SerializeField] private Button _settingButton;
    [SerializeField] private Button _signOut;

    [SerializeField] private GameObject _nameInputPanel;
    [SerializeField] private TMP_InputField _nameInputField;
    
    [SerializeField] private UnityAction<PlayerBalance> _onClaimReward;

    [SerializeField] private CollectPopupController _collectPopup;

    [Header("SCENE ADDITIVE")] 
    [SerializeField] private AssetReference _userProfile;
    [SerializeField] private AssetReference _mailbox;
    [SerializeField] private AssetReference _lobbySetting;
    [SerializeField] private AssetReference _gachaVFX;
    [SerializeField] private AssetReference _pets;

    [Header("Pop Up Manager")]
    [SerializeField] private DailyLoginManager _dailyLoginManager;
    [SerializeField] private WelcomePopUpManager _welcomePopUpManager;
    [SerializeField] private EntryPointManager _entryPointManager;

    [Header("Mailbox")]
    [SerializeField] private MailboxButtonController _mailButtonController;

    [Header("Transition")]
    [SerializeField] private List<CanvasTransition> _lobbyTransitions;


    public TMP_Text OrientationText;

	private MainSceneController _main { get; set; }
    private LobbyAnalyticEventHandler _lobbyAnalyticEvent { get; set; }
    private UserAnalyticEventHandler _userAnalyticEvent { get; set; }
    private OnboardingAnalyticEventHandler _onboardingAnalyticEvent { get; set; }
    private GameBackendController _gameBackend { get; set; }


    private void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft; // SEMENTARA
        _main = MainSceneController.Instance;
    }

    private async void Start()
    {
		await _main.Loading.StartLoading(LoadingScreen.LOADING_TYPE.LoadingInfo);
        await _main.AssetLibrary.LoadAllAssets();
        await _main.AddressableController.CheckDownloadSize();

        _userAnalyticEvent = new UserAnalyticEventHandler(_main.Analytic);
        _lobbyAnalyticEvent = new LobbyAnalyticEventHandler(_main.Analytic);
        _onboardingAnalyticEvent = new OnboardingAnalyticEventHandler(_main.Analytic);
        _gameBackend = _main.GameBackend;
        _lobbyAnalyticEvent.TrackLobbyEvent();

        Init();
    }

    private async void Init()
    {
        //FETCH AND INIT PLAYER DATA
        await _main.MainRequestController.FetchLobbyData();
        InitLobby();

        //START LOBBY
        StartLobby();
    }

    #region Lobby Handler
    private async void StartLobby()
    {
        //INIT ARCADE LIST
        _arcadeManager.InitArcadeList(_lobbyAnalyticEvent);

        //INIT STORE
        _storeManager.Init(_lobbyUI.UpdateBalanceLabel);//, () => LoadUserProfile(LobbyMenuEnum.Collection));

        //INIT NOTIFICATION EVENT
        _posterManager.CheckNotificationEvent.AddListener(() =>
        {
            _eventsNotificationBadge.SwitchStateBadge(CheckEventsNotificationState());
        });
        _eventsNotificationBadge.SwitchStateBadge(CheckEventsNotificationState());

        //INIT AUDIO
        _lobbyAudio.Init();
        _lobbyAudio.PlayBgm();

        //INIT BUTTON
        InitButtonCallback();

        //INIT CLAIM REWARD
        _autoClaimRewardController.Init(_lobbyAnalyticEvent);
        _collectPopup.Init();

        //INIT LOBBY SCENE DATA
        var lobbySceneData = LoadSceneHelper.PullData();
        //LoadSceneHelper.ClearData();

        if (lobbySceneData != null)
        {
            _onStartMenu = lobbySceneData.Key;
        }
        OpenMenu(_onStartMenu);
       

        foreach (var transition in _lobbyTransitions)
        {
            transition.TriggerTransition();
        }

        var onboardingResponse = await RequestHandler.Request(async () => await _gameBackend.OnBoarding(MainSceneController.SCENE_KEY.LOBBY));
        if (onboardingResponse.Error != null)
        {
            _main.Info.ShowSomethingWrong(onboardingResponse.Error.Code);
            return;
        }
        if (!onboardingResponse.Data.IsComplete)
        {
            StartLobbyOnBoarding(onboardingResponse);
        }
        else
        {
            _arcadeManager.SelectDefault();
            //_autoClaimRewardController.SetDummyClaim(false);
            await StartInitialLobbyPopUp();
        }

		_main.Loading.DoneLoading();
	}

    private void OpenMenu(LobbyMenuEnum menu)
    {
        Debug.Log(JsonConvert.SerializeObject(menu));

        _arcadeManager.ToggleButton.OnDeselect.Invoke();
        _storeManager.ToggleButton.OnDeselect.Invoke();
        _posterManager.ToggleButton.OnDeselect.Invoke();
        _petsManager.ToggleButton.OnDeselect.Invoke();
        // _settingManager.ToggleButton.OnDeselect.Invoke();

        switch (menu)
        {
            case LobbyMenuEnum.Arcade:
                _arcadeManager.ToggleButton.OnSelect.Invoke();
                break;
            case LobbyMenuEnum.Store:
                _storeManager.ToggleButton.OnSelect.Invoke();
                break;
            case LobbyMenuEnum.Poster:
                _posterManager.ToggleButton.OnSelect.Invoke();
                break;
            case LobbyMenuEnum.Pets:
                LoadSceneHelper.LoadSceneAdditive(_pets);
                break;
            case LobbyMenuEnum.Setting:
                // _settingManager.ToggleButton.OnSelect.Invoke();
                break;
            case LobbyMenuEnum.UserProfile:
                _arcadeManager.ToggleButton.OnSelect.Invoke();
                break;
            case LobbyMenuEnum.StoreLootbox:
                _storeManager.ToggleButton.OnSelect.Invoke();
                _storeManager.OpenStore(StoreManager.StoreType.Lootbox);
                break;
            case LobbyMenuEnum.StorePetbox:
                Debug.Log("open store pet box");
                _storeManager.ToggleButton.OnSelect.Invoke();
                _storeManager.OpenStore(StoreManager.StoreType.Petbox);
                break;
            default:
                _arcadeManager.ToggleButton.OnSelect.Invoke();
                break;
        }
    }

    private void InitButtonCallback()
    {
        _goldCoinAdd.onClick.AddListener(() => OpenMenu(LobbyMenuEnum.Store));
        _starCoinAdd.onClick.AddListener(() => OpenMenu(LobbyMenuEnum.Store));

        _profileButton.onClick.AddListener(() =>
        {
            LoadUserProfile();
            _lobbyAnalyticEvent.TrackClickUserProfileEvent();
        });

        _mailboxButton.onClick.AddListener(() =>
        {
            MainSceneController.Instance.Audio.PlaySfx(MAILBOX_OPEN);
            LoadSceneHelper.LoadSceneAdditive(_mailbox, new InitAdditiveBaseData
            {
                OnClose = (lobbyMenu) =>
                {
                    Debug.Log($"All mail {_main.Data.MailData.Count}");
                    _mailButtonController.SetNotificationVisible(_main.Data.MailData);
                    _lobbyUI.UpdateBalanceLabel(_main.Data.UserBalance);
                },
                Key = LobbyMenuEnum.Mailbox,
            });
            _lobbyAnalyticEvent.TrackClickMailboxMenuEvent();
        });

        _entryPointButton.onClick.AddListener(() =>
        {
            _entryPointManager.Open();
        });
    }

    private void StartLobbyOnBoarding(GenericResponseData<OnBoardingData> onboardingResponse)
    {
        //_autoClaimRewardController.SetDummyClaim(true);
        _onBoardingController.gameObject.SetActive(true);
        _onBoardingController.InitOnboarding(MainSceneController.SCENE_KEY.LOBBY, onboardingResponse.Data.CurrentState);
        _onBoardingController.OnStartOnBoarding.AddListener(() => _onboardingAnalyticEvent.TrackStartOnboardingEvent(MainSceneController.SCENE_KEY.LOBBY));
        _onBoardingController.OnEndOnBoarding.AddListener(async () =>
        {
            _onboardingAnalyticEvent.TrackEndOnboardingEvent(MainSceneController.SCENE_KEY.LOBBY);
			var reward = await MainSceneController.Instance.Data.ClaimCustomReward("onboarding_reward", async () =>
            {
                _arcadeManager.SelectDefault();
				await StartInitialLobbyPopUp();
			}, "Your Starter rewards", "Please enjoy your rewards!");

            _lobbyAnalyticEvent.TrackClaimLobbyOnboardingRewardEvent(reward.RewardGain.Select(d => new LobbyAnalyticEventHandler.RewardParameters()
            {
                Type = d.Type.ToString(),
                Amount = d.Amount
            }).ToList());
        });

        _onBoardingController.OnCompleteOnBoardingEvent.AddListener(state => _onboardingAnalyticEvent.TrackOnboardingEvent("lobby", state));
        _onBoardingController.OnSkipOnboarding.AddListener(state => _onboardingAnalyticEvent.TrackSkipOnboardingEvent("lobby", state));
        _onBoardingController.StartOnboarding();
    }

    private async Task StartInitialLobbyPopUp()
    {
        await InitDailyLogin();

        await Task.Delay(1000);
        Debug.Log("CURRENT ACTIVE SCENE TOTAL = " + SceneManager.sceneCount);

        while (SceneManager.sceneCount >= 2)
        {
            await Task.Yield();
        }

        InitWelcomePopUp();
    }

    private async Task InitDailyLogin()
    {
        await _main.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
        var dailyLoginResponse = await RequestHandler.Request(async () => await _gameBackend.GetDailyLoginRewards());
        if (dailyLoginResponse.Error != null)
        {
            _main.Info.ShowSomethingWrong(dailyLoginResponse.Error.Code);
            return;
        }

        _main.Data.dailyLoginData = dailyLoginResponse.Data;
        _main.Data.dailyLoginData.DailyLoginState = Agate.Starcade.Scripts.Runtime.DailyLogin.Data.DailyLoginEnum.Closed;

        await _dailyLoginManager.Init();
        MainSceneController.Instance.Loading.DoneLoading();
    }

    private async void InitWelcomePopUp()
    {
        await _main.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);

        _welcomePopUpManager.LoadScene();

        MainSceneController.Instance.Loading.DoneLoading();
    } 
    #endregion

    #region Fetch and Init
    private async void InitLobby()
    {
        var lobbyInitResponse = await RequestHandler.Request(async () => await _gameBackend.LobbyInit());
        if (lobbyInitResponse.Error != null)
        {
            _main.Info.ShowSomethingWrong(lobbyInitResponse.Error.Code);
            return;
        }

        _main.Data.ExperienceData.Data = lobbyInitResponse.Data.Data.ExperienceData;
        _main.Data.UserData = lobbyInitResponse.Data.Data;
        _main.Data.LobbyData = lobbyInitResponse.Data.Lobby;
        _main.Data.lobbyConfig = lobbyInitResponse.Data.LobbyConfig;
        _main.Data.UserBalance = lobbyInitResponse.Data.Balance;
        _main.Data.PlayerBalanceActions = new PlayerBalanceActions();
        _main.Data.PlayerBalanceActions.OnBalanceChanged = UpdateBalance;
        _main.Data.OnLootboxObtained = LoadGachaScene;

        if (_main.Data.UserData.Accessories.UnlockAvatar.Count > 0)
            _main.Data.AccessoryData.AvatarLibrary.UnlockedItems.AddRange(_main.Data.UserData.Accessories.UnlockAvatar);

        if (_main.Data.UserData.Accessories.UnlockFrame.Count > 0)
            _main.Data.AccessoryData.FrameLibrary.UnlockedItems.AddRange(_main.Data.UserData.Accessories.UnlockFrame);

        AccessoryData avatar = _main.Data.AccessoryData.UserAccessories[ItemTypeEnum.Avatar];
        avatar.Id = lobbyInitResponse.Data.Data.Accessories.UseAvatar ?? _main.Data.AccessoryData.DefaultActiveAvatar;

        if (avatar.Id != null) avatar.Data = _main.AssetLibrary.GetSpriteAsset(avatar.Id);
        if (avatar.Id == _main.Data.AccessoryData.DefaultActiveAvatar) avatar.Data = _main.Data.AccessoryData.PhotoUser ?? _main.GameConfig.UserProfilePhoto;

        AccessoryData frame = _main.Data.AccessoryData.UserAccessories[ItemTypeEnum.Frame];
        frame.Id = lobbyInitResponse.Data.Data.Accessories.UseFrame ?? _main.Data.AccessoryData.DefaultActiveFrame;
        if (frame.Id != null) frame.Data = _main.AssetLibrary.GetSpriteAsset(frame.Id);
        if (frame.Id == _main.Data.AccessoryData.DefaultActiveFrame) frame.Data = null;

        Debug.Log("mail data = " + JsonConvert.SerializeObject(_main.Data.MailData));
        _mailButtonController.SetNotificationVisible(_main.Data.MailData);

        _lobbyUI.UpdateUserName(lobbyInitResponse.Data.Data.Username);
        _lobbyUI.UpdateBalanceLabel(lobbyInitResponse.Data.Balance);

        _main.Data.UserProfileAction.OnAccessoryChanged = _lobbyUI.UpdatePhotoProfile;
        _lobbyUI.UpdatePhotoProfile(_main.Data.UserProfileData.UsedAvatar, _main.Data.UserProfileData.UsedFrame);

		var userData = lobbyInitResponse.Data.Data;

        _userAnalyticEvent.TrackUserLevelProperties(lobbyInitResponse.Data.Data.ExperienceData.Level);
        _userAnalyticEvent.TrackUserBalanceProperties(lobbyInitResponse.Data.Balance);
	}
    #endregion

    #region Scene Handler
    private async void LoadGachaScene(List<CollectibleItem> items, bool isPremium, LootboxRarityEnum lootboxType, Action onClose)
    {
        await MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);

        LoadSceneHelper.LoadSceneAdditive(_gachaVFX, new InitAdditiveBaseData()
        {
            Data = new GachaVFXData()
            {
                Camera = Camera.main,
                LootboxType = lootboxType,
                CollectibleItems = items,
                GachaType = isPremium ? GachaType.Premium : GachaType.Regular
            },
            OnClose = (lobbyMenu) =>
            {
                LoadSceneHelper.CloseSceneAdditive();
                onClose.Invoke();
            }
        });
    }

    private void LoadUserProfile(LobbyMenuEnum key = LobbyMenuEnum.UserProfile)
    {
        LoadSceneHelper.LoadSceneAdditive(_userProfile, new InitAdditiveBaseData
        {
            OnClose = (lobbyMenu) =>
            {
                Debug.Log("Closing User Profile");
                //_lobbyUI.UpdatePhotoProfile();
                _lobbyUI.UpdateUserName(_main.Data.UserData.Username);
                
                // Default Param should not be used
                if (lobbyMenu == LobbyMenuEnum.Arcade) return;
                OpenMenu(lobbyMenu);
            },
            Key = key
        });
    }

    public void LoadPets()
    {
        MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);

        LoadSceneHelper.LoadSceneAdditive(_pets, new InitAdditiveBaseData()
        {
            OnClose = (lobbyMenu) => OpenMenu(lobbyMenu)
        });
    }
    #endregion

    #region Update Handler
    private void UpdateBalance(PlayerBalance balance)
    {
        _lobbyUI.UpdateBalanceLabel(MainSceneController.Instance.Data.UserBalance);
		_userAnalyticEvent.TrackUserBalanceProperties(balance);
	}

    public async void UpdateName()
    {
        try
        {
            await _main.GameBackend.SetUsername(_nameInputField.text);
            _lobbyUI.UpdateUserName(_nameInputField.text);
        }
        catch
        {
            Debug.LogError("FAILED UPDATE NAME");
        }
    }
    #endregion

    #region Notification Handler

    private bool CheckEventsNotificationState()
    {
	    var dataEvents = PlayerPrefs.GetString("PosterState");
	    var data = JsonConvert.DeserializeObject<List<PosterStateReward>>(dataEvents);

	    Debug.Log("data event = " + dataEvents);
	    
	    foreach (var posterState in data)
	    {
		    if (posterState.IsViewed == false)
		    {
                Debug.Log("POSTER STATE " + posterState.RewardId + " is not done");
			    Debug.Log("SET FALSE NOTIF BADGE EVENTS");
			    return true;
		    }
	    }
	    Debug.Log("SET TRUE NOTIF BADGE EVENTS");
	    return false;
    }

    #endregion
    
    }
