using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class OnBoardingCharacter : MonoBehaviour, IOnBoardingTask
    {
        [SerializeField] private Image _characterImage;
        [SerializeField] private FadeTween _fade;
        private Image _fadeCharacterImage;
        private CharacterExpression _characterExpression;
        public bool IsDoingTask { get; set; }


        private Vector3 oldPos;

        #region Private Method

        private async void SetupCharacter(int indexExpression, Vector2 anchoredPosition, CharacterExpression characterExpression, bool isFlipped)
        {
            _characterExpression = characterExpression;
            FadeCharacter(indexExpression);
            if(anchoredPosition.Equals(this.gameObject.GetComponent<RectTransform>().anchoredPosition)) return;
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
            FlipCharacter(isFlipped);
            //MoveCharacter(newPos);
        }
        
        private void MoveCharacter(Vector2 EndPosition)
        {
            LeanTween.moveLocal(gameObject, EndPosition, 0.5f).setEaseInOutQuad();
        }

        private void FlipCharacter(bool isFlipped)
        {
            gameObject.transform.localRotation = isFlipped ? Quaternion.Euler(0,-180f,0) : Quaternion.Euler(0,0,0);
        }
        
        private async Task SetRectTransformData(RectTransformData rectTransformData)
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

        private void FadeCharacter(int index)
        {
            //if(index > _characterExpression.ListCharacterExpression.Count - 1)
            Debug.Log("change sprite");
            _characterImage.sprite = _characterExpression.ListCharacterExpression[index].Sprite;
        }

        private async Task HideCharacter()
        {
            Vector2 hidePos = new Vector2(gameObject.transform.localPosition.x,
                gameObject.gameObject.GetComponent<RectTransform>().sizeDelta.y * -1);
            LeanTween.moveLocal(gameObject, hidePos, 1f).setEaseInBack();
            await Task.Delay(1000);
        }
        
        private async Task HideCharacterInstantly()
        {
            Vector2 hidePos = new Vector2(gameObject.transform.localPosition.x,
                gameObject.gameObject.GetComponent<RectTransform>().sizeDelta.y * -1);
            LeanTween.moveLocal(gameObject, hidePos, 0f).setEaseInBack();
            await Task.Delay(1000);
        }

        #endregion

        public void StartTask(object onBoardingEvent,UnityAction onFinishTask, UnityAction onNextTask)
        {
            OnBoardingEvent currentEvent = (OnBoardingEvent)onBoardingEvent;
            IsDoingTask = true;
            if (currentEvent.HideCharacter)
            {
                HideCharacterInstantly();
            }
            SetupCharacter(currentEvent.CharacterExpressionId, currentEvent.CharacterAnchoredPosition, currentEvent.CharacterExpression, currentEvent.IsFlipped);
            Debug.Log("task character finish");
            onFinishTask.Invoke();
            IsDoingTask = false;
        }

        public void OnSkipTask()
        {
            IsDoingTask = false;
        }

        public void OnTaskFinish()
        {
            //Debug.Log("");
        }

        public void OnBoardingFinish()
        {
            Debug.Log("Hide interact");
            HideCharacter();
        }
    }
}
