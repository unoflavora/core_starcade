using System;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Lobby.Friends.UI.Navbar
{
    public class NavbarFriendsController : MonoBehaviour
    {
        private int _activeToggleID;
        
        private Action<string> _onNavbarValueChanged;

        // Event Action
        public void OnToggleValueChanged(Toggle toggle)
        {
            Debug.Log(toggle.GetInstanceID());
            
            // Prevents the same toggle from being selected twice
            if(_activeToggleID != 0 && toggle.GetInstanceID() == _activeToggleID) return;

            if (toggle.IsInteractable() == false) return;
            
            _onNavbarValueChanged?.Invoke(toggle.name);
            
            _activeToggleID = toggle.GetInstanceID();
        }
        
        public void OnNavbarValueChangedListener(Action<string> onNavbarValueChanged)
        {
            _onNavbarValueChanged = onNavbarValueChanged;
        }
        
        public void SetActiveToggle(string toggleName, bool invokeEvent = true)
        {
            foreach (Transform toggle in transform)
            {
                var isActive = toggle.name == toggleName;
                
                toggle.GetComponent<Toggle>().SetIsOnWithoutNotify(isActive);
                
                if (!isActive) continue;
                
                _activeToggleID = toggle.GetInstanceID();
            }
            
            if (invokeEvent) _onNavbarValueChanged?.Invoke(toggleName);
        }
    }
}
