using Facebook.Unity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.ThirdParty.Facebook
{
	public class FacebookAuthController
    {
		private static readonly List<string> LOGIN_PERMISSION = new List<string>()
		{
			"public_profile", "email"
		};

		private string _facebookAccessToken { get; set; } = null;
        
        public FacebookAuthController()
        {

        }
        
        public void Init()
        {
            if (!FB.IsInitialized)
            {
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
            }
        }

        public async Task<FacebookLoginResult> Login()
        {
            var res = new FacebookLoginResult();
            try
            {
				var taskCompletionSource = new TaskCompletionSource<FacebookLoginResult>();
				var task = taskCompletionSource.Task;
				Action<FacebookLoginResult> callback = taskCompletionSource.SetResult;

				if (_facebookAccessToken != null)
                {
                    res.Token = _facebookAccessToken;
                    return res;
                }
                else
                {
                    Debug.Log("LOGIN FACEBOOK");
                    
                    FB.LogInWithReadPermissions(LOGIN_PERMISSION, result =>
                    {
                        if (result?.Error != null || result?.AccessToken == null)
                        {
                            if (result.Error != null)
                            {
								res.Error = result.Error;                            
                            }
                            else
                            {
								res.Error = "Failed to bind facebook account.";
							}
                            Debug.LogError(result?.Error);
                        }
                        res.Token = result.AccessToken?.TokenString;
                        _facebookAccessToken = res.Token;
                        Debug.Log(res.Token);

						callback(res);
                    });
                    return await task;
 
                }
            }
			catch (Exception e)
            {
                Debug.LogError(e);
				res.Error = "Failed to bind facebook account.";
                return res;
			}
        }
        

        #region FACEBOOK MAIN HANDLER
        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                Debug.Log("[FACEBOOK] FACEBOOK SUCCESS INITIALIZED");
                FB.ActivateApp();
            }
        }

        public void Logout()
        {
            FB.LogOut();
        }
        

        //private void AuthCallBack(ILoginResult result)
        //{
        //    if (FB.IsLoggedIn)
        //    {
        //        var accessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
        //        _facebookAccessToken = accessToken.UserId;

        //        _isAlreadyLogin = true;
        //        _loginResult = result;

        //        //LoginFacebook(_facebookAccessToken);
        //    }
        //}
        
        //private void AuthBindingCallBack(ILoginResult result)
        //{
        //    if (FB.IsLoggedIn)
        //    {
        //        var accessToken = result.AccessToken;
        //        _facebookAccessToken = accessToken.UserId;

        //        _isAlreadyLogin = true;
        //        _loginResult = result;

        //        //BindingFacebook(_facebookAccessToken);
        //    }
        //}

        private void OnHideUnity (bool isGameShown)
        {
            Time.timeScale = !isGameShown ? 0 : 1;
        }

        #endregion
    }

	public class FacebookLoginResult
	{
		public string Token;
		public string Error;
	}

}