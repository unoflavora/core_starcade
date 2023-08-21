using System.Collections;
using System.Collections.Generic;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Main;
using UnityEngine;
using static Agate.Starcade.Runtime.Main.MainSceneController.AUDIO_KEY;

namespace Agate.Starcade
{
    public class LobbyAudio : MonoBehaviour
    {
        private AudioController _audioController;

        public enum SFX
        {
            BUTTON_GENERAL,
            BUTTON_PLAY,
            BUTTON_TAB,
            BUTTON_UNAVAILABLE,
            MENU_CLOSED,
            MENU_OPEN,
            NOTIFICATION_COIN,
        }
        
        public void Init()
        {
            _audioController = MainSceneController.Instance.Audio;
        }

        public void PlayBgm()
        {
            if(_audioController == null) return;
            _audioController.PlayBgm(BGM_LOBBY);
        }

        public void PlaySFX(string id)
        {
            if(_audioController == null) return;
            _audioController.PlaySfx(id);
        }
    }
}
