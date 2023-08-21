using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Agate.Starcade
{
    public class CoinReward : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private Transform[] _point;
        float count = 1f;


        async Task Emit()
        {
            await Task.Delay(5);
        }
        

        //private async void OnEnable()
        //{
            //await StartEmit();
        //}

        private void OnDisable()
        {
            
        }

        
        
    }
}
