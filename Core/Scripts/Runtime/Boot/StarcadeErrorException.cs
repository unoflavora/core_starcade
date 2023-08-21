using Agate.Starcade.Runtime.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Boot
{
    public class StarcadeErrorException : System.Exception
    {
        private ErrorScreen errorScreen;
        private LoadingScreen _loadingScreen;
        private List<StarcadeError> _starcadeErrorMessages;

        public void InitError(LoadingScreen loadingScreen)
        {
            _loadingScreen = loadingScreen;
        }
        public StarcadeErrorException()
        {
            Debug.Log("error");
            //MainSceneLauncher.Instance.Error.gameObject.SetActive(true);
        }

        public StarcadeErrorException(string message) : base(message)
        {
            MainSceneController.Instance.Loading.DoneLoading();
            //MainSceneLauncher.Instance.Error.TriggerErrorWithMessage(message,null,ErrorScreen.ErrorType.MustRestartGame);
        }

        public StarcadeErrorException(int errorCode)
        {
            MainSceneController.Instance.Loading.DoneLoading();
            //MainSceneLauncher.Instance.Error.TriggerError(errorCode);
        }
    }
}
