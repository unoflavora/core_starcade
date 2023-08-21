using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.OnBoarding
{
    public class Highlight : MonoBehaviour, IOnBoardingTask
    {
        [SerializeField] private GameObject[] _listHighlightedObject;
        public IEnumerable<GameObject> ListHighlightedObject => _listHighlightedObject;

        [Header("Highlight")]
        [SerializeField] private bool _isStretch;
        [SerializeField] private GameObject _fxHighlight;
        [SerializeField] private GameObject _trailHighlight;
        [SerializeField] private Vector2 _highlightOffset;
        [SerializeField] private GameObject _screenBlocker;
        private RectTransform _rectTransHighlight;

        [Header("Swipe Animation")] [SerializeField]
        private GameObject _swipeIcon;

        [SerializeField] private int _swipeOffset;
        private RectTransform _rectTransSwipe;

        [Header("Trigger Button")] [SerializeField]
        private Button _triggerButton;

        [SerializeField] private Vector2 _triggerButtonOffset;
        private RectTransform _rectTransTriggerButton;

        private Transform _onBoardingTransform;
        private Button _sceneButtonObject;

        private UnityAction _onNextTaskAction;
        public UnityEvent OnCustomInteraction;
        public bool IsDoingTask { get; set; }

        public enum InteractType
        {
            Highlight,
            ButtonTrigger,
            SwipeAnimation,
            DelayOnly,
            Activate,
            Custom
        }

        private void Awake()
        {
            _rectTransHighlight = _fxHighlight.gameObject.GetComponent<RectTransform>();
            _rectTransSwipe = _swipeIcon.gameObject.GetComponent<RectTransform>();
            _rectTransTriggerButton = _triggerButton.gameObject.GetComponent<RectTransform>();
        }

        private async Task HighlightObject(int? index)
        {
            if (index == -1)
            {
                HideHighlight();
                return;
            }

            await Task.Delay(200);

            _onBoardingTransform = gameObject.transform;
            _fxHighlight.gameObject.SetActive(true);
            var highlightGameObject = _listHighlightedObject[index.GetValueOrDefault()];

            Canvas strechCanvas = highlightGameObject.GetComponentInParent<Canvas>();

            _fxHighlight.transform.SetParent(highlightGameObject.transform);

            RectTransform stretchRect = highlightGameObject.GetComponent<RectTransform>();
            float stretchWidth = (stretchRect.anchorMax.x - stretchRect.anchorMin.x) * Screen.width + stretchRect.sizeDelta.x * strechCanvas.scaleFactor;
            float stretchHeight = (stretchRect.anchorMax.y - stretchRect.anchorMin.y) * Screen.height + stretchRect.sizeDelta.y * strechCanvas.scaleFactor;
            Vector2 size = _isStretch ? new Vector2(stretchWidth, stretchHeight) : highlightGameObject.GetComponent<RectTransform>().sizeDelta;
            _fxHighlight.GetComponent<RectTransform>().sizeDelta = size + _highlightOffset;

            Debug.Log($"Highlight size {size}");

            _fxHighlight.transform.localPosition = Vector3.zero;
            _fxHighlight.transform.SetParent(_onBoardingTransform);
            _fxHighlight.transform.SetAsFirstSibling();
        }

        private void SwipeAnimation(int? index)
        {
            Vector2 sizeDelta;
            LeanTween.value(_swipeIcon, _swipeOffset, (sizeDelta = _rectTransHighlight.sizeDelta).x - _swipeOffset,
                    1500 / sizeDelta.x)
                .setOnUpdate(
                    val =>
                    {
                        _rectTransSwipe.anchoredPosition = new Vector2(val,
                            _rectTransSwipe.anchoredPosition.y);
                    }).setLoopClamp().setEaseOutExpo().delay = 0.5f;
            ;
        }

        private void ActivateObject(int index)
        {
            if (index < 0) return;
            _listHighlightedObject[index].SetActive(!_listHighlightedObject[index].activeSelf);
        }

        private void ButtonTrigger(int? index)
        {
            _rectTransTriggerButton.sizeDelta = _rectTransHighlight.sizeDelta + _triggerButtonOffset;
            _rectTransTriggerButton.transform.localPosition = Vector3.zero;

            _sceneButtonObject = _listHighlightedObject[index.GetValueOrDefault()].GetComponent<Button>();
            _triggerButton.onClick.AddListener(StartTrigger);
        }

        private async Task Delay(int delay)
        {
            Debug.Log("START DELAY " + delay);
            await Task.Delay(1000 * delay);
            Debug.Log("END DELAY");
        }

        private void StartTrigger()
        {
            try
            {
                Debug.Log("Interact with Object");
                _sceneButtonObject.onClick.Invoke();
            }
            catch
            {
                Debug.Log("Cant Interact with Object");
            }

            if (_onNextTaskAction == null)
            {
                Debug.Log("Action null, check here");
                _onNextTaskAction.Invoke();
            }
            else
            {
                Debug.Log("task interact finish");
                _triggerButton.onClick.RemoveAllListeners();
                _onNextTaskAction.Invoke();
            }
        }

        private void HideHighlight()
        {
            _fxHighlight.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        }

        public void StartTask(object onBoardingEvent)
        {
            var currentEvent = (OnBoardingEvent)onBoardingEvent;
            HighlightObject(currentEvent.HighlightedObjectIndex);
        }

        public async void StartTask(object onBoardingEvent, UnityAction onFinishTask, UnityAction onNextTask)
        {
            _trailHighlight.SetActive(false);
            var currentEvent = (OnBoardingEvent)onBoardingEvent;
            IsDoingTask = true;

            switch (currentEvent.InteractType)
            {
                case InteractType.Highlight:
                    await HighlightObject(currentEvent.HighlightedObjectIndex);
                    _swipeIcon.SetActive(false);
                    _triggerButton.gameObject.SetActive(false);
                    _trailHighlight.SetActive(true);
                    _screenBlocker.SetActive(true);
                    await Delay(currentEvent.Delay);
                    onFinishTask.Invoke();
                    break;
                case InteractType.SwipeAnimation:
                    await HighlightObject(currentEvent.HighlightedObjectIndex);
                    _swipeIcon.SetActive(true);
                    _triggerButton.gameObject.SetActive(false);
                    _screenBlocker.SetActive(true);
                    SwipeAnimation(currentEvent.HighlightedObjectIndex);
                    await Delay(currentEvent.Delay);
                    onFinishTask.Invoke();
                    break;
                case InteractType.ButtonTrigger:
                    await HighlightObject(currentEvent.HighlightedObjectIndex);
                    _swipeIcon.SetActive(false);
                    _triggerButton.gameObject.SetActive(true);
                    _screenBlocker.SetActive(true);
                    _onNextTaskAction = onFinishTask;
                    ButtonTrigger(currentEvent.HighlightedObjectIndex);
                    await Delay(currentEvent.Delay);
                    break;
                case InteractType.DelayOnly:
                    Debug.Log("RUN DELAY");
                    await Delay(currentEvent.Delay);
                    _screenBlocker.SetActive(true);
                    onFinishTask.Invoke();
                    break;
                case InteractType.Activate:
                    await HighlightObject(currentEvent.HighlightedObjectIndex);
                    _swipeIcon.SetActive(false);
                    _triggerButton.gameObject.SetActive(false);
                    _screenBlocker.SetActive(true);
                    _trailHighlight.SetActive(false);
                    ActivateObject(currentEvent.HighlightedObjectIndex);
                    onFinishTask.Invoke();
                    break;
                case InteractType.Custom:
                    await HighlightObject(currentEvent.HighlightedObjectIndex);
                    _swipeIcon.SetActive(false);
                    _triggerButton.gameObject.SetActive(false);
                    _screenBlocker.SetActive(false);
                    _trailHighlight.SetActive(false);
                    OnCustomInteraction.AddListener(onFinishTask);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            IsDoingTask = false;
            Debug.Log("task highlight finish");
        }

        public void OnSkipTask()
        {
            IsDoingTask = false;
        }

        public void FinishCustomTask()
        {
            Debug.Log("WOy");
            OnCustomInteraction.Invoke();
            OnCustomInteraction.RemoveAllListeners();
        }

        public void OnTaskFinish()
        {
            IsDoingTask = false;
        }

        public void OnBoardingFinish()
        {
            Debug.Log("Hide highlight");
            HideHighlight();
        }
    }
}