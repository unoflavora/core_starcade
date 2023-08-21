using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Helper
{
    public static class AppHelper
    {
        public static bool ValidateAppVersion(string latestVersion)
        {
            if (string.IsNullOrEmpty(latestVersion)) return true;

            var latestVersionSplits = latestVersion.Split('.');
            var currentVersionSplits = Application.version.Split('.');

            if (latestVersionSplits?.Length < 2) return false;
            for(int i = 0;i<2;i++)
            {
                if (latestVersionSplits[i] != currentVersionSplits[i]) return false;
            }

            return true;
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}