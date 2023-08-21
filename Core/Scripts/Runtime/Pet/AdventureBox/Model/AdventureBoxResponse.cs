using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model
{
	public class AdventureBoxResponse
    {
        public AdventureBoxData BoxDetail { get; set; }
        public List<AdventureBoxData> BoxInventory { get; set; }
    }

    public class OpenAdventureBoxResponse
    {
        public int RemainingBox { get; set; }
        public List<AdventureBoxData> BoxInventory { get; set; }
        public List<RewardBase> RewardGain { get; set; }
    }

    public class AdventureBoxRewardGain
    {
        public RewardEnum Type { get; set; }
        public object Ref { get; set; }
        public int Amount { get; set; }
    }
}