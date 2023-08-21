using System;
using UnityEngine;
using UnityEngine.UI;

namespace Starcade.Core.Runtime.UI
{
    public class ShiningButtonController : MonoBehaviour
    {
        [SerializeField] private Button _shiningButton;

        [SerializeField] private Material _shineMat;

        private void OnEnable()
        {
            _shiningButton.image.material = null;
        }

        void LateUpdate()
        {
            _shiningButton.image.material = _shiningButton.interactable ? _shineMat : null;
        }

        private void OnDisable()
        {
            _shiningButton.image.material = null;
            _shiningButton.interactable = false;
        }
    }
}
