using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Runtime.Main;

namespace Agate.Starcade.Core.Runtime.Pet.Core.Model
{
    public static class PetInventoryExtensions
    {
        public static void AddPet(this List<PetInventoryData> petInventory, PetInventoryData petData)
        {
            petInventory.Add(petData);
            
            
            MainSceneController.Instance.Data.PetAlbum.Find(pet => pet.Id == petData.Id).HasOwned = true;
        }
        
        public static void AddPet(this List<PetInventoryData> petInventory, List<PetInventoryData> pets)
        {
            foreach (var petData in pets)
            {
                petInventory.AddPet(petData);
            }
        }

        public static bool IsNewPet(string petId)
        {
            return !MainSceneController.Instance.Data.PetAlbum.Find(pet => pet.Id == petId).HasOwned;
        }
    }

}