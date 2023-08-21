using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Main;
using Starcade.Core.ExtensionMethods;
using Starcade.Core.Friends.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Lobby.User_Profile.Friends_Manager.UI
{
    public class CopyUserId : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _userId;
        [SerializeField] private Button _copyButton;
        
        private void Start()
        {
            _copyButton.onClick.AddListener(() =>
            {
                MainSceneController.Instance.Analytic.LogEvent(FriendAnalyticEventHandler.ANALYTIC_KEY.COPY_FRIENDCODE_EVENT);

                // Remove "Your ID: " from the string
                string output = _userId.text.Replace("Your ID: ", "").Replace(" <sprite=0>", "");
                
                Debug.Log("Copied to clipboard: " + output);
                
                // Copy to clipboard
                output.CopyToClipboard();
            });
        }
    }
}