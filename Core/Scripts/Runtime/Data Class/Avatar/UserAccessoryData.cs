using Agate.Starcade.Runtime.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade
{
    [Serializable]
    public class UserAccessoryData
    {
        public Dictionary<ItemTypeEnum, AccessoryData> UserAccessories { get; set; }
        public Sprite PhotoUser;
        public string PhotoURL;

        public string DefaultActiveAvatar = "AVA_RewardLevel_00";
        public string DefaultActiveFrame = "FRM_RewardLevel_00";

        public AccessoryLibrary AvatarLibrary;
        public AccessoryLibrary FrameLibrary;
    }
}
