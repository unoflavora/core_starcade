using System.Runtime.Serialization;

namespace Agate.Starcade.Scripts.Runtime.Data_Class.Account
{
    public enum AccountTypesEnum
    {
        [EnumMember(Value = "None")]
        None = 0,
        [EnumMember(Value = "Guest")]
        Guest = 1,
        [EnumMember(Value = "Google")]
        Google = 2,
        [EnumMember(Value = "Apple")]
        Apple = 3,
        [EnumMember(Value = "Facebook")]
        Facebook = 4,
    }
}