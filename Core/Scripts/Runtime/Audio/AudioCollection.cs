using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Agate.Starcade.Runtime.Audio
{ 
	[CreateAssetMenu(fileName = "AudioCollection", menuName = "Audio/AudioCollection", order = 1)]
    [Serializable]
    public class AudioCollection : ScriptableObject
    {
        public string LabelCollection;
        public RawAudioData[] NewBgmCollection;
        public RawAudioData[] NewSfxCollection;
    }

    [Serializable]
    public class AudioData
    {
        public string id;
        public AudioClip audioClip;
        public string label;

        public AudioData(string newId, AudioClip newAudioClip, string newLabel)
        {
            id = newId;
            audioClip = newAudioClip;
            label = newLabel;
        }
    }
}