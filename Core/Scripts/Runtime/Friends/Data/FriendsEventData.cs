using Agate.Starcade.Core.Scripts.Runtime.Friends.Backend.Data;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.Data
{
    public class FriendRequestEventData
    {
        public long FriendCode { get; set; }
        public bool IsAccept { get; set; }
        
        public FriendRequestEventData(long friendCode, bool isAccept)
        {
            FriendCode = friendCode;
            IsAccept = isAccept;
        }
    }

    public class CurrentFriendInteractionData
    {
        public long FriendCode { get; set; }
        public string FriendName { get; set; }
        
        public CurrentFriendInteractionData(long friendCode, string friendName)
        {
            FriendCode = friendCode;
            FriendName = friendName;
        }
    }
    
    public class FriendStatusChangedEventData
    {
        public long FriendCode { get; set; }
        public FriendRequestStatus Status { get; set; }
        
        public FriendStatusChangedEventData(long friendCode, FriendRequestStatus status)
        {
            FriendCode = friendCode;
            Status = status;
        }
    }
}