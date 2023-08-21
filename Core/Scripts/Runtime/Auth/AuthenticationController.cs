using Agate.Starcade.Core.Runtime.ThirdParty.Facebook;
using Agate.Starcade.Core.Runtime.ThirdParty.IOS;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Data_Class.Account;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using static Agate.Starcade.Runtime.Main.MainSceneController.STATIC_KEY;

namespace Agate.Starcade.Runtime.Auth
{
	public class AuthenticationController
    {
        private MainSceneController _main { get; set; }

		private AuthenticationBackEndController _authBackend => _main.AuthBackend;

		private GoogleAuthController _googleAuth => _main.ThirdParty.GoogleAuth;
		private IOSAuthController _ios => _main.ThirdParty.IOS;
		private FacebookAuthController _facebook => _main.ThirdParty.Facebook;
		
		private bool _isBindedWithFacebook => _main.Data.UserAccounts.ContainsKey(AccountTypesEnum.Facebook);


		public UnityEvent OnSignOut { get; set; } = new UnityEvent();

        public string RefreshToken
        {
            get
            {
				var refreshToken = PlayerPrefs.GetString(REFRESH_TOKEN)?.FromBase64(); 
				return refreshToken;
			}
        }

		public AuthenticationController(MainSceneController main)
        {
			_main = main;
        }

		#region LOGIN
		public async Task<GenericResponseData<LoginData>> LoginAsGuest()
		{
			MainSceneController.Instance.Loading.StartTitleLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
			var res = await RequestHandler.Request(async () => await _authBackend.LoginAsGuest());
			if (res.Data != null)
			{
				SaveAuthData(res.Data);
			}
			MainSceneController.Instance.Loading.DoneLoading();
			return res;
		}

		public async Task<GenericResponseData<LoginData>> LoginGoogle()
		{
			GenericResponseData<LoginData> res = new GenericResponseData<LoginData>();

			string token = null;
#if UNITY_ANDROID && !UNITY_EDITOR
			var googleUserData = await _googleAuth.SignIn();
            if (googleUserData != null)
            {
				token = googleUserData.Token;
			}
#elif UNITY_EDITOR
			token = MainSceneController.Instance.ThirdPartyConfig.Google.DummyGoogleAuthToken;
#endif

			if (token != null)
			{
				res = await Login(AccountTypesEnum.Google, token);

				if (res.Error != null)
				{
					_googleAuth.SignOut();
				}
			}

			return res;
		}

		public async Task<GenericResponseData<LoginData>> LoginApple()
		{
			MainSceneController.Instance.Loading.StartTitleLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
			GenericResponseData<LoginData> res = new GenericResponseData<LoginData>();

			var result = await _ios.Login();
			if (result.Error != null)
			{
				//FAILED

			}
			else
			{
				res = await Login(AccountTypesEnum.Apple, result.Token);

			}

			MainSceneController.Instance.Loading.DoneLoading();
			return res;
		}

		private async Task<GenericResponseData<LoginData>> Login(AccountTypesEnum accountTypes, string token)
		{
			var res = await RequestHandler.Request(async () => await _authBackend.Login(accountTypes, token));

			SaveAuthData(res?.Data);

			return res;
		}

#endregion

		#region BINDING
		public async Task<GenericResponseData<LoginData>> BindGoogle()
		{
			MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
			GenericResponseData<LoginData> res = new GenericResponseData<LoginData>();

			string token = null;
#if UNITY_ANDROID && !UNITY_EDITOR
			var googleUserData = await _googleAuth.SignIn();
            if (googleUserData != null)
            {
				token = googleUserData.Token;
			}
#elif UNITY_EDITOR
			token = MainSceneController.Instance.ThirdPartyConfig.Google.DummyGoogleAuthToken;
#endif
			if (token != null)
			{
				res = await RequestHandler.Request(async () => await _authBackend.BindGoogle(token));

				if (res.Error != null)
				{
					if (res.Error.Code == "10308") //ERROR IF ACCOUNT ALREADY BINDING
					{
						Debug.Log("Error account already binded");
						await MainSceneController.Instance.Info.ShowDelay(InfoType.BindingAlreadyAuthenticated, null, null, 3);
					}
					else
					{
						MainSceneController.Instance.Info.ShowDelay(InfoType.BindingFailed, null, null, 3);

					}
					_googleAuth.SignOut();
				}
				else
				{
					SaveAuthData(res.Data);
					await MainSceneController.Instance.Info.ShowDelay(InfoType.BindingSuccess, null, null, 3);
				}
			}
			else
			{
				MainSceneController.Instance.Info.ShowDelay(InfoType.BindingFailed, null, null, 3);
			}

			MainSceneController.Instance.Loading.DoneLoading();
			return res;
		}
		public async Task<GenericResponseData<LoginData>> BindApple()
		{
			MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
			GenericResponseData<LoginData> res = new GenericResponseData<LoginData>();

			var result = await _ios.Login();
			if (result.Error != null)
			{
				//FAILED
				MainSceneController.Instance.Info.ShowDelay(InfoType.BindingFailed, null, null, 3);
				//MainSceneController.Instance.Info.Show(result.Error, "Bind Failed", InfoIconTypeEnum.Error, null, new InfoAction("Close", null));
			}
			else
			{
				res = await RequestHandler.Request(async () => await _authBackend.BindApple(result.Token));
				if (res.Error != null)
				{
					if (res.Error.Code == "10308") //ERROR IF ACCOUNT ALREADY BINDING
					{
						Debug.Log("Error account already binded");
						await MainSceneController.Instance.Info.ShowDelay(InfoType.BindingAlreadyAuthenticated, null, null, 3);
					}
					else
					{
						MainSceneController.Instance.Info.ShowDelay(InfoType.BindingFailed, null, null, 3);

					}
				}
				else
				{
					SaveAuthData(res.Data);
					await MainSceneController.Instance.Info.ShowDelay(InfoType.BindingSuccess, null, null, 3);
				}
			}

			MainSceneController.Instance.Loading.DoneLoading();
			return res;
		}

