using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Core.Runtime.Helper;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Auth;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Runtime.Title;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Info;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using static Agate.Starcade.Runtime.Main.MainSceneController.STATIC_KEY;

namespace Agate.Starcade.Core.Runtime.Title
{
	public class TitleSceneController : MonoBehaviour, IAudioScene
    {
        [SerializeField] private SceneData _lobbyScene;
        
        [SerializeField] private TitleScreenUI _titleScreenUI;

        [SerializeField] private AgreementController _agreementController;

        [SerializeField] private InputUsernameController _inputUsername;
        [SerializeField] private GameObject _inputNamePanel;
        [SerializeField] private TMP_InputField _inputFieldName;

        [SerializeField] private VideoPlayer _splashVideo;

        private AuthenticationController _auth;
        private GameBackendController _gameBackendController;
        private AudioController _audio;
        private GameInitData _gameData;
        
        private TitleAnalyticEventHandler _titleAnalyticHandler { get; set; }
        private UserAnalyticEventHandler _userAnalyticHandler { get; set; }

        //Test For Development
        private List<TitleScreenUI.LoginMode> _loginModes;

        void Start()
        {
            var mainInstance = MainSceneController.Instance;
            _titleAnalyticHandler = new TitleAnalyticEventHandler(mainInstance.Analytic);
            _titleAnalyticHandler.TrackOpenTitleScreenEvent();

            _userAnalyticHandler = new UserAnalyticEventHandler(mainInstance.Analytic);


            _auth = MainSceneController.Instance.Auth;
            _gameBackendController = MainSceneController.Instance.GameBackend;
            _audio = MainSceneController.Instance.Audio;
            


            //MainSceneLauncher.Instance.Info.Show("test","test",InfoIconTypeEnum.Default,new InfoAction("test",null),null);

            /*var res = await RequestHandler.Request(async () => await _authenticationBackEnd.RequestTestDelay());
            if (res.Error != null)
            {
                if (res.Error.Code == "6")
                {
                    MainSceneLauncher.Instance.Info.Show("Ini Error","6", InfoIconTypeEnum.Clock, new InfoAction()
                    {
                        ActionName = "KLIK INI DEBUG",
                        Action = () =>
                        {
                            Debug.Log("debug bro");
                        }
                    }, null);
                }
            }*/

            //End Here

            

            if (!string.IsNullOrEmpty(_auth.RefreshToken)) AutoLogin();
            else
            {
                InitTitleScreen();
            }
        }

        private void LoadNextScene()
        {
            //SceneManager.LoadSceneAsync(_lobbyScene, LoadSceneMode.Single);
        }


        private void InitTitleScreen()
        {
            //TODO DISABLE AUDIO ON TITLE SCREEN
            MainSceneController.Instance.Audio.StopBgm();
            
            _gameBackendController = MainSceneController.Instance.GameBackend;
                
			_loginModes = new List<TitleScreenUI.LoginMode> { TitleScreenUI.LoginMode.Guest };
#if UNITY_IOS
			_loginModes.Add(TitleScreenUI.LoginMode.IOS);

			Button iosLoginButton = _titleScreenUI.GetLoginButton(TitleScreenUI.LoginMode.IOS);
            iosLoginButton.onClick.AddListener(() => LoginWithIOS());
#elif UNITY_ANDROID || UNITY_EDITOR
			_loginModes.Add(TitleScreenUI.LoginMode.Google);

            Button googleLoginButton = _titleScreenUI.GetLoginButton(TitleScreenUI.LoginMode.Google);
            googleLoginButton.onClick.AddListener(async () => await LoginWithGoogle());
#endif

			Button guestLoginButton = _titleScreenUI.GetLoginButton(TitleScreenUI.LoginMode.Guest);
			guestLoginButton.onClick.AddListener(LoginAsGuest);


            _titleScreenUI.SetUi(_loginModes, false);
            SetupAudio();

            _titleScreenUI.PlayButton.onClick.AddListener(PlayGame);
        }

        private async void LoginAsGuest()
        {
            _titleScreenUI.ShowLoginButton(_loginModes, false);

            var res = await _auth.LoginAsGuest();

			if (res.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(res.Error.Code);
                _titleScreenUI.ShowLoginButton(_loginModes, true);
            }
            else
            {
                Debug.Log("Guest Login");
                _titleAnalyticHandler.TrackLoginEvent(Enum.GetName(typeof(LoginState), LoginState.GUEST));
                await MainSceneController.Instance.Info.ShowDelay(InfoType.LoginSuccess, null, null, 3);
                OnAuthComplete(res.Data);
            }
        }

