using System.Collections.Generic;
using Agate;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Utilities;
using DanielLochner.Assets.SimpleScrollSnap;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using static Agate.Starcade.Runtime.Main.MainSceneController.AUDIO_KEY;

public class ArcadeManager : MonoBehaviour, ILobbyBehaviour
{

    private ArcadeSO _currentSelectedArcade;
    [SerializeField] private ToggleButton _toggleButton;
    public ToggleButton ToggleButton
    {
        get => _toggleButton;
        set => _toggleButton = value;
    }

    [Header("Arcade Choice")]
    [SerializeField] private List<ArcadeSO> _listCurrentActiveArcade;
    [SerializeField] private GameObject _arcadeChoicePrefab;
    [SerializeField] private List<GameObject> _listArcadeChoice;
    [SerializeField] private Transform _arcadeScrollContent;
    [SerializeField] private int _minimumArcadeChoice;
    [SerializeField] private int _minimumTotalArcadeChoice;
    [SerializeField] private ArcadeSO _comingSoonArcadeSO;
    [SerializeField] private GameObject _currentSelected;

    [Header("Arcade Container")]    
    [SerializeField] private GameObject _arcadeInfoContainer;
    [SerializeField] private GameObject _arcadeMachinePosterContainer;
    [SerializeField] private GameObject _arcadeChoiceContainer;

    [Header("Arcade Component")] 
    [SerializeField] private SimpleScrollSnap _arcadeChoiceScroll;
    [SerializeField] private ArcadeVideoController _videoController;
    [SerializeField] private ArcadeModeController _arcadeModeController;
    
    [Header("Arcade Info")]
    [SerializeField] private TMP_Text _arcadeName;
    [SerializeField] private TMP_Text _arcadeDesc;
    [SerializeField] private Image _arcadeMachineImage;
    [SerializeField] private Button _arcadePlayButton;

    [Header("Transition")]
    [SerializeField] private List<CanvasTransition> _transitions;

    [Header("Other Component")] 
    [SerializeField] private VideoPlayer _backgroundVideoPlayer;

    [SerializeField] private GameObject _separator;

    [Header("Download")]
    [SerializeField] private ArcadeDownloadController _arcadeDownloadController;

    private GameBackendController _gameBackendController;
    private LobbyAnalyticEventHandler _lobbyAnalyticEvent;

    private UnityEvent<ArcadeSO> OnPlay;
    private UnityAction<ArcadeSO> OnPlayAction;
    
    public UnityEvent CheckNotificationEvent { get; set; }

    private bool _isReady = false;

    #region UNITY EVENT

    private void Awake()
    {
        _toggleButton.OnSelect.AddListener(OnOpen);
        _toggleButton.OnDeselect.AddListener(OnClose);
    }

    private void Start()
    {
        OnPlay = new UnityEvent<ArcadeSO>();
        OnPlay.AddListener(StartModeChoice);
        _arcadeChoiceScroll.OnPanelSelected.AddListener(OnEndDrag);
        _arcadeChoiceScroll.OnPanelSelecting.AddListener(OnStartDrag);
        _gameBackendController = MainSceneController.Instance.GameBackend;
    }

    private void OnEnable()
    {
        foreach (var transition in _transitions)
        {
            transition.TriggerTransition();
        }
    }

    private void Update()
    {
        if(_isReady) _currentSelected = _listArcadeChoice[_arcadeChoiceScroll.CenteredPanel];
    }

    #endregion

    #region SCROLL EVENT

