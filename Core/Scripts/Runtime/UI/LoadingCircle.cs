using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class LoadingCircle : MonoBehaviour
    {
        private Image _circle;
        private void OnEnable()
        {
            _circle = this.gameObject.GetComponent<Image>();
            Spin();
        }
      
        void Spin()
        {
            LeanTween.rotateAroundLocal(this.gameObject, Vector3.forward, 360, 1f).setLoopClamp();
        }

    }
}
