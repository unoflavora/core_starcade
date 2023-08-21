using Agate.Starcade.Runtime.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade
{
    [Serializable]
    public class AccessoryLibrary
    {
        public ItemTypeEnum Type;
        public string[] DefaultItemIds;
        [HideInInspector]public List<string> UnlockedItems;
    }
}
