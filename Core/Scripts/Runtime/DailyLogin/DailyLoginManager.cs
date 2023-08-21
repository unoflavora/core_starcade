using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.DailyLogin.Data;
using Agate.Starcade.Scripts.Runtime.Data;
using IngameDebugConsole;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.DailyLogin
{
    public class DailyLoginManager : MonoBehaviour
    {
        private static readonly string OPEN_DAILY_LOGIN_COMMAND = "open_daily_login";
        private static readonly string LEAP_DAY_COMMAND = "leapDay";

        //[SerializeField] private Button _openButton;
		[SerializeField] private AssetReference _dailyLoginScene;

        private DailyLoginData _dailyLoginData;
        private UnityAction<PlayerBalance> OnBalanceUpdate;

        private DailyLoginEnum _UIState;

        public bool IsActive;
 

        public async Task Init()
        {
            _dailyLoginData = MainSceneController.Instance.Data.dailyLoginData;

            if (_dailyLoginData.Reward != null)
            {
                await LoadScene();
            }

            DebugLogConsole.AddCommand(OPEN_DAILY_LOGIN_COMMAND, "Open Daily Login", async () => await LoadScene());
            DebugLogConsole.AddCommand(LEAP_DAY_COMMAND, "Leap Day Login", LeapDailyLogin);
        }

        private async void LeapDailyLogin()
        {
            await MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            var result = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.LeapDayDailyLogin());
            var dailyLoginResponse = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.GetDailyLoginRewards());
            if (dailyLoginResponse.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(dailyLoginResponse.Error.Code);
                return;
            }

            MainSceneController.Instance.Data.dailyLoginData = dailyLoginResponse.Data;
            MainSceneController.Instance.Data.dailyLoginData.DailyLoginState = Agate.Starcade.Scripts.Runtime.DailyLogin.Data.DailyLoginEnum.Closed;
            await LoadScene();
            MainSceneController.Instance.Loading.DoneLoading();
        }

        public async Task LoadScene()
        {
            await LoadSceneHelper.LoadSceneAdditive(_dailyLoginScene);
        }

        public void SceneIsActive()
        {
            
        }
    }
}