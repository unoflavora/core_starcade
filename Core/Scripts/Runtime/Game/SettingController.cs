using System;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Scripts.Runtime.UI;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Game
{
    public class SettingController : SceneAdditiveBehaviour
    {

        [SerializeField] private Slider _bgmVolume;
        [SerializeField] private Slider _sfxVolume;
        [SerializeField] private TMP_Text _semanticVersion;

        [SerializeField] private Button _tncButton;
        [SerializeField] private Button _liveSupport;
        [SerializeField] private Button _privacyButton;
        [SerializeField] private Button _surveyButton;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _webviewPopup;
        [SerializeField] private WebviewController _webviewController;

        [SerializeField] private Button _exitButton;

        [SerializeField] private SliderToggleGroup _FPSGroupToggle;
        [SerializeField] private SliderToggle _lowFPSToggle;
        [SerializeField] private SliderToggle _highFPSToggle;

        private string _termsUrl;
        private string _liveSupportUrl;
        private string _privacyUrl;

        private PermissionAnalyticEventHandler _permissionAnalyticEvent { get; set; }

        public Slider bgmVolume => _bgmVolume;

        public Slider sfxVolume => _sfxVolume;

        private bool _isChange;
        
        
        private void Start()
        {
            //gameObject.SetActive(false);
            _termsUrl = MainSceneController.Instance.Data.TermsAndConditionData.Config.TermsOfServiceUrl;
            _liveSupportUrl = MainSceneController.Instance.Data.TermsAndConditionData.Config.TermsOfServiceUrl;
            _privacyUrl = MainSceneController.Instance.Data.TermsAndConditionData.Config.PrivacyPolicyUrl;

            _surveyButton.onClick.AddListener(() => OpenSurvey());
            _tncButton.onClick.AddListener(() => OpenWebview(_termsUrl));
            _liveSupport.onClick.AddListener(() =>
            {
                MainSceneController.Instance.OpenSurvey("setting");
			});
            _privacyButton.onClick.AddListener(() => OpenWebview(_privacyUrl));

            _webviewController.onClose = () => _webviewPopup.SetActive(false);

            _permissionAnalyticEvent = new PermissionAnalyticEventHandler(MainSceneController.Instance.Analytic);

            Setup();
            
            OnClosePanel.AddListener(Close);
            
            //_fpsToggle.onValueChanged.AddListener(UpdateFPS);
            
            _lowFPSToggle.OnToggleActive.AddListener(() =>
            {
                UpdateFPS(30);
            });
            
            _highFPSToggle.OnToggleActive.AddListener(() =>
            {
                UpdateFPS(60);
            });

            InitFPSToggle();

            _bgmVolume.onValueChanged.AddListener(UpdateVolume);
            _sfxVolume.onValueChanged.AddListener(UpdateVolume);
        }

        private void OnDisable()
        {
            DisableSettingsPanel();
        }

        public void OpenSurvey()
        {
            MainSceneController.Instance.OpenSurvey("setting");
        }

        private void OpenWebview(string url)
        {
#if !UNITY_EDITOR
            _webviewPopup.SetActive(true);
            _webviewController.gameObject.SetActive(true);
#endif

            StartCoroutine(_webviewController.OpenWebView(url));
            _permissionAnalyticEvent.TrackClickTermsOfServiceEvent();
        }

        public void Close()
        {
            if (_isChange)
            {
                MainSceneController.Instance.Audio.SaveVolumeData();
                _isChange = false;
            }
		}

        public void UpdateVolume(float volume)
        {
            //Debug.Log($"UPDATE AUDIO BGM: {_bgmVolume.value}, SFX: {_sfxVolume.value}");
            MainSceneController.Instance.Audio.UpdateVolume(_bgmVolume.value, _sfxVolume.value);
            _isChange = true;
        }

        void UpdateFPS(int fps)
        {
            Application.targetFrameRate = fps;
            AppSystemSetting newAppData = MainSceneController.Instance.AppSystemSetting;
            newAppData.TargetFrameRate = fps;
            MainSceneController.Instance.UpdateAppSystemSetting(newAppData);
        }

        void InitFPSToggle()
        {
            int currentFPS = MainSceneController.Instance.AppSystemSetting.TargetFrameRate;
            Debug.Log("init fps toggle = " + currentFPS);
            switch (currentFPS)
            {
                case 60:
                    _FPSGroupToggle.InitToggleGroup(_highFPSToggle);
                    break;
                case 30:
                    _FPSGroupToggle.InitToggleGroup(_lowFPSToggle);
                    break;
                default:
                    _FPSGroupToggle.InitToggleGroup(_lowFPSToggle);
                    break;
            }
        }

        public void Setup()
        {
            if (_bgmVolume == null) return; // _bgmVolume 
            
            //_scrollRect.verticalNormalizedPosition = 1f;

            Debug.Log("UPDATE VOLUME - BGM = " + MainSceneController.Instance.GameConfig.AudioConfig.BgmVolume);
            Debug.Log("UPDATE VOLUME - SFX = " + MainSceneController.Instance.GameConfig.AudioConfig.SfxVolume);

            _bgmVolume.value = MainSceneController.Instance.GameConfig.AudioConfig.BgmVolume;
            _sfxVolume.value = MainSceneController.Instance.GameConfig.AudioConfig.SfxVolume;

            Debug.Log("UPDATE VOLUME - SFX = " + MainSceneController.Instance.GameConfig.AudioConfig.SfxVolume);
            Debug.Log("UPDATE VOLUME - SFX VALUE = " + _sfxVolume.value);

            if (_semanticVersion) _semanticVersion.text = "Version "+ Application.version + "\nSTARCADE. All Right Reserved. \n starcade@starcade.com";
        }

        protected void DisableSettingsPanel()
        {
			if (!_isChange) return;
			MainSceneController.Instance.Audio.SaveVolumeData();
			_isChange = false;
		}
    }
}