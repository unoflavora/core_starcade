using Agate.Starcade.Core.Runtime.Config;
using UnityEngine;

namespace Agate.Starcade.Runtime.Main
{
    [CreateAssetMenu(fileName = "Main", menuName = "Model SO", order = 0)]
    public class MainModelScriptableObject : ScriptableObject
    {
        [Header("Environment")] 
        public EnvironmentConfig Environment;
    }
}