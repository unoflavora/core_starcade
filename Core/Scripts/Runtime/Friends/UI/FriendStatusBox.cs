using System;
using TMPro;
using UnityEngine;

namespace Agate.Starcade.Lobby.Script.UserProfile.FriendsManager.UI
{
    public class FriendStatusBox : MonoBehaviour
    {
        [SerializeField] private GameObject _addedText;
        [SerializeField] private GameObject _recommendationFromText;
        [SerializeField] private TextMeshProUGUI _recommendationText;

        private void Start()
        {
            _recommendationText = _recommendationFromText.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetAddedTextActive(bool isEnable)
        {
            _recommendationFromText.SetActive(false);
            
            _addedText.SetActive(isEnable);
        }
        
        public void SetRecommendationText(bool isEnable, string recommendationText = "")
        {
            _addedText.SetActive(false);
            
            _recommendationFromText.SetActive(isEnable);
            
            _recommendationText.SetText(recommendationText);
        }
    }
}
