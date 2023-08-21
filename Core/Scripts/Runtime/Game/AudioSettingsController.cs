using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Main;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Game
{
    public class AudioSettingsController : MonoBehaviour
    {
        [SerializeField] private Slider _bgmVolume;
        [SerializeField] private Slider _sfxVolume;
        private bool _isChange;
        
        public void EnableAudioSettings()
        {
            _bgmVolume.value = MainSceneController.Instance.GameConfig.AudioConfig.BgmVolume;
            _sfxVolume.value = MainSceneController.Instance.GameConfig.AudioConfig.SfxVolume;
            _bgmVolume.onValueChanged.AddListener(UpdateVolume);
            _sfxVolume.onValueChanged.AddListener(UpdateVolume);
        }

        public void DisableAudioSettings()
        {
            _bgmVolume.onValueChanged.RemoveAllListeners();
            _sfxVolume.onValueChanged.RemoveAllListeners();

            if (!_isChange) return;
            MainSceneController.Instance.Audio.SaveVolumeData();
            _isChange = false;
        }

        private void UpdateVolume(float volume)
        {
            //Debug.Log("UPDATE AUDIO");
            MainSceneController.Instance.GameConfig.AudioConfig.BgmVolume = _bgmVolume.value;
            MainSceneController.Instance.GameConfig.AudioConfig.SfxVolume = _sfxVolume.value;
            MainSceneController.Instance.Audio.UpdateVolume(_bgmVolume.value, _sfxVolume.value);
            _isChange = true;
        }
    }
}
