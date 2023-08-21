namespace Agate.Starcade.Core.Scripts.Runtime.Friends.Data
{
    public class FriendProfile
    {
        public string Username { get; set; }
        public string UsedAvatar { get; set; }
        public string UsedFrame { get; set; }
        public long FriendCode { get; set; }
        public FriendCategory Category { get; set; }
        public string PhotoUrl { get; set; }
        public FriendRecommendationType FriendRecommendationType { get; set; }
        public bool IsNew { get; set; }
        
        public int Level { get; set; }
    }

    public class FriendsConfig
    {
        public int MaxFriends { get; set; }
        public int CurrentFriends { get; set; }
    }
    
    public enum FriendCategory { Friend, Recommendation, FriendRequest }
    public enum FriendRequestStatus { Approved, PendingApproval, NotFriend }
    public enum FriendsInteractionType { Add, Accept, Decline, Unfriend }
    public enum FriendRecommendationType { Default, Facebook }
    
}