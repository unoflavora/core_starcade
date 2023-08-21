using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Agate.Starcade.Runtime.Audio
{
	public class AudioController : MonoBehaviour
    {
        public enum AudioType
        {
            bgm,
            sfx
        }

        [SerializeField] private AudioSource _audioSourceBGM;
        [SerializeField] private AudioSource _audioSourceSFX;
        [SerializeField] private AudioCollection _audioCollection;

        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioMixerGroup _sfxMixer;
        [SerializeField] private AudioMixerGroup _sfxFocusMixer;

        private AudioData[] _bgmList;
        private AudioData[] _sfxList;

        [SerializeField] private List<AudioCollection> _listAudioCollections;
        private List<string> _listAudioCollectionKey;
        private List<AudioData> _listActiveBgm;
        private List<AudioData> _listActiveSfx;

        public bool IsSfxPlaying => _audioSourceSFX.isPlaying;

        private AddressableController _addressableController;
        public AudioSource AudioSourceBgm => _audioSourceBGM;
        public AudioSource AudioSourceSfx => _audioSourceSFX;
        
        public void InitAudio()
        {
			AudioConfig audioSetting = MainSceneController.Instance.GameConfig.AudioConfig;
            _audioSourceBGM.loop = true;

            _listActiveBgm = new List<AudioData>();
            _listActiveSfx = new List<AudioData>();
            _listAudioCollectionKey = new List<string>();

            UpdateVolume(MainSceneController.Instance.GameConfig.AudioConfig.BgmVolume, MainSceneController.Instance.GameConfig.AudioConfig.SfxVolume);
            //_audioSourceBGM.volume = audioSetting.BgmVolume;
            //_audioSourceSFX.volume = audioSetting.SfxVolume;
            
            SceneManager.activeSceneChanged += OnChangeScene;
            LoadSceneHelper.OnBeforeLoad.AddListener(StopAllAudio);

            _addressableController = MainSceneController.Instance.AddressableController;
        }
        
        public void PlayBgm(string id)
        {
            UnMuteBGM();
            _audioSourceBGM.clip = FindAudio(id, AudioType.bgm);
            if(_audioSourceBGM.clip == null) return;
            _audioSourceBGM.Play();
        }
        
        public void StopBgm()
        {
            _audioSourceBGM.Stop();
        }

        public void PlaySfx(string id)
        {
            // Debug.Log(id);
            _audioSourceSFX.outputAudioMixerGroup = _sfxMixer;
            _audioSourceSFX.PlayOneShot(FindAudio(id,AudioType.sfx));
        }

        public void PlayFocusSfx(string id)
        {
            // Debug.Log(id);
            _audioSourceSFX.outputAudioMixerGroup = _sfxFocusMixer;
            _audioSourceSFX.PlayOneShot(FindAudio(id,AudioType.sfx));
        }

        private void StopAllAudio()
        {
            Debug.Log("stop");
            _audioSourceBGM.Stop();
            _audioSourceBGM.clip?.UnloadAudioData();
            _audioSourceSFX.Stop();
            _audioSourceSFX.mute = true;
        }
        
        private void OnChangeScene(Scene from, Scene to)
        {
            StopBgm();
            _audioSourceSFX.mute = false;
        }
        
        private AudioClip FindAudio(string id, AudioType audioType)
        {
            List<AudioData> listAudio = audioType switch
            {
                AudioType.bgm => _listActiveBgm,
                AudioType.sfx => _listActiveSfx,
                _ => throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null)
            };

            foreach (var audioData in listAudio) 
            {
                if (audioData.id == id) return audioData.audioClip;
            }
            
            Debug.LogError("Can't find audio with id " + id + ", Make sure to load audio on init scene");
            return null;
        }

		public void UpdateVolume(float bgmVol, float sfxVol)
		{
			Debug.Log($"UPDATE AUDIO BGM: {bgmVol}, SFX: {sfxVol}");
			//_audioSourceBGM.volume = MainSceneLauncher.Instance.GameConfig.AudioSetting.BgmVolume;
			//         _audioSourceSFX.volume = MainSceneLauncher.Instance.GameConfig.AudioSetting.SfxVolume;
			MainSceneController.Instance.GameConfig.AudioConfig.BgmVolume = bgmVol;
			MainSceneController.Instance.GameConfig.AudioConfig.SfxVolume = sfxVol;

			SetVolumeBGM(bgmVol);
			SetVolumeSFX(sfxVol);
		}

		public void SaveVolumeData()
        {
            AudioConfig audioSetting = new AudioConfig()
            {
                BgmVolume = MainSceneController.Instance.GameConfig.AudioConfig.BgmVolume,
                SfxVolume = MainSceneController.Instance.GameConfig.AudioConfig.SfxVolume
            };
            PlayerPrefs.SetString(MainSceneController.STATIC_KEY.AUDIO_CONFIG, JsonConvert.SerializeObject(audioSetting));
            PlayerPrefs.Save();
            Debug.Log("SAVED AUDIO SETTING");
        }

        public void UnMuteBGM()
        {
            Debug.Log("audio source volume = " + _audioSourceBGM.volume);
            Debug.Log("audio setting volume = " + MainSceneController.Instance.GameConfig.AudioConfig.BgmVolume);
            LeanTween.value(_audioSourceBGM.volume, MainSceneController.Instance.GameConfig.AudioConfig.BgmVolume, 1f).setOnUpdate((float val) =>
            {
                _audioSourceBGM.volume = val;
            });
        }

        public void MuteBGM()
        {
            LeanTween.value(_audioSourceBGM.volume, 0, 1f).setOnUpdate((float val) =>
            {
                _audioSourceBGM.volume = val;
            });
        }

        public async Task LoadAudioData(string key)
        {
            if (_listAudioCollectionKey.Any(activeKey => activeKey == key))
            {
                Debug.Log("This key already loaded. Skipping load audio data");
                return;
            }
            
            AudioCollection RawAudioCollection = new AudioCollection();
            try
            {
                RawAudioCollection = GetAudioCollection(key);
                var RawBgm = RawAudioCollection.NewBgmCollection;
                var RawSfx = RawAudioCollection.NewSfxCollection;
                foreach (var rawAudioData in RawBgm)
                {
                    AudioData tempData = await _addressableController.LoadAudioAsset(rawAudioData);
                    _listActiveBgm.Add(tempData);
                }
                foreach (var rawAudioData in RawSfx)
                {
                    AudioData tempData = await _addressableController.LoadAudioAsset(rawAudioData);
                    _listActiveSfx.Add(tempData);
                }
                _listAudioCollectionKey.Add(key);
                Debug.Log("Success Load " + key + " Data");
            }
            catch(Exception e)
            {
                Debug.LogError("Something error happen on Load Audio Data - " + e.Message);
            }
        }

        public void UnloadAudioData(string key)
        {
            AudioCollection RawAudioCollection = new AudioCollection();
            try
            {
                RawAudioCollection = GetAudioCollection(key);
                _listActiveBgm.RemoveAll(audioData => audioData.label == key);
                _listActiveSfx.RemoveAll(audioData => audioData.label == key);
                _addressableController.UnloadAudioAssets(key, RawAudioCollection);
                _listAudioCollectionKey.Remove(key);
                Debug.Log("Success Unload " + key + " Data");
            }
            catch(Exception e)
            {
                Debug.LogError("Something error happen on Unload Audio Data - " + e.Message);
            }
        }

        private AudioCollection GetAudioCollection(string key)
        {
            if (_listAudioCollections.Any(AudioCollection => AudioCollection.LabelCollection == key))
            {
                return _listAudioCollections.Find(audioCollection => audioCollection.LabelCollection == key);
            }
            throw new Exception("Key not available");
        }

        public void SetVolumeBGM(float value)
        {
			value = Mathf.Clamp(value, 0.0001f, 1);
			var newVolume = Mathf.Log10(value) * 20;

            _audioSourceBGM.volume = value;
			_audioMixer.SetFloat("bgm", newVolume);
			_audioMixer.SetFloat("bgm2", newVolume);
            
        }
		private void SetVolumeSFX(float value)
		{
            value = Mathf.Clamp(value, 0.0001f, 1);
            var newVolume = Mathf.Log10(value) * 20;
            //Debug.Log(newVolume);
            _audioMixer.SetFloat("sfx", newVolume);
			_audioMixer.SetFloat("sfx2", newVolume);
		}
		#region Deprecated

		public void PlayBgm()
        {
            _audioSourceBGM.Play();
            _audioSourceBGM.loop = true;
        }

        public void PauseBgm()
        {
            _audioSourceBGM.Pause();
        }

        #endregion
    }
}
