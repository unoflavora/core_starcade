using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Core.Runtime.Game;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Core;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Lobby;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Data_Class.Account;
using Agate.Starcade.Scripts.Runtime.Info;
using Newtonsoft.Json;
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

namespace Agate.Starcade.Scripts.Runtime.Game
{
	public class UserProfileController : SceneAdditiveBehaviour
    {
        private UserNameData _userProfileData;

        [SerializeField] private RectTransform _bodyRectTransform;

        [SerializeField] private TMP_Text _username;

        [SerializeField] private Image _photoProfile;
        [SerializeField] private Image _frameProfile;
        [SerializeField] private Button _editPhotoProfile;
        //[SerializeField] private EditPhotoProfileController _editPhotoProfileController;

        [SerializeField] private Button _surveyButton;

        [SerializeField] private Button _googleBindingButton;
        [SerializeField] private Button _appleBindingButton;
        [SerializeField] private Button _facebookBindingButton;
        [SerializeField] private Button _logoutAccountButton;

        [SerializeField] private GameObject _nameInputPanel;

        [SerializeField] private TMP_Text _level;
        [SerializeField] private TMP_Text _levelProgress;
        [SerializeField] private Slider _levelBar;
        [SerializeField] private TMP_Text _nextMilestoneLevel;
        [SerializeField] private Image[] _milestoneRewards;

        [Header("Tab Button")] 
        [SerializeField] private Toggle _myProfileTab;
        [SerializeField] private Toggle _collectionTab;
        [SerializeField] private Toggle _friends;
        [SerializeField] private Toggle _statistcTab;
        
        [Header("Notifications")]
        [SerializeField] private NotificationBadge _friendsNotification;
        
        [Header("Scenes")]
        [SerializeField] private AssetReference _editProfileScene;
        [SerializeField] private AssetReference _collectibleScene;
        [SerializeField] private GameObject _friendsPanel;
        private AuthAnalyticEventHandler _authAnalyticEvent { get; set; }
        private UserAnalyticEventHandler _userAnalyticEvent { get; set; }
        private UserProfileAnalyticEventHandler _userProfileAnalyticEvent { get; set; }

        private UnityEvent<string> _onSuccessBinding = new UnityEvent<string>();
        
        private InitAdditiveBaseData _initData;

        private AudioController _audio;
        private UserProfileTab _currActiveUserProfileTab = UserProfileTab.MyProfile;
        public static Action<LobbyMenuEnum> BackToLobby;

        //private GameBackendController _gameBackend;

        [HideInInspector] public UnityEvent OnProfileUpdated = new UnityEvent();


        private async void Start()
        {
            Debug.Log("start init");

			_authAnalyticEvent = new AuthAnalyticEventHandler(MainSceneController.Instance.Analytic);
			_userAnalyticEvent = new UserAnalyticEventHandler(MainSceneController.Instance.Analytic);
			_userProfileAnalyticEvent = new UserProfileAnalyticEventHandler(MainSceneController.Instance.Analytic);
            OnOpenPanel.AddListener(SetData);
			//INIT BUTTON
#if UNITY_EDITOR
			BindButtonSetup(PlatformType.Editor);
#elif UNITY_IOS
            BindButtonSetup(PlatformType.IOS);
#else
            BindButtonSetup(PlatformType.Android);
#endif

			_surveyButton?.onClick.AddListener(() =>
			{
				MainSceneController.Instance.OpenSurvey("user_profile");
			});
			
            _username.text = MainSceneController.Instance.Data.UserData.Username;
            
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            await InitExperience();
            await InitItemAsset();
            InitTabListeners();
            InitFriends();

            SetPlayerAccessories();
            MainSceneController.Instance.Data.UserProfileAction.OnAccessoryChanged += UpdatePlayerAccessories;
            //OnProfileUpdated.AddListener(SetPlayerAccessories);
            //MainSceneController.Instance.Data.AccessoryData.UserAccessories[ItemTypeEnum.Avatar].OnAccessoryChanged.AddListener(SetPlayerAvatar);
            //MainSceneController.Instance.Data.AccessoryData.UserAccessories[ItemTypeEnum.Frame].OnAccessoryChanged.AddListener(SetPlayerFrame);

            _editPhotoProfile.onClick.AddListener(OnOpenEditProfile);

            MainSceneController.Instance.Data.ExperienceData.OnExperienceChanged.AddListener(OnExperienceChanged);
            MainSceneController.Instance.Data.ExperienceData.OnExperienceChanged.Invoke();

            SetMilestoneRewards();

            _bodyRectTransform.gameObject.SetActive(true);
            _audio = MainSceneController.Instance.Audio;
            
            _initData = LoadSceneHelper.PullData();
            
            LoadSceneHelper.ClearData(); //HOTFIX

            MainSceneController.Instance.Loading.DoneLoading();

            _collectionTab.isOn = _initData != null && _initData.Key == LobbyMenuEnum.Collection;
        }

