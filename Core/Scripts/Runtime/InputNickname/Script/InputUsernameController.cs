using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class InputUsernameController : MonoBehaviour, IAudioPanel
    {
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private Button _submitButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private TMP_Text _feedbackText;

        private string _currentNewUsername;
        public string CurrentNewUsername => _currentNewUsername;

        public UnityAction<string> successInputUsername;

        public AudioController Audio { get; set; }
        
        private bool _isDone;
        void Start()
        {
            _nameInputField.onValueChanged.AddListener(OnCheck);
            _deleteButton.onClick.AddListener(() => _nameInputField.text = String.Empty);
        }

        private void Update()
        {
            _deleteButton.gameObject.SetActive(_nameInputField.text != string.Empty);
        }

        public async Task Wait()
        {
            await PanelStacking.StartWait();
        }
        

        private void OnCheck(string input)
        {
            _submitButton.interactable = (Regex.IsMatch(_nameInputField.text,@"^[a-zA-Z0-9_]+$") || (_nameInputField.text != string.Empty));
            //_deleteButton.gameObject.SetActive(_nameInputField.text != string.Empty);
        }
        
        public async void UpdateName()
        {
            if (_nameInputField.text == "Kasar")
            {
                MainSceneController.Instance.Info.Show(WarningType.NameInappropriate,new InfoAction("Close",null),null);
                return;
            }
            
            var result = await RequestHandler.Request(async() =>
                await MainSceneController.Instance.GameBackend.SetUsername(_nameInputField.text));
            if (result.Error != null)
            {
                SetFeedback("username already exists, try another");
            }
            else
            {
                _currentNewUsername = _nameInputField.text;
                MainSceneController.Instance.Data.UserData.Username = _nameInputField.text;
                successInputUsername?.Invoke(_nameInputField.text);
                PanelStacking.StopWait();
                gameObject.SetActive(false);
            }
        }

        private async Task SetFeedback(string text)
        {
            _feedbackText.text = text;
            await Task.Delay(3000);
            _feedbackText.text = String.Empty;
        }

        
        public void SetupAudio(AudioController audioController)
        {
            Audio = audioController;
            //_deleteButton.onClick.AddListener(() => PlaySFX(TitleAudioKey.BUTTON_NEGATIVE));
            //_submitButton.onClick.AddListener(() => PlaySFX(TitleAudioKey.BUTTON_GENERAL));
            _nameInputField.onValueChanged.AddListener(input =>
            {
                PlaySFX(input == String.Empty ? MainSceneController.AUDIO_KEY.BUTTON_NEGATIVE : MainSceneController.AUDIO_KEY.BUTTON_GENERAL);
                //Handheld.Vibrate();
            });
        }

        public void PlaySFX(string audioKey)
        {
            Audio.PlaySfx(audioKey);
        }
    }
}
