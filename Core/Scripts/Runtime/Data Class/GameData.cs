using Agate.Starcade.Scripts.Runtime.Data_Class.Account;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    public class UserData
    {
        public long FriendCode { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
		public int Experience { get; set; }
        public UnlockedAccessoryData Accessories;
        public ExperienceData ExperienceData;
        public string SessionId;
    }
}