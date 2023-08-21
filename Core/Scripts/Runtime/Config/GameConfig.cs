using Agate.Starcade.Runtime.Audio;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Config
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "config/add new game config", order = 0)]
    public class GameConfig : ScriptableObject
	{
		[Header("Environment")]
		public EnvironmentConfig EnvironmentConfig;

		[Header("ThirdParty")]
		public ThirdPartyConfig ThirdPartyConfig;

		[Header("Backend")]
		public int Timeout;

		[Header("Audio")]
        public AudioConfig AudioConfig;

		[Header("Others")]
		public Sprite UserProfilePhoto;
    }
    
    
}
