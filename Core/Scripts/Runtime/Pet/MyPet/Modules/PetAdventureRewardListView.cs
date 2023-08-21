using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Starcade.Core.ExtensionMethods;
using Starcade.Core.Localizations;
using TMPro;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.Modules
{
    public class PetAdventureRewardListView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Color _rewardFromSkillColor;
        public void SetRewardFromBasicReward(RewardBase reward)
        {
            switch (reward.Type)
            {
                case RewardEnum.SpecialBox:
                    _text.SetTextFromLocalizedId(PetAdventureBoxLocalizations.Table, reward.Ref);
                    break;
                default:
                    var rewardName =  MainSceneController.Instance.Localizations.GetLocalizedString(RewardLocalizations.Table, Enum.GetName(typeof(RewardEnum), reward.Type));
                    var rewardText = $"{CurrencyHandler.Convert(reward.Amount)} {rewardName}";
                    _text.SetText(rewardText);
                    break;
            }
            
            _text.color = Color.white;
            
        }

        public void SetRewardFromSubSkill(SubSkill skill)
        {
            var boxName = MainSceneController.Instance.Localizations.GetLocalizedString(PetAdventureBoxLocalizations.Table, skill.Id);
            _text.SetTextFromLocalizedId(PetSkillsLocalizations.Table, PetSkillsLocalizations.SubSkillsKey, PetSkillsLocalizations.GetSubSkillsParam(boxName, skill.Value));
            _text.color = new Color(_rewardFromSkillColor.r, _rewardFromSkillColor.g, _rewardFromSkillColor.b, 1);
        }

        public void SetRewardFromBasicSkill(BasicSkill skill)
        {
            _text.GetComponentInChildren<TextMeshProUGUI>().SetTextFromLocalizedId(PetSkillsLocalizations.Table, skill.Type, new Dictionary<string, string>()
            {
                {"amount", skill.Amount.ToString()}
            });
            
            _text.color = new Color(_rewardFromSkillColor.r, _rewardFromSkillColor.g, _rewardFromSkillColor.b, 1);
        }


        private bool CheckIsNull<T>(T item)
        {
            if (item == null)
            {
                gameObject.SetActive(false);
                return true;
            }
            
            gameObject.SetActive(true);
            return false;
        }
            
        
    }
}