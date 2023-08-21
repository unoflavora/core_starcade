using System.Collections;
using System.Text;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Utilities;
using TMPro;
using UnityEngine;

namespace Starcade.Core.Scripts.Runtime.Utilities
{
    public class NumberCounter : MonoBehaviour
    {
        public TMP_Text Text;
        public bool EnableIcon = false;
        public int CountFPS = 30;
        public float Duration = .5f;
        private int _value = 0;

        private AudioController _audioController;
        
        public double Balance
        {
            get
            {
                return _value;
            }
            set => SetScore(value);
        }
        
        public (double,CurrencyTypeEnum) BalanceWithIcon
        {
            set => SetScore(value.Item1,value.Item2);
        }

        private void SetScore(double value, CurrencyTypeEnum type = default)
        {
            if (value < Mathf.Pow(10, 8))
            {
                if (value > 0)
                {
                    UpdateText(value);
                }
                else
                {
                    Text.SetText(value.ToString());
                }
            }
            else
            {
                Text.SetText(CurrencyHandler.Convert(value));
            }

            if (EnableIcon)
            {
               Text.text += SetIcon(type);
            }
            _value = (int)value;
        }

        private StringBuilder SetIcon(CurrencyTypeEnum currencyTypeEnum)
        {
            var stringBuilder = new StringBuilder(Text.text);
            string prefix = null;
            switch (currencyTypeEnum)
                {
                    case CurrencyTypeEnum.GoldCoin:
                        prefix = "<sprite=0>";
                        break;
                    case CurrencyTypeEnum.StarCoin:
                        prefix = "<sprite=1>";
                        break;
                    case CurrencyTypeEnum.StarTicket:
                        prefix = "<sprite=2>";
                        break;
                }
            stringBuilder.Insert(0, prefix);
            return stringBuilder;
        }

        private Coroutine _countingCoroutine;

        private void Awake()
        {
            if(!Text) Text = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            Debug.Log("Start number counter");
            _audioController = MainSceneController.Instance.Audio;
        }
        
        private void UpdateText(double newValue)
        {
            if (_countingCoroutine != null)
            {
                StopCoroutine(_countingCoroutine);
            }

            _countingCoroutine = StartCoroutine(CountText((int) newValue));

        }

        private IEnumerator CountText(int newValue)
        {
            var wait = new WaitForSeconds(1f / CountFPS);
            var previousValue = _value;
            int stepAmount;
            stepAmount = newValue - previousValue < 0 ? Mathf.FloorToInt((newValue - previousValue) / (CountFPS * Duration)) : Mathf.CeilToInt((newValue - previousValue) / (CountFPS * Duration));

            if (previousValue <= newValue)
            {
                while (previousValue < newValue)
                {
                    previousValue += stepAmount;
                    if (previousValue >= newValue)
                    {
                        previousValue = newValue;
                    }

                    Text.SetText(previousValue.ToString("n0"));

                    yield return wait;
                    
                    _audioController.PlaySfx(MainSceneController.AUDIO_KEY.BALANCE_COUNTING_ONCE);
                }
                
                Text.SetText(newValue.ToString("n0"));
                _audioController.PlaySfx(MainSceneController.AUDIO_KEY.BALANCE_COUNTING_END);
            }

            if (previousValue <= newValue)
            {
                yield break;
            }
            
            while (previousValue > newValue)
            {
                previousValue = newValue;
                Text.SetText(previousValue.ToString("n0"));

            }
        }
    }
}
