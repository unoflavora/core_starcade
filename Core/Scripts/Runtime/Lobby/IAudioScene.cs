using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate
{
    public interface IAudioScene
    {
        public void SetupAudio();
        public void PlayBGM();
        public void PlaySFX(string audioKey);

        public void MuteBGM();
        public void MuteSFX();
    }
}
