using System;
using System.Threading.Tasks;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Arcade.Global.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Starcade.Core.Scripts.Runtime.Arcade.Modules
{
    public class PanelController : MonoBehaviour
    {
        [SerializeField] private bool _autoInit;
        [SerializeField] private ChangeCoinPanel _changeCoinPanel;
        [SerializeField] private OptionMenuController _optionMenuController;
        [NonSerialized] public UnityEvent OnChangeCoin = new UnityEvent();
        public UnityEvent OnQuitArcade { get; set; } = new UnityEvent();


        private void Start()
        {
            if (_autoInit)
            {
                OnChangeCoin = new UnityEvent();
            }
        }

        public void Init(string gameId, UnityAction quitGame)
        {
            if (_autoInit)
            {
                Debug.Log("[PANEL CONTROLLER] Auto init is active, skip init..");
                return;
            }
            OnChangeCoin = new UnityEvent();
            OnQuitArcade.AddListener(quitGame);
            _optionMenuController.Init(MainSceneController.Instance.Analytic,QuitArcade  ,gameId);
            _changeCoinPanel.OnChangeModeSelected.AddListener(delegate
            {
                OnChangeCoin.Invoke();
            });
        }

        private void QuitArcade()
        {
            OnQuitArcade?.Invoke();
        }

        public async Task OpenChangeCoinPanel(GameModeEnum gameMode)
        {
            _changeCoinPanel.gameObject.SetActive(true);
            _changeCoinPanel.DisplayChangeCoin(gameMode);
            while (_changeCoinPanel.gameObject.activeSelf) await Task.Delay(50);
        }

        public async Task OpenOptionMenu()
        {
            _optionMenuController.ActivatePopup();
            while (_optionMenuController.IsPanelActive) await Task.Delay(50);
        }
        
        public void CloseAllPanel()
        {
            _changeCoinPanel.gameObject.SetActive(false);
            _optionMenuController.ClosePopUp();
        }
    }
}