using Spine.Unity;
using UnityEngine;

namespace Starcade.Core.Runtime.ScriptableObject
{
    [CreateAssetMenu(menuName = "Asset Library/Add New Pet Assets")]
    public class PetAssets : UnityEngine.ScriptableObject
    {
        [SerializeField] private Sprite _petSpriteAsset;
        [SerializeField] private Sprite _silhouetteSpriteAsset;
        [SerializeField] private Sprite _fragmentSpriteAsset;
        [SerializeField] private SkeletonDataAsset _skeletonDataAsset;

        public Sprite PetSpriteAsset => _petSpriteAsset;
        public Sprite SilhouetteSpriteAsset => _silhouetteSpriteAsset;
        public Sprite FragmentSpriteAsset => _fragmentSpriteAsset;
        public SkeletonDataAsset SkeletonDataAsset => _skeletonDataAsset;
    }
    
}
