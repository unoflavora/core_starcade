using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Main;
using UnityEngine;

namespace Agate.Starcade.Core.Scripts.Runtime.UI.Reward
{
    public static class AssetRewardHelper
    {
        public static Sprite GetRewardAsset(DailyLoginRewardData rewardData)
        {
            Debug.Log("GET ASSET = " + rewardData.RewardType + "_" + rewardData.Tier);


            bool is3D = MainSceneController.Instance.RemoteConfigData.IsPet3DEnabled;

            string is3Dstring;
            if (is3D)
            {
                is3Dstring = "3d";
            }
            else
            {
                is3Dstring = "2d";
            }

            string assetData = rewardData.Ref + "_" + rewardData.Tier + "_" + is3Dstring;

            return MainSceneController.Instance.AssetLibrary.GetSpriteAsset(assetData);
        }

        public static Sprite GetRewardAsset(string rewardAssetId)
        {
            bool is3D = MainSceneController.Instance.RemoteConfigData.IsPet3DEnabled;

            string is3Dstring;
            if (is3D)
            {
                is3Dstring = "_3d";
            }
            else
            {
                is3Dstring = "_2d";
            }

            string assetData = rewardAssetId + is3Dstring;

            Debug.Log("get asset = " + assetData);

            return MainSceneController.Instance.AssetLibrary.GetSpriteAsset(assetData);
        }

    }
}