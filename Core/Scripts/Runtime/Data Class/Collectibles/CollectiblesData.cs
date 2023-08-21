using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;

namespace Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles
{
    public class CollectibleSetData
    {
        public string CollectibleSetId { get; set; }
        public string CollectibleSetName { get; set; }
        
        public bool IsComingSoon { get; set; }
        public CollectibleRewardData Reward { get; set; }
        public List<CollectibleItem> CollectibleItems { get; set; }
    }
}