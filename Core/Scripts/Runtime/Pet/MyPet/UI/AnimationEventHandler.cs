using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.UI
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEventHandler : MonoBehaviour
    {
        private UnityEvent<string> _onAnimationTriggered;
        private Animator _animator;

        public void RegisterOnAnimationTrigger(UnityAction<string> onAnimationEvent)
        {
            if(_onAnimationTriggered == null) _onAnimationTriggered = new UnityEvent<string>();
            _onAnimationTriggered.AddListener(onAnimationEvent);
        }

        public void TriggerAnimation(string key)
        {
            _onAnimationTriggered.Invoke(key);
        }

        public void PlayAnimation(string animKey)
        {
            GetComponent<Animator>().Play(animKey);
            // finish
        }
    }
}