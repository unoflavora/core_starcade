using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using TMPro;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot.Item
{
    public class CollectibleItemCount : MonoBehaviour, ICollectibleItemUIObjects
    {
        [SerializeField] private TextMeshProUGUI count;
        [SerializeField] private GameObject countImage;
        [SerializeField] private GameObject newLabel;

        private CollectibleItem _itemData;
        
        public void SetData(CollectibleItem item, GridUIOptions uiOptions = null)
        {
            if (item == null)
            {
                countImage.SetActive(false);
                return;
            };

            _itemData = item;
            
            if (_itemData.Amount > 1)
            {
                countImage.SetActive(true);
                string displayedText = _itemData.Amount < 99 ? _itemData.Amount.ToString() : "99";
                count.text = $"x{displayedText}";
            }
            else
            {
                countImage.SetActive(false);
            }
            
            DisplayUiOptions(uiOptions);
        }

        private void DisplayUiOptions(GridUIOptions uiOptions)
        {
            if (uiOptions == null) return;

            countImage.SetActive(uiOptions.ShowItemCount && _itemData.Amount > 1);

            count.gameObject.SetActive(uiOptions.ShowItemCount  && _itemData.Amount > 1);
            
            newLabel.SetActive (uiOptions.ShowNewLabel &&  _itemData.IsItemNew() );

        }
    }
}
