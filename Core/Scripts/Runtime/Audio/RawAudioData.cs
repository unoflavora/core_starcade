using System;
using UnityEngine.AddressableAssets;

namespace Agate.Starcade.Runtime.Audio
{ 
	[Serializable]
    public class RawAudioData
    {
        public string id;
        public AssetReference audioReference;
        public string label;
    }
}
