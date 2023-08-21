using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Helper;
using Agate.Starcade.Runtime.Auth;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Agate.Starcade.Scripts.Runtime
{
    public class EditorController
    {
        private MainSceneController _main;
        private readonly string _targetScenePath;
        private readonly bool _isOfflineMode;
        
        public EditorController(MainSceneController main, bool isOfflineMode)
        {
            _main = main;
            _isOfflineMode = isOfflineMode;
        }

        public async Task StartGameOnEditor()
        {
            try
            {
                await Authenticate();
                await InitGame();
                await _main.LoadMainAsset();
            }
            catch(Exception ex) 
            {
                throw ex;
            }
		}
        
        private async Task Authenticate()
        {
            if (_isOfflineMode)
            {
                _main.Auth.SaveAuthData(null);
                return;
            }
            
            if (!string.IsNullOrEmpty(_main.Auth.RefreshToken))
            {
                await _main.Auth.FetchRefreshToken();
            }
            else
            {
                var result = await _main.Auth.LoginAsGuest();

				if (result.Error != null)
                {
                    MainSceneController.Instance.Info.ShowSomethingWrong(result.Error.Code);
                }
                else
                {
                    _main.Auth.SaveAuthData(result.Data);
                    
                }
            }
        }
        
        private async Task InitGame()
        {
            var res = await RequestHandler.Request(async () => await _main.GameBackend.GameInit());
			MainSceneController.Instance.Loading.DoneLoading();
			if (res.Error != null)
            {
                Debug.Log("GAME INIT MAIN ERROR");
                MainSceneController.Instance.Info.ShowSomethingWrong(res.Error.Code);
            }
            else
            {
				string currentAppversion = "1.0.0";
#if UNITY_IOS
					currentAppversion = res.Data.GameVersion.Ios;
#else
				currentAppversion = res.Data.GameVersion.Android;
#endif
				var isValidAppVersion = AppHelper.ValidateAppVersion(currentAppversion);

				if (!isValidAppVersion)
                {
                    Debug.LogWarning("ERROR WARNING VERSION NOT SYNC BETWEEN BACKEND - VERSION FRONTEND: " + Application.version + " - VERSION BACKEND: " + res.Data.GameVersion.Android);
                }
                MainSceneController.Instance.Data.SetGameInitData(res.Data);
                MainSceneController.Instance.Data.ExperienceData.Data = res.Data.data.ExperienceData;
            }

            try
            {
                await MainSceneController.Instance.MainRequestController.FetchMainData();
            }
            catch(Exception ex)
            {
                throw ex;
            }     
        }
    }
}