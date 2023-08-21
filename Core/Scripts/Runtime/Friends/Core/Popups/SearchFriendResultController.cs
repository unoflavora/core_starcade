using Agate.Starcade.Core.Scripts.Runtime.Friends.Backend.Data;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Runtime.Main;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.Core.Popups
{
    public class SearchFriendResultController : MonoBehaviour
    {
        [SerializeField] private FriendItemController _friendItemController;
        [SerializeField] private GameObject _emptyText;
        [SerializeField] private Button _onCloseButton;
        
        public void SearchData(FriendsResponseData friendData, UnityAction onClose)
        {
            
            if (friendData == null)
            {
                _emptyText.SetActive(true);
                _friendItemController.gameObject.SetActive(false);
            }
            else
            {
                switch (friendData.Status.status)
                {
                    case FriendRequestStatus.Approved:
                        friendData.Profile.Category = FriendCategory.Friend;
                        break;
                    case FriendRequestStatus.PendingApproval:
                        friendData.Profile.Category = FriendCategory.FriendRequest;
                        break;
                    case FriendRequestStatus.NotFriend:
                        friendData.Profile.Category = FriendCategory.Recommendation;
                        break;
                }
                
                _friendItemController.gameObject.SetActive(true);
                
;                _emptyText.SetActive(false);

                _friendItemController.Initialize(friendData.Profile, friendData.Profile.FriendCode != MainSceneController.Instance.Data.UserData.FriendCode);
            }
            
            AddListenerOnClose(onClose);

        }

        private void AddListenerOnClose(UnityAction onClose)
        {
            _onCloseButton.onClick.RemoveAllListeners(); 

            _onCloseButton.onClick.AddListener(() =>
            {
                onClose?.Invoke();
            });
        }
    }
}
