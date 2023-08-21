using System.Collections;
using System.Collections.Generic;
using Agate.Starcade;
using UnityEngine;

namespace Agate.Starcade.Runtime.Audio
{
    public interface IAudioPanel
    {
        [HideInInspector] AudioController Audio { set; get; }
        public void SetupAudio(AudioController audioController);
        public void PlaySFX(string audioKey);
    }
}
