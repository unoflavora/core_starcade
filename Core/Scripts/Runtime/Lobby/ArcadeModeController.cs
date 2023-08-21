using System;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class ArcadeModeController : MonoBehaviour
    {
        public enum ArcadeMode
        {
            GOLD_COIN,
            STAR_COIN,
            TUTORIAL,
            NONE
        }

		[SerializeField] private Image _machineImage;
		[SerializeField] private ArcadeChoiceObject _goldChoice;
        [SerializeField] private ArcadeChoiceObject _starChoice;

        [SerializeField] private Button _closeButton;

        [SerializeField] private Image _arcadeIcon;
        [SerializeField] private SlideInTween _slide;
        [SerializeField] private FadeTween _fade;

        [SerializeField] private Button _howToPlayButton;

        //ANALYTIC
        private LobbyAnalyticEventHandler _lobbyAnalyticEvent;

        private void Awake()
        {
            _closeButton.onClick.AddListener(Close);
            //float x = ((float)_slide.gameObject.GetComponent<RectTransform>().sizeDelta.x);
            //float y = ((float)_slide.gameObject.GetComponent<RectTransform>().sizeDelta.y);
            //if ((x / y) <= ((Screen.width * 1f) / Screen.height))
            //{
            //    float scale = ((float)Screen.height / (float)Screen.width) * 1.4f;
            //    _slide.gameObject.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1f);
            //}
        }

        public void OpenArcadeMode(ArcadeSO arcade, ArcadeSessionsData arcadeSessionsData, LobbyAnalyticEventHandler lobbyAnalyticEventHandler)
        {
            _lobbyAnalyticEvent = lobbyAnalyticEventHandler;
            gameObject.SetActive(true);
            _goldChoice.InitChoiceObject(arcade.ArcadeMachine, async () => await StartArcade(arcade, GameModeEnum.Gold, TutorialModeEnum.None), GetArcadeTimeRemaining(arcadeSessionsData.GoldSession));
            _starChoice.InitChoiceObject(arcade.ArcadeMachine, async () => await StartArcade(arcade, GameModeEnum.Star, TutorialModeEnum.None), GetArcadeTimeRemaining(arcadeSessionsData.StarSession));
            _arcadeIcon.sprite = arcade.ArcadeIcon;

            if(_machineImage != null) 
                _machineImage.sprite = arcade.ArcadeMachine;

            if (arcade.IsHaveTutorial)
            {
                _howToPlayButton.gameObject.SetActive(true);
                _howToPlayButton.onClick.AddListener(async () => await StartArcade(arcade, GameModeEnum.Star, TutorialModeEnum.Onboarding));
            }
            else
            {
                _howToPlayButton.gameObject.SetActive(false);
            }
            
            _fade.FadeIn();
            _slide.SlideIn();
        }
        
        public async Task StartArcade(ArcadeSO arcadeSo, GameModeEnum gameMode, TutorialModeEnum tutorialMode)
        {
            string sfxId = gameMode == GameModeEnum.Gold ? MainSceneController.AUDIO_KEY.BUTTON_GOLD_PLAY : MainSceneController.AUDIO_KEY.BUTTON_STAR_PLAY;
            MainSceneController.Instance.Audio.PlaySfx(sfxId);
            MainSceneController.Instance.AddressableController.RemoveAllListeners();
            MainSceneController.Instance.ArcadeMode = gameMode;
            MainSceneController.Instance.ArcadeTutorialMode = tutorialMode;
            
            ArcadeInitData arcadeInitData = new ArcadeInitData();
            arcadeInitData.GameMode = gameMode;
            arcadeInitData.TutorialMode = tutorialMode;
            SceneLaunchDataHelper.SetLaunchData(arcadeSo.ArcadeSceneData.ScenePath, arcadeInitData);
            if (arcadeSo.IsHaveTutorial)
            {
                bool isTutorialFinish = await TutorialChecker.IsTutorialFinished(arcadeSo.ArcadeSlug);
                if (!isTutorialFinish) MainSceneController.Instance.ArcadeTutorialMode = TutorialModeEnum.Onboarding;
            }
            _lobbyAnalyticEvent.TrackClickArcadePlayEvent(arcadeSo.ArcadeSlug, Enum.GetName(typeof(ArcadeMode), MainSceneController.Instance.ArcadeMode));
            LoadSceneHelper.LoadSceneArcade(arcadeSo.ArcadeSlug, arcadeSo.ArcadeSceneData);

			MainSceneController.Instance.Audio.StopBgm();
		}

        private async void Close()
        {
            await _fade.FadeOutAsync();
            _slide.SlideOut();
            gameObject.SetActive(false);
        }

        private double GetArcadeTimeRemaining(SessionData sessionData)
        {
            if (sessionData == null || sessionData.EndDate == null) return 0;
            //DateTime start = DateTime.UtcNow;
            var diff = (sessionData.EndDate.Value - DateTime.UtcNow).TotalSeconds;
            return diff;
        }
    }
}
