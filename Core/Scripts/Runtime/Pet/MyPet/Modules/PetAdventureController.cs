using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Main;
using Starcade.Core.Scripts.Runtime.UI;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.Modules
{
    public class PetAdventureController : MonoBehaviour
    {
        [SerializeField] private PetAdventureView _view;
        [SerializeField] private ToggleGroupWrapper _choices;
        private Action<int> _onDispatch;
        private int _currentChoice;

        private PetInventoryData _currentPet;
        private List<AdventureConfig> _configs;
        private Action _onClose;

        public void SetupDisplay(PetInventoryData pet, List<AdventureConfig> adventureConfig, Action<int> onDispatch)
        {
            _currentPet = pet;
            _configs = adventureConfig;
            
            _choices.SetValues(adventureConfig);
            _choices.OnNavbarValueChangedListener(OnNavbarValueChanged);
            _choices.SetActiveToggle(adventureConfig[0].Time);
            _onDispatch = onDispatch;
        }

        public void SetOnClosePopup(Action onClose)
        {
            _onClose = onClose;
        }
        
        private void OnNavbarValueChanged(int value)
        {
            _currentChoice = value;
            
            var i = Mathf.FloorToInt(value / 180) - 1;
            _view.SetupView(_currentPet, _configs[i]);
        }

        public void DispatchClicked()
        {
            _onDispatch?.Invoke(_currentChoice);
        }

        private void OnDisable()
        {
            _onClose?.Invoke();
        }
    }
}
