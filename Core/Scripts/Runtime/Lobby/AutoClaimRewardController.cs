using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Runtime.UI.FX;
using Agate.Starcade.Scripts.Runtime.DailyLogin.VFX;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class AutoClaimRewardController : MonoBehaviour
    {
        [Serializable]
        public class Target
        {
            public CurrencyTypeEnum CurrencyTarget;
            public Transform TargetTransform;
            public Texture2D TargetVFXTexture;
            public Sprite TargetIcon;
        }

        private int _interval;
        private double _secondRemaining;

        private DateTime _lastCollectDateTime;
        private DateTime _currentCollectDateTime;

        [SerializeField] private GameObject _claimPanel;
        [SerializeField] private TMP_Text _claimRewardDisplayTime;
        [SerializeField] private Button _claimRewardButton;
        [SerializeField] private GameObject _notification;
        [SerializeField] private CoinRewardFX _coinRewardFX;

        [SerializeField] private Image _iconButtomClaim;
        [SerializeField] private Image _iconRewardPopUp;

        [SerializeField] private GameObject _shadow;
        [SerializeField] private GameObject _rewardPanel;
        [SerializeField] private TMP_Text _rewardText;
        [SerializeField] private SlideInTween _slide;
        [SerializeField] private RewardVFX _rewardVFX;

        [Header("Type")]
        [SerializeField] private List<Target> _targets = new List<Target>();

        private RewardBase _reward;
        
        //[Header("OnBoarding")] 
        //[SerializeField] private GameObject _dummyClaimPanel;

        private Target _currentTarget;
        
        //private UnityAction<PlayerBalance> _onClaimReward;

        private GameBackendController _gameBackend;
        private AudioController _audio;
        private LobbyAnalyticEventHandler _lobbyAnalyticEvent { get; set; }

        public bool ForceClaim;
        private int _totalReward = 10000;
        private int _count;
        
        public void Init(LobbyAnalyticEventHandler AnalyticEventHandler)
        {
            CountSecondRemaining();
            DisplayTime();
            GetCurrentTarget();
            _claimRewardButton.onClick.AddListener(async() => await ClaimReward());
            _gameBackend = MainSceneController.Instance.GameBackend;
            _audio = MainSceneController.Instance.Audio;
            _lobbyAnalyticEvent = AnalyticEventHandler;
        }

        //public void SetDummyClaim(bool needDummy)
        //{
        //    //_claimPanel.SetActive(!needDummy);
        //    //_dummyClaimPanel.SetActive(needDummy);
        //}

        public void OnEnable()
        {
            if (MainSceneController.Instance?.Data?.lobbyConfig == null) return;

            CountSecondRemaining();
            DisplayTime();
            GetCurrentTarget();
        }

        void Update()
        {
            if (_secondRemaining > 0)
            {
                _claimRewardButton.gameObject.SetActive(false);
                _notification.SetActive(false);
                _secondRemaining -= Time.deltaTime;
                DisplayTime();
            }
            else
            {
                _claimRewardButton.gameObject.SetActive(true);
                _notification.SetActive(true);
                _claimRewardDisplayTime.text = String.Empty;
            }

            if (ForceClaim)
            {
                _claimRewardButton.gameObject.SetActive(true);
            }
        }

        public void SetSecondRemaining(DateTime lastCollect)
        {
            _lastCollectDateTime = lastCollect;
            CountSecondRemaining();
        }

        void CountSecondRemaining()
        {
            _interval = MainSceneController.Instance.Data.lobbyConfig.TimeBaseCoinRewardInterval; //10s
            Debug.Log("interval= " +  _interval);
            _lastCollectDateTime = DateTime.Parse(MainSceneController.Instance.Data.LobbyData.lastCollectCoinTime); //10:00:00
            Debug.Log("Lass Collect= " + _lastCollectDateTime);
            _currentCollectDateTime = _lastCollectDateTime.AddSeconds(_interval); //10:00:10
            Debug.Log("Currecn Collect= " + _currentCollectDateTime);
            TimeSpan timeSpanBetweenCollect = _currentCollectDateTime - DateTime.Now; //9

            Debug.Log("date time now= " + DateTime.Now);
            Debug.Log("date utc time now= " + DateTime.UtcNow);
            Debug.Log("tiemspan between= " + timeSpanBetweenCollect);
            _secondRemaining = timeSpanBetweenCollect.TotalSeconds; //9
            Debug.Log("s remaining= " + _secondRemaining);
        }

        void DisplayTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(_secondRemaining);
            _claimRewardDisplayTime.text = time.ToString(@"hh\:mm\:ss");
        }

        void GetCurrentTarget()
        {
            var target = _targets.Find(targetData => targetData.CurrencyTarget == MainSceneController.Instance.Data.lobbyConfig.TimeBaseCoinRewardType);
            if(target == null)
            {
                target = _targets[0];
            }
            _currentTarget = target;

            _iconButtomClaim.sprite = _currentTarget.TargetIcon;
            _iconRewardPopUp.sprite = _currentTarget.TargetIcon;

            RewardEnum rewardEnum = RewardEnum.GoldCoin;

            switch (MainSceneController.Instance.Data.lobbyConfig.TimeBaseCoinRewardType)
            {
                case CurrencyTypeEnum.GoldCoin:
                    rewardEnum = RewardEnum.GoldCoin;
                    break;
                case CurrencyTypeEnum.StarCoin:
                    rewardEnum = RewardEnum.StarCoin;
                    break;
                case CurrencyTypeEnum.StarTicket:
                    rewardEnum = RewardEnum.StarTicket;
                    break;
                default:
                    rewardEnum = RewardEnum.GoldCoin;
                    break;
            }

            _reward = new RewardBase()
            {
                Amount = MainSceneController.Instance.Data.lobbyConfig.TimeBaseCoinRewardAmount,
                Type = rewardEnum,
            };
        }

        private async Task<double> ClaimReward()
        {
            MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.BUTTON_CLAIM_REWARD);
            var result = await RequestHandler.Request(async () => await _gameBackend.CollectCoin());
            double reward = 0;
            if (result.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(result.Error.Code);
            }
            else
            {
                MainSceneController.Instance.Data.UserBalance = result.Data.Balance;
                MainSceneController.Instance.Data.LobbyData.lastCollectCoinTime = result.Data.LastCollectCoinTime;
                Debug.Log("Success Claim");
                reward = result.Data.GoldCoinCollected;
                _totalReward = (int)reward;
                CountSecondRemaining();
                await OnSuccessClaim();
                MainSceneController.Instance.Data.PlayerBalanceActions.OnBalanceChanged.Invoke(result.Data.Balance);
                //_onClaimReward.Invoke(MainSceneController.Instance.Data.UserBalance);
            }
            return reward;
        }

        public async void TestOnSuccessClaim()
        {
            await OnSuccessClaim();
        }

        public async void DummyClaim(int rewardDummy)
        {
            Debug.Log("claim dummy");
            double reward = await ClaimReward();
            //await OnSuccessDummyClaim((int) reward);
        }
        
        public async Task OnSuccessDummyClaim(int reward)
        {
            Debug.Log("On Dummy Success");
            OpenRewardPanel();
            await Count();
            await Task.Delay(2000);
             CloseRewardPanel();
            _coinRewardFX.StartFX(reward/100);
            _shadow.SetActive(false);
            //SetDummyClaim(false);
            //_slide.SlideOut();
        }

        public async Task OnSuccessClaim()
        {
            _lobbyAnalyticEvent.TrackClaimTimeBasedCoinRewardEvent(_currentTarget.CurrencyTarget.ToString(), _totalReward);
            RewardBase[] reward = new RewardBase[] { _reward };
            StartVfx();
            //MainSceneController.Instance.Info.ShowReward("Your Reward", "Enjoy your rewards", reward, new InfoAction("Close", StartVfx), null);
        }

        private async void StartVfx()
        {
            _rewardVFX.gameObject.SetActive(true);
            RewardEnum rewardEnum = MainSceneController.Instance.Data.lobbyConfig.TimeBaseCoinRewardType switch
            {
                CurrencyTypeEnum.GoldCoin => RewardEnum.GoldCoin,
                CurrencyTypeEnum.StarCoin => RewardEnum.StarCoin,
                CurrencyTypeEnum.StarTicket => RewardEnum.StarTicket,
                _ => RewardEnum.GoldCoin,
            };
            _rewardVFX.InitDailyRewardVFX(MainSceneController.Instance.Data.lobbyConfig.TimeBaseCoinRewardAmount, rewardEnum);
            //_audio.PlaySfx("reward_coin");
            await Task.Delay(3000);
            //CloseRewardPanel();
            _rewardVFX.gameObject.SetActive(false);
            _audio.PlaySfx("goal_jackpot");
            _coinRewardFX.StartFX(_totalReward / 100, _currentTarget.TargetTransform, _currentTarget.TargetVFXTexture);
            _shadow.SetActive(false);
        }

        private async Task Count()
        {
            LeanTween.value( gameObject, 0, _totalReward, 1f).setOnUpdate( (float val)=>
            {
                int valInt = (int)val;

                var rewardConvert = CurrencyHandler.Convert((double)valInt);

                _rewardText.text = rewardConvert;
            });
        }
        
        private void OpenRewardPanel()
        {
            _shadow.SetActive(true);
            LeanTween.scale(_rewardPanel, Vector3.one, 0.25f).setEaseOutExpo();
        }

        private void CloseRewardPanel()
        {
            LeanTween.scale(_rewardPanel, Vector3.zero, 0.25f).setEaseOutExpo();
        }

    }
}
