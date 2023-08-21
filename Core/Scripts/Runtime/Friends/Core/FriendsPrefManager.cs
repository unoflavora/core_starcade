using UnityEngine;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.Core
{
    public class FriendsPrefManager
    {
        private static string FriendKeyPrefix = "Friend_";
        private static string FriendRequestKeyPrefix = "FriendRequest_";

        public static bool FriendExistsInPlayerPrefs(long friendId)
        {
            string key = GetFriendKey(friendId);
            return PlayerPrefs.HasKey(key);
        }

        public static void SaveFriendToPlayerPrefs(long friendId)
        {
            string key = GetFriendKey(friendId);
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }

        public static bool FriendRequestExistsInPlayerPrefs(long friendId)
        {
            string key = GetFriendRequestKey(friendId);
            return PlayerPrefs.HasKey(key);
        }

        public static void SaveFriendRequestToPlayerPrefs(long friendId)
        {
            string key = GetFriendRequestKey(friendId);
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }
        
        public static void RemoveFriendFromPlayerPrefs(long friendId)
        {
            string key = GetFriendKey(friendId);
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
        
        public static void RemoveFriendRequestFromPlayerPrefs(long friendId)
        {
            string key = GetFriendRequestKey(friendId);
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        private static string GetFriendKey(long friendId)
        {
            return $"{FriendKeyPrefix}{friendId}";
        }

        private static string GetFriendRequestKey(long friendId)
        {
            return $"{FriendRequestKeyPrefix}{friendId}";
        }
    }
}