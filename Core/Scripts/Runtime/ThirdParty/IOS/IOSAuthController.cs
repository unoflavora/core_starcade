using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.ThirdParty.IOS
{
    public class IOSAuthController
    {
        private IAppleAuthManager _appleAuthManager;

        private string _token;
        private string _error;

        //public event System.Action OnLoginPerformed;
        //public event System.Action<string> OnLoginSuccess;
        //public event System.Action<string> OnLoginFailure;

        public IOSAuthController()
        {

        }

        public void Init()
        {
            Debug.Log("Is Platform can log in with IOS ? " + AppleAuthManager.IsCurrentPlatformSupported);

            if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                PayloadDeserializer deserializer = new PayloadDeserializer();
                _appleAuthManager = new AppleAuthManager(deserializer);
            }
        }

        public void Update()
        {
            if(_appleAuthManager != null)
            {
                _appleAuthManager.Update();
            }
        }


        public void Login(UnityAction<string> OnSuccessBinding, UnityAction<string> OnFailedBinding)
        {
            if (_appleAuthManager == null)
            {
                OnFailedBinding.Invoke("Apple Login Is Not Supported");
                return;
            };

            AppleAuthLoginArgs loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            _appleAuthManager.LoginWithAppleId(loginArgs, 
                credential => 
                {
                    IAppleIDCredential appleIDCredential = credential as IAppleIDCredential;
                    if (appleIDCredential != null)
                    {
                        string idToken = Encoding.UTF8.GetString(
                            appleIDCredential.IdentityToken,
                            0,
                            appleIDCredential.IdentityToken.Length
                        );
                        Debug.Log("Sign-in with Apple successfully done. IDToken: " + idToken);
                        _token = idToken;

                        OnSuccessBinding?.Invoke(_token);
                    }
                    else
                    {
                        Debug.Log("Sign-in with Apple error. Message: appleIDCredential is null");
                        _error = "Retrieving Apple Id Token failed.";
                        OnFailedBinding.Invoke(_error);
                    }
                }, 
                error => 
                {
                    Debug.Log("Sign-in with Apple error. Message: appleIDCredential is null");
                    _error = "Retrieving Apple Id Token failed.";
                    OnFailedBinding?.Invoke(_error);
                });
        }

		public async Task<AppleLoginResult> Login()
		{
            var res = new AppleLoginResult(); 
			if (_appleAuthManager == null)
			{
                res.Error = "Apple Login Is Not Supported";
				return res;
			};

			AppleAuthLoginArgs loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

			var taskCompletionSource = new TaskCompletionSource<AppleLoginResult>();
			var task = taskCompletionSource.Task;
			Action<AppleLoginResult> callback = taskCompletionSource.SetResult;

			_appleAuthManager.LoginWithAppleId(loginArgs,
				credential =>
				{
					IAppleIDCredential appleIDCredential = credential as IAppleIDCredential;
					if (appleIDCredential != null)
					{
						string idToken = Encoding.UTF8.GetString(
							appleIDCredential.IdentityToken,
							0,
							appleIDCredential.IdentityToken.Length
						);
						Debug.Log("Sign-in with Apple successfully done. IDToken: " + idToken);

                        res.Token = idToken;
                        callback(res);
					}
					else
					{
						Debug.Log("Sign-in with Apple error. Message: appleIDCredential is null");
						res.Error = "Retrieving Apple Id Token failed.";
                        callback(res);
					}
				},
				error =>
				{
					Debug.Log("Sign-in with Apple error. Message: appleIDCredential is null");
					res.Error = "Retrieving Apple Id Token failed.";
                    callback(res);
				});

			return await task;
		}
	}
	public class AppleLoginResult
	{
		public string Token;
		public string Error;
	}

}
