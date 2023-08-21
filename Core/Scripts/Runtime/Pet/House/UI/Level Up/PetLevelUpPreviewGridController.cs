using System.Collections.Generic;
using System.Linq;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.House.UI.Level_Up.UI;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.Pet.House.UI.Level_Up
{
    public class PetLevelUpPreviewGridController : MonoBehaviour
    {
        [SerializeField] private Transform _grid;
        [SerializeField] private PetLevelUpProgressView _progress;
        private List<PetLevelUpPreviewCollectibleItem> _previews;
        private PetLevelUpConfig _levelUpConfig;
        public List<CollectibleItem> SelectedItems => (from preview in _previews where preview.ItemData != null select preview.ItemData).ToList();
        
        // Start is called before the first frame update
        void Start()
        {
            _previews = new List<PetLevelUpPreviewCollectibleItem>();

            foreach (Transform slot in _grid)
            {
                _previews.Add(slot.GetComponent<PetLevelUpPreviewCollectibleItem>());
            }
        }

        public void SetProgressConfig(PetExperienceData petExperienceData, PetLevelUpConfig levelUpConfig)
        {
            _levelUpConfig = levelUpConfig;
            _progress.SetPetCurrentProgress(petExperienceData, levelUpConfig);
        }

        public bool AddItem(CollectibleItem data, UnityAction<CollectibleItem> onItemClicked)
        {
            foreach (var slot in _previews)
            {
                if (slot.ItemData == null)
                {
                    slot.SetupData(data, (collectibleItem) =>
                    {
                        slot.SetupData(null);
                        SortByNull();
                        onItemClicked(data);
                    });
                    
                    return true;
                }
            }
            return false;
        }

        public int UpdateProgress()
        {
            return _progress.SetPetProgress(SelectedItems);
        }

        private void SortByNull()
        {
            foreach (var slot in _previews)
            {
                slot.transform.parent = null;
            }
            
            _previews.Sort(CompareSlot);

            foreach (var slot in _previews)
            {
                slot.transform.parent = _grid;
            }
        }

        private int CompareSlot(PetLevelUpPreviewCollectibleItem x, PetLevelUpPreviewCollectibleItem y)
        {
            if (x.ItemData == null) return 1;
            
            if (y.ItemData == null) return -1;
            
            return 0;
        }

        public void Clear()
        {
            if (_previews == null) return;
            
            foreach (var slot in _previews)
            {
                slot.SetupData(null);
            }

            UpdateProgress();
        }
    }
}
