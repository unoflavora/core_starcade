using Agate.Starcade.Core.Runtime.Lobby.Lootbox.VFX;
using Agate.Starcade.Runtime.Main;
using UnityEngine;

namespace Starcade.Core.Runtime.Lobby.Lootbox.VFX
{
    public class PinsAnimationEventHandler : MonoBehaviour
    {
        public void OnPinDrop()
        {
            MainSceneController.Instance.Audio.PlaySfx(GachaLootboxAudioKeys.SFX_LOOTBOX_BOX_RESULT);
        }
    }
}