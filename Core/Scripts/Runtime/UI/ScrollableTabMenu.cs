using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Scripts.Runtime.UI
{
    public class ScrollableTabMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _selectedVfx;
        [SerializeField] List<TabData> _tabs;
        private int _activeToggleID;

        private Action<TabData> _onNavbarValueChanged;
        public List<GameObject> Contents => _tabs.ConvertAll(tab => tab.Content);
        public List<TabData> Tabs => _tabs;
        
        private void Awake()
        {
            foreach (var tab in _tabs)
            {
                tab.Toggle.onValueChanged.AddListener(delegate { OnToggleValueChanged(tab.Toggle); });
            }
        }

        private void OnEnable()
        {
            _tabs[0].Toggle.isOn = true;
        }

        private void OnToggleValueChanged(Toggle toggle)
        {
            if (toggle.isOn == false) return;
            
            // Prevents the same toggle from being selected twice
            if(_activeToggleID != 0 && toggle.GetInstanceID() == _activeToggleID) return;

            if (toggle.IsInteractable() == false) return;
            
            SetActiveContent(toggle);
        }

        public void RegisterOnValueChanged(Action<TabData> action)
        {
            _onNavbarValueChanged += action;
        }

        // 
        public void SetActiveContent([NotNull] Toggle toggle)
        {
            if (toggle == null) throw new ArgumentNullException(nameof(toggle));
            
            _activeToggleID = toggle.GetInstanceID();

            _selectedVfx.transform.SetParent(toggle.transform); // Set the parent to the toggle
            _selectedVfx.transform.SetAsFirstSibling(); // Set the vfx to the first child of the toggle
            _selectedVfx.transform.localPosition = Vector3.zero; // Set the vfx to the center of the toggle
            
            // unknown defect in unity where the vfx is not set to active when the toggle is selected
            _selectedVfx.SetActive(false);
            _selectedVfx.SetActive(true); 

            foreach (var tab in _tabs)
            {
                if (tab.Content == null) continue;
                
                // Set the content of the tab to active if the tab name matches the toggle name
                var isTabSelected = tab.Toggle == toggle;
                
                tab.Content.SetActive(isTabSelected);

                if (isTabSelected)
                {
                    tab.Content.GetComponent<FadeTween>()?.FadeIn();
                    _onNavbarValueChanged?.Invoke(tab);
                }
            }
        }
    }
    
    
    [Serializable]
    public struct TabData
    {
        [FormerlySerializedAs("Name")] public Toggle Toggle;
        public GameObject Content;
    }
}
