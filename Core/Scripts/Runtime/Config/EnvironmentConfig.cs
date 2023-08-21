using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Config
{
    [CreateAssetMenu(fileName = "EnvironmentConfig", menuName = "config/add new environment config", order = 0)]
    public class EnvironmentConfig : ScriptableObject
    {

		[Header("Game URL")]
		public string IdentityBaseUrl;
        public string GameBaseUrl;
        public string ExternalBaseUrl;

		[Header("Secret")]
        public string BasicToken;

		[Header("Game")]
		public bool EnableDebugLogViewer;
        public bool EnableVersionCheck = true;
        public bool ForceUseRemoteConfig = false;

		[Header("CP01")]
		public bool IsConnectToHeadless;
        public string RoomAllocationBaseUrl;
		public string HeadlessUrl;

		[Header("CP02")]
		public string CP02GameBaseUrl;
		public string RPSGameBaseUrl;

		[Header("Others")]
		public bool IsSkipLootboxEnabled;
	}
}