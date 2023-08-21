using Agate.Starcade.Runtime.Main;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Lobby.Lootbox.VFX.Animators
{
    public class LootboxChestVfxAnimator : MonoBehaviour
    {
        public void OnChestDrop()
        {
            MainSceneController.Instance.Audio.PlaySfx(GachaLootboxAudioKeys.SFX_LOOTBOX_BOX_DROP);
        }

        public void OnChestOpen()
        {
            MainSceneController.Instance.Audio.PlaySfx(GachaLootboxAudioKeys.SFX_LOOTBOX_OPEN);
        }

        public void OnTopDrop()
        {
            MainSceneController.Instance.Audio.PlaySfx(GachaLootboxAudioKeys.SFX_LOOTBOX_BOX_DROP);
        }
        
    }
}