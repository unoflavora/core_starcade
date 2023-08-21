using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Data_Class.Account;
using Agate.Starcade.Scripts.Runtime.Login;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Agate.Starcade.Runtime.Backend
{
	public class AuthenticationBackEndController
    {
        private string _basicToken;//TODO STORE SAFELY
        private int _timeoutRequest; 
        
        private WebRequestHelper webRequestHelper;
        private string _baseUrl;


		public void Init()
        {
            
        }
		public AuthenticationBackEndController(string baseUrl, string basicToken, int timeoutRequest)
        {
            _basicToken = basicToken;
            _timeoutRequest = timeoutRequest;
            
            webRequestHelper = new WebRequestHelper(_timeoutRequest);
            webRequestHelper.InitWebRequest(_timeoutRequest);

            _baseUrl = baseUrl;
        }

        #region LOGIN

        public async Task<GenericResponseData<LoginData>> LoginAsGuest()
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Basic " + _basicToken.ToBase64() },
            };
            var res =
                await webRequestHelper.PostRequest<LoginData>(_baseUrl + "auth/login-as-guest", header, String.Empty);

            return res;
        }        

        public async Task<GenericResponseData<LoginData>> Login(AccountTypesEnum accountTypes, string token)
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Basic " + _basicToken.ToBase64() },
                { "Content-Type", "application/json" }
            };
            BindData bindData = new BindData(token);
            string body = JsonConvert.SerializeObject(bindData);
            Debug.Log("START CALL BACKEND");
            string url;
            switch (accountTypes)
            {
                case AccountTypesEnum.None:
                    url = "";
                    break;
                case AccountTypesEnum.Guest:
                    url = "auth/login-as-guest";
                    break;
                case AccountTypesEnum.Google:
                    url = "auth/login-with-google";
                    break;
                case AccountTypesEnum.Apple:
                    url = "auth/login-with-apple";
                    break;
                case AccountTypesEnum.Facebook:
                    url = "auth/login-with-facebook";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(accountTypes), accountTypes, null);
            }

            var res = await webRequestHelper.PostRequest<LoginData>(_baseUrl + url, header, body);

            return res;
        }
        #endregion

        #region TOKEN HANDLER

        public async Task<GenericResponseData<LoginData>> FetchRefreshToken(string refreshToken)
        {
            
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + refreshToken }
            };
            var res = await webRequestHelper.GetRequest<LoginData>(_baseUrl + "auth/refresh-token", header);

            return res;
        }

        #endregion
        
        public async Task<GenericResponseData<LoginData>> BindGoogle(string token)
        {
			Dictionary<string, string> header = new Dictionary<string, string>()
				{
					{ "Authorization", "Bearer " + MainSceneController.Token},
					{ "Content-Type", "application/json" }
				};
			BindData googleBindData = new BindData(token);
			string body = JsonConvert.SerializeObject(googleBindData);
			var res = await webRequestHelper.PostRequest<LoginData>(_baseUrl + "auth/bind-google-account", header, body);

			return res;
        }

		public async Task<GenericResponseData<LoginData>> BindApple(string token)
		{
			Dictionary<string, string> header = new Dictionary<string, string>()
			{
				{ "Authorization", "Bearer " + MainSceneController.Token},
				{ "Content-Type", "application/json" }
			};

			GoogleTokenData googleTokenData = new GoogleTokenData()
			{
				IdToken = token
			};
			string body = JsonConvert.SerializeObject(googleTokenData);
			var result = await webRequestHelper.PostRequest<LoginData>(_baseUrl + "auth/bind-apple-account", header, body);

			return result;
		}


		public async Task<GenericResponseData<LoginData>> BindFacebook(string token)
        {
			Dictionary<string, string> header = new Dictionary<string, string>()
				{
					{ "Authorization", "Bearer " + MainSceneController.Token},
					{ "Content-Type", "application/json" }
				};
			string body = JsonConvert.SerializeObject(new BindData(token));
			var res = await webRequestHelper.PostRequest<LoginData>(_baseUrl + "auth/bind-facebook-account", header, body);

            return res;
        }

        public async Task<GenericResponseData<LoginData>> Unbind()
        {
                
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await webRequestHelper.PostRequest<LoginData>(_baseUrl + "auth/unbind", header, String.Empty);

            return result;
        }

		/*public async Task<GenericResponseData<LoginData>> RequestTestDelay()
        {
            int rand = Random.Range(1, 3);
            Debug.Log(rand);
            string testUrl = "https://app.requestly.io/delay/"+ 10000 +"/https://pokeapi.co/api/v2/pokemon/ditto";
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneController.Token},
                { "Content-Type", "application/json" }
            };
            var result = await webRequestHelper.GetRequest<LoginData>(testUrl, null);
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }*/
        
        #region Backup
        public void BindGoogleBackup()
        {
            /*MainSceneLauncher.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.TitleScreen);
            GoogleUserData googleUserData = new GoogleUserData();
#if UNITY_ANDROID && !UNITY_EDITOR
			googleUserData = await googleAuthController.OnSignInGoogle();
            if (googleUserData == null)
            {
                MainSceneLauncher.Instance.Loading.DoneLoading();
            }
#elif UNITY_EDITOR
            googleUserData.Token = MainSceneLauncher.Instance.MainModel.TestingData.GoogleToken;
#endif
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + MainSceneLauncher.Token},
                { "Content-Type", "application/json" }
            };
                
            GoogleTokenData googleTokenData = new GoogleTokenData()
            {
                IdToken = googleUserData.Token
            };
            string body = JsonConvert.SerializeObject(googleTokenData);
            var result = await webRequestHelper.PostRequest<LoginData>(_baseUrl + "auth/bind-google-account", header, body);
                
            if (result.Error == null)
            {
                BindSuccess(result.Data);
                OnSuccessBinding?.Invoke();
            }
            else
            {
                googleAuthController.SignOut();
            }
            MainSceneLauncher.Instance.Loading.DoneLoading();
            return result;*/
        }
//        public async Task<GenericResponseData<LoginData>> BackupLoginGoogle()
//        {
//            MainSceneController.Instance.Loading.StartTitleLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
//            GoogleUserData googleUserData = new GoogleUserData();

//#if UNITY_ANDROID && UNITY_EDITOR
//			googleUserData.Token = MainSceneController.Instance.MainModel.TestingData.GoogleToken;
//#else
//            googleUserData = await googleAuthController.OnSignInGoogle();
//            if (googleUserData == null)
//            {
//                MainSceneController.Instance.Loading.DoneLoading();
//                GenericResponseData<LoginData> badResult = new GenericResponseData<LoginData>()
//                {
//                    Error = new Error()
//                    {
//                        Code = string.Empty,
//                        Message = string.Empty
//                    }
//                };
//                return badResult;
//            }
            
//#endif
//            Dictionary<string, string> header = new Dictionary<string, string>()
//            {
//                { "Authorization", "Basic " + _clientId.ToBase64() },
//                { "Content-Type", "application/json" }
//            };

//            GoogleTokenData googleTokenData = new GoogleTokenData()
//            {
//                IdToken = googleUserData.Token
//            };
//            string body = JsonConvert.SerializeObject(googleTokenData);
//            var result = await webRequestHelper.PostRequest<LoginData>(_baseUrl + "auth/login-with-google", header, body);
//            MainSceneController.Instance.Loading.DoneLoading();
//            return result;
//        }

        #endregion
    }
}

