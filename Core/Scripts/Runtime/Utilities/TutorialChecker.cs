using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Utilities;
using System.Threading.Tasks;
using UnityEngine;

namespace Agate.Starcade
{
    public class TutorialChecker
    {
        public static async Task<bool> IsTutorialFinished(string field)
        {
            int tutorialState = PlayerPrefs.GetInt($"{field}_IsTutorial", -1);
            Debug.Log($"arcade isTutorial state {tutorialState}");
            if (tutorialState == -1)
            {
                var gameBackend = MainSceneController.Instance.GameBackend;
                var result = await RequestHandler.Request(async () => await gameBackend.OnBoarding(field));

                if (result.Error != null)
                {
                    MainSceneController.Instance.Info.ShowSomethingWrong(result.Error.Code);
                    PlayerPrefs.SetInt($"{field}_IsTutorial", 0);
                }
                else if (!result.Data.IsComplete)
                {
                    PlayerPrefs.SetInt($"{field}_IsTutorial", 0);
                }
                else
                {
                    PlayerPrefs.SetInt($"{field}_IsTutorial", 1);
                }
            }
            tutorialState = PlayerPrefs.GetInt($"{field}_IsTutorial", -1);
            Debug.Log($"arcade isTutorial state {tutorialState}");
            return tutorialState == 1;
        }

        public static void SetTutorialFinish(string field)
        {
            PlayerPrefs.SetInt($"{field}_IsTutorial", 1);
        }
    }
}