    private void OnStartDrag(int index)
    {
        MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.SWIPE);
        foreach (GameObject choice in _listArcadeChoice)
        {
            choice.GetComponent<ArcadeChoiceController>().Hover(false);
        }
        _arcadeInfoContainer.GetComponent<SlideInTween>().SlideOut();
        _arcadeMachinePosterContainer.GetComponent<SlideInTween>().SlideOut();
        _videoController.Stop();
    }

    private void OnEndDrag(int index)
    {
        MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.SWIPE_TIC);
        if (_listArcadeChoice.Count <= 0) return;

        foreach (GameObject choice in _listArcadeChoice)
        {
            choice.GetComponent<ArcadeChoiceController>().Hover(false);
        }
        _currentSelected = _listArcadeChoice[_arcadeChoiceScroll.CenteredPanel];
        _currentSelected.GetComponent<ArcadeChoiceController>().Hover(true);
        SetArcadeInfo(_listCurrentActiveArcade[_arcadeChoiceScroll.CenteredPanel]);
    }

    #endregion

    public void InitArcadeList(LobbyAnalyticEventHandler lobbyAnalyticEventHandler)
    {
        _lobbyAnalyticEvent = lobbyAnalyticEventHandler;
        _isReady = false;
        int index = 0;
        MainSceneController.Instance.AddressableController.RemoveAllListeners();
        foreach (ArcadeSO arcade in _listCurrentActiveArcade)
        {
            GameObject arcadeChoice = Instantiate(_arcadeChoicePrefab, _arcadeScrollContent, false);
            arcadeChoice.name = arcade.ArcadeName + "Choice_Dynamic";
            arcadeChoice.GetComponent<ArcadeChoiceController>().InitChoice(index,arcade,_arcadeChoiceScroll);
            arcadeChoice.GetComponent<ArcadeChoiceController>().ChoiceButton.onClick.AddListener(() => OnArcadePlay(arcadeChoice, arcade));
            arcadeChoice.GetComponent<ArcadeChoiceController>().ScrollButton.onClick.AddListener(StopVideo);
            SetupDownload(arcadeChoice, arcade);
            _arcadeChoiceScroll.Setup();
            _listArcadeChoice.Add(arcadeChoice);
            index++;
        }

        if (_minimumArcadeChoice > _listCurrentActiveArcade.Count)
        {
            int totalComingSoon = _minimumArcadeChoice - _listCurrentActiveArcade.Count;

            for (int i = 0; i < totalComingSoon; i++)
            {
                GameObject arcadeChoice = Instantiate(_arcadeChoicePrefab, _arcadeScrollContent, false);
                arcadeChoice.name = "ComingSoon" + "Choice_Dynamic";
                arcadeChoice.GetComponent<ArcadeChoiceController>().InitChoice(index,_comingSoonArcadeSO,_arcadeChoiceScroll);
                arcadeChoice.GetComponent<ArcadeChoiceController>().isComingSoon = true;
                arcadeChoice.GetComponent<ArcadeChoiceController>().ScrollButton.onClick.AddListener(StopVideo);
                _listArcadeChoice.Add(arcadeChoice);
                _listCurrentActiveArcade.Add(_comingSoonArcadeSO);
                index++;
            }
            _arcadeChoiceScroll.Setup();
        }

        if (_minimumTotalArcadeChoice > _listArcadeChoice.Count)
        {
            int totalDummy = _minimumTotalArcadeChoice - _listArcadeChoice.Count;

            for (int i = 0; i < totalDummy; i++)
            {
                ArcadeSO arcade = _listCurrentActiveArcade[i];
                _listCurrentActiveArcade.Add(_listCurrentActiveArcade[i]);
                GameObject arcadeChoice = Instantiate(_listArcadeChoice[i], _arcadeScrollContent, false);
                arcadeChoice.GetComponent<ArcadeChoiceController>().InitChoice(i+totalDummy,_listCurrentActiveArcade[i], _arcadeChoiceScroll);
                arcadeChoice.GetComponent<ArcadeChoiceController>().isComingSoon = _listArcadeChoice[i].GetComponent<ArcadeChoiceController>().isComingSoon;
                if (!_listCurrentActiveArcade[i].IsLocked) arcadeChoice.GetComponent<ArcadeChoiceController>().ChoiceButton.onClick.AddListener(() => OnArcadePlay(arcadeChoice, arcade));
                arcadeChoice.GetComponent<ArcadeChoiceController>().ScrollButton.onClick.AddListener(StopVideo);
                SetupDownload(arcadeChoice, arcade);
                _listArcadeChoice.Add(arcadeChoice);
            }
            _arcadeChoiceScroll.Setup();
        }

        _isReady = true;
    }

    private async void StartModeChoice(ArcadeSO arcadeSo)
    {
		MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.BUTTON_OPEN);
		var result = await RequestHandler.Request(async () =>
            await _gameBackendController.GetArcadeSession(arcadeSo.ArcadeSlug));
		if (result.Error != null)
		{
			MainSceneController.Instance.Info.ShowSomethingWrong(result.Error.Code);
			return;
		}
        _arcadeModeController.OpenArcadeMode(arcadeSo, result.Data, _lobbyAnalyticEvent);
        _lobbyAnalyticEvent.TrackClickSelectArcadeEvent(arcadeSo.ArcadeSlug);
    }

    private void SetArcadeInfo(ArcadeSO arcade)
    {
        _arcadeMachinePosterContainer.GetComponent<SlideInTween>().SlideOut();
        _arcadeInfoContainer.GetComponent<SlideInTween>().SlideIn();
        Debug.Log("Slide in me hello");
        _arcadeName.text = arcade.ArcadeName;
        _arcadeDesc.text = arcade.ArcadeDescription;
        _arcadeMachineImage.sprite = arcade.ArcadeMachine;
        _arcadePlayButton.onClick.RemoveAllListeners();
        _arcadePlayButton.onClick.AddListener(() => OnPlay.Invoke(arcade));
        if (arcade.ArcadeVideo != null)
        {
            _videoController.Play(arcade.ArcadeVideo);
        }
        else
        {
            _videoController.Play(null);
        }
    }

    public void SelectDefault()
    {
        var first = _listArcadeChoice[0];
        first.GetComponent<ArcadeChoiceController>().Hover(true);
        SetArcadeInfo(_listCurrentActiveArcade[0]);
    }

    private void StopVideo()
    {
        _videoController.Stop();
    }

    private void OnArcadePlay(GameObject arcadeChoice, ArcadeSO arcade)
    {
        if (arcade.AssetId == AddressableController.AssetID.None)
        {
            OnPlay.Invoke(arcade);
        }
        
        AssetData.DownloadState state = MainSceneController.Instance.AddressableController.GetDownloadStatus(arcade.AssetId);
        long size = MainSceneController.Instance.AddressableController.GetDownloadSize(arcade.AssetId);

        if (state == AssetData.DownloadState.Finish)
        {
            OnPlay.Invoke(arcade);
        }
        else if (state == AssetData.DownloadState.None)
        {
            float formattedSize = Mathf.Round(size / Mathf.Pow(10f, 5));
            _arcadeDownloadController.Show(formattedSize / 10f, () =>
            {
                _lobbyAnalyticEvent.TrackDownloadArcade(arcade.ArcadeSlug);

                MainSceneController.Instance.AddressableController.InvokeOnStartEvent(arcade.AssetId);
                MainSceneController.Instance.AddressableController.AddDownload(arcade.AssetId);
                if (!MainSceneController.Instance.AddressableController.IsDownloading())
                {
                    MainSceneController.Instance.AddressableController.StartDownload();
                }
            });
        }
    }

    private void SetupDownload(GameObject arcadeChoice, ArcadeSO arcade)
    {
        if (arcade.AssetId == AddressableController.AssetID.None)
        {
            return;
        }
        
        MainSceneController.Instance.AddressableController.SetOnStartEvent(arcade.AssetId, () =>
        {
            if (arcadeChoice != null)
            {
                long size = MainSceneController.Instance.AddressableController.GetDownloadSize(arcade.AssetId);
                arcadeChoice.GetComponent<ArcadeChoiceController>().SetDownloadVisible(true);
                arcadeChoice.GetComponent<ArcadeChoiceController>().SetDownloadBar(0, size);
            }
        });
        MainSceneController.Instance.AddressableController.SetOnProgressEvent(arcade.AssetId, (current, total) => 
        {
            if (arcadeChoice != null)
            {
                arcadeChoice.GetComponent<ArcadeChoiceController>().SetDownloadIconVisible(true);
                arcadeChoice.GetComponent<ArcadeChoiceController>().SetDownloadVisible(true);
                arcadeChoice.GetComponent<ArcadeChoiceController>().SetDownloadBar(current, total);
            }
        });
        MainSceneController.Instance.AddressableController.SetOnCompleteEvent(arcade.AssetId, () =>
        {
            if (arcadeChoice != null)
            {
                arcadeChoice.GetComponent<ArcadeChoiceController>().SetDownloadIconVisible(false);
                arcadeChoice.GetComponent<ArcadeChoiceController>().SetDownloadVisible(false);

				_lobbyAnalyticEvent.TrackArcadeDownloadSuccess(arcade.ArcadeSlug);
			}
        });
        AssetData.DownloadState state = MainSceneController.Instance.AddressableController.GetDownloadStatus(arcade.AssetId);
        Debug.Log($"{arcade.AssetId}, {state}");
        arcadeChoice.GetComponent<ArcadeChoiceController>().SetDownloadIconVisible(state == AssetData.DownloadState.None);

    }
    
    public void OnOpen()
    {
        Debug.Log("OPEN ARCADE");
        _lobbyAnalyticEvent.TrackClickArcadeMenuEvent();
        
        _separator.SetActive(true);
        gameObject.SetActive(true);
        gameObject.GetComponent<FadeTween>().FadeIn();
        _arcadeInfoContainer.GetComponent<SlideInTween>().SlideIn();
        _arcadeChoiceContainer.GetComponent<SlideInTween>().SlideIn();
        
        _arcadeChoiceScroll.GoToPanel(0);
        OnStartDrag(0);
        OnEndDrag(0);
        _videoController.Stop();
    }

    public async void OnClose()
    {
        Debug.Log("CLOSE ARCADE");
        //_arcadeInfoContainer.GetComponent<SlideInTween>().SlideOut();
        //_arcadeChoiceContainer.GetComponent<SlideInTween>().SlideOut();
        //_arcadeMachinePosterContainer.GetComponent<SlideInTween>().SlideOut();
        gameObject.SetActive(false);
        _videoController.Stop();
        await gameObject.GetComponent<FadeTween>().FadeOutAsync();
    }
}
