using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.Pet.Fragment.Object
{
	public class PetFragmentEvent : MonoBehaviour
    {
        public UnityEvent OnStartVFX;
        public UnityEvent OnResultVFX;

        private async void Start()
        {
            //await MainSceneController.Instance.Audio.LoadAudioData("pet_audio");
        }

        public void TriggerStartEvent()
        {
            OnStartVFX?.Invoke();
        }

        public void TriggerResultVFX()
        {
            OnResultVFX?.Invoke();
        }
    }
}