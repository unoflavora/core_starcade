using Agate.Starcade.Runtime.Main;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.UI
{
    public class InputNumber : MonoBehaviour
    {
        public static string NUMBER_COUNTER = "number_counter";

        public int Value;
        public int MinValue;
        public int MaxValue;

        public int MinValueWarning;
        public GameObject MinValueWarningInfo;
        public int MaxValueWarning;
        public GameObject MaxValueWarningInfo;

        public bool UseWarning;

        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private Button _increaseButton;
        [SerializeField] private Button _decreaseButton;

        public UnityEvent<int> OnIncrease = new UnityEvent<int>();
        public UnityEvent<int> OnDecrease = new UnityEvent<int>();

        private int _initialValue;

        private void Start()
        {
            _initialValue = Value;
            _numberText.text = Value.ToString();
            _increaseButton.onClick.AddListener(IncreaseValue);
            _decreaseButton.onClick.AddListener(DecreaseValue);
            CheckValue();
        }

        private void Update()
        {
            CheckValue();
        }

        private void IncreaseValue()
        {
            MainSceneController.Instance.Audio.PlaySfx(NUMBER_COUNTER);
            if(Value == MaxValueWarning)
            {
                ShowWarning(true);
                return;
            }
            Value++;
            _numberText.text = Value.ToString();
            CheckValue();
            //CheckWarning();
            OnIncrease.Invoke(Value);
        }

        private void DecreaseValue()
        {
            MainSceneController.Instance.Audio.PlaySfx(NUMBER_COUNTER);
            if (Value == MinValueWarning)
            {
                ShowWarning(false);
                return;
            }

            Value--;
            _numberText.text = Value.ToString();
            CheckValue();
            //CheckWarning();
            OnDecrease.Invoke(Value);
        }

        private void CheckValue()
        {
            if(Value <= MinValue)
            {
                _decreaseButton.interactable = false;
            }
            else
            {
                _decreaseButton.interactable = true;
            }

            if(Value >= MaxValue) 
            {
                _increaseButton.interactable = false;
            }
            else
            {
                _increaseButton.interactable = true;
            }
        }

        private void CheckWarning()
        {
            if(Value == MinValueWarning)
            {
                ShowWarning(false);
            }

            if(Value == MaxValueWarning)
            {
                ShowWarning(true);
            }
        }

        private void ShowWarning(bool isMax)
        {
            if (isMax) MaxValueWarningInfo.SetActive(true);
            else MinValueWarningInfo.SetActive(true);

            LeanTween.delayedCall(3f,CloseWarning);
        }

        private void CloseWarning()
        {
            MaxValueWarningInfo.SetActive(false);
            MinValueWarningInfo.SetActive(false);   
        }

        public void ResetInput()
        {
            Value = _initialValue;
            _numberText.text = Value.ToString();
        }

    }
}