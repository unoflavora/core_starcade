using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot.Item
{
    public interface ICollectibleItemUIObjects
    {
        public void SetData(CollectibleItem item, GridUIOptions uiOptions = null);
    }
}