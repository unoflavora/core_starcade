using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Boot
{
    public class ErrorScreen : MonoBehaviour
    {
        public enum ErrorType
        {
            IsCloseable,
            MustRetry,
            MustRestartGame,
        }
        
        [SerializeField] private TMP_Text _errorMessage;
        [SerializeField] private UnityEvent _onRetry;
        
        [SerializeField] private StarcadeError[] _starcadeErrors;

        [SerializeField] private Image _errorIcon;
        
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _closeGameButton;

        public void Init(StarcadeErrorList starcadeErrorList)
        {
            _starcadeErrors = starcadeErrorList.ListStarcadeError;
            
            _closeButton.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });
            
            _restartButton.onClick.AddListener(() =>
            {
                Debug.Log("TODO IMPLEMENTATION");
            });
            
            _closeGameButton.onClick.AddListener(Application.Quit);
        }
        
        public void TriggerErrorWithMessage(string errorMessage, UnityAction onRetryAction, ErrorType errorType)
        {
            this.gameObject.SetActive(true);
            _errorMessage.text = errorMessage;
        }

        public void TriggerError(StarcadeErrorCode errorCode)
        {
            StarcadeError currentError = FindError((int)errorCode);
            this.gameObject.SetActive(true);
            _errorMessage.text = currentError.ErrorMessage;
            SetUI(currentError.ErrorType);
        }
        
        public void TriggerError(int errorCode)
        {
            StarcadeError currentError = FindError(errorCode);
            Debug.LogError("current error " + errorCode);
            this.gameObject.SetActive(true);
            _errorMessage.text = currentError.ErrorMessage;
            _errorIcon.sprite = currentError.ErrorIcon;
            SetUI(currentError.ErrorType);
        }

        private StarcadeError FindError(int errorCode)
        {
            foreach (var starcadeError in _starcadeErrors.Where(starcadeError => starcadeError.ErrorCode == errorCode))
            {
                return starcadeError;
            }
            return new StarcadeError()
            {
                ErrorCode = 1000,
                ErrorMessage = "Something Wrong Happen"
            }; //return error code 1000 if not implemented
        }

        private void SetUI(ErrorType errorType)
        {
            switch (errorType)
            {
                case ErrorType.IsCloseable:
                    _closeButton.gameObject.SetActive(true);
                    _restartButton.gameObject.SetActive(false);
                    _closeGameButton.gameObject.SetActive(false);
                    break;
                case ErrorType.MustRetry:
                    _closeButton.gameObject.SetActive(false);
                    _restartButton.gameObject.SetActive(true);
                    _closeGameButton.gameObject.SetActive(true);
                    break;
                case ErrorType.MustRestartGame:
                    _closeButton.gameObject.SetActive(false);
                    _restartButton.gameObject.SetActive(false);
                    _closeGameButton.gameObject.SetActive(true);
                    break;
                default:
                    _closeButton.gameObject.SetActive(false);
                    _restartButton.gameObject.SetActive(true);
                    _closeGameButton.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
