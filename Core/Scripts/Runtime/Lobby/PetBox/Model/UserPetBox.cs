using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;
using System;
using System.Collections;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Lobby.PetBox.Model
{
    [Serializable]
    public class UserPetBox
    {
        public string ItemId;
        public double Cost;
        public CurrencyTypeEnum CostCurrency;
        public StoreCategoryTypeEnum StoreCategory;
        public LootboxCategoryEnum LootboxCategory;
        public int Interval;
        public string Type;
    }
}