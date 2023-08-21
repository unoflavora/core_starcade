using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Lobby;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Agate.Starcade.Runtime.Lobby.StoreManager;

namespace Agate.Starcade.Core.Runtime.Lobby.Store
{
    public class LobbyStoreItemUI : MonoBehaviour
    {
        [Header("UI Tab")]
        [SerializeField] private Image Icon;
        [SerializeField] private Image SelectedBackground;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] public Toggle toggle;
        
        [Header("UI Panel")]
        [SerializeField] private FadeTween activatedPanel;

        [Header("Data")]
        [SerializeField] private StoreType _tabStoreType;

        public StoreType StoreType => _tabStoreType;
        
        private Color _disabledColor = new Color(1, 1, 1, 0.25f);

        private StoreAnalyticEventHandler _analytic;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(SetActiveTab);
        }

        public void RegisterAnalytic(StoreAnalyticEventHandler analytic)
        {
            _analytic = analytic;
        }

        private void SetActiveTab(bool isOn)
        {
            if (isOn)
            {
                Icon.color = Color.white;
                SelectedBackground.color = Color.white;
                titleText.color = Color.white;
                
                activatedPanel.gameObject.SetActive(true);
                activatedPanel.FadeIn();

                switch (_tabStoreType)
                {
                    default:
                        Debug.LogWarning("This type not available for analytic");
                        break;
                }
            }
            else
            {
                SelectedBackground.color = new Color(1, 1, 1, 0);
                Icon.color = _disabledColor;
                titleText.color = _disabledColor;
                
                activatedPanel.gameObject.SetActive(false);
                activatedPanel.FadeOut();
            }
        }
    }
}