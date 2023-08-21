using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agate.Starcade.Core.Runtime.Pet.Fragment
{
	public class PetFragment
    {
        public List<PetFragmentInventory> Inventory { get; set; }

        public void UpdatePetFragmentInvetory(PetFragmentInventory petFragmentInventory)
        {
            var data = Inventory.FirstOrDefault(x => x.PetId == petFragmentInventory.PetId);
            if(petFragmentInventory.Owned <= 0)
            {
                Inventory.Remove(data);
            }
            else
            {
                data.Owned = petFragmentInventory.Owned;
                data.ObtainedDate = petFragmentInventory.ObtainedDate;
            }
        }

        public void AddPetFragment(string petId, int amount, DateTime addedDateTime)
        {
            PetFragmentInventory data = Inventory.FirstOrDefault(x => x.PetId == petId);

            if(data == null)
            {
                data = new PetFragmentInventory(petId,amount, addedDateTime.ToString());
                Inventory.Add(data);
            }
            else
            {
                data.Owned += amount;
                data.ObtainedDate = addedDateTime.ToString();
            }
        }

        public void ReducePetFragment(string petId, int amount)
        {
            var data = Inventory.FirstOrDefault(x => x.PetId == petId);
            data.Owned -= amount;
            if(data.Owned <= 0)
            {
                Inventory.Remove(data);
            }
        }
    }
}