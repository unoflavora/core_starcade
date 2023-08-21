using System;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Model
{
    [CreateAssetMenu(fileName = "URL Data", menuName = "Model SO/Url Data", order = 0)]
    public class UrlCollection : ScriptableObject
    {
        public string SurveyUrl;
        public string FacebookGroupPageUrl;
    }
}