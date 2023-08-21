using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.Fragment.Model
{
	public class CombinePetFragmentResponse : MonoBehaviour
    {
        public PetFragmentInventory AffectedFragment { get; set; }
        public List<PetInventoryData> GrantedPets { get; set; }
    }
}