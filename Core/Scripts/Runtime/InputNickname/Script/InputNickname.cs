using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Agate.Starcade.Boot
{
    public class InputNickname : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI debuggerText;
        [SerializeField] TMP_InputField inputField;
        [SerializeField] float NextSceneWaitTime = 1f;

        [SceneReference, SerializeField]
        private string _nextScene = default;



        private string usernameKey = "username";

        public void Submit()
        {
            PlayerPrefs.SetString(usernameKey, inputField.text);
            debuggerText.SetText("Username: " + inputField.text);

            Invoke("LoadNextScene", NextSceneWaitTime);
        }

        public void LoadNextScene()
        {
            SceneManager.LoadScene(_nextScene);
        }
        
        
    }

}
