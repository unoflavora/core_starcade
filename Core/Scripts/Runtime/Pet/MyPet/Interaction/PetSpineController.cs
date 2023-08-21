using Agate.Starcade.Scripts.Runtime.Utilities;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.Interaction
{
    public class PetSpineController : MonoBehaviour
    {
        [SerializeField] Spine.Unity.SkeletonGraphic _skeletonAnimation;
        public SkeletonDataAsset SpineAsset => _skeletonAnimation.skeletonDataAsset;
        
        // If user is not clicking the pet, play "idle" animation on spine
        public void OnPetIdle()
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
        }
        
        public async void OnPetInteract()
        {
            TrackEntry track = _skeletonAnimation.AnimationState.SetAnimation(0, "Interact", false);
           
            // await for track to finish
            await AsyncUtility.WaitUntil(() => track.IsComplete);
            
            // then play idle animation
            OnPetIdle();
        }

        public void SetPetAsset(SkeletonDataAsset asset)
        {
            this.gameObject.SetActive(false);
            _skeletonAnimation.Clear();
            _skeletonAnimation.skeletonDataAsset = asset;
			OnPetIdle();
            this.gameObject.SetActive(true);
		}
    }
}