		public async Task<GenericResponseData<LoginData>> BindFacebook()
		{

			MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
			GenericResponseData<LoginData> res = new GenericResponseData<LoginData>();
			var result = await _facebook.Login();
			if (result.Error != null)
			{
				//FAILED
				MainSceneController.Instance.Info.ShowDelay(InfoType.BindingFailed, null, null, 3);
				//MainSceneController.Instance.Info.Show(result.Error, "Bind Failed", InfoIconTypeEnum.Error, null, new InfoAction("Close", null));
			}
			else if(result.Token != null) 
			{
				res = await RequestHandler.Request(async () => await _authBackend.BindFacebook(result.Token));
				if (res.Error != null)
				{
					if (res.Error.Code == "10308") //ERROR IF ACCOUNT ALREADY BINDING
					{
						Debug.Log("Error account already binded");
						await MainSceneController.Instance.Info.ShowDelay(InfoType.BindingAlreadyAuthenticated, null, null, 3);
					}
					else
					{
						MainSceneController.Instance.Info.ShowDelay(InfoType.BindingFailed, null, null, 3);
						//MainSceneController.Instance.Info.Show(res.Error.Message, "ERROR", InfoIconTypeEnum.Error, null, new InfoAction("Close", null));

					}
					_facebook.Logout();
				}
				else
				{
					SaveAuthData(res.Data);
					await MainSceneController.Instance.Info.ShowDelay(InfoType.BindingSuccess, null, null, 3);
				}
			}

			MainSceneController.Instance.Loading.DoneLoading();
			return res;
		}
		#endregion
		
		
		public async Task<LoginData> FetchRefreshToken()
        {
			MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
			var res = await RequestHandler.Request(async () => await _authBackend.FetchRefreshToken(RefreshToken));
			Debug.Log("REFRESH TOKEN IS = " + RefreshToken);
			if(res == null)
			{
				return null;
			}
			if (res.Error != null)
            {
				if(res.Error.Code != "0") MainSceneController.Instance.Loading.DoneLoading();
                MainSceneController.Instance.Info.ShowSomethingWrong(res.Error.Code);
				return null;
            }
            else
            {
                MainSceneController.Instance.Loading.DoneLoading();
                Debug.Log("Success");
				SaveAuthData(res.Data);
				return res.Data;
            }
		}

		public async Task<string> FetchFacebookToken()
		{
			if (!_isBindedWithFacebook)
			{
				// await _main.Info.ShowDelay(InfoType.LoginFailed, null, null, 3); // TODO COPY ERROR
				// _main.Loading.DoneLoading();
				return null;
			}
			_main.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);

			var fetchResult = await _facebook.Login();

			if (fetchResult.Error != null)
			{
				await _main.Info.ShowDelay(InfoType.LoginFailed, null, null, 3); // TODO COPY ERROR
				_main.Loading.DoneLoading();
				return null;
			}

			return fetchResult.Token;
		}

		public async Task<GenericResponseData<LoginData>> Unbind()
		{
			MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);

			var res = await RequestHandler.Request(async () => await _authBackend.Unbind());

			SignOut();
			MainSceneController.Instance.Loading.DoneLoading();
			return res;
		}


		public void SignOut()
		{
			SignOutThirdParty();
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
			MainSceneController.Instance.ResetData();
			OnSignOut.Invoke();
		}

		public void SaveAuthData(LoginData loginData)
		{
			if(loginData == null)
			{
				MainSceneController.Token = null;
				PlayerPrefs.DeleteKey(REFRESH_TOKEN);
			}
			else
			{
				MainSceneController.Token = loginData.Token;
				PlayerPrefs.SetString(REFRESH_TOKEN, loginData.RefreshToken.ToBase64());
			}

			UpdateAuthBindData(loginData.BindedAccounts);
			PlayerPrefs.Save();
			Debug.Log("Player Pref saved!");
		}


		public void UpdateAuthBindData(UserAccountData[] userAccounts)
		{
			if (userAccounts == null)
			{
				_main.Data.UserAccounts = null;
				return;
			}

			Dictionary<AccountTypesEnum, UserAccountData> data = new Dictionary<AccountTypesEnum, UserAccountData>();
			foreach (var account in userAccounts)
			{
				if (data.ContainsKey(account.Type)){
					data[account.Type] = account;
				}
				else
				{
					data.Add(account.Type, account);
				}
			}

			_main.Data.UserAccounts = data;
		}

		public void SignOutThirdParty()
		{
			try
			{
				if (_main.Data.UserAccounts.ContainsKey(AccountTypesEnum.Google))
				{
					_googleAuth.SignOut();
				}

				if (_main.Data.UserAccounts.ContainsKey(AccountTypesEnum.Apple))
				{
					//LOGOUT APPLE
				}

				if (_main.Data.UserAccounts.ContainsKey(AccountTypesEnum.Facebook))
				{
					_facebook.Logout();
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
			}

		}

		public bool IsAuthenticated()
		{
			var userAccounts = MainSceneController.Instance.Data.UserAccounts;
			return userAccounts.ContainsKey(AccountTypesEnum.Google) || userAccounts.ContainsKey(AccountTypesEnum.Apple);
		}
	}
}