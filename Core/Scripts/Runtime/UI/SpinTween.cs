using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade
{
    public class SpinTween : MonoBehaviour
    {
        private void Awake()
        {
            
        }

        private void Start()
        {
            var seq = LeanTween.sequence();
            seq.append(LeanTween.value(this.gameObject, SetRotate, 0, 360, 3f).setEaseInQuad());
            seq.append(LeanTween.value(this.gameObject, SetRotate, 0, 3600, 4f));
            seq.append(LeanTween.value(this.gameObject, SetRotate, 0, 360, 2f).setEaseInExpo());
        }


        public void StartSpin()
        {
            LeanTween.value(this.gameObject, SetRotate, 0, 360, 3f).setEaseInQuad();
        }

        public void Spin()
        {
            LeanTween.value(this.gameObject, SetRotate, 0, 3600, 4f).setLoopCount(10);
        }

        public void EndSpin()
        {
            LeanTween.value(this.gameObject, SetRotate, 0, 360, 2f).setEaseInQuad();
        }

        public void SetRotate(float rotate)
        {
            this.transform.localRotation = Quaternion.Euler(0,rotate,0);
        }
    }
}
