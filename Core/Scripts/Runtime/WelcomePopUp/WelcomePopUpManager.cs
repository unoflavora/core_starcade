using Agate.Starcade.Runtime.Helper;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Agate.Starcade.Core.Scripts.Runtime.WelcomePopUp
{
    public class WelcomePopUpManager : MonoBehaviour
    {
        [SerializeField] private AssetReference _welcomePopUp;
        public void LoadScene()
        {
            Debug.Log("OPEN WELCOME POP UP SCENE");
            LoadSceneHelper.LoadSceneAdditive(_welcomePopUp);
        }
    }
}