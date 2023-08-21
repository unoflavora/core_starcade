using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.Fragment.Dummy
{
	[CreateAssetMenu(fileName = "PetFragmentsDummy", menuName = "Pet/Dummy/PetFragments", order = 1)]
    public class PetFragmentsDummy : ScriptableObject
    {
        public List<PetFragmentInventory> PetFragmentDatas;
    }
}