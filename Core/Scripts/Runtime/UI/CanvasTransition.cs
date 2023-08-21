using Agate.Starcade.Runtime.Main;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasTransition : MonoBehaviour
    {
        public enum TransitionType
        {
            None,
            SlideFromBottom,
            SlideFromTop,
            SlideFromLeft,
            SlideFromRight,
            SlideFromCustom
        }

        private enum SlideDirection 
        {
            Top,
            Left,
            Right,
            Bottom,
            Custom,
        }

        [SerializeField] private bool _useExternalTrigger;
        [SerializeField] private bool _useFade;
        [SerializeField] private TransitionType _transitionType;
        [SerializeField] private LeanTweenType _easeType;

        [SerializeField] bool _useCustomStartPosition;
        [SerializeField] Vector2 _customStartPosition;

        [SerializeField, Range(0,2000)] private float _marginHorizontal;
        [SerializeField, Range(0, 1000)] private float _marginVertical;

        [SerializeField] private float _delay;
        [SerializeField] private float _trasitionTime;
        [SerializeField] private RectTransform _mainPanelRectTransform;

        private CanvasGroup _group;

        private Vector2 _targetSlide;

        private void Awake()
        {
            _group = GetComponent<CanvasGroup>();

            if (_useFade) _group.alpha = 0;
        }

        private void OnEnable()
        {
            _group = this.gameObject.GetComponent<CanvasGroup>();
            if (_useExternalTrigger)
            {
                Setup();
                return;
            }

            if (_transitionType != TransitionType.None) _targetSlide = _mainPanelRectTransform.anchoredPosition;

            TriggerTransition();
        }

        private void OnDisable()
        {
            if (_useFade) _group.alpha = 0;
        }

        private void Start()
        {
            _group = this.gameObject.GetComponent<CanvasGroup>();
            if (_useExternalTrigger)
            {
                Setup();
                return;
            }

            if(_transitionType != TransitionType.None) _targetSlide = _mainPanelRectTransform.anchoredPosition;

            TriggerTransition();
        }

        private void Setup()
        {
            if (_useFade)
            {
                _group.alpha = 0;
            }

            switch (_transitionType)
            {
                case TransitionType.None:
                    break;
                case TransitionType.SlideFromBottom:
                    _mainPanelRectTransform.anchoredPosition = new Vector2(_mainPanelRectTransform.anchoredPosition.x, -1000f + _marginVertical);
                    break;
                case TransitionType.SlideFromTop:
                    _mainPanelRectTransform.anchoredPosition = new Vector2(_mainPanelRectTransform.anchoredPosition.x, 1000f - _marginVertical);
                    break;
                case TransitionType.SlideFromLeft:
                    _mainPanelRectTransform.anchoredPosition = new Vector2(-2000f + _marginHorizontal, _mainPanelRectTransform.anchoredPosition.y);
                    break;
                case TransitionType.SlideFromRight:
                    _mainPanelRectTransform.anchoredPosition = new Vector2(2000f - _marginHorizontal, _mainPanelRectTransform.anchoredPosition.y);
                    break;
                case TransitionType.SlideFromCustom:
                    _mainPanelRectTransform.anchoredPosition = _customStartPosition;
                    break;
            }
        }
        
        public async void TriggerTransition(string sfx = null)
        {
            if(_delay > 0)
            {
                float delayInMs = _delay * 1000;
                await Task.Delay((int)delayInMs);
            }

            if(sfx != null) MainSceneController.Instance.Audio.PlaySfx(sfx);

            if (_useFade)
            {
                Fade();
            }

            switch (_transitionType)
            {
                case TransitionType.None:
                    break;
                case TransitionType.SlideFromBottom:
                    Slide(SlideDirection.Bottom);
                    break;
                case TransitionType.SlideFromTop:
                    Slide(SlideDirection.Top);
                    break;
                case TransitionType.SlideFromLeft:
                    Slide(SlideDirection.Left);
                    break;
                case TransitionType.SlideFromRight:
                    Slide(SlideDirection.Right);
                    break;
                case TransitionType.SlideFromCustom:
                    Slide(SlideDirection.Custom);
                    break;
            }
        }

        public void TriggerFadeOut()
        {
            _group.alpha = 1;

            LeanTween.value(1, 0, _trasitionTime).setOnUpdate((float value) =>
            {
                if (_group == null) return;
                _group.alpha = value;
            }).setEase(_easeType);
        }

        public void TriggerSlideOut()
        {
            switch (_transitionType)
            {
                case TransitionType.None:
                    break;
                case TransitionType.SlideFromBottom:
                    SlideOut(SlideDirection.Bottom);
                    break;
                case TransitionType.SlideFromTop:
                    SlideOut(SlideDirection.Top);
                    break;
                case TransitionType.SlideFromLeft:
                    SlideOut(SlideDirection.Left);
                    break;
                case TransitionType.SlideFromRight:
                    SlideOut(SlideDirection.Right);
                    break;
                case TransitionType.SlideFromCustom:
                    SlideOut(SlideDirection.Custom);
                    break;
            }
        }

        private void Fade()
        {
            if (_group == null) return;
            _group.alpha = 0;

            LeanTween.value(0, 1, _trasitionTime).setOnUpdate((float value) =>
            {
                if (_group == null) return;
                _group.alpha = value;
            }).setEase(_easeType);
        }

        private void Slide(SlideDirection slideDirection)
        {
            if (_mainPanelRectTransform == null) return;
            //_targetSlide = _mainPanelRectTransform.anchoredPosition;

            switch (slideDirection)
            {
                case SlideDirection.Top:
                    _mainPanelRectTransform.anchoredPosition = new Vector2(_mainPanelRectTransform.anchoredPosition.x, 1000f - _marginVertical);
                    break;
                case SlideDirection.Left:
                    _mainPanelRectTransform.anchoredPosition = new Vector2(-2000f + _marginHorizontal, _mainPanelRectTransform.anchoredPosition.y);
                    break;
                case SlideDirection.Right:
                    _mainPanelRectTransform.anchoredPosition = new Vector2(2000f - _marginHorizontal, _mainPanelRectTransform.anchoredPosition.y);
                    break;
                case SlideDirection.Bottom:
                    _mainPanelRectTransform.anchoredPosition = new Vector2(_mainPanelRectTransform.anchoredPosition.x, -1000f + _marginVertical);
                    break;
                case SlideDirection.Custom:
                    _mainPanelRectTransform.anchoredPosition = _customStartPosition;
                    break;
            }

            LeanTween.move(_mainPanelRectTransform, _targetSlide, _trasitionTime).setEase(_easeType);
        }

        private void SlideOut(SlideDirection slideDirection)
        {
            if (_mainPanelRectTransform == null) return;
            //_targetSlide = _mainPanelRectTransform.anchoredPosition;

            Vector2 targetPos = Vector2.zero;

            switch (slideDirection)
            {
                case SlideDirection.Top:
                    targetPos = new Vector2(_mainPanelRectTransform.anchoredPosition.x, 1000f - _marginVertical);
                    break;
                case SlideDirection.Left:
                    targetPos = new Vector2(-2000f + _marginHorizontal, _mainPanelRectTransform.anchoredPosition.y);
                    break;
                case SlideDirection.Right:
                    targetPos = new Vector2(2000f - _marginHorizontal, _mainPanelRectTransform.anchoredPosition.y);
                    break;
                case SlideDirection.Bottom:
                    targetPos = new Vector2(_mainPanelRectTransform.anchoredPosition.x, -1000f + _marginVertical);
                    break;
                case SlideDirection.Custom:
                    targetPos = _customStartPosition;
                    break;
            }

            Vector2 startPos = _mainPanelRectTransform.anchoredPosition;

            LeanTween.move(_mainPanelRectTransform, targetPos, _trasitionTime).setEase(_easeType).setOnComplete(() =>
            {
                _mainPanelRectTransform.anchoredPosition = startPos;
            });
        }

    }
}