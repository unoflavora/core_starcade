using UnityEngine;

namespace Starcade.Core.Friends.Core
{
    public static class Extensions
    {
        /// <summary>
        /// Puts the string into the Clipboard.
        /// </summary>
        public static void CopyToClipboard(this string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }

    }
}
