using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.DailyLogin.SO;
using Agate.Starcade.Scripts.Runtime.DailyLogin.VFX;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.DailyLogin
{
    public class VFXManager : MonoBehaviour
    {
        public const string TITLE_REWARD_TEXT = "Daily Login Reward Day ";

        [SerializeField] private RewardVFX _dailyRewardVfx;


        public void ShowRewardVFX(DailyLoginRewardData rewardData, DailyLoginSO _dailyLoginSo)
        {
            _dailyRewardVfx.gameObject.SetActive(true);
            _dailyRewardVfx.GetComponent<ParticleSystem>().Play();

            if(rewardData != null ) { }

            if (CheckAvailabilityVFX(rewardData.RewardType))
            {
                _dailyRewardVfx.InitDailyRewardVFX(rewardData.Amount, rewardData.RewardType, TITLE_REWARD_TEXT + rewardData.Day);
            }
            else
            {
                _dailyRewardVfx.InitDailyRewardNonVFX(rewardData);
            }
        }


        private bool CheckAvailabilityVFX(RewardEnum rewardEnum)
        {
            switch (rewardEnum)
            {
                case RewardEnum.GoldCoin:
                    return true;
                case RewardEnum.StarCoin:
                    return true;
                case RewardEnum.StarTicket:
                    return true;
                case RewardEnum.Lootbox:
                    return false;
                case RewardEnum.Pet:
                    return false;
                default:
                    return false;
            }
        }

        public void HideRewardVFX()
        {
            Debug.Log("hide");
            _dailyRewardVfx.gameObject.SetActive(false);
        }

    }
}