        private void InitTabListeners()
        {
	        _myProfileTab.onValueChanged.AddListener(arg0 => 
            { 
                SetUserProfileTab(UserProfileTab.MyProfile, arg0);
				if (arg0) _userProfileAnalyticEvent.TrackClickMyProfileTabMenuEvent();
			});
	        _collectionTab.onValueChanged.AddListener(arg0 => 
            { 
                SetUserProfileTab(UserProfileTab.Collectible, arg0);
				if (arg0) _userProfileAnalyticEvent.TrackClickCollectibleTabMenuEvent();
			});
	        _friends.onValueChanged.AddListener(arg0 => 
            { 
                SetUserProfileTab(UserProfileTab.Friends, arg0);
				if (arg0) _userProfileAnalyticEvent.TrackClickFriendlistTabMenuEvent();

			});
	        _statistcTab.onValueChanged.AddListener(arg0 => 
            { 
                SetUserProfileTab(UserProfileTab.Statistic, arg0);
				if (arg0) _userProfileAnalyticEvent.TrackClickStatisticTabMenuEvent();

			});
	        _closeButton.onClick.AddListener(async () =>
	        {
		        // There's another callback on SceneAdditiveBehaviour
		        // Then after that callback is called, below function is called;
		        await CloseAllAdditiveScenes();
		        _initData?.OnClose();
	        });

	        BackToLobby = async (menu) =>
	        {
		        await CloseAllAdditiveScenes();
		        
		        LoadSceneHelper.CloseSceneAdditive();
	        
		        _initData.OnClose(menu);
	        };
        }

        private async Task CloseAllAdditiveScenes()
        {
	        _userProfileAnalyticEvent.TrackClickCloseUserProfiileEvent(_currActiveUserProfileTab.ToString());
	        switch (_currActiveUserProfileTab)
	        {
		        case UserProfileTab.MyProfile:
			        return;
		        case UserProfileTab.Collectible:
			        var task = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 2));
			        while (!task.isDone)
			        {
				        await Task.Yield();
			        }
			        
