using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Agate.Starcade.Runtime.Helper
{
	public static class SceneLaunchDataHelper
    {
        private static readonly Dictionary<string, object> SceneLaunchData = new Dictionary<string, object>();
        private static void ClearLaunchDataFor(string scenePath)
            => SceneLaunchData[scenePath] = null;
        public static object GetLaunchData(this Scene scene)
            => SceneLaunchData.ContainsKey(scene.path) ? SceneLaunchData[scene.path] : null;
        public static void SetLaunchData(string scenePath, object launchData)
            => SceneLaunchData[scenePath] = launchData;
        public static void Setup()
            => SceneManager.sceneUnloaded += scene => ClearLaunchDataFor(scene.path);
    }
}

