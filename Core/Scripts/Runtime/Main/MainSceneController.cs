using Agate.Starcade.Core.Runtime.Main;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Config;
using Agate.Starcade.Core.Runtime.ThirdParty.Firebase.Data;
using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Auth;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
//using Google.Play.Review;
using Agate.Starcade.Runtime.Lobby;
using Agate.Starcade.Core.Runtime.ThirdParty;
using Agate.Starcade.Scripts.Runtime;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.Model;
using IngameDebugConsole;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Starcade.Core.Localizations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Agate.Starcade.Core.Runtime;
using Agate.Starcade.Core.Runtime.Lobby.UserProfile;

namespace Agate.Starcade.Runtime.Main
{
	public class MainSceneController : MonoBehaviour
    {
        #region Static

        public static class STATIC_KEY
        {
            public const string GAME_CONFIG = "game_config";
            public const string AUDIO_CONFIG = "audio_config";
            public const string APP_SYSTEM_CONFIG = "app_system_config";
            public const string REFRESH_TOKEN = "refresh_token";
            public const string LOGIN_STATE = "login_state";
        }

        public static class SCENE_KEY
        {
            public const string LOBBY = "Lobby";
            public const string PLINKO = "Plinko";
            public const string MATCHTHREE = "Match3";
            public const string MONSTAMATCH = "MonStarmatch";
            public const string COINPUSHER = "CP01";
        }
        
        public static class AUDIO_KEY
        {
            public const string BGM_LOBBY = "bgm_lobby";

            public const string BUTTON_GENERAL = "button_general";
            public const string BUTTON_PLAY = "button_play";
            public const string BUTTON_TAB = "button_tab";
            public const string BUTTON_OPEN = "button_open";
            public const string BUTTON_CLOSE = "button_close";
            public const string BUTTON_NEGATIVE = "button_negative";
            public const string BUTTON_UNAVAILABLE = "button_unavailable";
            public const string NOTIFIICATION_COIN = "notification_coin";
            public const string BALANCE_COUNTING_ONCE = "balance_counting_once";
            public const string BALANCE_COUNTING_END = "balance_counting_end";

            public const string MAILBOX_CLAIM = "mailbox_button_claim";
            public const string MAILBOX_OPEN = "mailbox_button_open";

            public const string CLAIM_REWARD = "reward_coin";

            public const string BUTTON_GOLD_PLAY = "ui_goldmachine_play";
            public const string BUTTON_STAR_PLAY = "ui_starmachine_play";

            public const string BUTTON_CLAIM_REWARD = "ui_claim_reward";
            public const string BUTTON_SHOP_BUY = "ui_shop_buy";
            public const string SWIPE = "ui_swipe";
            public const string SWIPE_TIC = "ui_tic_swipe";

        }

        public static string RefreshToken;
        public static string Token;

        #endregion
          
        #region Singleton
        public static MainSceneController Instance { get; private set; }
        public bool SetupInstance()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                return true;
            }

