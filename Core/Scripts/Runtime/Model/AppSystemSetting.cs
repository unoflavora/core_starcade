using System;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Model
{
    [CreateAssetMenu(fileName = "AppSystemSetting", menuName = "Model SO/App System Setting", order = 0)]
    public class AppSystemSetting : ScriptableObject
    {
        public int TargetFrameRate;
        public int VerticalSyncCount;
        public bool EnableMultiTouch;
        public bool EnableRotateScreenSystem;
    }
    
}