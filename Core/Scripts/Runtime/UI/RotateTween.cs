using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade
{
    public class RotateTween : MonoBehaviour
    {
        private RectTransform rectTransform;

        [SerializeField] private float Duration;
        [SerializeField] private float TotalRotation;
        
        private void Awake()
        {
            rectTransform = this.GetComponent<RectTransform>();
        }

        public void RotateIn()
        {
            transform.LeanRotateAroundLocal(Vector3.forward, TotalRotation, Duration).setEaseOutExpo().delay = 0.1f;
        }

        public void RotateOut()
        {
            transform.LeanRotateAroundLocal(Vector3.forward, TotalRotation * -1, Duration).setEaseOutExpo().delay = 0.1f;
        }

        public void RotateIn(float rotation)
        {
            transform.LeanRotateAroundLocal(Vector3.forward, rotation, Duration).setEaseOutExpo().delay = 0.1f;
        }

        public void RotateOut(float rotation)
        {
            transform.LeanRotateAroundLocal(Vector3.forward, rotation * -1, Duration).setEaseOutExpo().delay = 0.1f;
        }

    }
}
