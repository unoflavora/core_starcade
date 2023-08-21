using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Agate.Starcade.Runtime.Main
{
    public static class InitialSceneLauncher
    {
        private const string MAIN_SCENE_NAME = "MainScene";

        public static string FirstLoadedScenePath { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {

#if UNITY_ANDROID || UNITY_EDITOR || UNITY_IOS
            Debug.Log("Before first Scene loaded");

            if (SceneManager.GetActiveScene().name == MAIN_SCENE_NAME) return;
            FirstLoadedScenePath = SceneManager.GetActiveScene().path;
            Debug.Log("Current first scene loaded : " + FirstLoadedScenePath);
            SceneManager.LoadScene(MAIN_SCENE_NAME, LoadSceneMode.Single); 
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoadRuntimeMethod()
        {
#if UNITY_ANDROID || UNITY_EDITOR
            Debug.Log("After first Scene loaded");
#endif
        }

        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeMethodLoad()
        {
#if UNITY_ANDROID || UNITY_EDITOR
            Debug.Log("RuntimeMethodLoad: After first Scene loaded");
#endif
        }
    }
}

