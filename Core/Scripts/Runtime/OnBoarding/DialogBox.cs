using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Main;

namespace Agate.Starcade
{
    public class DialogBox : MonoBehaviour, IOnBoardingTask
    {
        private string _dialog;
        [SerializeField] private Canvas _onBoardingCanvas;
        [SerializeField] private TMP_Text _dialogText;
        [SerializeField] private TMP_Text _hintText;
        [SerializeField] private Blinking _iconBlink;
        [SerializeField] private FadeTween _fade;
        [SerializeField] private RectTransformData rectTransformData;
        [SerializeField] private bool _isPortait;

        private Transform _startPos;

        private RectTransform rectTransform;

        private Vector2 _startOffsetMax;
        private Vector2 _startOffsetMin;

        private bool isTalking;
        public bool IsTalking => isTalking;
        public bool IsDoingTask { get; set; }

        private CancellationTokenSource _tokenSource;

        #region Unity Method

        private void Awake()
        {
            _startPos = this.transform;
            rectTransform = this.gameObject.GetComponent<RectTransform>();
            _startOffsetMax = gameObject.GetComponent<RectTransform>().offsetMax;
            _startOffsetMin = gameObject.GetComponent<RectTransform>().offsetMin;
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Use <seealso cref="StartDialog(OnBoardingEvent, CancellationToken)"/> instead for canceling
        /// </summary>
        /// <param name="onBoardingEvent"></param>
        /// <returns></returns>
        [Obsolete]
        private async Task StartDialog(OnBoardingEvent onBoardingEvent)
        {
            isTalking = true;
            await _fade.FadeOutAsync();
            SetDialog(onBoardingEvent.DialogText, onBoardingEvent.DialogWidthRatio, onBoardingEvent.DialogAnchoredPosition, onBoardingEvent.DialogAnchorMin, onBoardingEvent.DialogAnchorMax);
            _fade.FadeIn();
            _iconBlink.IsBlinking = false;
            _hintText.text = string.Empty;
            await Task.Delay(1500);
            isTalking = false;
            //_hintText.text = "Tap anywhere to continue";
        }

        private async Task StartDialog(OnBoardingEvent onBoardingEvent, CancellationToken token)
        {
            isTalking = true;
            await _fade.FadeOutAsync(token);
            SetDialog(onBoardingEvent.DialogText, onBoardingEvent.DialogWidthRatio, onBoardingEvent.DialogAnchoredPosition, onBoardingEvent.DialogAnchorMin, onBoardingEvent.DialogAnchorMax);
            _fade.FadeInAsync(token);
            _iconBlink.IsBlinking = false;
            _hintText.text = string.Empty;

            bool isComplete = token.IsCancellationRequested;
            if (!isComplete)
            {
                LeanTween.value(gameObject, 0, 1, 1.5f).setOnComplete(() => isComplete = true);
            }

            while (!isComplete)
            {
                if (token.IsCancellationRequested)
                {
                    isComplete = true;
                    LeanTween.cancel(gameObject);
                    break;
                }
                await Task.Delay(10);
            }

            isTalking = false;
        }

        public async Task SetDialog(string dialog, float widthRatio, Vector2 anchoredPos, Vector2 anchorMin, Vector2 anchorMax)
        {
            _dialogText.text = dialog;
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
            this.gameObject.GetComponent<RectTransform>().anchorMin = anchorMin;
            this.gameObject.GetComponent<RectTransform>().anchorMax = anchorMax;
            this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                widthRatio * _onBoardingCanvas.GetComponent<RectTransform>().sizeDelta.x,
                _dialogText.gameObject.GetComponent<RectTransform>().sizeDelta.y);
        }
// #if UNITY_EDITOR
//             if(_isPortait) 
//             else this.gameObject.GetComponent<RectTransform>().sizeDelta  = new Vector2(widthRatio * 1920, _dialogText.gameObject.GetComponent<RectTransform>().sizeDelta.y);
// #else
//             this.gameObject.GetComponent<RectTransform>().sizeDelta  = new Vector2(widthRatio * Screen.width, _dialogText.gameObject.GetComponent<RectTransform>().sizeDelta.y);
// #endif
//         }

