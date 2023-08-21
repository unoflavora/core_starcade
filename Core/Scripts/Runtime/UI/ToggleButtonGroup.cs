using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class ToggleButtonGroup : MonoBehaviour
    {
        List<ToggleButton> ToggleButtons;
        public ToggleButton DefaultToggleButton;
        
        public void Setup(ToggleButton toggleButton)
        {
            foreach (var toggle in ToggleButtons)
            {
                if(toggle == toggleButton) continue;
                toggle.DisableCheckmark();
            }
        }

        public void AddToggleButton(ToggleButton toggleButton)
        {
            ToggleButtons ??= new List<ToggleButton>();
            
            ToggleButtons.Add(toggleButton);
        }
    }
}
