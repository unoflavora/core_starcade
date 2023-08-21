using System;
using System.Collections;
using System.Collections.Generic;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class AgreementController : MonoBehaviour, IAudioPanel
    {
        [SerializeField] private Toggle _agreeToggle;
        [SerializeField] private Button _submitButton;
        [SerializeField] private Button _tncButton;
        [SerializeField] private Button _privacyButton;
        [SerializeField] private WebviewController _webviewController;

        private string _termsUrl;
        private string _privacyUrl;
        
        private PermissionAnalyticEventHandler _permissionAnalyticEvent { get; set; }
        private GameBackendController _gameBackend { get; set; }
        
        public AudioController Audio { get; set; }
        

        private void Awake()
        {
            _submitButton.onClick.AddListener(OnSubmitAgreement);
            _tncButton.onClick.AddListener(OpenTnc);
            _privacyButton.onClick.AddListener(OpenPrivacy);

            _permissionAnalyticEvent = new PermissionAnalyticEventHandler(MainSceneController.Instance.Analytic);
            _gameBackend = MainSceneController.Instance.GameBackend;
        }

        private void Update()
        {
            _submitButton.interactable = _agreeToggle.isOn;
        }
        
        public void OpenAgreement(TermsAndConditionConfig termsAndConditionConfig)
        {
            gameObject.SetActive(true);
            _permissionAnalyticEvent.TrackOpenPermissionEvent();
            _termsUrl = termsAndConditionConfig.TermsOfServiceUrl;
            _privacyUrl = termsAndConditionConfig.PrivacyPolicyUrl;
        }

        private void OpenTnc()
        {
            _webviewController.gameObject.SetActive(true);
            _permissionAnalyticEvent.TrackClickTermsOfServiceEvent();
            StartCoroutine(_webviewController.OpenWebView(_termsUrl));
        }

        private void OpenPrivacy()
        {
            _webviewController.gameObject.SetActive(true);
            _permissionAnalyticEvent.TrackClickPrivacyPolicyEvent();
            StartCoroutine(_webviewController.OpenWebView(_privacyUrl));
        }

        private async void OnSubmitAgreement()
        {
            var res = await RequestHandler.Request(_gameBackend.SignTermsAndCondition);
            if (res.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(res.Error.Code);
                return;
            }
            _permissionAnalyticEvent.TrackAcceptPermissionEvent();
            gameObject.SetActive(false);
            Debug.Log("Agreement submitted!");
        }

        public void SetupAudio(AudioController audio)
        {
            Audio = audio;
            //_privacyButton.onClick.AddListener(() => PlaySFX(TitleAudioKey.BUTTON_GENERAL));
            //_submitButton.onClick.AddListener(() => PlaySFX(TitleAudioKey.BUTTON_GENERAL));
            //_agreeToggle.onValueChanged.AddListener(value =>
            //{
            //    PlaySFX(TitleAudioKey.BUTTON_GENERAL);
            //});
            //_tncButton.onClick.AddListener(() => PlaySFX(TitleAudioKey.BUTTON_GENERAL));
        }

        public void PlaySFX(string audioKey)
        { 
            Audio.PlaySfx(audioKey);
        }
    }
}
