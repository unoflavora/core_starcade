using System.Collections.Generic;
using Agate.Starcade.Runtime.Main;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.Core.Model
{
    public class PetBaseData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public BasicSkill BasicSkill { get; set; }
        public Sprite GetImage(bool isSilhouette = false)
        {
            var petObject = MainSceneController.Instance.AssetLibrary.GetPetObject(Id);

            if (petObject == null) return null;
            
            return isSilhouette ? petObject.SilhouetteSpriteAsset : petObject.PetSpriteAsset;
        }
    }
    
    public class PetInventoryData : PetBaseData
    {
        public string UniqueId { get; set; }
        public bool IsActive { get; set; }
        public PetExperienceData ExperienceData { get; set; }
        public string ObtainedDate { get; set; }
        public List<SubSkill> SubSkills { get; set; }
    }
    
    public class PetAlbumData : PetBaseData
    {
        public bool HasOwned;
    }
    
}