            Destroy(gameObject);
            return false;
        }

		#endregion Singleton

		#region Component

		[Header("Data")]
		[SerializeField] private MainModel _data;

		[Header("Manager")]
		[SerializeField] private MainSceneManager _sceneManager;
		[SerializeField] private ThirdPartyManager _thirdParty;

		[Header("Configurations")]
		[SerializeField] private GameConfig _gameConfig;
		[SerializeField] private AppSystemSetting _appSystemSetting;

		[Header("Game Data")]
		[SerializeField] private UrlCollection _urlCollection;

		[Header("Controller")]
		[SerializeField] private AddressableController _addressable;
		[SerializeField] private AssetLibrary _assetLibrary;
		[SerializeField] private AudioController _audio;
		[SerializeField] private LoadingScreen _loading;
		[SerializeField] private InfoPopUpHandler _info;

		[Header("Scenes")]
		[SceneReference, SerializeField] private string _initScene = default;
		[SerializeField] private SceneData _titleSceneData;
		[SerializeField] private SceneData _lobbySceneData = default;

		[Header("Debugger")]
		[SerializeField] private GameObject _inGameDebug;
        
		public GameModeEnum ArcadeMode { get; set; } = GameModeEnum.Star;
        public TutorialModeEnum ArcadeTutorialMode { get; set; } = TutorialModeEnum.None;

		private UserAnalyticEventHandler _userAnalytic { get; set; }
		private SurveyAnalyticEventHandler _surveyAnalytic { get; set; }
		private AssetBundleAnalyticEventHandler _assetBundleAnalytic { get; set; }
		private AuthenticationBackEndController _authBackend { get; set; }
		private AuthenticationController _auth { get; set; }
		private GameBackendController _gameBackend { get; set; }
        private MainRequestController _mainRequestController { get; set; }
        private LocalizationsController _localizations { get; set; }
        #endregion


        #region Properties
        public AddressableController AddressableController => _addressable;
        public AssetLibrary AssetLibrary => _assetLibrary;
        public AudioController Audio => _audio;
        public LoadingScreen Loading => _loading;
        public InfoPopUpHandler Info => _info;
        public LocalizationsController Localizations => _localizations;
        public MainModel Data => _data;
        public MainSceneManager Scene => _sceneManager;
		public GameConfig GameConfig => _gameConfig;
		public UrlCollection UrlCollection => _urlCollection;
		public EnvironmentConfig EnvironmentConfig => _gameConfig.EnvironmentConfig;
		public ThirdPartyConfig ThirdPartyConfig => _gameConfig.ThirdPartyConfig;
		public ThirdPartyManager ThirdParty => _thirdParty;
		public RemoteConfigData RemoteConfigData => _thirdParty.Firebase.RemoteConfigData;

        public AuthenticationController Auth => _auth;
		public AuthenticationBackEndController AuthBackend => _authBackend;
        public GameBackendController GameBackend => _gameBackend;

		public IAnalyticController Analytic => _thirdParty.Firebase.Analytic;

        public AppSystemSetting AppSystemSetting => _appSystemSetting;

        public MainRequestController MainRequestController => _mainRequestController;
        public MainSceneManager MainSceneManager => _sceneManager;

        #endregion

        #region Unity Events

        private void Awake()
        {
            if (!SetupInstance()) return;
        }
        
        private async void Start()
        {
            await Init();
        }
        
        #endregion


        private async Task Init()
        {
            SetupMainData();

            SceneLaunchDataHelper.Setup();
            LoadSceneHelper.Setup();

            _authBackend = new AuthenticationBackEndController(EnvironmentConfig.IdentityBaseUrl, EnvironmentConfig.BasicToken, GameConfig.Timeout);
            _auth = new AuthenticationController(this);
            _auth.OnSignOut.AddListener(GoToTitle);

            _gameBackend = new GameBackendController();
            
            _thirdParty.Init(GameConfig.ThirdPartyConfig);

            _localizations = new LocalizationsController();
            //_reviewManager = new ReviewManager();

            _userAnalytic = new UserAnalyticEventHandler(Analytic);
            _surveyAnalytic = new SurveyAnalyticEventHandler(Analytic);
            _assetBundleAnalytic = new AssetBundleAnalyticEventHandler(Analytic);

            SetAppSystemSetting();
            SetGameConfig();
            SetupAssetLibrary();

            //SetupUser();

            InitListener();

            SetDebugMode(GameConfig.EnvironmentConfig.EnableDebugLogViewer);

            AddressableController.Init();

            Audio.InitAudio();
            await Audio.LoadAudioData("main_audio");
            await Localizations.LoadAllTables();

            _mainRequestController = new MainRequestController();
            _mainRequestController.Init();


            //Disable URP Debug menu
            UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;

            StartGame();
        }

        public void SetupUser()
        {
            SetDefaultAccessory();
            SetupExperience();
            SetupMail();
        }

        private void SetAppSystemSetting()
        {
            if (PlayerPrefs.HasKey(STATIC_KEY.APP_SYSTEM_CONFIG))
            {
                AppSystemSetting loadedAppSystem = JsonConvert.DeserializeObject<AppSystemSetting>(PlayerPrefs.GetString(STATIC_KEY.APP_SYSTEM_CONFIG));
                _appSystemSetting = loadedAppSystem;
            }
            else
            {
                string stringData = JsonConvert.SerializeObject(_appSystemSetting);
                PlayerPrefs.SetString(STATIC_KEY.APP_SYSTEM_CONFIG, stringData);
                PlayerPrefs.Save();
            }

            Application.targetFrameRate = _appSystemSetting.TargetFrameRate;
            QualitySettings.vSyncCount = _appSystemSetting.VerticalSyncCount;
            Input.multiTouchEnabled = _appSystemSetting.EnableMultiTouch;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public void UpdateAppSystemSetting(AppSystemSetting appSystemSetting)
        {
            //change setting that changeable
            _appSystemSetting = appSystemSetting;
            string stringData = JsonConvert.SerializeObject(_appSystemSetting);
            PlayerPrefs.SetString(STATIC_KEY.APP_SYSTEM_CONFIG, stringData);
            PlayerPrefs.Save();
        }
        
        private void InitListener()
        {
            RequestHandler.OnError.AddListener((arg0, positiveAction, negativeAction) =>
            {
                if (arg0 is WarningType)
                {
                    Instance.Info.Show((WarningType)arg0, positiveAction, negativeAction);
                }else if (arg0 is ErrorType)
                {
                    Instance.Info.Show((ErrorType)arg0, positiveAction, negativeAction);
                }else if (arg0 is InfoType)
                {
                    Instance.Info.Show((InfoType)arg0, positiveAction, negativeAction);
                }
            });
        }
        
        private void SetupMainData()
        {
			Data.UserProfileThirdPartyData = new UserProfileThirdPartyData();
		}

        private void SetGameConfig()
        {

            //SETUP AUDIO CONFIG
            AudioConfig audioSetting;
            if (PlayerPrefs.HasKey(STATIC_KEY.AUDIO_CONFIG))
            {
                Debug.Log("LOAD FROM PLAYER PREF");
                audioSetting = JsonConvert.DeserializeObject<AudioConfig>(PlayerPrefs.GetString(STATIC_KEY.AUDIO_CONFIG));
            }
            else
            {
                Debug.Log("LOAD FROM DEFAULT");
                audioSetting = GameConfig.AudioConfig;
            }
            GameConfig.AudioConfig = audioSetting;
            MainSceneController.Instance.GameConfig.AudioConfig = audioSetting;
            PlayerPrefs.SetString(STATIC_KEY.AUDIO_CONFIG, JsonConvert.SerializeObject(audioSetting));
            PlayerPrefs.Save();
        }

        private void SetDefaultAccessory()
        {
            Instance.Data.AccessoryData.PhotoUser = MainSceneController.Instance.GameConfig.UserProfilePhoto;
            Instance.Data.AccessoryData.UserAccessories = new Dictionary<ItemTypeEnum, AccessoryData>();
            Instance.Data.AccessoryData.PhotoURL = string.Empty;
            AccessoryLibrary[] libraries = new AccessoryLibrary[] 
            {
                Instance.Data.AccessoryData.AvatarLibrary,
                Instance.Data.AccessoryData.FrameLibrary
            };
            Debug.Log(Instance.Data.AccessoryData.AvatarLibrary);
            for (int i = 0; i < libraries.Length; i++)
            {
                libraries[i].UnlockedItems = new List<string>();
                libraries[i].UnlockedItems.AddRange(libraries[i].DefaultItemIds);
                AccessoryData accessory = new AccessoryData();
                accessory.Type = libraries[i].Type;
                accessory.OnAccessoryChanged = new UnityEvent();
                Data.AccessoryData.UserAccessories.Add(accessory.Type, accessory);
            }
        }

        private void SetupExperience()
        {
            Data.ExperienceData = new UserExperienceData();
            Data.ExperienceData.Data = new ExperienceData();
            Data.ExperienceData.OnExperienceChanged = new UnityEvent();
            Data.ExperienceData.OnLevelUpChanged = new UnityEvent<int>();
            Data.ExperienceData.OnMilestoneReached = new UnityEvent();

            Data.ExperienceData.OnLevelUpChanged.AddListener((level) =>
            {
                _userAnalytic.TrackUserLevelProperties(level);
                _userAnalytic.TrackUserLevelUpEvent(level);
            });
		}

        private void SetupMail()
        {
            Data.MailData = new List<Runtime.Data.MailboxDataItem>();
        }

        private void SetupAssetLibrary()
        {
            AssetLibrary.Init();
        }

        private void SetDebugMode(bool active)
        {
            if (active)
            {
                _inGameDebug.SetActive(true);
                
                DebugLogConsole.AddCommand("forceSignOut", "Force user to sign out", () => Auth.SignOut());
                DebugLogConsole.AddCommand("forceRefreshToken", "Force user to refresh token", () => Auth.FetchRefreshToken());
                //DebugLogConsole.AddCommand("unBindGoogle", "Force unbind account from GOOGLE account", UnbindGoogleAccount);
                DebugLogConsole.AddCommand("fastFoward", "fast forward game time", () =>
                {
                    Time.timeScale *= 2; 
                });
                DebugLogConsole.AddCommand("slowMotion", "slow motion game time", () =>
                {
                    Time.timeScale /= 2;
                });

				DebugLogConsole.AddCommand("coinPusherLive", "Force CP01 play as Live version", (Action)(() =>
                {
					_gameConfig.EnvironmentConfig.IsConnectToHeadless = true;
                }));
				DebugLogConsole.AddCommand("coinPusherHost", "Force CP01 play as Host version", (Action)(() =>
                {
					_gameConfig.EnvironmentConfig.IsConnectToHeadless = false;
                }));
            }
            else
            {
                _inGameDebug.SetActive(false);
            }
        }

        public async Task<bool> LoadMainAssetBundle()
        {
			Loading.StartLoadingDownload();

			var assetBundleKey = AddressableController.AssetID.Main;
			await AddressableController.CheckDownloadSize();
           

			var taskCompletionSource = new TaskCompletionSource<bool>();
			var task = taskCompletionSource.Task;
			Action<bool> callback = taskCompletionSource.SetResult;

			AssetData.DownloadState state = AddressableController.GetDownloadStatus(assetBundleKey);
            if(state == AssetData.DownloadState.Finish)
            {
                callback(true);
            }else if(state == AssetData.DownloadState.None)
            {
                Debug.Log("Setup main asset bundle");
				AddressableController.RemoveAllListeners();

				AddressableController.SetOnStartEvent(assetBundleKey, () =>
			    {
                    _assetBundleAnalytic.TrackMainAssetDownloadStartEvent();
			    });
			    AddressableController.SetOnProgressEvent(assetBundleKey, (current, total) =>
			    {
                    Debug.Log($"Download Asset bundle [Main] :{current}/{total}");
                    float percentage = (current / total) * 100;
                    var text = "Downloading... " + Mathf.Floor(percentage) + "%";

					Loading.UpdateLoadingDownloadIndicatorText(text);
			    });
			    AddressableController.SetOnFailedEvent(assetBundleKey, () =>
			    {
				    Debug.LogError($"Download main asset bundle failed!");
					_assetBundleAnalytic.TrackMainAssetDownloadFailedEvent();
					AddressableController.RemoveAllListeners();
					callback(false);
				});
			    AddressableController.SetOnCompleteEvent(assetBundleKey, () =>
			    {
				    Debug.Log($"Download Main Asset bundle complete");
					Loading.UpdateLoadingDownloadIndicatorText("Preparing Assets...");
					_assetBundleAnalytic.TrackMainAssetDownloadCompleteEvent();
				    AddressableController.RemoveAllListeners();
					callback(true);
				});

				AddressableController.InvokeOnStartEvent(assetBundleKey);
				AddressableController.AddDownload(assetBundleKey);
				AddressableController.StartDownload();
			}
			
			return await task;
		}

		public async Task LoadMainAsset()
        {
			await RequestHandler.Request(() => LoadMainAssetBundle());
			await Audio.LoadAudioData("general_audio");
		}

        private async Task StartGame()
        {
            try
            {
                string nextScene = InitialSceneLauncher.FirstLoadedScenePath ?? _initScene;
                var editorExcludedScenes = new List<string>() { _titleSceneData.ScenePath, _initScene };
                Debug.Log(JsonConvert.SerializeObject(editorExcludedScenes));
                Debug.Log(nextScene);
                if (!editorExcludedScenes.Contains(nextScene))
                {
                    var editorController = new EditorController(this, false);
                    await editorController.StartGameOnEditor();
                }
                Debug.Log("disiniii");
                SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error before start game - Error code " + ex.Message);
            }
        }

		#region DELETE 

		public void SetArcadeGameModeEnum(GameModeEnum mode, TutorialModeEnum tutorialMode = TutorialModeEnum.None)
		{
            ArcadeMode = mode;
            ArcadeTutorialMode = tutorialMode;
		}

		public void DisableArcadeTutorialMode()
		{
            ArcadeTutorialMode = TutorialModeEnum.None;
		}

		#endregion
        
        #region Global Method
        
        public void OpenSurvey(string clickLocation)
        {
            _surveyAnalytic.TrackClickSurvey(clickLocation);

			Application.OpenURL(_urlCollection.SurveyUrl);
            MainSceneController.Instance.Info.Show(InfoType.FillSurvey, new InfoAction("Close", null), null);
            
            string data = PlayerPrefs.GetString("PosterState");
            List<PosterStateReward> list = new List<PosterStateReward>();
            list = JsonConvert.DeserializeObject<List<PosterStateReward>>(data);
            list.Find(reward => reward.RewardId == "survey").IsDone = true;
            list.Find(reward => reward.RewardId == "survey").IsViewed = true;
            Debug.Log("set viewed survey");
            PlayerPrefs.SetString("PosterState", JsonConvert.SerializeObject(list));
            PlayerPrefs.Save();
        }

        public void GoToLobby(LobbyMenuEnum type)
        {
            LoadSceneHelper.PushData(new InitAdditiveBaseData{Key = type});
            LoadSceneHelper.LoadScene(_lobbySceneData);
        }


		public void GoToTitle()
		{
            Debug.Log("start load title scene");
            Instance.Loading.StartLoading();
            LoadSceneHelper.LoadScene(_titleSceneData,true,false);
		}


		public void ResetData()
        {
            SetDefaultAccessory();
            Instance.Data.AccessoryData.PhotoUser = MainSceneController.Instance.GameConfig.UserProfilePhoto;
            Instance.Data.AccessoryData.PhotoURL = String.Empty;
            Instance.Data.UserAccounts = null; 
            MainSceneController.Token = String.Empty;
        }

        #endregion

        #region CP
        public bool IsConnectToHeadless()
        {
            if (_gameConfig.EnvironmentConfig.ForceUseRemoteConfig)
            {
				return RemoteConfigData.IsCP01RealtimeEnabled;
			}

			return _gameConfig.EnvironmentConfig.IsConnectToHeadless;
		}
		#endregion
	}
}

