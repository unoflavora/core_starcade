using System;
using System.Collections.Generic;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles;
using Newtonsoft.Json;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set
{
    public class CollectibleItem
    {
        public int Amount { get; set; }  = 0;
        public string CollectibleItemId { get; set; } = null;
        public string CollectibleItemName { get; set; }
        public int Rarity { get; set; }
        
        private readonly Sprite _icon = null;

        public CollectibleItem() {}

        public CollectibleItem(CollectibleItem item)
        {
            Amount = item.Amount;
            CollectibleItemId = item.CollectibleItemId;
            CollectibleItemName = item.CollectibleItemName;
            Rarity = item.Rarity;
        }
        
        public string GetItemId()
        {
            return CollectibleItemId;
        }

        public string GetDisplayName()
        {
            return CollectibleItemName;
        }
        
        public int GetStarCount()
        {
            return Rarity;
        }

        public Sprite GetIcon()
        {
            return _icon;
        }
        
        public bool IsItemNew()
        {
            var itemAmount = FindCollectibleItemById(CollectibleItemId).Amount;
            return  itemAmount == 0;
        }
        
        public static CollectibleItem FindCollectibleItemById(string itemId)
        {
            var data = MainSceneController.Instance.Data.CollectiblesData;
        
            if (data == null) return null;

            foreach (CollectibleSetData set in data)
            {
                foreach (CollectibleItem item in set.CollectibleItems)
                {
                    if (item.CollectibleItemId == itemId)
                    {
                        return item;
                    }
                }
            }
            
            return null;
        }
    }
}
