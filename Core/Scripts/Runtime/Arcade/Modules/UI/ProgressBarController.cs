using System;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules.UI
{
    public class ProgressBarController : MonoBehaviour
    {
        private float _minimum;
        private float _maximum { set; get; }
        private float _current { set; get; }
        private float _nextFill;
        [SerializeField]
        private Image _fill;
        [SerializeField]
        private float _fillAmount;

        public void UpdateProgressBar(float current, float maximum)
        {
            if (!(maximum > 0)) return;
            _current = current;
            var o = gameObject;
            var prevFill = _fill.GetComponent<Image>().fillAmount;
            var nextFill = current / maximum;
            _nextFill = nextFill;
            LeanTween.value(o, Fill, prevFill, nextFill, 0.5f);
        }

        private void Update()
        {
            /*if (_fill.GetComponent<Image>().fillAmount < _nextFill)
            {
                _fill.GetComponent<Image>().fillAmount += _fillAmount;
            }
            else if (_fill.GetComponent<Image>().fillAmount > _nextFill)
            {
                _fill.GetComponent<Image>().fillAmount -= _fillAmount;
            }
            else
            {
                
            }*/
        }

        private void Fill(float fillAmount)
        {
            _fill.GetComponent<Image>().fillAmount = fillAmount;
        }


        public void SetCurrent(float value)
        {
            _current = value;
        }

        public Image GetFill()
        {
            return _fill;
        }

        public float GetCurrent()
        {
            return _current;
        }

        public void SetMaximum(float value)
        {
            _maximum = value;
        }

        public float GetMaximum()
        {
            return _maximum;
        }
    }
}