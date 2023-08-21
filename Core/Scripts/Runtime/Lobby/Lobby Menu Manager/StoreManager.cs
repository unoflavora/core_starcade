using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Core.Runtime.Lobby.Store.Controller;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Core.Runtime.Lobby.PetBox;
using System.Collections.Generic;
using System.Linq;
using Agate.Starcade.Core.Runtime.Lobby.Store;
using Agate.Starcade.Core.Runtime.Lobby.Lootbox;
using Agate.Starcade.Core.Scripts.Runtime.UI;
using Agate.Starcade.Runtime.Helper;

namespace Agate.Starcade.Runtime.Lobby
{
    public class StoreManager : MonoBehaviour, ILobbyBehaviour
    {
        public enum StoreType
        {
            General,
            Lootbox,
            Petbox,
        }

        [SerializeField] private ToggleButton _toggleButton;
        public ToggleButton ToggleButton
        {
            get => _toggleButton;
            set => _toggleButton = value;
        }
        
        [Header("Controller")]
        [SerializeField] private StoreController _storeController;
        [SerializeField] private LootboxStoreController _lootboxStoreController;
        [SerializeField] private PetBoxController _petBoxController;

        [Header("Tab")]
        [SerializeField] private ScrollableTabMenu _storeTabs;
        
        public UnityEvent CheckNotificationEvent { get; set; }
        private UnityAction<PlayerBalance> _onBuyComplete;

        //ANALYTIC
        private StoreAnalyticEventHandler _storeAnalyticEvent { get; set; }
        private LobbyAnalyticEventHandler _lobbyAnalyticEvent { get; set; }

        private TaskCompletionSource<bool> _initProcess;

        private StoreType _onOpenStoreType = StoreType.General;

        private bool _isInit = false;

        // Lobby Actions
        private void Awake()
        {
            _toggleButton.OnSelect.AddListener(OnOpen);
            _toggleButton.OnDeselect.AddListener(OnClose);
            
            gameObject.SetActive(false);
        }

        public void Init(UnityAction<PlayerBalance> OnBuyComplete)//, Action onGoToCollectibles)
        {
            _storeAnalyticEvent = new StoreAnalyticEventHandler(MainSceneController.Instance.Analytic);
            
            _lobbyAnalyticEvent = new LobbyAnalyticEventHandler(MainSceneController.Instance.Analytic);

            _onBuyComplete += OnBuyComplete;
            
            InitGeneralStore();
            InitLootboxStore();
            InitPetBoxStore();
            _storeTabs.RegisterOnValueChanged(StoreAnalyticHandler);

            _isInit = true;
        }

        private void StoreAnalyticHandler(TabData tab)
        {
            var store = tab.Content.GetComponent<LobbyStore>();

            switch (store.Type)
            {
                case StoreType.General:
                    _storeAnalyticEvent.TrackClickStoreCoinsTabEvent();
                    break;
                case StoreType.Lootbox:
                    _storeAnalyticEvent.TrackClickStoreLootboxTabEvent();
                    break;
                case StoreType.Petbox:
                    _storeAnalyticEvent.TrackClickStorePetboxTabEvent();
                    break;
            }
        }

        private void InitPetBoxStore()
        {
            _petBoxController.Init(OpenStore);
            _petBoxController.RegisterAnalytic(_storeAnalyticEvent);
        }

        private void InitLootboxStore()
        {
            _lootboxStoreController.OnPurchaseItem = (balance) => _onBuyComplete(balance);
            _lootboxStoreController.RegisterAnalytics(_storeAnalyticEvent);
            _lootboxStoreController.Init();
        }

        private void InitGeneralStore()
        {
            _storeController.Init();
            _storeController.RegisterAnayltic(_storeAnalyticEvent);
        }

        public async void OnOpen()
        {
            //_initProcess = new TaskCompletionSource<bool>();

            //_initProcess.SetResult(true);

            gameObject.SetActive(true);
            GetComponent<FadeTween>().FadeIn();

            _lobbyAnalyticEvent.TrackClickStoreMenuEvent();
        }

        public void OnClose()
        {
            GetComponent<FadeTween>().FadeOut();
            gameObject.SetActive(false);
        }

        // TODO refactor
        public async void OpenStore(StoreType type)
        {
            if (!_isInit) await Task.Yield();

            Debug.Log(type);

            foreach(var tab in _storeTabs.Tabs)
            {
                tab.Toggle.isOn = false;
            }

            var toggle = _storeTabs.Tabs.Find(store => store.Content.GetComponent<LobbyStore>().Type == type).Toggle;

            toggle.isOn = true;
        }
    }
}
