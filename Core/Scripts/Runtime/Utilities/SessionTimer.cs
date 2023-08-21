using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Scripts.Runtime.Utilities
{
    public class SessionTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField]  private string _prefix;
        [SerializeField]  private string _suffix;

        public UnityEvent OnSessionTimeout;
        
        private float _sessionTime;
        private DateTime? _endDate;
        private int _sessionDuration;
        private bool _isStarted;
        private bool _timeOut;
       
        
        //Use prefab, call in the beginning
        public void InitTimer(DateTime? endDate, bool isStarted, int sessionDuration)
        {
            _endDate = endDate;
            _isStarted = isStarted;
            _sessionDuration = sessionDuration;

            SetSessionTime();
        }


        private void Update()
        {
            UpdateSessionTime();
        }
        
        private void UpdateSessionTime()
        {
            if (_sessionTime <= 0 || !_isStarted || _timeOut) return;
            TimeSpan currentTime = GetCurrentTime();
            DisplayTime(currentTime);
            DeductTime();
        }
        
        private void SetSessionTime()
        {
            TimeSpan time;
            Time.timeScale = 1;
            if (_isStarted && _endDate != null)
            {
                time = _endDate.Value - DateTime.UtcNow;
            }
            else
            {
                time = TimeSpan.FromMinutes(_sessionDuration);
            }
            _sessionTime = (float)time.TotalSeconds;
        }
        
        private TimeSpan GetCurrentTime()
        {
            return TimeSpan.FromSeconds(_sessionTime);
        }

        private void DeductTime()
        {
            _sessionTime -= Time.deltaTime;
            if (_sessionTime <= 0) TimeOut();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            Debug.Log("RETURNING FOCUS " + hasFocus);
            if (hasFocus) SetSessionTime();
        }

        private void TimeOut()
        {
            if (_timeOut) return;
            _timeOut = true;

            OnSessionTimeout.Invoke();
            print("TIME OUT");
        }

        private void DisplayTime(TimeSpan timeSpan)
        {
            _timer.text = _prefix + timeSpan.ToString(@"hh\:mm\:ss")+_suffix;
        }
    }
}