			        break;
	        }
        }
        

        private void SetUserProfileTab(UserProfileTab tab, bool isActive)
        {
            try
            {
                if (tab == UserProfileTab.Collectible && isActive)
                {
	                if (_currActiveUserProfileTab != UserProfileTab.Collectible)
		                LoadSceneHelper.LoadSceneAdditive(_collectibleScene, null as InitAdditiveBaseData);
                }
                else if (_currActiveUserProfileTab == UserProfileTab.Collectible)
				{
                    //TODO: NEED TO PROPERLY CHECK SCENE LOADED OR NOT
                    //_collectibleScene.UnLoadScene();
                    
                    LoadSceneHelper.CloseSceneAdditive();
                    //SceneManager.UnloadSceneAsync(_collectibleScene);
                }
            }
            catch { }
			_currActiveUserProfileTab = tab;
		}

        private async Task InitExperience()
        {
            MainSceneController.Instance.Data.ExperienceData.OnMilestoneReached.AddListener(GetMilestoneRewardEvent);
            UserExperienceData experience = MainSceneController.Instance.Data.ExperienceData;
            bool isLoaded = experience.NextMilestone?.Level > experience.Data.Level;
            if (!isLoaded)
                await GetMilestoneReward();
        }
        
        private void InitFriends()
        {
	        var friendData = MainSceneController.Instance.Data.FriendsData;
	        
	        if(friendData == null)
		        return;
	        
	        var notifyCount = 
		        friendData.Friends.Count(friend => FriendsPrefManager.FriendExistsInPlayerPrefs(friend.Profile.FriendCode) == false) 
		        + friendData.Pendings.Count(friend => FriendsPrefManager.FriendRequestExistsInPlayerPrefs(friend.Profile.FriendCode) == false);

	        if(notifyCount > 0)
				_friendsNotification.EnableBadgeWithCounter(notifyCount);
	        else
		        _friendsNotification.DisableBadge();
        }
        
        private async Task InitItemAsset()
        {
            await MainSceneController.Instance.AssetLibrary.LoadAllAssets();
        }

        private async void GetMilestoneRewardEvent()
        {
            await GetMilestoneReward();
        }

        private async Task GetMilestoneReward()
        {
            var response = await MainSceneController.Instance.GameBackend.GetNextMilestoneReward();
            if (response.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(response.Error.Code);
            }
            MainSceneController.Instance.Data.ExperienceData.NextMilestone = response.Data;
			MainSceneController.Instance.Data.ExperienceData.NextLevel = response.Data;
		}

        private void SetData()
        {
            _username.text = MainSceneController.Instance.Data.UserData.Username;
            Debug.Log("pulling data");
            _initData = LoadSceneHelper.PullData();
            LoadSceneHelper.ClearData(); //HOTFIX
        }

        private void OnExperienceChanged()
        {
            var levelData = MainSceneController.Instance.Data.ExperienceData.Data;
            _level.text = $"<sprite=0>{levelData.Level}";
            var expProgress = levelData.Experience - levelData.BottomLevelUpExp;
            var expMax = levelData.NextLevelUpExp - levelData.BottomLevelUpExp;
            _levelProgress.text = $"{expProgress}/{expMax}";
            //_levelBar.fillAmount = (float)expProgress / (float)expMax;
			var targetExp = (float)expProgress / (float)expMax;
            _levelBar.value = targetExp;
			//_levelBar.localScale = new Vector3((float)expProgress / (float)expMax, 1, 1);
		}

        private void RemoveAllExperienceListeners()
        {
            MainSceneController.Instance.Data.ExperienceData.OnExperienceChanged.RemoveListener(OnExperienceChanged);
            MainSceneController.Instance.Data.ExperienceData.OnMilestoneReached.RemoveListener(GetMilestoneRewardEvent);
        }

        private void SetMilestoneRewards()
        {
            NextExperienceRewardData reward = MainSceneController.Instance.Data.ExperienceData.NextMilestone;
            _nextMilestoneLevel.text = "-";
            
            if (reward.RewardGain == null) return;
            
            _nextMilestoneLevel.text = $"<sprite=0>{reward.Level}";
            for (int i = 0; i < reward.RewardGain.Length; i++)
            {
                Sprite sprite = null;
                if (reward.RewardGain[i].Type != RewardEnum.GoldCoin.ToString() ||
                    reward.RewardGain[i].Type != RewardEnum.StarCoin.ToString() ||
                    reward.RewardGain[i].Type != RewardEnum.StarTicket.ToString()) 
                    sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.RewardGain[i].ItemId);
                if (sprite != null)
                {
                    _milestoneRewards[i].sprite = sprite;
                    _milestoneRewards[i].gameObject.SetActive(true);
                }
            }
        }

        private void OnDestroy()
        {
            //MainSceneController.Instance.Data.AccessoryData.UserAccessories[ItemTypeEnum.Avatar].OnAccessoryChanged.RemoveListener(SetPlayerAvatar);
            //MainSceneController.Instance.Data.AccessoryData.UserAccessories[ItemTypeEnum.Frame].OnAccessoryChanged.RemoveListener(SetPlayerFrame);
            //if(MainSceneLauncher.Instance.photoUrl != String.Empty) _lobbyUI.UpdatePhotoProfile();
            
            MainSceneController.Instance.Data.UserProfileAction.OnAccessoryChanged -= UpdatePlayerAccessories;

            //RemoveAllExperienceListeners();
            Debug.Log("PROFILE DESTROYED");
        }

        private void BindButtonSetup(PlatformType platformType)
        {
            switch (platformType)
            {
                case PlatformType.Editor:
                    _googleBindingButton?.gameObject.SetActive(true);
                    _appleBindingButton?.gameObject.SetActive(false);
                    _facebookBindingButton?.gameObject.SetActive(true);
                    break;
                case PlatformType.Android:
                    _googleBindingButton?.gameObject.SetActive(true);
                    _appleBindingButton?.gameObject.SetActive(false);
                    _facebookBindingButton?.gameObject.SetActive(true);
                    break;
                case PlatformType.IOS:
                    _googleBindingButton?.gameObject.SetActive(false);
                    _appleBindingButton?.gameObject.SetActive(true);
                    _facebookBindingButton?.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platformType), platformType, null);
            }
            
            //KEEP FACEBOOK BINDING FOR NOW
            _facebookBindingButton?.gameObject.SetActive(true);
            
            _logoutAccountButton?.onClick.AddListener(SignOut);
            _googleBindingButton?.onClick.AddListener(async () => await BindGoogleAccount());
            _appleBindingButton?.onClick.AddListener(async () => await BindAppleAccount());
            _facebookBindingButton?.onClick.AddListener(async () => await BindFacebookAccount());
            Debug.Log("FINISH SETUP BUTTON");
        }

        public async Task BindGoogleAccount()
        {
			Debug.Log("BINDING GOOGLE ACCOUNT");
			if (MainSceneController.Instance.Data.UserAccounts.ContainsKey(AccountTypesEnum.Google))
			{
				MainSceneController.Instance.Info.ShowDelay(InfoType.BindingAlreadyAuthenticated, null, null, 3);
                return;
			}

			_authAnalyticEvent.TrackClickBindAccountEvent(LoginState.GOOGLE.ToString());

			var res = await MainSceneController.Instance.Auth.BindGoogle();
            if(res.Data != null && res.Error == null)
            {
                BindSuccessUserProfile(res.Data, AccountTypesEnum.Google);
			}
        }

		public async Task BindAppleAccount()
		{
			if (MainSceneController.Instance.Data.UserAccounts.ContainsKey(AccountTypesEnum.Apple))
			{
				await MainSceneController.Instance.Info.ShowDelay(InfoType.BindingAlreadyAuthenticated, null, null, 3);
				return;
			}

			_authAnalyticEvent.TrackClickBindAccountEvent(LoginState.APPLE.ToString());

			var res = await MainSceneController.Instance.Auth.BindApple();
			if (res.Data != null && res.Error == null)
			{
				BindSuccessUserProfile(res.Data, AccountTypesEnum.Apple);
			}
		}
        
        public async Task BindFacebookAccount()
        {
            var userAccounts = MainSceneController.Instance.Data.UserAccounts;

            var isAuthenticated = userAccounts.ContainsKey(AccountTypesEnum.Google) || userAccounts.ContainsKey(AccountTypesEnum.Apple);
			if (!isAuthenticated)
            {
                var desc = "";
#if UNITY_IOS
                desc = "Bind your account with Apple ID first before connecting to Facebook.";
#else
				desc = "Bind your account with Google Account first before connecting to Facebook.";
#endif

				MainSceneController.Instance.Info.Show(desc, "Bind Facebook Failed", InfoIconTypeEnum.Error, null, new InfoAction("Close", null));
				return;
			}else if (MainSceneController.Instance.Data.UserAccounts.ContainsKey(AccountTypesEnum.Facebook))
            {
				await MainSceneController.Instance.Info.ShowDelay(InfoType.BindingAlreadyAuthenticated, null, null, 3);
				return;
			}
            _authAnalyticEvent.TrackClickBindAccountEvent(LoginState.FACEBOOK.ToString());

			var res = await MainSceneController.Instance.Auth.BindFacebook();
			if (res.Data != null && res.Error == null)
			{
				BindSuccessUserProfile(res.Data, AccountTypesEnum.Facebook);
			}
		}
        
        private void BindSuccessUserProfile(LoginData loginData, AccountTypesEnum accountTypesEnum)
        {
            if(loginData == null) return; //HOTFIX
            
            _authAnalyticEvent.TrackBindAccountSuccessEvent(accountTypesEnum.ToString());
            
            //update photo and username
            string newUsername = _nameInputPanel.GetComponent<InputUsernameController>().CurrentNewUsername;

            if (accountTypesEnum == AccountTypesEnum.Google)
            {
                //HARDCODED STATE REWARD FIX MORE BETTER APPROACH LATER
                string data = PlayerPrefs.GetString("PosterState");
                List<PosterStateReward> list = new List<PosterStateReward>();
                list = JsonConvert.DeserializeObject<List<PosterStateReward>>(data);
                list.Find(reward => reward.RewardId == "bind_account").IsDone = true;
                PlayerPrefs.SetString("PosterState", JsonConvert.SerializeObject(list));
            }
            
            //update photo and username
            MainSceneController.Instance.Data.AccessoryData.PhotoURL = loginData.Data.PhotoUrl;

            if (accountTypesEnum is AccountTypesEnum.Google or AccountTypesEnum.Apple)
            {
                _nameInputPanel.GetComponent<InputUsernameController>().successInputUsername += OnSuccessInputUsername;
                _nameInputPanel.SetActive(true);
            }

			_userAnalyticEvent.TrackAccountTypeProperties(accountTypesEnum.ToString());
		}

        private async void OnSuccessInputUsername(string username)
        {
            MainSceneController.Instance.Data.UserData.Username = username;
            _username.text = username;
            _nameInputPanel.GetComponent<InputUsernameController>().successInputUsername -= OnSuccessInputUsername;


            await MainSceneController.Instance.GameBackend.DownloadPhotoProfile();
            _photoProfile.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentAvatar();

            _userAnalyticEvent.TrackUserDisplayNameProperties(username);
        }

        public void SetPlayerAccessories()
        {
            _photoProfile.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentAvatar();
            _frameProfile.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentFrame();
        }

        private void UpdatePlayerAccessories(Dictionary<ItemTypeEnum, string> data)
        {
            if (data.ContainsKey(ItemTypeEnum.Avatar))
            {
                if (_photoProfile == null) Debug.Log("PHOTO PROFILE IMAGE MISSING!");
                _photoProfile.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(data[ItemTypeEnum.Avatar]);
            }

            if (data.ContainsKey(ItemTypeEnum.Frame))
            {
                _frameProfile?.gameObject.SetActive(true);
                _frameProfile.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(data[ItemTypeEnum.Frame]);
            }
        }

        private void SetPlayerAvatar()
        {
            _photoProfile.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentAvatar();
        }

        private void SetPlayerFrame()
        {
            if (MainSceneController.Instance.Data.UserProfileData.GetCurrentAvatar() != null)
            {
                _frameProfile.sprite = MainSceneController.Instance.Data.UserProfileData.GetCurrentAvatar();
                _frameProfile.gameObject.SetActive(true);
            }
            else
            {
                _frameProfile.gameObject.SetActive(false);
            }
        }

        public void SignOut()
        {
			if (!MainSceneController.Instance.Auth.IsAuthenticated()) // GUEST SIGNOUT
			{
                Debug.Log("logout as guest");
				MainSceneController.Instance.Info.Show(
					WarningType.GuestLogOutWarning,
					new InfoAction("Yes", delegate
					{
						MainSceneController.Instance.Auth.SignOut();
						MainSceneController.Instance.Analytic.Reset();
                        _authAnalyticEvent.TrackLogoutEvent();
					}),
					new InfoAction("No", null));
			}
			else
			{
                Debug.Log("logout as google");
                MainSceneController.Instance.Info.Show(
					WarningType.LogOutWarning,
					new InfoAction("Yes", delegate
					{
						MainSceneController.Instance.Auth.SignOut();
						MainSceneController.Instance.Analytic.Reset();
						_authAnalyticEvent.TrackLogoutEvent();
					}),
					new InfoAction("No", null));
			}
        }

        private void OnOpenEditProfile()
        {
            _userProfileAnalyticEvent.TrackClickEditProfiileEvent();
            LoadSceneHelper.LoadSceneAdditive(_editProfileScene, OnProfileUpdated);
        }
    }

    public class UserNameData
    {
        public string Username;
    }

	public enum UserProfileTab
	{
		MyProfile,
        Collectible,
        Friends,
        Statistic
	}

    public enum PlatformType
    {
        Editor,
        Android,
        IOS
    }
}