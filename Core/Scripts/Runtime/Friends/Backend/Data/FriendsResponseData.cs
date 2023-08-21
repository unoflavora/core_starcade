using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Runtime.Enums;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.Backend.Data
{
    public class FriendsData
    {
        public List<FriendsResponseData> Friends { get; set; }
        public List<FriendsResponseData> Pendings { get; set; }
        public FriendConfig FriendConfig { get; set; }
    }

    public class FriendsResponseData
    {
        public FriendProfile Profile { get; set; }
        public FriendServerStatus Status { get; set; }
    }

    public class FriendServerStatus
    {
        public string userId;
        public DateTime requestedAt;
        public DateTime acceptedAt;
        public DateTime rejectedAt;
        public FriendRequestStatus status;
    }

    public class FriendConfig
    {
        public int TotalFriend { get; set; }
        public int TotalPendingFriend { get; set; }
        public int MaxFriend { get; set; }
        public int MaxPendingFriend { get; set; }
    }

    public class SendCollectibleToFriendData
    {
        public long FriendCode { get; set; }
        public List<string> CollectibleIds { get; set; }
    }

    public class UserGiftDailyLimit
    {
        public int LeftAmount { get; set; }
        public long CostForNextBonus { get; set; }
        public CurrencyTypeEnum CurrencyType { get; set; }
    }
    
    public class ResetDailyLimitData
    {
        public CurrencyTypeEnum Type { get; set; }
        public long Amount { get; set; }
    }

}