using UnityEngine;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Main;

namespace Agate.Starcade.Scripts.Development
{
    public class SimpleErrorException : System.Exception
    {
        private ErrorScreen errorScreen;
        private LoadingScreen _loadingScreen;
        
        public void InitError(LoadingScreen loadingScreen)
        {
            _loadingScreen = loadingScreen;
        }
        public SimpleErrorException()
        {
            Debug.Log("error");
            //MainSceneLauncher.Instance.Error.gameObject.SetActive(true);
        }

        public SimpleErrorException(string message) : base(message)
        {
            MainSceneController.Instance.Loading.DoneLoading();
            //MainSceneLauncher.Instance.Error.TriggerError(message,null,ErrorScreen.ErrorType.NeedRestartGame);
        }
    }
}
