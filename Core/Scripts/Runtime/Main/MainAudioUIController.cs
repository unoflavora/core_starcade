using UnityEngine;
using static Agate.Starcade.Runtime.Main.MainSceneController;
using UnityEngine.EventSystems;
using Agate.Starcade.Scripts.Runtime.Utilities;
using UnityEngine.UI;
using Agate.Starcade.Runtime.Audio;

namespace Agate.Starcade.Runtime.Main
{
    public class MainAudioUIController : MonoBehaviour
    {
		[SerializeField] private AudioController _audioController;

		private void Update()
        {
			//Play Generic Button Audio
			if (Input.GetMouseButtonDown(0))
			{
				var result = EventSystem.current.GetPointerOverUIObjects();
				if (result != null && result.Count >= 1)
				{
					var gameobject = result[0].gameObject;
					var tag = gameobject.tag;

					if (result.Count >= 2 && gameobject.GetComponent<Text>() != null)
					{
						gameobject = result[1].gameObject;
						tag = gameobject.tag;
					}

					Debug.Log($"PLAY AUDIO: {gameobject.name} - {tag}");
					switch (tag)
					{
						case "ButtonDefault":
							_audioController.PlaySfx(AUDIO_KEY.BUTTON_GENERAL);
							//if (gameobject.IsInteractableAndEnabled())
							//{
							//} 
							break;
						case "ButtonOpen":
							_audioController.PlaySfx(AUDIO_KEY.BUTTON_OPEN);
							break;
						case "ButtonClose":
							_audioController.PlaySfx(AUDIO_KEY.BUTTON_CLOSE);
							break;
						case "ButtonNegative":
							_audioController.PlaySfx(AUDIO_KEY.BUTTON_NEGATIVE);
							break;
						case "ButtonTab":
							//if (gameObject.IsToggleAndOn()) return;

							if (gameobject.IsInteractableAndEnabled())
							{
								_audioController.PlaySfx(AUDIO_KEY.BUTTON_TAB);
							}
							else
							{
								_audioController.PlaySfx(AUDIO_KEY.BUTTON_UNAVAILABLE);
							}
							break;
						case "ButtonPlay":
							_audioController.PlaySfx(AUDIO_KEY.BUTTON_PLAY);
							break;
						case "ButtonUnavailable":
							_audioController.PlaySfx(AUDIO_KEY.BUTTON_UNAVAILABLE);
							break;
					}
				}

			}
		}
    }
}
