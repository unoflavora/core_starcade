using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Info;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Game
{
    public class FillSurvey : MonoBehaviour
    {
        public string SurveyUrl;
        public void OpenSurvey()
        {
            Application.OpenURL(SurveyUrl);
            MainSceneController.Instance.Info.Show(InfoType.FillSurvey, new InfoAction("Close", null), null);
        }
    }
}