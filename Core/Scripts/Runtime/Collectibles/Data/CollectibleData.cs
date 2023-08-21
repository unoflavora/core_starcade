using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Data
{
    public class CollectibleData
    {
        public List<CollectibleSetData> Sets { get; set; }
    }

    public class CollectibleSetData
    {
        public List<CollectibleItem> Items { get; set; }
    }
    
}