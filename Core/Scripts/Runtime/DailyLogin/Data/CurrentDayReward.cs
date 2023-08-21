using Agate.Starcade.Runtime.Data.DailyLogin;

namespace Agate.Starcade.Scripts.Runtime.DailyLogin.Data
{
    [System.Serializable]
    public class CurrentDayReward
    {
        public DailyLoginRewardEnum Type;
        public int Amount;
    }
}
