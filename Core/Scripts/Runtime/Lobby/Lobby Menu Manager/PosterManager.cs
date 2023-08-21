using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Runtime.UI.FX;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Game;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Agate.Starcade.Runtime.Main.MainSceneController.STATIC_KEY;

namespace Agate.Starcade.Runtime.Lobby
{
	public class PosterManager : MonoBehaviour, ILobbyBehaviour
    {
        [SerializeField] private ToggleButton _toggleButton;
        
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private List<PosterData> _listPosterData;
        [SerializeField] private GameObject _posterPrefab;

        [SerializeField] private ScrollRect _posterScrollRect;

        [SerializeField] private GameObject _dummyPoster;

        [SerializeField] private GameObject _heightFinder;
        [SerializeField] private HorizontalLayoutGroup _horizontalLayout;
        [SerializeField] private Canvas _canvas;

        [SerializeField] private List<PosterObject> _listPosterObject;
        
        [SerializeField] private GameObject _shadow;
        [SerializeField] private GameObject _rewardPanel;
        [SerializeField] private TMP_Text _rewardText;
        [SerializeField] private CoinRewardFX _coinRewardFX;

        [SerializeField] private LobbyUI _lobbyUI;
        
        public UnityEvent CheckNotificationEvent { get; set; }

        public List<RewardData> ListReward;
        private List<PosterStateReward> _listPosterState;

        private float _posterMaxHeight;

        private WebRequestHelper _webRequestHelper;
        private string _baseUrl;
        private AudioController _audio;
        private GameBackendController _gameBackend;
        public ToggleButton ToggleButton
        {
            get => _toggleButton;
            set => _toggleButton = value;
        }
        
        private LobbyAnalyticEventHandler _lobbyAnalyticEvent { get; set; }
        private PosterAnalyticEventHandler _posterAnalyticEvent { get; set; }

        private void Awake()
        {

        }

        private void Start()
        {
			_posterMaxHeight = _dummyPoster.GetComponent<RectTransform>().sizeDelta.y;
			_webRequestHelper = new WebRequestHelper(20);
			_baseUrl = MainSceneController.Instance.EnvironmentConfig.GameBaseUrl;
			_gameBackend = MainSceneController.Instance.GameBackend;
			_audio = MainSceneController.Instance.Audio;

            CheckNotificationEvent = new UnityEvent();
            
			if (PlayerPrefs.HasKey("PosterState"))
			{
				Debug.Log("Load poster state");
				string stateString = PlayerPrefs.GetString("PosterState");
                Debug.Log("state "+stateString);
				_listPosterState = new List<PosterStateReward>();
				_listPosterState = JsonConvert.DeserializeObject<List<PosterStateReward>>(stateString);
			}
			else
			{
				Debug.Log("Create poster state");
				_listPosterState = new List<PosterStateReward>();
			}

			//CHECK DEFAULT POSTER

			Setup();

            _toggleButton.OnSelect.AddListener(OnOpen);
			_toggleButton.OnDeselect.AddListener(OnClose);
			gameObject.SetActive(false);

			_lobbyAnalyticEvent = new LobbyAnalyticEventHandler(MainSceneController.Instance.Analytic);
            _posterAnalyticEvent = new PosterAnalyticEventHandler(MainSceneController.Instance.Analytic);
        }

        public async void OnOpen()
        {
            gameObject.SetActive(true);
            _lobbyAnalyticEvent?.TrackClickPosterMenuEvent();
            _posterScrollRect.horizontalNormalizedPosition = 0f;
            
            //TODO: handle erro response
            var res = await RequestHandler.Request(async () => await _gameBackend.GetRewards());
            if (res.Error != null) return;

            ListReward = new List<RewardData>();
            ListReward = res.Data;

            _listPosterState =
                JsonConvert.DeserializeObject<List<PosterStateReward>>(PlayerPrefs.GetString("PosterState"));

            foreach (var posterObject in _listPosterObject)
            {
                posterObject.SetClaimState(GetClaimState(posterObject.RewardId));
                posterObject.SetAvailableRewardState(GetAvailableRewardState(posterObject.RewardId));
            }
        }

        private void Update()
        {
            
        }

        public void OnClose()
        {
            gameObject.SetActive(false);
        }

        

