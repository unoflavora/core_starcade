using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Audio;
using UnityEngine;

namespace Agate
{
    public class RewardCollider : MonoBehaviour
    {
        private AudioController _audio;
        private int delay;
        private bool isPlaySFX = false;

        private void Start()
        {
            // _audio = MainSceneLauncher.Instance.Audio;
        }

        private async void OnParticleCollision(GameObject other)
        {
            //PlaySFX();
        }
        
        private async Task PlaySFX()
        {
            if (!isPlaySFX)
            {
                isPlaySFX = true;
                _audio.PlaySfx("goal_normal");
                await Task.Delay(750);
                isPlaySFX = false;
            }
            Debug.Log("COLLISION BRO");
        }
        
    }
}
