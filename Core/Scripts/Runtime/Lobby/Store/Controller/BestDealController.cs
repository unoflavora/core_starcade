using Agate.Starcade.Scripts.Runtime.Utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Lobby.Store.Controller
{
    public class BestDealController : MonoBehaviour
    {
        public enum BestDealType
        {
            Default,
            Cooldown
        }

        [SerializeField] private Button _bestDealButton;
        [SerializeField] private Image _bestDealImage;
        [SerializeField] private Image _bestDealLabelImage;
        [SerializeField] private Image _costIconImage;
        [SerializeField] private GameObject _costGameobject;
        [SerializeField] private GameObject _cooldownGameobject;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private TextMeshProUGUI _cooldownText;
        [SerializeField] private TextMeshProUGUI _costText;

        private string _id;
        private float _interval;
        private float _secondRemaining;
        private BestDealType _type = BestDealType.Default;

        public void SetCooldown(float second)
        {
            if (_type != BestDealType.Cooldown) return;
            SetCooldownTimerVisible(true);
            _secondRemaining = second;
        }

        public void ResetCooldown()
        {
            if (_type != BestDealType.Cooldown) return;
            _secondRemaining = _interval;
        }

        public void AddBestDeaListener(UnityAction<string> onClick)
        {
            _bestDealButton.onClick.RemoveAllListeners();
            _bestDealButton.onClick.AddListener(() => onClick(_id));
        }

        public void SetContent(BestDealSO content)
        {
            _id = content.ItemId;
            _interval = content.Interval;
            if (_interval < 0f) _type = BestDealType.Default;
            else _type = BestDealType.Cooldown;

            if (content.Cost <= 0)
            {
                _costText.text = $"CLAIM";
                _costIconImage.gameObject.SetActive(false);
            }
            else
            {
                _costText.text = $"{CurrencyHandler.Convert(content.Cost)}";
                _costIconImage.sprite = content.CostSprite;
                _costIconImage.gameObject.SetActive(true);
            }

            _cooldownText.text = content.CooldownText;
            _bestDealImage.sprite = content.Background;
            _bestDealLabelImage.sprite = content.Label;
        }

        public void SetButtonInteractable(bool interactable)
        {
            _bestDealButton.interactable = interactable;
        }

        private void DisplayTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(_secondRemaining);
            _timerText.text = time.ToString(@"hh\:mm\:ss");
        }

        private void SetCooldownTimerVisible(bool visible)
        {
            _costGameobject.SetActive(!visible);
            _cooldownGameobject.SetActive(visible);
            SetButtonInteractable(!visible);
        }

        // Update is called once per frame
        void Update()
        {
            if (_type != BestDealType.Cooldown) return;
            if (_secondRemaining > 0)
            {
                SetCooldownTimerVisible(true);
                _secondRemaining -= Time.deltaTime;
                DisplayTime();
            }
            else
            {
                SetCooldownTimerVisible(false);
            }
        }
    }
}
