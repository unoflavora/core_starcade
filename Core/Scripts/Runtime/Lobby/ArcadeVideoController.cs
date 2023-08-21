using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using UnityEngine;
using UnityEngine.Video;

namespace Agate.Starcade
{
    public class ArcadeVideoController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private RenderTexture _render;
        [SerializeField] private FadeTween _fadeTween;
        [SerializeField] private AudioSource _videoAudioSource;
        private VideoClip _videoClip;

        private AudioController _audioController;

        public int Delay;

        public int FadeOutInterval;


        private void Awake()
        {
            _videoPlayer.gameObject.SetActive(false);
        }

        private void Start()
        {
            _audioController = MainSceneController.Instance.Audio;
            LoadSceneHelper.OnBeforeLoad.AddListener(ForceStop);
        }

        private void Update()
        {
            if(_videoPlayer.gameObject == null) return;
            if(_videoPlayer == null) return;
            if(_videoPlayer.clip == null) return;
            if(_videoClip == null) return;
            if (_videoPlayer.time >= _videoClip.length - FadeOutInterval)
            {
                EndVideo(_videoPlayer);
            }
        }

        public async void Play(VideoClip videoClip)
        {
            _videoClip = videoClip;
            _videoPlayer.clip = videoClip;
            await StartVideo();
        }

        public void ForceStop()
        {
            if(_videoPlayer == null) return;
            _videoPlayer.Stop();
        }

        public async void Stop()
        {
            await HideVideo();
            if(_videoPlayer == null) return;
            _videoPlayer.Stop();
        }
        
        private async Task StartVideo()
        {
            try
            {
                await Task.Delay(1000);
                if (_videoClip == null)
                {
                    _render.Release();
                    return;
                }
                ShowVideo();
                _videoPlayer?.Play();
            }
            catch { }
        }

        private async void EndVideo(VideoPlayer videoPlayer)
        {
            //_audioController.UnMuteBGM();
            _videoPlayer.Stop();
            ClearRender();
            await HideVideo();
            await StartVideo();
        }

        private void ShowVideo()
        {
            if (!_videoPlayer) return;
            //_audioController.MuteBGM();
            _videoPlayer.SetDirectAudioVolume(0,MainSceneController.Instance.GameConfig.AudioConfig.BgmVolume);
            _videoAudioSource.volume = MainSceneController.Instance.GameConfig.AudioConfig.BgmVolume;
            _videoPlayer.gameObject.SetActive(true);
            _fadeTween.FadeIn();
        }

        public async Task HideVideo()
        {
            if(_audioController == null) return;
            //_audioController.UnMuteBGM();
            await _fadeTween.FadeOutAsync();
            if(_videoPlayer == null) return;
            _videoPlayer.gameObject.SetActive(false);
            _render.Release();
        }


        public void ClearRender()
        {
            Debug.Log("CLEAR");
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = _render;
            GL.Clear(true,true,Color.clear);
            RenderTexture.active = rt;
        }
    }
}
