using System;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Lobby.Friends.UI.Navbar
{
    public class NavbarItemFriendsController : MonoBehaviour
    {
        [SerializeField] private Image _border;
        
        [SerializeField] private Toggle _toggle;
        
        private void OnGUI()
        {
            _border.enabled = _toggle.IsInteractable() && _toggle.isOn;
        }
    }
}
