using System;
using System.Collections;
using System.Collections.Generic;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Game;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Agate.Starcade.Runtime.Main.MainSceneController.STATIC_KEY;
using Button = UnityEngine.UI.Button;

namespace Agate.Starcade
{
    public class SettingManager : MonoBehaviour, ILobbyBehaviour
    {
        [SerializeField] private SettingController _setting;
        [SerializeField] private ToggleButton _toggleButton;
       

        [SerializeField] private Button _openSettingButton;
        [SerializeField] private Button _closeSettingButton;
        [SerializeField] private GameObject _blocker;
        
        private LobbyAnalyticEventHandler _lobbyAnalyticEvent { get; set; }
        
        public UnityEvent CheckNotificationEvent { get; set; }
        
        public ToggleButton ToggleButton
        {
            get => _toggleButton;
            set => _toggleButton = value;
        }
        
        [SerializeField] private SlideInTween _panelSlideInTween;
        private bool _isChange;
        
        private void Awake()
        {
            // _toggleButton.OnSelect.AddListener(OnOpen);
            // _toggleButton.OnDeselect.AddListener(OnClose);
            
            _openSettingButton.onClick.AddListener(OnOpen);
            _closeSettingButton.onClick.AddListener(OnClose);
            _closeSettingButton.onClick.AddListener(_setting.Close);
            
            // _toggleButton.OnDeselect.AddListener(_setting.OnClose);
            _panelSlideInTween.setOnComplete(() => {
                //gameObject.SetActive(false);
            });
            
            
        }

        private void Start()
        {
            _lobbyAnalyticEvent = new LobbyAnalyticEventHandler(MainSceneController.Instance.Analytic);
        }

        public void OnOpen()
        {
            Debug.Log("OPENN");
            _setting.Setup();
            gameObject.SetActive(true);
            //_lobbyAnalyticEvent.trackclicks
            if (_panelSlideInTween)
            {
                _panelSlideInTween.SlideIn();
            }
            _blocker.SetActive(true);
        }

        public void OnClose()
        {
            if (_panelSlideInTween)
            {
                _panelSlideInTween.SlideOut();
            }
            _blocker.SetActive(false);
            //_scrollRect.verticalNormalizedPosition = 0f;
            if (!_isChange) return;
            PlayerPrefs.SetString(GAME_CONFIG,JsonConvert.SerializeObject(MainSceneController.Instance.GameConfig));
            PlayerPrefs.Save();
            Debug.Log("Settings saved!");
            
            gameObject.SetActive(false);
            _isChange = false;
            
            
        }
    }
}
