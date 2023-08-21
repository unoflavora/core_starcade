using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Agate.Starcade.Core.Scripts.Runtime.UI
{
    public class SliderToggle : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent OnToggleActive = new UnityEvent();

        private SliderToggleGroup _sliderToggleGroup;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnToggleActive.Invoke();
        }

        public void SetActiveToggle()
        {
            if (_sliderToggleGroup == null) return;
            _sliderToggleGroup.SelectToggles(this);
        }

        public void SetSliderToggleGroup(SliderToggleGroup sliderToggleGroup)
        {
            _sliderToggleGroup = sliderToggleGroup;
            OnToggleActive.AddListener(() =>
            {
                _sliderToggleGroup.SelectToggles(this);
            });
        }
    }
}