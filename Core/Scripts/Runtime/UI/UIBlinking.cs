using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIBlinking : MonoBehaviour
    {
        [SerializeField] private float _blinkTime;
        private CanvasGroup _canvasGroup;

        private LTDescr _blink;

        void Start()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
            BlinkIn();
        }

        void BlinkIn()
        {
            _canvasGroup.alpha = 0;
            _blink = LeanTween.value(0, 1, _blinkTime).setOnUpdate((float value) =>
            {
                if (_canvasGroup == null) return;
                _canvasGroup.alpha = value;
            }).setOnComplete(() =>
            {
                if (_canvasGroup == null) return;
                BlinkOut();
            });
        }

        void BlinkOut()
        {
            _canvasGroup.alpha = 1;
            LeanTween.value(1, 0, _blinkTime).setOnUpdate((float value) =>
            {
                if (_canvasGroup == null) return;
                _canvasGroup.alpha = value;
            }).setOnComplete(() => 
            {
                if (_canvasGroup == null) return;
                BlinkIn();
            });
        }

        private void OnDisable()
        {
            LeanTween.cancel(this.gameObject);
        }

        private void OnDestroy()
        {
            LeanTween.cancel(this.gameObject);
        }
    }
}
