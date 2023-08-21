using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Runtime.Main;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid
{
    /// <summary>
    /// This script is used to dynamically populate a grid UI with a list of items.
    /// </summary>
    public class PoolableGridUI : MonoBehaviour
    {
        // Transform must implement IPoolableGridItem
        [FormerlySerializedAs("collectibleSlotPrefab")] [SerializeField] private Transform _poolableItemPrefab;
        
        private List<Transform> _uiObjects;
        public List<Transform> ItemObjects => _uiObjects;

        /// <summary>
        ///  This public method is used to draw a list of items on the UI.
        /// </summary>
        /// <param name="items">List of item that is going to be inserted into each grid object</param>
        /// <param name="onItemClicked">Action that will be invoked if item is clieked on UI</param>
        /// <param name="uiOptions">Options for the displayed item</param>
        /// <param name="playAudioOnClick"> does clicking the item play the default sfx?</param>
        /// <typeparam name="T"> Type of item that is drawn</typeparam>
        public void Draw <T> (List<T> items, UnityAction<Transform> onItemClicked = null, GridUIOptions uiOptions = null, bool playAudioOnClick = true)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var itemObject = GetPooledItem<T>(i);

                var itemUI = itemObject.GetComponent<IPoolableGridItem<T>>();
                
                itemUI.SetupData(items[i], i, uiOptions);
                
                if (onItemClicked != null)
                {
                    itemUI.OnItemClicked = t =>
                    {
                        if (playAudioOnClick) MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.BUTTON_GENERAL);
                        onItemClicked.Invoke(itemObject);
                    };
                }
            }
            
            HideExceedingItem(items);
        }

        private void HideExceedingItem<T>(List<T> items)
        {
            if(_uiObjects == null) return;
            
            if (items.Count >= _uiObjects.Count) return;
            
            int deficit = _uiObjects.Count - items.Count;
        
            for (int i = 0; i < deficit; i++)
            {
                _uiObjects[_uiObjects.Count - 1 - i].gameObject.SetActive(false);
            }
        }

        private Transform GetPooledItem<T>(int i)
        {
            if (_uiObjects == null) _uiObjects = new List<Transform>();
            
            foreach (Transform child in transform)
            {
                if (!_uiObjects.Contains(child) && child.GetComponent<IPoolableGridItem<T>>() != null)
                {
                    _uiObjects.Add(child);
                }
            }
            
            int currentPoolCount = _uiObjects.Count;
            
            Transform itemObject;

            if (i < currentPoolCount)
            {
                itemObject = _uiObjects[i];
                itemObject.gameObject.SetActive(true);
            }
            else
            {
                itemObject = Instantiate(_poolableItemPrefab, transform);
                itemObject.gameObject.SetActive(true);
                _uiObjects.Add(itemObject);
            }

            return itemObject;
        }
    }
}