        private async Task LoginWithGoogle()
        {
			MainSceneController.Instance.Loading.StartTitleLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
			_titleScreenUI.ShowLoginButton(_loginModes, false);
            Debug.Log("START");
            var res = await _auth.LoginGoogle();
            
            MainSceneController.Instance.Loading.DoneLoading();
            
			if (res.Data != null && res.Error == null)
            {
				Debug.Log("LOGIN WITH GOOGLE SUCCESS");
				_titleAnalyticHandler.TrackLoginEvent(Enum.GetName(typeof(LoginState), LoginState.GOOGLE));
				await MainSceneController.Instance.Info.ShowDelay(InfoType.LoginSuccess, null, null, 3);
				OnAuthComplete(res.Data);
			}
            else
            {
				if (res.Error != null)
				{
					MainSceneController.Instance.Info.ShowSomethingWrong(res.Error.Code);
				}

				await MainSceneController.Instance.Info.ShowDelay(InfoType.LoginFailed, null, null, 3);
				_titleScreenUI.ShowLoginButton(_loginModes, true);
			}
        }

        private async void LoginWithIOS()
        {
			MainSceneController.Instance.Loading.StartTitleLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
			_titleScreenUI.ShowLoginButton(_loginModes, false);

            var res = await _auth.LoginApple();
			MainSceneController.Instance.Loading.DoneLoading();

			if (res.Data != null && res.Error == null)
            {
				Debug.Log("LOGIN WITH IOS SUCCESS");
				_titleAnalyticHandler.TrackLoginEvent(Enum.GetName(typeof(LoginState), LoginState.APPLE));
				await MainSceneController.Instance.Info.ShowDelay(InfoType.LoginSuccess, null, null, 3);

				OnAuthComplete(res.Data);

			}
            else
            {
				if (res.Error != null)
				{
					MainSceneController.Instance.Info.ShowSomethingWrong(res.Error.Code);
				}

				await MainSceneController.Instance.Info.ShowDelay(InfoType.LoginFailed, null, null, 3);
				_titleScreenUI.ShowLoginButton(_loginModes, true);
			}
		}

		private async void AutoLogin()
        {
			Debug.Log($"REFRESH TOKEN: {_auth.RefreshToken}");
			var loginData = await _auth.FetchRefreshToken();
            Debug.Log(JsonConvert.SerializeObject(loginData));

            if(loginData == null || JsonConvert.SerializeObject(loginData) == null)
            {
                Debug.Log("auto login null");
                return;
            }

            if (loginData != null)
            {
                Debug.Log("auto login not null");
                InitTitleScreen();
                //TODO FIX BETTER 
                LoginState loginState = (LoginState)PlayerPrefs.GetInt(LOGIN_STATE);
				Debug.Log($"AUTO LOGIN SUCCESS {loginState}");
				OnAuthComplete(loginData);
			}
        }

