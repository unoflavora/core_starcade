using System;
using UnityEngine;

namespace Agate.Starcade.Runtime.Audio
{
    [Serializable]
    public class AudioConfig
    {
		[Range(0f, 1f)]
		public float BgmVolume = 1;
		[Range(0f, 1f)]
		public float SfxVolume = 1;
    }
}