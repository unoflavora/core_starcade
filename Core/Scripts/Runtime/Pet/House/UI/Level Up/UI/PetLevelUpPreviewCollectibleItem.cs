using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.House.UI.Level_Up.UI
{
    public class PetLevelUpPreviewCollectibleItem : MonoBehaviour
    {
        [SerializeField] private CollectibleSlot _collectible;

        [SerializeField] private Button _removeButton;
        // Start is called before the first frame update

        public CollectibleItem ItemData => _collectible.ItemData;

        public void SetupData(CollectibleItem data, UnityAction<CollectibleItem> onRemoveClicked = null)
        {
            _collectible.gameObject.SetActive(data != null);

            _removeButton.gameObject.SetActive(data != null);

            _removeButton.onClick.RemoveAllListeners();

            if (onRemoveClicked != null) _removeButton.onClick.AddListener(() => onRemoveClicked.Invoke(data));
            
            _collectible.SetupData(data);
        }
    }
}
