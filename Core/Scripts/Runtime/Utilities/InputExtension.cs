using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Agate.Starcade.Scripts.Runtime.Utilities
{
    public static class InputExtension
    {
		public static List<RaycastResult> GetPointerOverUIObjects(this EventSystem eventSys)
		{
			PointerEventData eventDataCurrentPosition = new PointerEventData(eventSys)
			{
				position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
			};
			List<RaycastResult> results = new List<RaycastResult>();
			eventSys.RaycastAll(eventDataCurrentPosition, results);

			return results;
		}

		public static bool IsPointerOverUiObject(this EventSystem eventSys)
        {
            return GetPointerOverUIObjects(eventSys).Count > 0;
        }
    }
}
