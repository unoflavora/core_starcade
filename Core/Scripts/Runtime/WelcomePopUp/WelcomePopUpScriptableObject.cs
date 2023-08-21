using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Agate.Starcade.Core.Scripts.Runtime.WelcomePopUp
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "Starcade/WelcomePopUp/ListData", order = 1)]
    public class WelcomePopUpScriptableObject : ScriptableObject
    {
        public string WelcomePopUpDataId;
        public List<WelcomePopUpData> ListActiveWelcomePopUpData;
    }
}