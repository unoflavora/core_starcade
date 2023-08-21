using Agate.Starcade.Runtime.Main;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Lobby.Lootbox.VFX.Animators
{
    public class PinVFXAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject _pinVFX;

        public void OnPinShowed()
        {
            MainSceneController.Instance.Audio.PlaySfx(GachaLootboxAudioKeys.SFX_LOOTBOX_BOX_RESULT);
        }
        public void ShowPinsVFX()
        {
            _pinVFX.SetActive(true);
        }

        public void HidePinsVFX()
        {
            _pinVFX.SetActive(false);
        }
    }
}