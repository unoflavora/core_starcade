using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Starcade.Core.Localizations;
using System;

namespace Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model
{
	[Serializable]
    public class AdventureBoxData
    {
        public string AdventureBoxId;
        public string Caption;
        public string Name => MainSceneController.Instance.Localizations.GetLocalizedString(PetAdventureBoxLocalizations.Table, AdventureBoxId);
        public int Amount;
        public int Tier;
        public AdventureBoxReward[] Rewards;
    }

    [Serializable]
    public class AdventureBoxReward
    {
        public RewardEnum Type;
        public string Id;
        public float MaxAmount;
        public float MinAmount;

    }

    public class AdventureBoxRequest
    {
        public string BoxId { get; set; }
        public int Amount { get; set; }

        public AdventureBoxRequest(string boxId, int amount)
        {
            BoxId = boxId;
            Amount = amount;
        }
    }
}