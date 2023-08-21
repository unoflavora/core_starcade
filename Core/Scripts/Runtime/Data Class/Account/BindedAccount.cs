using System;

namespace Agate.Starcade.Scripts.Runtime.Data_Class.Account
{
    [Serializable]
    public class UserAccountData
    {
        public AccountTypesEnum Type;
        public string Email;
        public string PhotoUrl;
        public string DisplayName;
    }
}