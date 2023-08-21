using System.Collections.Generic;
using UnityEngine;
using System;
using Agate.Starcade.Runtime.Enums;
using Starcade.Core.Scripts.Runtime.Utilities;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules
{
    public class RewardController : MonoBehaviour
    {
        [Header("Score Data")]
        [NonSerialized] public double TotalScore = 0;
        [NonSerialized] public double SpinScore = 0;
        
        [Header("GameObjects")]
        [SerializeField] NumberCounter ScoreDisplay;
        [SerializeField] private Image _currentImage;
        [SerializeField] private Sprite _goldCoin;
        [SerializeField] private Sprite _starCoin;
        [SerializeField] private Sprite _starTicket;
        
        public void Init(CurrencyTypeEnum currencyType)
        {
            ScoreDisplay.Balance = 0;

            _currentImage.sprite = currencyType switch
            {
                CurrencyTypeEnum.StarCoin => _starCoin,
                CurrencyTypeEnum.GoldCoin => _goldCoin,
                CurrencyTypeEnum.StarTicket => _starTicket,
                _ => _currentImage.sprite
            };
        }

        public void AddScore(double _value)
        {
            TotalScore += _value;
            SpinScore += _value;
            UpdateDisplay();
        }
        
        public void ResetDisplay()
        {
            SpinScore = 0;
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            ScoreDisplay.Balance = SpinScore;
        }
    }
}
