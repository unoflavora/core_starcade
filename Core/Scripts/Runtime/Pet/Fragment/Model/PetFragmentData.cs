using System;

namespace Agate.Starcade.Core.Runtime.Pet.Core.Model
{
	[Serializable]
    public class PetFragmentDataOld
    {
        public string Id;
        public int Owned;
        public DateTime ObtainedDate;
        public string ObtainedDateString;
        public int RequirementAmount;
    }
}