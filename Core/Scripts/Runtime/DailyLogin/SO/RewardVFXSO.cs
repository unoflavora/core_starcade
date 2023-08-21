using System;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Enums;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.DailyLogin.SO
{
    [CreateAssetMenu(menuName = "Starcade/DailyLogin/RewardVFXSO")]
    [Serializable]
    public class RewardVFXSO : ScriptableObject
    {
        //TODO: Change Based on Artist Requirement
        [SerializeField] public Material _defaultParticleTexture;
        [SerializeField] public Material _defaultIcon;

        [SerializeField] public Material _goldParticleTexture;
        [SerializeField] public Material _goldIcon;
        
        [SerializeField] public Material _starParticleTexture;
        [SerializeField] public Material _starIcon;
        
        [SerializeField] public Material _ticketParticleTexture;
        [SerializeField] public Material _ticketIcon;

        [SerializeField] public Material _lootBoxParticleTexture;
        [SerializeField] public Material _lootBoxIcon;

        [SerializeField] public Material _petParticleTexture;
        [SerializeField] public Material _petBoxIcon;


        public (Material,Material) GetVFXSprite(Starcade.Runtime.Enums.RewardEnum rewardEnum)
        {
            Material particle = null;
            Material icon = null;
            switch (rewardEnum)
            {
                case Starcade.Runtime.Enums.RewardEnum.GoldCoin:
                    particle = _goldParticleTexture;
                    icon = _goldIcon;
                    break;
                case Starcade.Runtime.Enums.RewardEnum.StarCoin:
                    particle = _starParticleTexture;
                    icon = _starIcon;
                    break;
                case Starcade.Runtime.Enums.RewardEnum.StarTicket:
                    particle = _ticketParticleTexture;
                    icon = _ticketIcon;
                    break;
                case Starcade.Runtime.Enums.RewardEnum.Lootbox:
                    particle = _lootBoxParticleTexture;
                    icon = _lootBoxIcon;
                     break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rewardEnum), rewardEnum, null);
            }
            return (particle,icon);
        }

        public (Material, Material) GetVFXSprite(Starcade.Runtime.Data.DailyLogin.DailyLoginRewardEnum rewardEnum)
        {
            Material particle = null;
            Material icon = null;

            switch (rewardEnum)
            {
                case Starcade.Runtime.Data.DailyLogin.DailyLoginRewardEnum.GoldCoin:
                    particle = _goldParticleTexture;
                    icon = _goldIcon;
                    break;
                case Starcade.Runtime.Data.DailyLogin.DailyLoginRewardEnum.StarCoin:
                    particle = _starParticleTexture;
                    icon = _starIcon;
                    break;
                case Starcade.Runtime.Data.DailyLogin.DailyLoginRewardEnum.StarTicket:
                    particle = _ticketParticleTexture;
                    icon = _ticketIcon;
                    break;
                case Starcade.Runtime.Data.DailyLogin.DailyLoginRewardEnum.LootBox:
                    particle = null;
                    icon = null;
                    break;
                case Starcade.Runtime.Data.DailyLogin.DailyLoginRewardEnum.Pet:
                    particle = null;
                    icon = null;
                    break;
                default:
                    break;
            }

            return (particle, icon);
        }

    }
}
