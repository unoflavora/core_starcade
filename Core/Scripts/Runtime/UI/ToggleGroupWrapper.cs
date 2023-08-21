using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Starcade.Core.Scripts.Runtime.UI
{
    public class ToggleGroupWrapper : MonoBehaviour
    {
        private int _activeToggleID;
        
        private Action<int> _onNavbarValueChanged;
        
        [SerializeField] private RectTransform _selectedFX;

        private LTDescr _currentAnim;

        // Event Action
        public void OnToggleValueChanged(ToggleWithCustomValue toggle)
        {
            Debug.Log(toggle.GetInstanceID());
            
            // Prevents the same toggle from being selected twice
            if(_activeToggleID != 0 && toggle.GetInstanceID() == _activeToggleID) return;
            
            _onNavbarValueChanged?.Invoke(toggle.Value);
            
            _activeToggleID = toggle.GetInstanceID();
            
            if(_currentAnim != null) LeanTween.cancel(_currentAnim.id);
            
            _currentAnim = LeanTween.moveX(_selectedFX,toggle.GetComponent<RectTransform>().anchoredPosition.x , .2f).setEase(LeanTweenType.easeOutExpo);
        }
        
        public void OnNavbarValueChangedListener(Action<int> onNavbarValueChanged)
        {
            _onNavbarValueChanged = onNavbarValueChanged;
        }
        
        public void SetActiveToggle(int value, bool invokeEvent = true)
        {
            foreach (Transform item in transform)
            {
                var toggle = item.GetComponent<ToggleWithCustomValue>();
                if(toggle == null) continue;

                var isActive = toggle.Value == value;
                
                toggle.GetComponent<Toggle>().SetIsOnWithoutNotify(isActive);
                
                if (!isActive) continue;
                
                if (invokeEvent)
                {
                    OnToggleValueChanged(toggle);
                }
            }
        }

        public void SetValues(List<AdventureConfig> configs)
        {
            var i = 0;
            foreach (Transform item in transform)
            {
                var toggle = item.GetComponent<ToggleWithCustomValue>();
                if(toggle == null) continue;
                
                var config = configs[i];
                toggle.SetValue(config.Id.Substring(0, 1).ToUpper() + config.Id.Substring(1), config.Time);
                i++;
            }
        }
    }
}
