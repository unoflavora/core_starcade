using System;
using System.Collections;
using System.Collections.Generic;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Main;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Runtime.Lobby
{
    public class PetsManager : MonoBehaviour,ILobbyBehaviour
    {
        [SerializeField] private ToggleButton _toggleButton;
        public ToggleButton ToggleButton
        {
            get => _toggleButton;
            set => _toggleButton = value;
        }
        
        private LobbyAnalyticEventHandler _lobbyAnalyticEvent { get; set; }
        
        public UnityEvent CheckNotificationEvent { get; set; }

        private void Awake()
        {
            _toggleButton.OnSelect.AddListener(OnOpen);
            _toggleButton.OnDeselect.AddListener(OnClose);
            
            gameObject.SetActive(false);
        }

        private void Start()
        {
            _lobbyAnalyticEvent = new LobbyAnalyticEventHandler(MainSceneController.Instance.Analytic);
        }

        public void OnOpen()
        {
            Debug.Log("OPEN PETS");
            gameObject.SetActive(true);
            
            if (_lobbyAnalyticEvent == null) _lobbyAnalyticEvent = new LobbyAnalyticEventHandler(MainSceneController.Instance.Analytic);
            
            _lobbyAnalyticEvent.TrackClickPetMenuEvent();
        }

        public void OnClose()
        {
            gameObject.SetActive(false);
        }
    }
}
