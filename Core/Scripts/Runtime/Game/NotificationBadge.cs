using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Agate.Starcade.Core.Runtime.Game
{
    public class NotificationBadge : MonoBehaviour
    {
        [SerializeField] private GameObject _container;
        [SerializeField] private TMP_Text _number;
        [SerializeField] private GameObject _badge;
        [SerializeField] private GameObject _longBadge;

        [SerializeField] private bool _disableOnAwake = true;

        private void Awake()
        {
            if(_disableOnAwake) _container.SetActive(false);
        }

        public void EnableBadge()
        {
            _container.SetActive(true);
            _number.text = String.Empty;
        }

        public void EnableBadgeWithCounter(int number)
        {
            if (number > 99)
            {
                _longBadge.SetActive(true);
                _number.text = 99 + "+";
                return;
            }
            
            _longBadge.SetActive(false);
            
            _container.SetActive(true);
            
            _number.text = "" + number;
        }

        public void EnableBadgeWithDuplicated(int number)
        {
            if (number > 99)
            {
                _longBadge.SetActive(true);
                _number.text = "x99";
                return;
            }

            _container.SetActive(true);

            _number.text = "x" + number;
        }

        public void EnableNewBadge()
        {
            _longBadge.SetActive(true);
            _number.text = "NEW";
        }

        public void DisableBadge()
        {
            _container.SetActive(false);
            _longBadge.SetActive(false);
            _number.text = string.Empty;
        }

        public void SwitchStateBadge(bool state)
        {
            if (state)
            {
                EnableBadge();
            }
            else
            {
                DisableBadge();
            }
        }
    }
}