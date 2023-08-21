using Agate.Starcade.Runtime.Enums;
using System;
using Agate.Starcade.Runtime.Main;
using Starcade.Core.Localizations;

namespace Agate.Starcade.Runtime.Data
{
    [Serializable]
    public class RewardBase
    {
        public RewardEnum Type;
        public string Ref;
        public object RefObject;
        public long Amount;
        public float Chance;

        public string GetRewardName()
        {
            switch (Type)
            {
                case RewardEnum.SpecialBox:
                    return MainSceneController.Instance.Localizations.GetLocalizedString(PetAdventureBoxLocalizations.Table, Ref);
                default:
                    return MainSceneController.Instance.Localizations.GetLocalizedString(RewardLocalizations.Table, Enum.GetName(typeof(RewardEnum), Type));
            }
        }
    }
}