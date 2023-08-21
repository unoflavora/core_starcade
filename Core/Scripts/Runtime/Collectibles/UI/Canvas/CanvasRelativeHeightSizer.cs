using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.UI.Canvas
{
    
    [ExecuteAlways]
    public class CanvasRelativeHeightSizer : MonoBehaviour, ILayoutSelfController
    {
        [SerializeField] private float heightPercentage;
        [SerializeField] private RectTransform reference;

        private void OnGUI()
        {
            SetLayoutVertical();
        }

        public void SetLayoutHorizontal() {}

        public void SetLayoutVertical()
        {
            if (reference == null) return;
            
            var parentSize = reference.sizeDelta;
            var currentTransform = GetComponent<RectTransform>();
            currentTransform.sizeDelta = new Vector2(currentTransform.sizeDelta.x, heightPercentage * Mathf.Abs(parentSize.y));
        }
        
    }
}
