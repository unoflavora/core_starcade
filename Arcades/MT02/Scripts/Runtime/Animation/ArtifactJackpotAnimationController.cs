using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Animation
{
    public class ArtifactJackpotAnimationController : MonoBehaviour
    {
        [FormerlySerializedAs("_isAnimationFinish")] public bool IsAnimationFinish;

        public void StartAnimation()
        {
            // IsAnimationFinish = false;
        }
        
        public void FinishOpenAnimation()
        {
            // IsAnimationFinish = true;
            // gameObject.SetActive(false);
        }
    }
}
