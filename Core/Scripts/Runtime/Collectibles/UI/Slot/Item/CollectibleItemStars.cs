using System.Collections.Generic;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot.Item
{
    public class CollectibleItemStars : MonoBehaviour, ICollectibleItemUIObjects
    {
        [SerializeField] private StarAssetData starPin;
        private List<Transform> _stars;
        
        public void SetData(CollectibleItem item, GridUIOptions uiOptions = null)
        {
            if (_stars == null)
            {
                _stars = new List<Transform>();
                foreach (Transform star in transform)
                {
                    _stars.Add(star);
                }
            }

            if (item == null)
            {
                DisplayStar(0, false);
                
                return;
            }
            
            DisplayStar(item.GetStarCount(), item.Amount > 0);
        }

        private void DisplayStar(int starCount, bool isStarAvailable = true)
        {
            for (int i = 0; i < _stars.Count; i++)
            {
                var starPrefab = isStarAvailable ? starPin.availableStar : starPin.unavailableStar;

                if (i < starCount)
                {
                    _stars[i].gameObject.SetActive(true);
                    _stars[i].GetComponent<Image>().sprite = starPrefab.GetComponent<Image>().sprite;
                }
                else
                {
                    _stars[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
