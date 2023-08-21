using System.Collections.Generic;

namespace Agate.Starcade.Scripts.Runtime.Data_Class.Lootbox
{
    public class RarityConfig
    {
        public string RarityId { get; set; }
        public List<RarityConfiguration> Configuration { get; set; }
        public int Amount { get; set; }
    }
}