        private async void Setup()
        {
            _heightFinder.SetActive(true);
            float height = HeightFinder(_canvas.GetComponent<RectTransform>().sizeDelta.y);
            var res = await RequestHandler.Request(async () => await _gameBackend.GetRewards());
            ListReward = new List<RewardData>();
            ListReward = res.Data;
            foreach (var posterData in _listPosterData)
            {
                GameObject poster = Instantiate(_posterPrefab, _contentTransform);
                poster.GetComponent<PosterObject>().SetupPoster(posterData,this,PosterDefaultAction,ClaimReward,height);
                
                if (string.IsNullOrEmpty(posterData.PosterId)) continue;
                
                _listPosterObject.Add(poster.GetComponent<PosterObject>());
                
                if (_listPosterState.Exists(reward => reward.RewardId == posterData.PosterId))
                {
                    Debug.Log("Poster state exist = " + posterData.PosterId);
                    poster.GetComponent<PosterObject>().SetAvailableRewardState(GetAvailableRewardState(posterData.PosterId));
                    poster.GetComponent<PosterObject>().SetViewState(GetViewedState(posterData.PosterId));
                }
                else
                {
                    Debug.Log("Poster state not exist = " + posterData.PosterId);
                    _listPosterState.Add(new PosterStateReward()
                    {
                        RewardId = posterData.PosterId, IsDone = false, IsViewed = false,
                    });
                    poster.GetComponent<PosterObject>().SetAvailableRewardState(GetAvailableRewardState(posterData.PosterId));
                    poster.GetComponent<PosterObject>().SetViewState(GetViewedState(posterData.PosterId));
                }
            }
            
            SetDefaultPosterState();
            
            PlayerPrefs.SetString("PosterState",JsonConvert.SerializeObject(_listPosterState));
            PlayerPrefs.Save();
            _heightFinder.SetActive(false);
        }

        private float HeightFinder(float currentHeight)
        {
            Debug.Log("current canvas height " + currentHeight);
            float x = currentHeight * 644 / 1080;
            Debug.Log("HEIGHT PREDICTED = " + x);
            return x;
        }

        private void SetDefaultPosterState()
        {
            var googleLoginState = PlayerPrefs.GetInt(LOGIN_STATE);
            if (googleLoginState == 1)
            {
                _listPosterState.Find(reward => reward.RewardId == "bind_account").IsDone = true;
            }
            foreach (var posterObject in _listPosterObject)
            {
                posterObject.SetClaimState(GetClaimState(posterObject.RewardId));
                posterObject.SetAvailableRewardState(GetAvailableRewardState(posterObject.RewardId));
                posterObject.SetViewState(GetViewedState(posterObject.RewardId));
            }

        }

        private void PosterDefaultAction(string id)
        {
            _posterAnalyticEvent.TrackClickPosterItem(id);
            var poster = _listPosterObject.Find(posterO => posterO.RewardId == id);
            if(poster.IsClaimableOnClickPoster) poster.SetAvailableRewardState(true);
            
            //poster.SetViewState(true);
            //poster.SetViewState(GetViewedState(id));
            
            string data = PlayerPrefs.GetString("PosterState");
            List<PosterStateReward> list = new List<PosterStateReward>();
            list = JsonConvert.DeserializeObject<List<PosterStateReward>>(data);
            list.Find(reward => reward.RewardId == id).IsViewed = true;
            PlayerPrefs.SetString("PosterState", JsonConvert.SerializeObject(list));
            PlayerPrefs.Save();
            Debug.Log("SAVED "+ JsonConvert.SerializeObject(list));
            _listPosterState = list;
            
            CheckNotificationEvent.Invoke();
        }

        private async void ClaimReward(string id)
        {
            var claimRewardData = await MainSceneController.Instance.Data.ClaimCustomReward(id, null, "Your Rewards", "Enjoy your rewards!");
            if(claimRewardData != null)
            {
                var poster = _listPosterObject.Find(posterO => posterO.RewardId == id);
                poster.SetClaimState(true);
                _posterAnalyticEvent.TrackClaimRewardEvent(id, claimRewardData.RewardGain.Select(d => new PosterAnalyticEventHandler.RewardParameters()
                {
                    Type = d.Type.ToString(),
                    Amount = d.Amount
                }).ToList());
            }
        }

        private bool GetClaimState(string rewardId) //CHECK IF REWARD ALREADY CLAIMED
        {
            return ListReward.Exists(data => data.RewardId == rewardId) && ListReward.Find(data => data.RewardId == rewardId).IsClaim;
        }

        private bool GetAvailableRewardState(string rewardId) // CHECK IF REWARD TASK ALREADY DONE
        {
            return _listPosterState.Exists(data => data.RewardId == rewardId) &&
                   _listPosterState.Find(data => data.RewardId == rewardId).IsDone;
        }

        private bool GetViewedState(string rewardId)
        {
            return _listPosterState.Exists(data => data.RewardId == rewardId) &&
                   _listPosterState.Find(data => data.RewardId == rewardId).IsViewed;
        }

        #region REWARD SPLASH MOVE LATER

        public async Task OnSuccessClaim(int totalReward)
        {
            //TODO: Change analytic event
            //_lobbyAnalyticEvent.TrackClaimRewardEvent();
            OpenRewardPanel();
            _audio.PlaySfx("reward_coin");
            await Count(totalReward);
            await Task.Delay(2000);
            CloseRewardPanel();
            _audio.PlaySfx("goal_jackpot");
            _coinRewardFX.StartFX(totalReward / 100);
            _shadow.SetActive(false);
        }
        
        private async Task Count(int totalReward)
        {
            LeanTween.value( gameObject, 0, totalReward, 1f).setOnUpdate( (float val)=>
            {
                int valInt = (int)val;
                _rewardText.text = valInt.ToString();
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

        #endregion
    }

    public class PosterStateReward
    {
        public string RewardId;
        public bool IsViewed;
        public bool IsDone;
    }
}
