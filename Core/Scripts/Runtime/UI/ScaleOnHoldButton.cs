using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Starcade.Core.Runtime.UI
{
    public class ScaleOnHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private List<Graphic> _graphics;
        [SerializeField] private Button _button;

        private LTDescr _currentAnim;
        private void Start()
        {
            _button = GetComponentInParent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(_currentAnim != null) LeanTween.cancel(_currentAnim.id);
            
            _graphics.ForEach(graphic =>
            {
                _currentAnim = LeanTween.scale(graphic.rectTransform, Vector3.one * .8f, .2f);
                graphic.color = _button.colors.pressedColor;
            });
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(_currentAnim != null) LeanTween.cancel(_currentAnim.id);

            _graphics.ForEach(graphic =>
            {
                _currentAnim = LeanTween.scale(graphic.rectTransform, Vector3.one, .2f);
                graphic.color = _button.colors.normalColor;
            });
        }
    }
}
