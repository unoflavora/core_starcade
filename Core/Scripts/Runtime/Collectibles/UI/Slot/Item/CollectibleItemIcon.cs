using System;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot.Item
{
    /// <summary>
    /// To be put on the icon representing an inventory item. Allows the slot to
    /// update the icon and number.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class CollectibleItemIcon : MonoBehaviour, ICollectibleItemUIObjects
    {
        [SerializeField] private Sprite _questionMark;
        [SerializeField] private Sprite _defaultSprite;

        public void SetData(CollectibleItem item, GridUIOptions uiOptions = null)
        {
            Image icon = GetComponent<Image>();

            if (item == null)
            {
                icon.enabled = false;
                
                return;
            };

            icon.enabled = true;

            if (uiOptions is { ShowQuestionMark: true })
            {
                icon.sprite = _questionMark;
                return;
            }
            
            if (item.Amount > 0)
            {
                var sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(item.GetItemId());
                
				icon.sprite = sprite != null ? sprite : _defaultSprite;
                
                if (sprite == null) Debug.LogError("Could not find Sprite for :" + item.GetItemId());
            }
            else
            {
                icon.sprite = _questionMark;
            }
        }
    }
}
