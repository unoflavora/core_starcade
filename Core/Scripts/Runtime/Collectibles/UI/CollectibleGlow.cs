using System.Collections.Generic;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot.Item;
using UnityEngine;

namespace Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI
{
    
    public class CollectibleGlow : MonoBehaviour, ICollectibleItemUIObjects
    {
        [SerializeField] List<GameObject> _tiersVFX;

        public void SetData(CollectibleItem item, GridUIOptions uiOptions = null)
        {
            foreach (var tierVfx in _tiersVFX)
            {
                tierVfx.SetActive(false);
            }
            
            if (item.Amount <= 0) return;
            
            _tiersVFX[item.Rarity - 1].SetActive(true);
        }
    }
}
