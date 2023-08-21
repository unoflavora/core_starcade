using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Main;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.Core.Model
{
    public class PetBackendData
    {
        public List<PetAlbumData> Pets { get; set; }
        public ActivePet ActivePet { get; set; }
        public List<PetInventoryData> PetInventory { get; set; }
        public List<PetFragmentInventory> PetFragmentInventory { get; set; }
        public PetConfigs PetConfigs { get; set; }
        public PetAdventureData AdventureData { get; set; }
    }
    
    public class ActivePet
    {
        public string UniqueId { get; set; }
        public PetAdventureData AdventureData { get; set; }
    }

    public class AdventureConfig
    {
        public string Id { get; set; }
        public int Tier { get; set; }
        public int Time { get; set; }
        public List<RewardBase> Rewards { get; set; }
    }

    public class PetAdventureData
    {
        public bool IsDispatched { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        
        public string AdventureSessionId { get; set; }
        public List<RewardBase> Rewards { get; set; }
    }

    public class BasicSkill
    {
        public string Type { get; set; }
        public float Amount { get; set; }
    }
    public class PetExperienceData
    {
        public int CurrentExp { get; set; }
        public int Level { get; set; }
        public int NextExp { get; set; }
        public int BottomExp { get; set; }
    }

    public class ExpGainConfig
    {
        public int Rarity { get; set; }
        public int ExpAmount { get; set; }
    }

    public class FragmentConfig
    {
        public int MaxPetCombined { get; set; }
        public int RequiredAmount { get; set; }
    }

    public class SwitchData
    {
        public string UniqueId;
        public PetAdventureData AdventureData;
    }

    public class PetLevelUpFormula
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class PetLevelUpConfig
    {
        public int MaxLevel { get; set; }
        public List<PetExpGainFormula> ExpGainFormulas { get; set; }
        public PetLevelUpFormula LevelUpFormula { get; set; }
    }

    public class PetExpGainFormula
    {
        public int CollectibleRarity;
        public int ExpAmount;
    }

    public class PetConfigs
    {
        public List<ExpGainConfig> expGainConfig { get; set; }
        public PetLevelUpConfig LevelUpConfig { get; set; }
        public List<AdventureConfig> AdventureConfigs { get; set; }
        public FragmentConfig FragmentConfig { get; set; }
    }

    [Serializable]
    public class PetFragmentInventory
    {
        public string PetId;
        public int Owned;
        public string ObtainedDate;
        public int RequirementAmount => MainSceneController.Instance.Data.PetConfigs.FragmentConfig.RequiredAmount;
        public DateTime ObtainedDateTime { get => DateTime.Parse(ObtainedDate); set => DateTime.Parse(ObtainedDate); }
        public string PetName => MainSceneController.Instance.Data.PetAlbum.Find(pet => pet.Id == PetId)?.Name;

        public PetFragmentInventory(string petId, int owned, string obtainedDate)
        {
            PetId = petId;
            Owned = owned;
            ObtainedDate = obtainedDate;
        }
    }

    public class PetFragmentRefObject
    {
        public string PetName;
    }

    public class PetInventory
    {
        public string Id { get; set; }
        public string UniqueId { get; set; }
        public PetExperienceData ExperienceData { get; set; }
        public string obtainedDate { get; set; }
        public BasicSkill basicSkill { get; set; }
        public List<SubSkill> subSkills { get; set; }
    }

    public class PetRewardData
    {
        public string type { get; set; }
        public string Ref { get; set; }
        public int amount { get; set; }
    }

    public class SubSkill
    {
        public string Id { get; set; }
        public double Value { get; set; }
        public int Tier { get; set; }
        public int Level { get; set; }
    }
    
    public class CombinePetFragmentRequest
    {
        public string petId { get; set; }
        public int amount { get; set; }

        public CombinePetFragmentRequest(string petId, int amount)
        {
            this.petId = petId;
            this.amount = amount;
        }

    }

    public class PetFragmentAddRequest
    {
        public string petId { get; set; }
        public int amount { get; set; }

        public PetFragmentAddRequest(string petId, int amount)
        {
            this.petId = petId;
            this.amount = amount;
        }
       
    }

    public class PetDispatchData
    {
        public string PetId { get; set; }
        public string UniqueId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class PetCancelAdventureData
    {
        public string UniqueId { get; set; }
        public PetAdventureData AdventureData { get; set; }
    }

    public class PetLevelUpData
    {
        public int ExpGain { get; set; }
        public int LevelGain { get; set; }
        public PetInventoryData Pet { get; set; }

    }

    public class PetLevelUpBody
    {
        public string PetUniqueId;
        
        public List<string> CollectibleItems;
    }

    public class GiftPetData
    {
        public long FriendCode;
        public string PetUniqueId;
    }
}