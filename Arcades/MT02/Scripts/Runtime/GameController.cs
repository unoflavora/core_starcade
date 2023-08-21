using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Agate;
using Agate.Modules.Hexa.Pathfinding;
using Agate.Starcade;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Data.Enums;
using Agate.Starcade.Core.Runtime.Analytics;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Arcade.MiniGames.LightsRoulette.SO;
using Agate.Starcade.Scripts.Runtime.Arcade.MiniGames.MonsterSlayer.SO;
using Agate.Starcade.Scripts.Runtime.Arcade.Modules;
using Agate.Starcade.Scripts.Runtime.Arcade.Modules.Navbar;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend.Data;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Data.Enums;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Grid;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Jackpots;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.UI;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.VFX;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.OnBoarding;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Starcade.Arcades.MT02.Scripts.Runtime.Hex.Calculator;
using Starcade.Arcades.MT02.Scripts.Runtime.Hex.Grid;
using Starcade.Core.Runtime.Analytics.Handlers;
using Starcade.Core.Scripts.Runtime.Arcade.Modules;
using Starcade.Minigames.KillTheMonster.Scripts.Runtime;
using Starcade.Minigames.LightsRoulette.Scripts.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Starcade.Arcades.MT02.Scripts.Runtime
{
    // Ordered state
    public enum GameState {Idle, Minigame, Jackpot, VFX, Spinning, Matching, Init }
    
    [SuppressMessage("ReSharper", "AsyncVoidLambda")]
    public class GameController : MonoBehaviour
    {
        [Header("Game Modules")]
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private UIController _uiController;
        [SerializeField] private RewardController _rewardController;
        [SerializeField] private JackpotController _jackpotController;
        [SerializeField] private PlayerBalanceController _playerBalanceController;
        [SerializeField] private PanelController _panelController;
        [SerializeField] private VFXController _vfxController;
        [SerializeField] private OnBoardingController _onBoardingController;
        private IMonstamatchBackend _monstamatchBackend;
        private MonstamatchAnalyticEventHandler _monstamatchAnalyticEventHandler;
        
        [Header("Minigame Modules")]
        [SerializeField] private LightsRouletteController _lightsRouletteController;
        [SerializeField] private LightsRouletteSpriteData _lightsRouletteSpriteData;
        [SerializeField] private KillTheMonsterController _killTheMonsterController;
        [FormerlySerializedAs("_killTheMonsterSpriteData")] [SerializeField] private KillTheMonsterData killTheMonsterData;
        
        [Header("Game Configs")]
        [SerializeField] private SceneData _thisScene;
        [SerializeField] private bool _offlineMode;

        [Header("Game State")]
        [SerializeField] private GameState _currentState;
        [SerializeField] private GameModeEnum _gameMode;
        [SerializeField] private DaytimeEnums _currentTime;
        [SerializeField] private TutorialModeEnum _tutorialMode = TutorialModeEnum.None;
        private bool _isTutorial;
        private bool _timeOut;
        private GameState gameState => _currentState;
        
        [Header("Scene References")]
		[SerializeField] private SceneData _lobbyScene;

        [Header("Misc Modules")] 
        [SerializeField] private Highlight _highlight;

		#region Game Variables
		private MonstamatchInitData _initData { get; set; }
        private MonstamatchGameData _gameData { get; set; }
        
        private MonstamatchGameConfig _gameConfig;
        private List<MonstamatchJackpotEnums> _jackpotQueue;
        #endregion
        
        private async void Start()
        {
            _currentState = GameState.Init;
            
            _uiController.SetMainSpinButtonActive(false);
            // _gameMode = _offlineMode ? GameModeEnum.Star : MainSceneController.Instance.ArcadeMode;
            
            // if tutorial mode is not set from scene, get it from main scene controller
            // set tutorial mode from scene should only be used for testing
            if (_tutorialMode == TutorialModeEnum.None)
            {
                _tutorialMode = MainSceneController.Instance.ArcadeTutorialMode;
            }
            
            _isTutorial = _tutorialMode != TutorialModeEnum.None;
            
            _monstamatchBackend = _isTutorial || _offlineMode ? new MonstamatchBackendDummyController()
                : new MonstamatchBackendController(QuitArcade);
            
            _initData = await _monstamatchBackend.GetInitData(_gameMode);
            _gameData = _initData.Game;
            _gameConfig = _initData.Config;
            _currentTime = DaytimeEnums.Day;
            _currentState = GameState.Idle;

            await InitExperience();
            await InitModules();
            RegisterEvents();
            await InitMinigames();
            await InitJackpot();
            await InitItemAsset();

            if (!_offlineMode) MainSceneController.Instance.Loading.DoneLoading();
            
            if (_currentState != GameState.Minigame) ArcadeAudioHelper.PlayMainBGM();
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _vfxController.PlayTouchVFX();
            }
            
            _uiController.SetMainSpinButtonActive(gameState == GameState.Idle && !_gridManager.IsRemovingPaths);
            
            if (_initData == null) return;
            
            // Button still interactable but show disabled state since it will show a popup that prompt user to buy more coins
            _uiController.SetButtonDisabledColor(_initData.Config.Cost > _playerBalanceController.Balance);
        }
        
        private void OnDestroy()
        {
            _monstamatchAnalyticEventHandler?.TrackEndSessionEvent();
        }

        private async Task InitMinigames()
        {
            EnableMainGameComponents(true);
            
            // Initializing light roulette
            _lightsRouletteController.Init(_initData.Config.JackpotConfig, _lightsRouletteSpriteData, _monstamatchAnalyticEventHandler);
            _lightsRouletteController.InitEvents(LaunchRoulette,
                async (rewardTier, currency, reward) => await OnFinishMinigame(rewardTier, currency, reward, MinigameTypeEnums.LightsRoulette)
                , StartKillTheMonster);

            // Initializing Kill The Monster mini game
            var data = new KillTheMonsterConfig(_initData.Config.KillMonsterConfig);
            
            await _killTheMonsterController.Init(data, killTheMonsterData, _initData.Config.KillMonsterConfig.AdditionalBulletConfig,
                async (targetBodyPart, bodypartIndex) =>
                {
                    await _monstamatchBackend.UseBullet(_initData.SessionId, bodypartIndex);
                    _monstamatchAnalyticEventHandler.TrackShootMonsterJackpot(targetBodyPart.ToString());
                    //_killTheMonsterController.Shoot();
                },
                async (index) => 
                { 
                    await BuyBullet(index); 
                }, 
                async (rewardTier, currency, reward) =>
                {
                    await OnFinishMinigame(rewardTier, currency, reward, MinigameTypeEnums.KillTheMonster);
				});
            
            // If there is previous session of kill the monster game, play the game first.
            if (!_gameData.KillMonsterProgress.State) return;
            _currentState = GameState.Minigame;
            StartKillTheMonster();
        }

        private async Task InitExperience()
        {
            if (_isTutorial) return;
            
            MainSceneController.Instance.Data.ExperienceData.OnMilestoneReached.AddListener(GetMilestoneRewardEvent);
            await GetMilestoneReward();
        }

        private async Task InitItemAsset()
        {
            await MainSceneController.Instance.AssetLibrary.LoadAllAssets();
        }

        private async void GetMilestoneRewardEvent()
        {
            await GetMilestoneReward();
        }

        private async Task GetMilestoneReward()
        {
            var response = await MainSceneController.Instance.GameBackend.GetNextMilestoneReward();
            if (response.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(response.Error.Code);
            }
            MainSceneController.Instance.Data.ExperienceData.NextMilestone = response.Data;
			MainSceneController.Instance.Data.ExperienceData.NextLevel = response.Data;
		}

        private async Task BuyBullet(int amount)
        {
            try
            {
                var index = _initData.Config.KillMonsterConfig.AdditionalBulletConfig.FindIndex(s =>
                    s.BulletAmount == amount);
                
                var bulletConfig = _initData.Config.KillMonsterConfig.AdditionalBulletConfig[index];

                if (bulletConfig.Cost > _playerBalanceController.Balance)
                {
                    InsufficientBalanceHandler(_killTheMonsterController.OpenShopMenu);
                    return;
                }

                await _monstamatchBackend.BuyBullet(_initData.SessionId, index);
                _playerBalanceController.Balance -= bulletConfig.Cost;
                _monstamatchAnalyticEventHandler.TrackBuyAmmoOnMonsterJackpot(new ()
                {
                    Cost = bulletConfig.Cost,
                    CostCurrency = Enum.GetName(typeof(CurrencyTypeEnum), bulletConfig.CostCurrency),
                    ItemId = index.ToString()
                });
                _killTheMonsterController.AddAmmo(amount);
                
            }
            catch (Exception e)
            {
                Debug.Log("Error config data " + e.Message);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private async Task OnFinishMinigame(int rewardTier, CurrencyTypeEnum rewardCurrency, double reward, MinigameTypeEnums minigameTypeEnums)
        {
            await _vfxController.CloseCurtain();
            
            EnableMainGameComponents(true);
            _lightsRouletteController.CloseGame();

            if (minigameTypeEnums == MinigameTypeEnums.KillTheMonster)
            {
                var data = await _monstamatchBackend.ClaimReward(_initData.SessionId);
                _monstamatchAnalyticEventHandler.TrackCompleteMonsterJackpot(new ()
                {
                    RewardTier = rewardTier,
                    RewardCurrency = Enum.GetName(typeof(CurrencyTypeEnum), data.ClaimedReward.Type),
                    TotalReward = data.ClaimedReward.Amount
                });
                
                _killTheMonsterController.CloseGame();
            }
            
            await _vfxController.OpenCurtain();

            AddReward(rewardCurrency, reward);
            ArcadeAudioHelper.PlayMainBGM();
            if (_gridManager.RemainingSpinChance == 0) _uiController.DisplayGridOverlayPopup(true, GridOverlayText.InsufficientMatch);
            _currentState = GameState.Idle;
        }

        private async void StartKillTheMonster()
        {
           _uiController.DisplayGridOverlayPopup(false, GridOverlayText.InsufficientMatch);

           await _vfxController.CloseCurtain();
           
           EnableMainGameComponents(false);
           ChangeDay(_gameData.JackpotCount);
           _lightsRouletteController.CloseGame();
           
            ArcadeAudioHelper.StopBGM();
            _monstamatchAnalyticEventHandler.TrackStartMonsterJackpot();
            _killTheMonsterController.StartGame(_gameData.KillMonsterProgress.BulletsLeft, _gameData.KillMonsterProgress.CurrentHealth);

			await _vfxController.OpenCurtain(100);
		}

        private void EnableMainGameComponents(bool isEnabled)
        {
            _uiController.EnableGameComponents(isEnabled);
            _gridManager.gameObject.SetActive(isEnabled);
        }

        private async Task InitJackpot()
        {
            _jackpotQueue = _gameData.JackpotsQueue ?? new List<MonstamatchJackpotEnums>();
            
            if (_jackpotQueue.Count > 0)
            {
                await ProcessJackpot();
            }
        }

        private void StartTutorial()
        {
            var onboardingAnalyticEvent = new OnboardingAnalyticEventHandler(MainSceneController.Instance.Analytic);
            _playerBalanceController.SetBalance(MainSceneController.Instance.Data.UserBalance);

            _onBoardingController.gameObject.SetActive(true);
            _onBoardingController.InitOnboarding(StarcadeConstants.ARCADE_CODE_NAME_MONSTAMATCH, 0);
            _onBoardingController.OnStartOnBoarding.AddListener(() => onboardingAnalyticEvent.TrackStartOnboardingEvent(StarcadeConstants.ARCADE_CODE_NAME_MONSTAMATCH));
            _onBoardingController.OnSkipOnboarding.AddListener((state) => onboardingAnalyticEvent.TrackSkipOnboardingEvent(StarcadeConstants.ARCADE_CODE_NAME_MONSTAMATCH, state));
            _onBoardingController.OnCompleteOnBoardingEvent.AddListener((state) =>
            {
                OnNextTutorialSequenceEventHandler(onboardingAnalyticEvent, state);
            });
			
            _onBoardingController.OnEndOnBoarding.AddListener(() =>
            {
                EnableMainGameComponents(true);
                onboardingAnalyticEvent.TrackEndOnboardingEvent(StarcadeConstants.ARCADE_CODE_NAME_MONSTAMATCH);
                if (_tutorialMode == TutorialModeEnum.Onboarding)
                {
                    MainSceneController.Instance.DisableArcadeTutorialMode();
                    RestartScene();
                }
                else
                {
                    QuitArcade();
                }
            });
			_onBoardingController.StartOnboarding();
		}
        
        private void OnNextTutorialSequenceEventHandler(OnboardingAnalyticEventHandler onboardingAnalyticEvent, int state)
        {
            onboardingAnalyticEvent.TrackOnboardingEvent(StarcadeConstants.ARCADE_CODE_NAME_MONSTAMATCH, state);
            
            switch (state)
            {
                // Tutorial On How To Play Roulette
                case 18:
                    EnableMainGameComponents(false);
                    _lightsRouletteController.StartRoulette(isThisTutorial: true);
                    break;
                
                // Tutorial on How To Play Kill The Monster
                case 21:
                    _lightsRouletteController.CloseGame();
                    _killTheMonsterController.StartGame(10, 99, isThisTutorial: true);
                    break;
                
                case 25:
                    _killTheMonsterController.CloseGame();
                    EnableMainGameComponents(true);
                    break;
            }
        }
        private async Task InitModules()
        {
            await ArcadeAudioHelper.Init();

            _monstamatchAnalyticEventHandler = new MonstamatchAnalyticEventHandler(
                MainSceneController.Instance.Analytic, Enum.GetName(typeof(GameModeEnum), _initData.Mode),
                _isTutorial == false);
            
            _monstamatchAnalyticEventHandler.TrackStartSessionEvent(_initData.SessionId);

            ChangeDay(_gameData.JackpotCount);
            
            _rewardController.Init(_gameConfig.RewardCurrency);
            
            _playerBalanceController.GameMode = _gameMode;
            
            _gridManager.Init(_initData);

            await _vfxController.Init();

            _jackpotController.Init(_gameConfig.JackpotConfig, _gameData);
            
            _playerBalanceController.SetBalance(_initData.Balance);

            _uiController.SetProfilePhoto(MainSceneController.Instance.Data.UserProfileData.GetCurrentAvatar());
            _uiController.SetFramePhoto(MainSceneController.Instance.Data.UserProfileData.GetCurrentFrame());
            _uiController.SetupMode(_initData.Mode);
            _uiController.RegisterAllExperienceListeners();

            if (_isTutorial)
            {
                _uiController.SetAllButtonInteractableState(false);
                _playerBalanceController.SetBalance(MainSceneController.Instance.Data.UserBalance);
            }
            else
            {
                _uiController.SetAllButtonInteractableState(true);
            }
            
            _uiController.RegisterOnClickSetting(async () =>
            {
                // _gridManager.CurrentlyPanelIsOpening = true;
                await _panelController.OpenOptionMenu();
                // _gridManager.CurrentlyPanelIsOpening = false;
            });
            
            _uiController.RegisterOnFinishTimer(TimeOut);
            
            _panelController.Init(StarcadeConstants.ARCADE_CODE_NAME_MONSTAMATCH, QuitArcade);

            if (_initData.IsStarted)
            {
                _uiController.SetupTimer(_initData.EndDate, _initData.IsStarted, _initData.Config.SessionDuration);
            }
            else
            {
                _uiController.DisplayGridOverlayPopup(true, GridOverlayText.StartToPlay);
            }

            if (_gameData.SpinSession != null)
            {
                _gridManager.InitiateRandom(_gameData.SpinSession.lcgKey);
                _gridManager.RemainingSpinChance = _gameData.SpinSession.RemainingMatchChance;
                if (_gridManager.RemainingSpinChance == 0) _uiController.DisplayGridOverlayPopup(true, GridOverlayText.InsufficientMatch);
                _gridManager.ShowSymbols(_gameData.SpinSession.BoardState);
                _jackpotController.AddCoin(_gameData.SpinSession.InstantJackpotProgress.TotalCollected, activateIcon: true);
                
                _uiController.EnableSpinCount(true);
                _uiController.SetSpinCount(_gameData.SpinSession.RemainingMatchChance, _initData.Config.PaytableConfig.MaxMatchPerSpin);
            }
            
            if (_isTutorial)
            {
                Debug.Log("Start");
                StartTutorial();
            }
        }

        private void QuitArcade()
        {
            ArcadeAudioHelper.Unload();
            _uiController.RemoveAllExperienceListeners();
            MainSceneController.Instance.Data.ExperienceData.OnMilestoneReached.RemoveListener(GetMilestoneRewardEvent);
            //SceneManager.LoadScene(_lobbyScene);
            LoadSceneHelper.LoadScene(_lobbyScene);
		}
        
        private void TimeOut()
        {
            if (_timeOut) return;
            
            _timeOut = true;
            
            _panelController.CloseAllPanel();
            MainSceneController.Instance.Info.Show(InfoType.SessionTimeOut,
            new InfoAction("Quit", QuitArcade), null
            );

            _monstamatchAnalyticEventHandler.TrackTimeoutEvent();
        }

        private void RegisterEvents()
        {
            _uiController.RegisterOnClickSpin(SpinHandler);
            _gridManager.OnMatchTiles.AddListener(OnMatchTileHandler);
            _jackpotController.OnJackpot.AddListener(OnJackpotHandler);
            
            _uiController.OnChangeModeClicked.AddListener(async () =>
            {
                await _panelController.OpenChangeCoinPanel(_gameMode);
            });
            _panelController.OnChangeCoin.AddListener(ChangeMode);
            
            _uiController.RegisterGoToStoreButtonEvent(() =>
            {
                // _gridManager.CurrentlyPanelIsOpening = true;
                MainSceneController.Instance.Info.Show(InfoType.GoToStore, new InfoAction("Go To Store", () => {
                    _uiController.RemoveGoToStoreButtonEvent();
                    MainSceneController.Instance.GoToLobby(LobbyMenuEnum.Store);
                }), new InfoAction("Close", () =>
                {
                    // _gridManager.CurrentlyPanelIsOpening = false;
                }));
            });
        }

        public async void SpinHandler()
        {
            if (gameState != GameState.Idle || _gridManager.IsRemovingPaths) return;
            
            _currentState = GameState.Spinning;

            _gridManager.ProcessingSpin = true;
            
            Debug.Log("SPIN RECEIVED");

            TaskCompletionSource<bool> spinTask = new TaskCompletionSource<bool>();
            if (_initData.Config.Cost <= _playerBalanceController.Balance)
            {
                _gridManager.RunTimer(false);
                    
                _uiController.DisplayGridOverlayPopup(false, GridOverlayText.InsufficientMatch);
                
                _uiController.DisplayGridOverlayPopup(false, GridOverlayText.StartToPlay);
                    
                ArcadeAudioHelper.PlaySpinButton();
                 
                var spinData = await _monstamatchBackend.Spin(_initData.SessionId);

                if (_isTutorial)
                {
                    _initData.SessionId = _initData.SessionId switch
                    {
                        "Spin0" => "Spin1",
                        "Spin1" => "Spin2",
                        _ => _initData.SessionId
                    };
                        
                    await _gridManager.Spin(spinData.SpinSession.lcgKey, spinData.SpinSession.BoardState);
                    spinTask.TrySetResult(true);
                }
                else
                {
                    _monstamatchAnalyticEventHandler.TrackStartEvent(new()
                    {
                        SpinSessionId = _initData.SessionId, Cost = _initData.Config.Cost, CostCurrency = Enum.GetName(typeof(CurrencyTypeEnum),_initData.Config.CostCurrency)
                    });

                    await _gridManager.Spin(spinData.SpinSession.lcgKey);
                    spinTask.TrySetResult(true);
                }

                await spinTask.Task;
                _jackpotController.ResetCoinJackpot();

                _gridManager.RemainingSpinChance = spinData.SpinSession.RemainingMatchChance;

                UpdateExperience(spinData.experienceData, spinData.nextLevelUpReward);

                _rewardController.ResetDisplay();
                _playerBalanceController.Balance -= _initData.Config.Cost;
                _uiController.EnableSpinCount(true);
                _uiController.SetSpinCount(_gridManager.RemainingSpinChance, _initData.Config.PaytableConfig.MaxMatchPerSpin);
 
                // Setup timer if not initialized
                if (!_initData.IsStarted)
                {
                    _uiController.SetupTimer(spinData.endDate, spinData.isStarted, 0);
                }
                    
                _gridManager.RunTimer(true);
                _gridManager.ProcessingSpin = false;
                _currentState = GameState.Idle;
            }
            else
            {
                InsufficientBalanceHandler();
                spinTask.SetResult(false);
                _gridManager.ProcessingSpin = false;
                _currentState = GameState.Idle;
            }
            DebugCurrentState();
        }
        

        private void InsufficientBalanceHandler(UnityAction onFinish = null)
        {
            _monstamatchAnalyticEventHandler.TrackInsufficientBalanceEvent();
            
            if (onFinish == null)  onFinish = () => { };

            MainSceneController.Instance.Info.Show(InfoType.InsufficientBalance,
                new InfoAction("Go To Store", () => { MainSceneController.Instance.GoToLobby(LobbyMenuEnum.Store); }),
                new InfoAction("Close", onFinish));
        }

        private async void OnMatchTileHandler(CalculatedMatchResult result, List<Coordinate> coordinates)
        {
            Debug.Log("MATCH TILE RECEIVED");
            
            DebugCurrentState();
            
            _currentState = GameState.Matching;
            _gridManager.ProcessingSpin = true;
            
            var matchData = await _monstamatchBackend.Match(_initData.SessionId, coordinates);
            
            _gameData = matchData.Game;
            
            _gridManager.ProcessingSpin = false;
            
            var animations = new List<Func<Task>>();

            AddReward(_gameConfig.RewardCurrency, result.GeneralReward);

            _uiController.SetSpinCount(_gridManager.RemainingSpinChance, _initData.Config.PaytableConfig.MaxMatchPerSpin);

            if (result.SpecialCoinCollected > 0)
            {
                var coins = _jackpotController.AddCoin(result.SpecialCoinCollected, activateIcon: false);
                var coinCount = 0;
                foreach (var coin in coins)
                {
                    var from = result.SpecialCoinPositions[coinCount];
                    coinCount++;
                    ArcadeAudioHelper.PlayCoinObtained();
                    _vfxController.PlayCoinTrailVFX(coin.CoinImage, from, coin.transform.position);
                }
            }


            if (result.IsPuzzleCollected)
            {
                _currentState = GameState.VFX;
                _monstamatchAnalyticEventHandler.TrackReceivePuzzlePiece();

                var puzzlePos = _gameData.CurrentPuzzleJackpotProgress;
                
                var (sprite, puzzleIndex) = _jackpotController.ConfigPuzzle(puzzlePos, false);

                // play vfx
                var gridPos = _gridManager.Grid.GridArray[coordinates[1].x, coordinates[1].y].transform.position;
                //Before
                ArcadeAudioHelper.PlayPuzzlePieces();

                animations.Add(async () =>
                {
                    await _vfxController.PlayPuzzleVFX(puzzleIndex);
                    await _vfxController.PlayPuzzleTrailVFX(sprite, gridPos, sprite.transform.position);
                });
            }

            animations.Add(async () => await _jackpotController.AddProgress((float)result.JackpotExpReward));
            
            _monstamatchAnalyticEventHandler.TrackMatchEvent(new()
            {
                SpinSessionId = _initData.SessionId, TotalReward = result.GeneralReward, 
                RewardCurrency = Enum.GetName(typeof(CurrencyTypeEnum), _initData.Config.RewardCurrency)
            });

            var matchAnim = animations.Select(anim => anim()).ToList();
            matchAnim.Add(_gridManager.FillEmptyPaths());
            
            // Wait for all animations finished
            await Task.WhenAll(matchAnim);
            
            if (gameState != GameState.Jackpot) _currentState = GameState.Idle;
            if (_gridManager.RemainingSpinChance == 0)  _uiController.DisplayGridOverlayPopup(true, GridOverlayText.InsufficientMatch);

            if (_isTutorial)
            {
                _highlight.FinishCustomTask();
            }
        }

        private void DebugCurrentState()
        {
            Debug.Log("CURRENT GAME STATE: " + Enum.GetName(typeof(GameState), _currentState));
        }

        private async void OnJackpotHandler(MonstamatchJackpotEnums jackpotType)
        {
            _jackpotQueue.Add(jackpotType);
            Debug.Log("Jackpot" + jackpotType);
            if(gameState != GameState.Jackpot) await ProcessJackpot();
        }

        private async Task ProcessJackpot()
        {
            while (_jackpotQueue.Count > 0)
            {
                while (gameState != GameState.Idle) await Task.Delay(50);
                
                var currentJackpot = _jackpotQueue[0];
                _jackpotQueue.RemoveAt(0);
                
                _currentState = GameState.Jackpot;
                _gridManager.ProcessingJackpot = true;

                switch (currentJackpot)
                {
                    case MonstamatchJackpotEnums.Coin:
                    {
                        _uiController.EnableOverlayBackground(true);
                        foreach (var coinJackpot in _jackpotController.CurrentInstantJackpot)
                        {
                            ArcadeAudioHelper.PlayCoinJackpot();
                            await _vfxController.PlayInstantJackpotVFX(coinJackpot);

                            var config = _gameConfig.InstantJackpotConfig.Find(config => config.RewardType == coinJackpot);
                            var reward = config.RewardAmount;
                            AddReward(config.RewardCurrency, reward);
                            _monstamatchAnalyticEventHandler.TrackReceiveSpecialJackpot(new()
                            {
                                RewardCurrency = Enum.GetName(typeof(CurrencyTypeEnum), _initData.Config.RewardCurrency),
                                RewardType = Enum.GetName(typeof(InstantJackpotType), coinJackpot),
                                TotalReward = reward
                            });
                        }
                        _uiController.EnableOverlayBackground(false);
                        _currentState = GameState.Idle;

                        break;
                    }
                    case MonstamatchJackpotEnums.Jackpot:
                    {
                        ArcadeAudioHelper.PlayArtifactJackpot();
                        ArcadeAudioHelper.StopBGM();
                        
                        await _vfxController.PlayArtifactJackpotVFX();
                        await _vfxController.CloseCurtain();
                        
                        EnableMainGameComponents(false);
                        _uiController.DisplayGridOverlayPopup(false, GridOverlayText.InsufficientMatch);
                        _lightsRouletteController.StartRoulette();
                        _currentState = GameState.Minigame;
                        
                        await _vfxController.OpenCurtain();

                        await AsyncUtility.WaitUntil(() => gameState == GameState.Idle);

                        break;
                    }
                    case MonstamatchJackpotEnums.PuzzleJackpot:
                    {
                        ArcadeAudioHelper.PlayPuzzleJackpot();
                        
                        await _jackpotController.StartPuzzleSeq();
                        
                        _uiController.EnableOverlayBackground(true);
                        
                        var jackpotPuzzleData = await _monstamatchBackend.UsePuzzle(_initData.SessionId);
                        _gameData = jackpotPuzzleData.Game;
                        
                        _jackpotController.ConfigPuzzle(_gameData.CurrentPuzzleJackpotProgress);
                        
                        await _vfxController.PlayPuzzleVFX();
                        
                        _uiController.EnableOverlayBackground(false);
                        
                        AddReward(jackpotPuzzleData.ClaimedReward.Type, jackpotPuzzleData.ClaimedReward.Amount);
                        
                        await _jackpotController.StopPuzzleSeq();
                        
                        _monstamatchAnalyticEventHandler.TrackCompletePuzzleJackpot(new()
                        {
                            RewardCurrency = Enum.GetName(typeof(CurrencyTypeEnum), _initData.Config.RewardCurrency), 
                            TotalReward = jackpotPuzzleData.ClaimedReward.Amount
                        });
                        _currentState = GameState.Idle;

                        break;
                    }
                    default:
                        _currentState = GameState.Idle;
                        throw new ArgumentOutOfRangeException();
                }
                await Task.Delay(50);
                _gridManager.ProcessingJackpot = false;
            }
        }

        /*
         * Changing day is done if user got a progress bar jackpot
         * To make the day sync with backend, we use the total jackpot count that user already got
         */
        private void ChangeDay(int jackpotCount)
        {
            _currentTime = jackpotCount % 2 == 0 ? DaytimeEnums.Day : DaytimeEnums.Night;
            _uiController.SetTimeOfDay(_currentTime);
        }

        private void AddReward(CurrencyTypeEnum rewardType, double reward)
        {
            _rewardController.AddScore(reward);
            
            switch (rewardType)
            {
                case CurrencyTypeEnum.GoldCoin:
                case CurrencyTypeEnum.StarCoin:
                    _playerBalanceController.Balance += reward;
                    break;
                case CurrencyTypeEnum.StarTicket:
                    _playerBalanceController.StarTicket += reward;
                    break;
            }
        }

        private void ChangeMode()
        {
            var gameMode = _gameMode == GameModeEnum.Gold ? GameModeEnum.Star : GameModeEnum.Gold;
            MainSceneController.Instance.ArcadeMode = gameMode;
            _panelController.CloseAllPanel();
            
            _monstamatchAnalyticEventHandler.TrackChangeModeEvent(Enum.GetName(typeof(GameModeEnum), gameMode));

            RestartScene();
        }

        private void RestartScene()
        {
			MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.LoadingInfo);
			LoadSceneHelper.LoadSceneArcade(_thisScene.SceneKey, _thisScene);
		}

        private async void LaunchRoulette()
        {
            //if (_lightsRouletteController.GetGameState() != GameStateEnums.Play) return;
            try
            {
                var jackpotData = await _monstamatchBackend.UseJackpot(_initData.SessionId);
                _monstamatchAnalyticEventHandler.TrackReceiveRouletteJackpot();
                _gameData = jackpotData.Game;
                _lightsRouletteController.LaunchRoulette(jackpotData, _gameConfig.CostCurrency);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                MainSceneController.Instance.Info.Show("Jackpot Data is Not Available", "", InfoIconTypeEnum.Error, new InfoAction("Quit",
                    QuitArcade), null);
            }
        }

        private void UpdateExperience(ExperienceData data, NextExperienceRewardData reward)
        {
            var levelData = data;
            bool isLevelUp = levelData.Level > MainSceneController.Instance.Data.ExperienceData.Data.Level;
            bool isMileStoneReached = isLevelUp && MainSceneController.Instance.Data.ExperienceData.NextMilestone.Level == levelData.Level;
            
            MainSceneController.Instance.Data.ExperienceData.Data = levelData;
            MainSceneController.Instance.Data.ExperienceData.NextLevel = reward;
            MainSceneController.Instance.Data.ExperienceData.OnExperienceChanged.Invoke();
            
            if (isMileStoneReached)
            {
                MainSceneController.Instance.Data.ExperienceData.OnMilestoneReached.Invoke();
            }else if (isLevelUp)
            {
                MainSceneController.Instance.Data.ExperienceData.OnLevelUpChanged.Invoke(data.Level);
            }
        }
    }
}