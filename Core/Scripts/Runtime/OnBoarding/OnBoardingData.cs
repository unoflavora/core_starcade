using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.OnBoarding
{
    public class OnBoardingData 
    {
        public int CurrentState { get; set; }
        public bool IsComplete { get; set; }
        public OnBoardingEventsData OnBoardingEvents { get; set; }
    }
}