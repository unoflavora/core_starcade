using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Utilities
{
    public static class UIExtension
    {
		public static bool IsInteractableAndEnabled(this GameObject gameobject)
		{
			if (gameobject.GetComponent<Button>() != null)
			{
				return gameobject.GetComponent<Button>().isActiveAndEnabled;
			}else if (gameobject.GetComponent<Toggle>() != null)
			{
				return gameobject.GetComponent<Toggle>().isActiveAndEnabled;
			}
			else if (gameobject.GetComponent<ToggleButton>() != null)
			{
				return gameobject.GetComponent<ToggleButton>().isActiveAndEnabled;
			}

			return false;
		}

		public static bool IsToggleAndOn(this GameObject gameobject)
		{
			if (gameobject.GetComponent<Toggle>() != null)
			{
				return gameobject.GetComponent<Toggle>().isOn;
			}

			return false;
		}
	}
}
