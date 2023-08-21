using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Data_Class.Account;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Lobby.UserProfile
{
    public class UserProfileData
    {
        public const string DefaultAvatar = "AVA_DEFAULT_00";

        public string UsedAvatar { get; set; }
        public string UsedFrame { get; set; }
        public List<AccessoryItemData> Avatars { get; set; }
        public List<AccessoryItemData> Frames { get; set; }

        public Sprite GetCurrentAvatar()
        {
            if (UsedAvatar == DefaultAvatar  && MainSceneController.Instance.Data.UserAccounts.ContainsKey(AccountTypesEnum.Google))
            {
                return MainSceneController.Instance.Data.UserProfileThirdPartyData.GoogleAvatar; //FORCE RETURN GOOGLE AVATAR
            }

            return MainSceneController.Instance.AssetLibrary.GetSpriteAsset(UsedAvatar);
        }

        public Sprite GetCurrentFrame()
        {
            return MainSceneController.Instance.AssetLibrary.GetSpriteAsset(UsedFrame);
        }
    }

    public class UserProfileAction
    {
        public Action<Dictionary<ItemTypeEnum, string>> OnAccessoryChanged { get; set; }

        
    }

    public class UserProfileThirdPartyData
    {
        public Sprite GoogleAvatar { get; set; }
    }

    public class AccessoryItemData
    {
        public string Id { get; set; }
        public AccessoryStatusEnum Type { get; set; }
        public int Cost { get; set; }
        public CurrencyTypeEnum CurrencyType { get; set; }
        public bool IsOwned { get; set; }
        public string Caption { get; set; }
    }


    public enum AccessoryStatusEnum
    {
        [EnumMember(Value = "Locked")]
        Locked = 0,
        [EnumMember(Value = "Premium")]
        Premium = 1,
        [EnumMember(Value = "Default")]
        Default = 2,
        [EnumMember(Value = "Paid")]
        Paid = 3,
    }
}