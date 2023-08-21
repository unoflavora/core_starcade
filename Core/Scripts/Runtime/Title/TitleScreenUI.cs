using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Runtime.Title
{
    public class TitleScreenUI : MonoBehaviour
    {
        public enum LoginMode
        {
            Google,
            IOS,
            Guest,
        }

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _googleLoginButton;
        [SerializeField] private Button _guestLoginButton;
        [SerializeField] private Button _iosLoginButton;
        [SerializeField] private Button _logOutButton;

        [SerializeField] private GameObject _infoText;
        
        public Button PlayButton => _playButton;
        public Button LogOutButton => _logOutButton;
        public GameObject InfoText => _infoText;

        public Button GetLoginButton(LoginMode loginButton)
        {
            if (loginButton == LoginMode.Google) return _googleLoginButton;
            else if (loginButton == LoginMode.IOS) return _iosLoginButton;
            else if (loginButton == LoginMode.Guest) return _guestLoginButton;
            else return null;
        }

        public void ShowLoginButton(List<LoginMode> loginModes, bool show)
        {
            foreach(LoginMode loginMode in loginModes) GetLoginButton(loginMode).gameObject.SetActive(show);
            _infoText.gameObject.SetActive(show);
		}

        public void SetUi(List<LoginMode> loginModes = null, bool isAlreadyLogin = false)
        {
            _playButton.gameObject.SetActive(isAlreadyLogin);
            _infoText.SetActive(!isAlreadyLogin);

            foreach(LoginMode loginMode in loginModes) 
                GetLoginButton(loginMode).gameObject.SetActive(!isAlreadyLogin);
        }
    }
}