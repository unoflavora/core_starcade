using System;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Agate.Starcade.Scripts.Runtime.TitleScreen.Script
{
    public class SplashScreen: MonoBehaviour
    {
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private VideoClip _splashVideo;
        [SerializeField] private int _fadeOutInterval;
        [SerializeField] private FadeTween _fadeTween;
        [SceneReference,SerializeField] private string _nextScene;

        //[SerializeField] private SplashScreenAnalyticEventHandler _splashScreenAnalyticEvent;

        private bool _isPlaying;

        private void Start()
        {
            //_splashScreenAnalyticEvent = new SplashScreenAnalyticEventHandler(MainSceneLauncher.Instance.Analytic);
            _videoPlayer.loopPointReached += (vp) =>
            {
                StopVideo();
            };
        }

        private void OnEnable()
        {
            _isPlaying = true;
        }
        
        private async void StopVideo()
        {

			//_splashScreenAnalyticEvent.TrackSplashScreenEvent();

			_isPlaying = false;
            await _fadeTween.FadeOutAsync();
            await Task.Delay(3000);
            SceneManager.LoadSceneAsync(_nextScene);
        }
    }
}