using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.UI
{
    public class ErrorMessage : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private FadeTween _errorMessage;
        [SerializeField] private float _timeoutDuration = 5f;
        [SerializeField] private TextMeshProUGUI _errorText;

        private float _timeoutTimer;

        public async void SetErrorText(string text)
        {
            await Task.Delay(400);
            
            _errorText.SetText(text);
        }

        private void Start()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void Update()
        {
            // Check for mouse click
            if (Input.GetMouseButtonDown(0))
            {
                DisableErrorMessage();
            }
        
            // Check for timeout
            if (Time.time > _timeoutTimer)
            {
                DisableErrorMessage();
            }
        }

        private void OnButtonClicked()
        {
            _errorMessage.FadeIn();
            
            // Set the timeout timer when the script is enabled
            _timeoutTimer = Time.time + _timeoutDuration;
        }

        private void OnDisable()
        {
            // Disable the error message when the script is disabled
            DisableErrorMessage();
        }

        private void DisableErrorMessage()
        {
            _errorMessage.FadeOut();
        }

    }
}