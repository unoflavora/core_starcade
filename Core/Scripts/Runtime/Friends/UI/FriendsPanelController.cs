using Agate.Starcade.Core.Scripts.Runtime.Friends.Core;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Lobby.User_Profile.Friends_Manager.UI
{
    public class FriendsPanelController : MonoBehaviour
    {
        [SerializeField] private int minChildObject;
        [SerializeField] private GameObject _noFriendText;
        [SerializeField] private GameObject _viewport;
        public Transform friendItems => _viewport.transform;
        
        private int _itemCount;
        void Update()
        {
            // minChildObject is the minimum number of child object, excluding title etc.
            _noFriendText.SetActive(friendItems.childCount < minChildObject);
        }
    }
}
