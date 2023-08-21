using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class ToggleButton : MonoBehaviour
    {
        private Button button;
        public FadeTween CheckmarkFade;
        public ToggleButtonGroup ToggleButtonGroup;

        public TMP_Text CheckmarkText;

        public float FadeSpeed;

        public UnityEvent OnSelect;
        public UnityEvent OnDeselect;

        public EventSystem EventSystem;

        private void Awake()
        {
            OnSelect.AddListener(() => FadeImage(false));
            OnDeselect.AddListener(() => FadeImage(true));
            
            button = this.gameObject.GetComponent<Button>();
            //CheckmarkFade.gameObject.SetActive(true);
            if (CheckmarkText != null) CheckmarkText.text = this.GetComponent<TMP_Text>().text;
            
            button.onClick.AddListener(() =>
            {
                if (ToggleButtonGroup != null)
                {
                    ToggleButtonGroup.Setup(this);
                }
                OnSelect.Invoke();
            });
            
            if (ToggleButtonGroup != null)
            {
                ToggleButtonGroup.AddToggleButton(this);
                FadeImage(this != ToggleButtonGroup.DefaultToggleButton);

                if(button != null && button.onClick != null && this == ToggleButtonGroup?.DefaultToggleButton)
                {
                    try
                    { 
                        button.onClick.Invoke();
                    }
                    catch { }

				}
            }
            
        }

        public void Init(ToggleButtonGroup buttonGroup)
        {
            ToggleButtonGroup = buttonGroup;
            ToggleButtonGroup.AddToggleButton(this);
            FadeImage(this != ToggleButtonGroup.DefaultToggleButton);
            if(this == ToggleButtonGroup.DefaultToggleButton) button.onClick.Invoke();
        }

        public void DisableCheckmark()
        {
            Debug.Log("Disable");
            if (CheckmarkFade != null)
            {
                if (CheckmarkFade.State())
                {
                    FadeImage(true);
                    OnDeselect.Invoke();
                }
            }
            else
            {
                OnDeselect.Invoke();
            }
        }

        private void FadeImage(bool fadeAway)
        {
            if (CheckmarkFade != null)
            {
                CheckmarkFade.gameObject.SetActive(!fadeAway);
            }
        }
        
        /*IEnumerator FadeImage(bool fadeAway)
        {
            float counter = 0f;
            float start;
            float end;

            if (fadeAway)
            {
                start = 1;
                end = 0;
            }
            else
            {
                start = 0;
                end = 1;
            }
            
            while(counter < FadeSpeed)
            {
                counter += Time.deltaTime;
                Checkmark.alpha = Mathf.Lerp(start, end, counter / FadeSpeed);

                yield return null; 
            }
        }*/
    }
}
