using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Agate.Starcade
{
    public class FadeTween : MonoBehaviour
    {
        [SerializeField] private float duration;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private float _maxAlpha = 1f;
        [SerializeField] private float _minAlpha = 0f;

        public CanvasGroup Canvas
        {
            get { return _canvasGroup; }
        }
        
        private void Awake()
        {
            _canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        }

        public void FadeIn()
        {
            _canvasGroup.LeanAlpha( _maxAlpha, duration);
        }
        public void FadeOut()
        {
            _canvasGroup.LeanAlpha( _minAlpha, duration);
        }

        public void ForceFadeOut(bool isForceRemoveFade = true)
        {
            if (isForceRemoveFade)
            {
                ForceRemoveFade();
            }

            _canvasGroup.alpha = _minAlpha;
        }

        public void ForceFadeIn(bool isForceRemoveFade = true)
        {
            if (isForceRemoveFade)
            {
                ForceRemoveFade();
            }
            
            _canvasGroup.alpha = _maxAlpha;
        }

        public void ForceRemoveFade()
        {
            LeanTween.cancel(this.gameObject);
            LeanTween.cancel(_canvasGroup.gameObject);
        }

        public async Task FadeOutAsync()
        {
            _canvasGroup.LeanAlpha( _minAlpha, duration);
            var delayFloat = duration * 1000;
            var delayInt = (int)delayFloat;
            await Task.Delay(delayInt);
        }

        public async Task FadeOutAsync(CancellationToken token)
        {
            FadeOut();

            bool isComplete = token.IsCancellationRequested;

            LeanTween.value(gameObject, 0, 1, duration).setOnComplete(() => isComplete = true);

            while (!isComplete) 
            { 
                if (token.IsCancellationRequested)
                {
                    isComplete = true;
                }
                await Task.Delay(10);
            }

            if (token.IsCancellationRequested)
            {
                ForceFadeOut();
            }

            return;
        }

        public async Task FadeInAsync(CancellationToken token)
        {
            FadeIn();

            bool isComplete = token.IsCancellationRequested;
            LeanTween.value(gameObject, 0, 1, duration).setOnComplete( () => isComplete = true);
            
            while (!isComplete)
            {
                if (token.IsCancellationRequested)
                {
                    isComplete = true;
                    break;
                }

                await Task.Delay(10);
            }

            if (token.IsCancellationRequested)
            {
                ForceFadeIn();
            }

            return;
        }
        
        public async Task FadeInAsyncWithAutoFadeOut(CancellationToken token)
        {
            
            return;
        }

        public bool State()
        {
            return _canvasGroup.alpha == _maxAlpha;
        }
    }
}
