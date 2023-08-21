using System.Net;
using System.Threading.Tasks;
using UnityEngine;

	public class GoogleAuthController
	{
#if UNITY_ANDROID
		private static Google.GoogleSignIn googleSignInController;
		private Google.GoogleSignInConfiguration _configuration;
#endif

	private string _webClientId;

		public GoogleAuthController(string webClientId)
		{
		_webClientId = webClientId;
#if UNITY_ANDROID
			_configuration = new Google.GoogleSignInConfiguration
			{
				WebClientId = webClientId,
				RequestIdToken = true,
				RequestProfile = true,
				RequestEmail = true,
				UseGameSignIn = false
			};
#endif
		}


		public async Task<GoogleUserData> SignIn()
		{
#if UNITY_EDITOR
		return null;
#endif

#if UNITY_ANDROID
	    try
		{
            Debug.Log("Login please wait");

			if(Google.GoogleSignIn.Configuration == null)
			{
				Google.GoogleSignIn.Configuration = new Google.GoogleSignInConfiguration
                {
                    WebClientId = _webClientId,
                    RequestIdToken = true,
                    RequestProfile = true,
                    RequestEmail = true,
                    UseGameSignIn = false
                };
            }

			Google.GoogleSignInUser signIn = await Google.GoogleSignIn.DefaultInstance.SignIn();
            Debug.Log("Welcome, " + signIn.DisplayName);
            Debug.Log("ID TOKEN " + signIn.IdToken);
            //onSignIn.Invoke();

            GoogleUserData googleUserData = new GoogleUserData()
            {
                Token = signIn.IdToken,
                DisplayName = signIn.DisplayName,
                Email = signIn.Email,
                PhotoUrl = signIn.ImageUrl.ToString(),
            };
            return googleUserData;
        }
        catch(Google.GoogleSignIn.SignInException googleError)
        {
            Debug.Log(googleError.Message);
            switch (googleError.Status)
            {
                case Google.GoogleSignInStatusCode.Canceled:
                    Debug.Log("Google Sign In Canceled!");
                    break;
                case Google.GoogleSignInStatusCode.Error:
                    Debug.Log("Google Sign In Error!");
                    break;
                case Google.GoogleSignInStatusCode.Interrupted:
                    Debug.Log("Google Sign In Interrupted!");
                    break;
                case Google.GoogleSignInStatusCode.Timeout:
                    Debug.Log("Google Sign In Timeout!");
                    break;
            }

			SignOut();
            return null;
        }
#else
		return null;
#endif
		}

		public void SignOut()
		{
			try
			{
#if UNITY_ANDROID && !UNITY_EDITOR
        Google.GoogleSignIn.DefaultInstance.SignOut();
#endif
			}
			catch { }

		}

		#region UNUSED CODE CHECK LATER
		//    /*public string CheckAuth()
		//    {
		//        if (MainSceneLauncher.LoadLoginData() == null)
		//        {
		//            Debug.Log("[AUTH] Login Data is Null");
		//            return null;
		//        }
		//        else
		//        {
		//            Debug.Log("[AUTH] Login Data is Found. Login with that data");
		//            return MainSceneLauncher.LoadLoginData();
		//        }
		//    }*/



		//    public void OnSignInSilentlyGoogle()
		//    {

		//        Google.GoogleSignIn.Configuration = _configuration;
		//        Google.GoogleSignIn.Configuration.UseGameSignIn = false;
		//        Google.GoogleSignIn.Configuration.RequestIdToken = true;
		//        //Google.GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuth);
		//    }
		//    public void GuestSignIn()
		//    {
		//        if (MainSceneLauncher.Instance)
		//        {
		//            //MainSceneLauncher.Instance.playerData.PlayerDeviceId = SystemInfo.deviceUniqueIdentifier;
		//        }
		//        PlayerPrefs.SetInt(isLoggedInKey, 1);
		//        //LoadNextScene();
		//    }


		//    public void SignOut()
		//    {
		//        PlayerPrefs.DeleteAll();
		//#if UNITY_EDITOR
		//        Debug.Log("Log Out");
		//        return;
		//#endif
		//        Google.GoogleSignIn.DefaultInstance.SignOut();
		//    }

		#endregion

	}

	public class GoogleUserData
	{
		public string Token;
		public string DisplayName;
		public string Email;
		public string PhotoUrl;
	}
