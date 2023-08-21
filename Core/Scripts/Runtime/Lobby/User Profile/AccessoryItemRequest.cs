using Agate.Starcade.Runtime.Enums;
using System.Collections;
using UnityEngine;

namespace Agate.Starcade.Core.Scripts.Runtime.Lobby.Script.User_Profile
{
    public class AccessoryItemRequest
    {
        public string AvatarId;
        public string FrameId;

        public AccessoryItemRequest(ItemTypeEnum itemType, string id)
        {
            switch (itemType)
            {
                case ItemTypeEnum.Avatar:
                    AvatarId = id;
                    break;
                case ItemTypeEnum.Frame:
                    FrameId = id;
                    break;
                default:
                    break;
            }
        }
    }
}