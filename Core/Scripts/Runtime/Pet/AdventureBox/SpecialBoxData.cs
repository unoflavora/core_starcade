using System;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Pet.Core.SpecialBox
{
	[Serializable]
    public class SpecialBoxData
    {
        public string Name;
        public int Type;
        public List<SpecialBoxReward> Rewards;
        public string ObtainedDateString;
        public DateTime ObtainedDate => DateTime.Parse(ObtainedDateString);
    }

    [Serializable]
    public class SpecialBoxReward
    {
        public string Id;
        public string Desc;
    }
}