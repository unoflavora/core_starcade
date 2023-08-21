using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.Lobby.PetBox
{
    public class PetBoxGachaEvent : MonoBehaviour
    {
        

        public UnityEvent StarAppearEvent;
        public UnityEvent PortalActivationEvent;
        public UnityEvent BoxAppearEvent;
        public UnityEvent BoxHypeEvent;
        public UnityEvent BoxOpenEvent;
        public UnityEvent BoxPopUpEvent;
        public UnityEvent ResultEvent;

        public void TriggerStarAppear()
        {
            StarAppearEvent?.Invoke();
        }
        public void TriggerPortalActivation()
        {
            PortalActivationEvent?.Invoke();
        }
        public void TriggerBoxAppear()
        {
            BoxAppearEvent?.Invoke();
        }
        public void TriggerBoxHype()
        {
            BoxHypeEvent?.Invoke();
        }
        public void TriggerBoxOpen()
        {
            BoxOpenEvent?.Invoke();
        }
        public void TriggerBoxPopUp()
        {
            BoxPopUpEvent?.Invoke();
        }
        public void TriggerResult()
        {
            ResultEvent?.Invoke();
        }
    }
}