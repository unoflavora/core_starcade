using System.Runtime.Serialization;

namespace Agate.Starcade.Scripts.Runtime.Data_Class.Reward
{
    public enum UserRewardEnum
    {
        [EnumMember(Value = "Unknown")]
        Unknown = 0,

        [EnumMember(Value = "NotCompleted")]
        NotCompleted = 1,

        [EnumMember(Value = "Granted")]
        Granted = 2,

        [EnumMember(Value = "Claimed")]
        Claimed = 3,
    }
    
}