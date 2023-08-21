using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Core.Scripts.Runtime.UI
{
    public class SliderToggleGroup : MonoBehaviour
    {
        public List<SliderToggle> SliderToggles;
        public RectTransform SelectionRectTransform;

        private bool _isMoving;

        private void Start()
        {
            foreach (var sliderToggle in SliderToggles)
            {
                sliderToggle.SetSliderToggleGroup(this);
            }

            //SelectionRectTransform.gameObject.transform.SetParent(SliderToggles[0].gameObject.transform);
            //SelectionRectTransform.transform.localPosition = Vector3.zero;
        }

        public void InitToggleGroup(SliderToggle selectedToggle)
        {
            SelectionRectTransform.gameObject.transform.SetParent(selectedToggle.gameObject.transform);
            SelectionRectTransform.transform.localPosition = Vector3.zero;
        }

        public void SelectToggles(SliderToggle target)
        {
            if (_isMoving)
            {
                LeanTween.cancel(SelectionRectTransform);
            }
            _isMoving = true;
            
            SelectionRectTransform.gameObject.transform.SetParent(target.gameObject.transform);
            LeanTween.move(SelectionRectTransform, Vector3.zero, 0.1f).setOnComplete(() =>
            {
                _isMoving = false;
            });
        }
    }
}