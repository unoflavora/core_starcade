using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Agate.Starcade
{
    public class ScrollDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public bool isDrag { get; private set; }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDrag = false;
        }
    }
}
