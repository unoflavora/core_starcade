using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Core.Runtime.ThirdParty.Facebook;
using Facebook.Unity;
using Facebook.Unity.Settings;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.ThirdParty.Facebook
{
    public class FacebookInteractionHandler
    {
        private static FacebookInteractionHandler instance;
        private FacebookInteractionHandler() {}
        
        public static FacebookInteractionHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FacebookInteractionHandler();
                }
                return instance;
            }
        }

        public void GetFriendList()
        {
            FB.API("me/friends/", HttpMethod.GET, result => { Debug.Log(result.RawResult); });
        }
    }

}