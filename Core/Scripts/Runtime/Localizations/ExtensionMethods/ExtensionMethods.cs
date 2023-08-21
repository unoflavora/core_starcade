using System.Collections.Generic;
using Agate.Starcade.Runtime.Main;
using Starcade.Core.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace Starcade.Core.ExtensionMethods
{
    public static class LocalizationsExtension
    {
        public static void SetTextFromLocalizedId(this TextMeshProUGUI text, TableReference tableReference, string key, Dictionary<string, string> param = null)
        {
            var value = MainSceneController.Instance.Localizations.GetLocalizedString(tableReference, key, param);
            
            text.SetText(value);
        }

        public static void SetTextAsSubSkill(this TextMeshProUGUI text, string skillId, double value)
        {
            var name = MainSceneController.Instance.Localizations.GetLocalizedString(PetAdventureBoxLocalizations.Table, skillId);
            
            var subSkill= MainSceneController.Instance.Localizations.GetLocalizedString(PetSkillsLocalizations.Table, PetSkillsLocalizations.SubSkillsKey, PetSkillsLocalizations.GetSubSkillsParam(name, value));
            
            text.SetText(subSkill);
        }
    }
}
