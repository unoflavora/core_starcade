using Agate.Starcade.Core.Runtime.ThirdParty.Firebase;
using Agate.Starcade.Core.Runtime.ThirdParty.Facebook;
using Agate.Starcade.Core.Runtime.ThirdParty.IOS;
using Agate.Starcade.Core.Runtime.Config;
using UnityEngine;
using Agate.Starcade.Core.Runtime.ThirdParty.AppFlyer;

namespace Agate.Starcade.Core.Runtime.ThirdParty
{
	public class ThirdPartyManager: MonoBehaviour
    {
		private ThirdPartyConfig _config;

		private GoogleAuthController _googleAuth { get; set; }
		private IOSAuthController _ios { get; set; }
		private FacebookAuthController _facebook { get; set; }
		private FirebaseController _firebase { get; set; }
		private AppsFlyerController _appFlyer { get; set; }


		#region Properties
		public GoogleAuthController GoogleAuth => _googleAuth;
		public IOSAuthController IOS => _ios;
		public FacebookAuthController Facebook => _facebook;
		public FirebaseController Firebase => _firebase;

		#endregion

		public void Init(ThirdPartyConfig config)
		{
			_config = config;
			_googleAuth = new GoogleAuthController(_config.Google.GoogleWebClientId);

			_ios = new IOSAuthController();
			_ios.Init();
			
			_facebook = new FacebookAuthController();
			_facebook.Init();
			
			_firebase = new FirebaseController();
			_firebase.Init();

			_appFlyer = new AppsFlyerController(_config.AppFlyer, null);
			_appFlyer.Init();
		}

		public void Update()
		{
			if (_ios != null) { _ios.Update(); }
		}
	}
}