        private async void OnAuthComplete(LoginData loginData)
        {
            Debug.Log("AUTH COMPLETED!");

            Debug.Log(loginData.Data.PhotoUrl);
            Debug.Log(MainSceneController.Instance.Data.AccessoryData);
            MainSceneController.Instance.Data.AccessoryData.PhotoURL = loginData.Data.PhotoUrl;
            await _gameBackendController.DownloadPhotoProfile();

            bool alreadyLogin = PlayerPrefs.HasKey(REFRESH_TOKEN);
            _titleScreenUI.SetUi(_loginModes, alreadyLogin);

            GameInitData gameInitData = new GameInitData();
            Debug.Log("AUTH " + MainSceneController.Token);
            var res = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.GameInit());
            MainSceneController.Instance.Loading.DoneLoading();
            if (res.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(res.Error.Code);
                return;
            }
            else
            {
                if (MainSceneController.Instance.GameConfig.EnvironmentConfig.EnableVersionCheck)
                {
                    string currentAppversion = "1.0.0";
#if UNITY_IOS
					currentAppversion = res.Data.GameVersion.Ios;
#else
					currentAppversion = res.Data.GameVersion.Android;
#endif
                    var isValidAppVersion = AppHelper.ValidateAppVersion(currentAppversion);

					if (!isValidAppVersion)
                    {
						Debug.LogError("ERROR WARNING VERSION NOT SYNC BETWEEN BACKEND - VERSION FRONTEND: " + Application.version + " - VERSION BACKEND: " + currentAppversion);

#if !UNITY_EDITOR
						MainSceneController.Instance.Info.Show(ErrorType.InvalidVersion, new InfoAction("Update", () =>
						{
#if UNITY_IOS
							AppStoreHelper.OpenAppleAppStore();
#else
					        AppStoreHelper.OpenGooglePlayStore();
#endif
							AppHelper.Quit();

						}), new InfoAction("Quit", AppHelper.Quit));
						return;
#endif
					}

                }

				MainSceneController.Instance.Data.SetGameInitData(res.Data);
                MainSceneController.Instance.Data.ExperienceData.Data = res.Data.data.ExperienceData;
                gameInitData = res.Data;
                var user = loginData.Data;

				_userAnalyticHandler.TrackUserEmailProperties(user.Email);
				_userAnalyticHandler.TrackAccountTypeProperties(user.AccountType.ToString());
				_userAnalyticHandler.TrackUserDisplayNameProperties(gameInitData.data.Username);
				_userAnalyticHandler.TrackFriendcodeProperties(gameInitData.data.FriendCode);
				_userAnalyticHandler.TrackRegionProperties();
			}
                
            if (MainSceneController.Instance.Data.UserData.Username == null)
            {
                _inputNamePanel.SetActive(true);
                await _inputUsername.Wait();
            }
            
            
            if (gameInitData.TermsAndCondition.SignedVersion != gameInitData.TermsAndCondition.Config.Version)
            {
                _agreementController.gameObject.SetActive(true);
                _agreementController.OpenAgreement(MainSceneController.Instance.Data.TermsAndConditionData.Config);
            }
            
            Debug.Log("Session is clear to play");
        }

        private async void PlayGame()
        {
            await StartGame();
        }
        
        private void OnLoadComplete(Scene scene, Scene scene1)
        {
            MainSceneController.Instance.Loading.DoneLoading();
        }
        
        public async void SetName()
        {
            await MainSceneController.Instance.GameBackend.SetUsername(_inputFieldName.text);
			_userAnalyticHandler.TrackUserDisplayNameProperties(_inputFieldName.text);
		}

        public void SetupAudio()
        {
            PlayBGM();
            
            //_titleScreenUI.PlayButton.onClick.AddListener(() => PlaySFX(TitleAudioKey.BUTTON_GENERAL));

            //foreach (TitleScreenUI.LoginMode loginMode in _loginModes)
            //    _titleScreenUI.GetLoginButton(loginMode).onClick.AddListener(() => PlaySFX(TitleAudioKey.BUTTON_GENERAL));

            _agreementController.SetupAudio(_audio);
            _inputUsername.SetupAudio(_audio);
        }

        private async Task StartGame()
        {
            //PlaySFX(MainSceneLauncher.AUDIO_KEY.BUTTON_PLAY);
            _titleScreenUI.PlayButton.gameObject.SetActive(false);
            MainSceneController.Instance.Loading.StartTitleLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            await Task.Delay(700);
            //MainSceneController.Instance.Loading.DoneLoading();
            _titleAnalyticHandler.TrackTapToPlayEvent();
            try
            {
                MainSceneController.Instance.SetupUser();
                await MainSceneController.Instance.MainRequestController.FetchMainData();
                await MainSceneController.Instance.LoadMainAsset();

                LoadSceneHelper.LoadScene(_lobbyScene, false);
            }
            catch(Exception ex)
            {
                _titleScreenUI.PlayButton.gameObject.SetActive(true);
                Debug.Log("lobby error - " + ex.Message);
            }
        }

        public void PlayBGM()
        {
            _audio.PlayBgm(TitleAudioKey.BGM);
        }

        public void PlaySFX(string audioKey)
        {
            _audio.PlaySfx(audioKey);
        }

        public void MuteBGM()
        {
            throw new NotImplementedException();
        }

        public void MuteSFX()
        {
            throw new NotImplementedException();
        }
    }
}
