using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Game
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _semanticVersion;
        [SerializeField] private Button _tncButton;
        [SerializeField] private Button _liveSupport;
        [SerializeField] private Button _privacyButton;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _webviewPopup;
        [SerializeField] private WebviewController _webviewController;
        [SerializeField] private AudioSettingsController _audioSettingsController;

        private string _termsUrl;
        private string _liveSupportUrl;
        private string _privacyUrl;
        private PermissionAnalyticEventHandler _permissionAnalyticEvent { get; set; }

        private bool _isChange;

        protected void InitSettings()
        {
            //gameObject.SetActive(false);
            if (_termsUrl != null)
                _termsUrl = MainSceneController.Instance.Data.TermsAndConditionData.Config.TermsOfServiceUrl;
            if (_liveSupportUrl != null)
                _liveSupportUrl = MainSceneController.Instance.Data.TermsAndConditionData.Config.TermsOfServiceUrl;
            if (_privacyUrl != null)
                _privacyUrl = MainSceneController.Instance.Data.TermsAndConditionData.Config.PrivacyPolicyUrl;
            if (_tncButton != null) _tncButton.onClick.AddListener(() => OpenWebview(_termsUrl));
            if (_liveSupport != null) _liveSupport.onClick.AddListener(() => OpenWebview(_liveSupportUrl));
            if (_privacyButton != null) _privacyButton.onClick.AddListener(() => OpenWebview(_privacyUrl));
            if (_webviewController != null && _webviewPopup != null)
                _webviewController.onClose = () => _webviewPopup.SetActive(false);

            _permissionAnalyticEvent = new PermissionAnalyticEventHandler(MainSceneController.Instance.Analytic);
            EnableSettingsPanel();
        }

        private void OnDisable()
        {
            DisableSettingsPanel();
        }

        private void OnEnable()
        {
            EnableSettingsPanel();
        }

        private void OpenWebview(string url)
        {
            _webviewPopup.SetActive(true);
            _webviewController.gameObject.SetActive(true);
            _permissionAnalyticEvent.TrackClickTermsOfServiceEvent();
            StartCoroutine(_webviewController.OpenWebView(url));
        }

        private void EnableSettingsPanel()
        {
            if (_scrollRect != null) _scrollRect.verticalNormalizedPosition = 1f;
            _audioSettingsController.EnableAudioSettings();

            if (_semanticVersion != null)
                _semanticVersion.text = "Version " + Application.version + " (BETA)"+
                                        "\nSTARCADE. All Right Reserved. \n starcade@starcade.com";
        }

        private void DisableSettingsPanel()
        {
            _audioSettingsController.DisableAudioSettings();
            if (!_isChange) return;
            MainSceneController.Instance.Audio.SaveVolumeData();
            _isChange = false;
        }
    }
}