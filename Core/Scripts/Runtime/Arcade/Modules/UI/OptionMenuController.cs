using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Arcade.Modules.SO;
using Agate.Starcade.Scripts.Runtime.Arcade.Modules.UI;
using Agate.Starcade.Scripts.Runtime.Arcade.Modules.UI.ToolTips;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Global.UI
{
    public class OptionMenuController : MonoBehaviour
    {
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _infoButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _closeButton;

        [SerializeField] private ArcadeSettingsPanel _arcadeSettingsPanel;
        [SerializeField] private ToolTipsController _toolTipsController;

        [SerializeField] private SceneData _lobbyScene;
         
        private IngameOptionAnalyticEventHandler _ingameOptionAnalyticEventHandler { get; set; }


        

        public Button SettingsButton
        {
            get => _settingsButton;
            set => _settingsButton = value;
        }

        public Button InfoButton
        {
            get => _infoButton;
            set => _infoButton = value;
        }

        public Button QuitButton
        {
            get => _quitButton;
            set => _quitButton = value;
        }

        private bool _isTooltipActive => _toolTipsController.GetIsActive();
        private bool _isArcadeSettingPanelActive => _arcadeSettingsPanel.GetIsActive();
        

        public bool IsPanelActive => _toolTipsController != null && _isTooltipActive == false && _isArcadeSettingPanelActive == false && gameObject.activeSelf;

        public UnityEvent OnQuitArcade;
        public void Init(IAnalyticController analytic, UnityAction onQuit = null, string gameId = "")
        {
            OnQuitArcade.AddListener(onQuit);
            _infoButton.onClick.AddListener(DisplayToolTipsPanel);
            _settingsButton.onClick.AddListener(DisplaySettingsPanel);
            _quitButton.onClick.AddListener(BackToLobby);
            _arcadeSettingsPanel.Init(ActivatePopup);
            _closeButton.onClick.AddListener(ClosePopUp);
            if (_toolTipsController != null) _toolTipsController.InitButton(ActivatePopup);

            _ingameOptionAnalyticEventHandler = new IngameOptionAnalyticEventHandler(analytic, gameId);
            _ingameOptionAnalyticEventHandler.TrackOpenEvent();
        }

        public void Init(string gameId = "")
        {
            //OnQuitArcade.AddListener(onQuit);
            _infoButton.onClick.AddListener(DisplayToolTipsPanel);
            _settingsButton.onClick.AddListener(DisplaySettingsPanel);
            _quitButton.onClick.AddListener(BackToLobby);
            _arcadeSettingsPanel.Init(ActivatePopup);
            _closeButton.onClick.AddListener(ClosePopUp);
            if (_toolTipsController != null) _toolTipsController.InitButton(ActivatePopup);

            _ingameOptionAnalyticEventHandler = new IngameOptionAnalyticEventHandler(MainSceneController.Instance.Analytic, gameId);
            _ingameOptionAnalyticEventHandler.TrackOpenEvent();
        }

        private void BackToLobby()
        {
            OnQuitArcade.Invoke();
            //LoadSceneHelper.LoadScene(_lobbyScene);
            _ingameOptionAnalyticEventHandler.TrackClickQuitEvent();
		}

        public void ClosePopUp() 
        {
            gameObject.SetActive(false);
        }

        private void DisplaySettingsPanel()
        {
            _arcadeSettingsPanel.DisplayPanel();
            if(_arcadeSettingsPanel.GetIsActive())
                ClosePopUp();

            _ingameOptionAnalyticEventHandler.TrackClickGameSettingEvent();

		}

        private void DisplayToolTipsPanel()
        {
            _toolTipsController.DisplayPanel();
            if(_toolTipsController.GetIsActive())
                ClosePopUp();

			_ingameOptionAnalyticEventHandler.TrackClickTooltipsEvent();
		}
        
        public void ActivatePopup()
        {
            _arcadeSettingsPanel.GetComponent<ArcadeSettingsPanel>().enabled = true;
            gameObject.SetActive(true);
        }
        
        public void InitToolTips(List<ToolTipsPageData> toolTipsPageData)
        {
            if(_toolTipsController == null) return;
            Debug.Log(toolTipsPageData[0].PageTitle);
            _toolTipsController.Init(toolTipsPageData);
        }

    }
}
