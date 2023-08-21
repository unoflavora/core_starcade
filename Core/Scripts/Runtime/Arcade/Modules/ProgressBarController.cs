using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules
{
    public class ProgressBarController : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        
        private float _progress = 0;
        public float MaxValue => slider.maxValue;
        
        public UnityEvent onJackpot;

        public void Init(int totalProgress, double progress)
        {
            onJackpot = new UnityEvent();
            
            _progress = (float) progress;
            
            slider.value = _progress;

            slider.maxValue = totalProgress;
            
            onJackpot = new UnityEvent();
            
            // SetSliderSize();
        }

        private void SetSliderSize()
        {
            float gridHeight = GameObject.FindGameObjectWithTag("Background").GetComponent<RectTransform>().rect.height;
            RectTransform rect = slider.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.rect.width, gridHeight);
        }

        public async Task AddProgress(float reward)
        {
            var isCountComplete = false;
            
            _progress += reward;
            
            if (_progress >= slider.maxValue)
            {
                LeanTween.value(slider.gameObject, f => slider.value = f, slider.value, slider.maxValue, .25f)
                    .setOnComplete(() =>
                    {
                        slider.value = 0;
                        _progress = _progress % slider.maxValue;
                        LeanTween.value(slider.gameObject, f => slider.value = f, slider.value, _progress, .25f);
                        onJackpot.Invoke();
                        isCountComplete = true;
                    });
            }
            else
            {
                LeanTween.value(slider.gameObject, f => slider.value = f, slider.value, _progress, .25f).setOnComplete(
                    () =>
                    {
                        isCountComplete = true;
                    });
            }

            while (!isCountComplete) await Task.Delay(50);
        }
    }
}
