using System;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.UI
{
    public class SliderValueText : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private float _multiplier = 1f;
        [SerializeField] private string _prefix = String.Empty;
        [SerializeField] private string _suffix = String.Empty;

        private TMP_Text _text;

        private void Awake()
        {
            _slider.onValueChanged.AddListener(UpdateText);
        }

        private void Start()
        {
            _text = gameObject.GetComponent<TMP_Text>();
            UpdateText(_slider.value);
        }

        private void UpdateText(float value)
        {
            var realValue = value * _multiplier;

            realValue = MathF.Floor(realValue);

            if (_text == null) return;

            _text.text = _prefix + realValue + _suffix;
        }
    }
}