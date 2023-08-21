using System;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.Modules
{
    public class PetActivityView : MonoBehaviour
    {
        [SerializeField] private Slider _progressBar;
        [FormerlySerializedAs("_titleDesc")] [SerializeField] private TextMeshProUGUI _currentPetActivity;
        [SerializeField] private GameObject _activityDesc;
        [SerializeField] private TextMeshProUGUI _timeRemaining;
        [SerializeField] private GameObject _progressGameObject;
        [SerializeField] private Button _claimButton;
        [SerializeField] private Button _adventure;
        [SerializeField] private Button _cancel;
    
        private TimeSpan _remainingTime;
        private DateTime _endDate;
        private string _petName;
        private bool _isTimerRunning;

        private UnityEvent _onAdventureFinished;
    

        private void Update()
        {
            if (_isTimerRunning == false) return;
        
            DisplayTime();

            if (_remainingTime <= TimeSpan.Zero)
            {
                _isTimerRunning = false;
                _onAdventureFinished.Invoke();
            }
        }

        public void SetupCurrentActivity(string petName, PetAdventureData petAdventureData)
        {
            _isTimerRunning = false;
            _petName = petName;
            DateTime endDate;
            DateTime startDate;
        
            switch (petAdventureData.IsDispatched)
            {
                case true:
                    // Pet is Dispatched and rewards are not yet claimable by user.
                    startDate = DateTime.Parse(petAdventureData.StartDate);
                    endDate = DateTime.Parse(petAdventureData.EndDate);
                    SetupInteractables(true, false);
                    StartTimer(startDate, endDate);
                    _currentPetActivity.SetText($"{petName} is exploring...");
                    break;
                case false:
                    if (petAdventureData.Rewards == null)
                    {
                        // Pet is not Dispatched
                        SetupInteractables(false, false);
                        _currentPetActivity.SetText($"{petName} is idle" );
                    }
                    else
                    {
                        // Pet is Dispatched and rewards are claimable by user but not yet claimed
                        SetupInteractables(false, true);
                        _currentPetActivity.SetText($"{_petName} has returned!");
                        _timeRemaining.text = "Adventure Finished";
                    }
                    break;
            }
        }

        private void SetupInteractables(bool isDispatched, bool rewardClaimable)
        {
            _progressGameObject.SetActive(isDispatched);
            _cancel.gameObject.SetActive(isDispatched);
            _activityDesc.gameObject.SetActive(isDispatched == false);
            _adventure.gameObject.SetActive(isDispatched == false && rewardClaimable == false);
            _claimButton.gameObject.SetActive(isDispatched == false && rewardClaimable);
        }

        private void StartTimer(DateTime startDate, DateTime endDate)
        {
            _endDate = endDate;
            _remainingTime = endDate - DateTime.Now;
            _isTimerRunning = true;

            _progressBar.minValue = 0;
            _progressBar.maxValue = (float)(endDate - startDate).TotalSeconds;
            if (_progressBar.maxValue == 0) _progressBar.maxValue = 1;
        }

        void DisplayTime()
        {
            _remainingTime = _endDate - DateTime.Now;

            string formattedTimeRemaining = $"Time Remaining: {_remainingTime.Hours}h {_remainingTime.Minutes}min";
        
            _timeRemaining.SetText(formattedTimeRemaining);

            float progress = (float)((_progressBar.maxValue - _remainingTime.TotalSeconds));

            _progressBar.value = progress;

        }
    
        public void RegisterOnAdventureFinished(UnityAction onAdventureFinished)
        {
            _onAdventureFinished ??= new UnityEvent();

            _onAdventureFinished.AddListener(onAdventureFinished);
        }
    }
}
