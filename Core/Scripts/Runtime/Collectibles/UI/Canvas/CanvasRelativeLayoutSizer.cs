using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agate
{
    [ExecuteAlways]
    [RequireComponent(typeof(LayoutElement))]
    public class CanvasRelativeLayoutSizer : MonoBehaviour, ILayoutElement
    {
        [Header("Min Height")] 
        [SerializeField] private bool minHeightActive = true;
        [SerializeField] private RectTransform relative;
        [SerializeField] private float percentage = 1;

        [Header("Preferred Height")]
        [SerializeField] private bool preferredHeightActive = true;
        [SerializeField] private RectTransform relativeHeightTransform;
        [SerializeField] private float percentageHeight = 1;

        #region properties
        public float minWidth { get; }
        public float preferredWidth { get; }
        public float flexibleWidth { get; }
        public float minHeight { get; }
        public float preferredHeight { get; }
        public float flexibleHeight { get; }
        public int layoutPriority { get; }
        #endregion
        
        private void OnGUI()
        {
            CalculateLayoutInputVertical();
        }

        public void CalculateLayoutInputHorizontal() { }

        public void CalculateLayoutInputVertical()
        {
            var layoutElement = GetComponent<LayoutElement>();

            if (minHeightActive)
            {
                if (relative == null) return;
                layoutElement.minHeight = percentage * relative.sizeDelta.y;
            }

            if (preferredHeightActive)
            {
                if (relativeHeightTransform == null) return;
                layoutElement.preferredHeight = percentageHeight * relativeHeightTransform.sizeDelta.y;
            }
        }

    }
}
