using Agate.Starcade.Scripts.Runtime.Data_Class.Account;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    public class LoginData
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Expires { get; set; }
        public ProfileUserData Data { get; set; }
        public UserAccountData[] BindedAccounts { get; set; }
    }
}