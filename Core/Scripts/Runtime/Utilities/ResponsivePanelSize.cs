using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade
{
    [ExecuteInEditMode]
    public class ResponsivePanelSize : MonoBehaviour
    {
        [SerializeField] private Vector2 _potraitSize;
        [SerializeField] private Vector2 _landscapeSize;
        private RectTransform _rect;

        private void Awake()
        {
            _rect = this.gameObject.GetComponent<RectTransform>();
        }

        private void Update()
        {
            OnChangeOrientation();
        }

        private void OnChangeOrientation()
        {
            _rect.sizeDelta = CheckScreenOrientation() ? _landscapeSize : _potraitSize;
        }
        
        private bool CheckScreenOrientation()
        {
            return Screen.width > Screen.height;
        }
    }
}
