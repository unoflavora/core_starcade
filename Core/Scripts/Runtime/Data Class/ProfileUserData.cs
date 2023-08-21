using Agate.Starcade.Scripts.Runtime.Data_Class.Account;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    public class ProfileUserData
    {
        public string DisplayName { get; set; }
        public long FriendCode { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public string UserId { get; set; }
        public AccountTypesEnum AccountType { get; set; }
    }
}