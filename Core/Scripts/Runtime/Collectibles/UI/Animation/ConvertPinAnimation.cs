using System;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Animation
{
    public class ConvertPinAnimation : MonoBehaviour
    {
        private Animator _animator;
        private UnityEvent _onPinsCombined;
        private static readonly int PlayAnim = Animator.StringToHash("Start");

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _onPinsCombined = new UnityEvent();
        }

        public void PinCombined()
        {
            _onPinsCombined.Invoke();
            
            Stop();

        }
        
        public void AddListenerOnce(UnityAction action)
        {
            _onPinsCombined.AddListener(action);
            
            _onPinsCombined.AddListener(() => _onPinsCombined.RemoveListener(action));
        }

        public void Play()
        {
            _animator.enabled = true;
            
            _animator.SetTrigger(PlayAnim);
        }

        private void Stop()
        {
            _animator.ResetTrigger(PlayAnim);
            
            _animator.enabled = false;
        }
    }
}