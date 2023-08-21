using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class CustomScrollRect : ScrollRect
    {
        public void CustomDragSetVerticalNormalizedPosition(float value)
        {
            float anchoredYBeforeSet = content.anchoredPosition.y;
            SetNormalizedPosition(value, 1);
            m_ContentStartPosition += new Vector2(0f, content.anchoredPosition.y - anchoredYBeforeSet);
        }

        //public void CustomSetVerticalNormalizedPosition(float value)
        //{
        //    if (m_Dragging)
        //    {
        //        float anchoredYBeforeSet = content.anchoredPosition.y;
        //        SetNormalizedPosition(value, 1);
        //        m_ContentStartPosition += new Vector2(0f, content.anchoredPosition.y - anchoredYBeforeSet);
        //    }
        //    else
        //        SetNormalizedPosition(value, 1);
        //}
    }
}
