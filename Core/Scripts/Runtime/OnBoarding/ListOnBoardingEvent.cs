using System;
using System.Collections;
using System.Collections.Generic;
using Agate.Starcade;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Agate
{
    [Serializable]
    [CreateAssetMenu(fileName = "ListOnBoardingEvent", menuName = "OnBoarding/ListOnBoardingEvent", order = 2)]
    public class ListOnBoardingEvent : ScriptableObject
    {
        public Object test;
    }
}
