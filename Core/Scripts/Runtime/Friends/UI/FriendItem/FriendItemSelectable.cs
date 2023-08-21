using System;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Core;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.UI.FriendItem
{
    public class FriendItemSelectable : MonoBehaviour
    {
        [SerializeField] private GameObject _selected;
        
        private FriendItemController _friendItemController;
        public bool IsSelected { get; set; }
        public long Id => _friendItemController.FriendProfile.FriendCode;

        private void Start()
        {
            _friendItemController = GetComponent<FriendItemController>();
        }

        void Update()
        {
            _selected.SetActive(IsSelected);
        }
        
        // Register on item clicked event
        // Used when friend item click mechanic is needed (see implementations)
        public void RegisterOnItemClicked(UnityAction<FriendProfile> onClick)
        {
            var button = GetComponent<Button>();
            if (button == null) return;
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick(_friendItemController.FriendProfile));
        }
    }
}
