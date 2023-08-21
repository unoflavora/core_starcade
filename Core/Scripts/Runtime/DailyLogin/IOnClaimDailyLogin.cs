using Agate.Starcade.Runtime.Data.DailyLogin;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.DailyLogin
{
    public interface IOnClaimDailyLogin 
    {
        public object Meta { get; set; }
        public void ShowClaim(DailyLoginRewardData rewardData, Sprite rewardSprite, int DayReward, float duration, UnityAction<DailyLoginRewardData> onClaimReward, UnityAction onRewardPanelClose);
        public void ClaimReward(DailyLoginRewardData dailyLoginRewardData);
    }
}