        public async Task SetDialogDebugLandscape(string dialog, float widthRatio, Vector2 anchoredPos, Vector2 anchorMin, Vector2 anchorMax)
        {
            Debug.Log("test");
            _dialogText.text = dialog;
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
            this.gameObject.GetComponent<RectTransform>().anchorMin = anchorMin;
            this.gameObject.GetComponent<RectTransform>().anchorMax = anchorMax;
            this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(widthRatio * _onBoardingCanvas.GetComponent<RectTransform>().sizeDelta.x,
                _dialogText.gameObject.GetComponent<RectTransform>().sizeDelta.y);


        }
        public async Task SetDialogDebugPortrait(string dialog, float widthRatio, Vector2 anchoredPos, Vector2 anchorMin, Vector2 anchorMax)
        {
            Debug.Log("test");
            _dialogText.text = dialog;
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
            this.gameObject.GetComponent<RectTransform>().anchorMin = anchorMin;
            this.gameObject.GetComponent<RectTransform>().anchorMax = anchorMax;
            this.gameObject.GetComponent<RectTransform>().sizeDelta  = new Vector2(widthRatio * _onBoardingCanvas.GetComponent<RectTransform>().sizeDelta.x, _dialogText.gameObject.GetComponent<RectTransform>().sizeDelta.y);
        }

        private async Task MoveDialog(Vector2 offsetMax, Vector2 offsetMin)
        {
            _fade.FadeIn();
            LeanTween.value(gameObject,rectTransform.offsetMax, offsetMax, 0.5f).setOnUpdate( (Vector2 val)=>
            {
                rectTransform.offsetMax = val;
            } );
            
            LeanTween.value(gameObject,rectTransform.offsetMin, offsetMin, 0.5f).setOnUpdate( (Vector2 val)=>
            {
                rectTransform.offsetMin = val;
            } );
            await Task.Delay(500);
        }

        private string CheckRegex(string dialog)
        {
            return MainSceneController.Instance.Data.UserData.Username != null
                ? Regex.Replace(dialog, "@Username", MainSceneController.Instance.Data.UserData.Username)
                : Regex.Replace(dialog, "@Username", "Player");
        }
        
        private async Task EndDialog()
        {
            _iconBlink.gameObject.SetActive(true);
            _iconBlink.IsBlinking = true;
            _iconBlink.Blink();
            _hintText.text = "Tap anywhere to continue";
        }

        private async Task PlayDialog(string dialog)
        {
            _dialogText.text = String.Empty;
            _hintText.text = "Tap anywhere to skip";
            isTalking = true;
            foreach (char c in dialog)
            {
                _dialogText.text += c;
                if (!isTalking)
                {
                    _dialogText.text = dialog;
                    Task.Yield();
                    return;
                }
                await Task.Delay(25);
            }
            _hintText.text = "Tap anywhere to continue";
            isTalking = false;
        }

        private void SkipDialog()
        {
            //isTalking = false;
            //_hintText.text = "Tap anywhere to continue";
        }
        
        private async void HideDialog()
        {
            await _fade.FadeOutAsync();
            _dialogText.text = String.Empty;
        }

        private void SetRectTransformData(RectTransformData rectTransformData)
        {
            this.gameObject.GetComponent<RectTransform>().localPosition = rectTransformData.LocalPosition;
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = rectTransformData.AnchoredPosition;
            this.gameObject.GetComponent<RectTransform>().sizeDelta = rectTransformData.SizeDelta;
            this.gameObject.GetComponent<RectTransform>().anchorMin = rectTransformData.AnchorMin;
            this.gameObject.GetComponent<RectTransform>().anchorMax = rectTransformData.AnchorMax;
            this.gameObject.GetComponent<RectTransform>().pivot = rectTransformData.Pivot;
            this.gameObject.GetComponent<RectTransform>().localScale = rectTransformData.Scale;
            this.gameObject.GetComponent<RectTransform>().localRotation = rectTransformData.Rotation;
        }

        #endregion
        
        public async void StartTask(object onBoardingEvent, UnityAction onFinishTask, UnityAction onNextTask)
        {
            OnBoardingEvent currentEvent = (OnBoardingEvent)onBoardingEvent;
            if (currentEvent.HideDialog)
            {
                HideDialog();
            }
            else
            {
                //_hintText.gameObject.SetActive(currentEvent.Delay <= 0);
                IsDoingTask = true;
                _tokenSource = new CancellationTokenSource();
                await StartDialog(currentEvent, _tokenSource.Token);
                _tokenSource.Dispose();
                Debug.Log("task dialog finish");
            }

            IsDoingTask = false;
            onFinishTask.Invoke();
        }

        public void OnSkipTask()
        {
            _tokenSource?.Cancel();
        }

        public void OnTaskFinish()
        {
            _iconBlink.gameObject.SetActive(true);
            _iconBlink.IsBlinking = true;
            _iconBlink.Blink();
            _hintText.text = "Tap anywhere to continue";
            EndDialog();
        }

        public void OnBoardingFinish()
        {
            Debug.Log("Hide dialog");
            EndDialog();
            HideDialog();
        }
    }
}
