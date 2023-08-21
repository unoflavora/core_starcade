using System;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.UI.Canvas
{
    [ExecuteAlways]
    public class CanvasGridItemFlexibleSizer : MonoBehaviour,ILayoutSelfController
    {
        [SerializeField] private GridLayoutGroup _layoutGroup;
        [SerializeField] private RectTransform _relativeToRect;
        public float height;
        
        private void OnGUI()
        {
           SetLayoutVertical();
        }

        public void SetLayoutHorizontal() {}

        public void SetLayoutVertical()
        {
            height = _relativeToRect.rect.height;
            
            Vector2 newSize = new Vector2(height, height);
            
            _layoutGroup.cellSize = newSize;
        }
    }
}
