using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using System.Collections.Generic;
using System.Threading.Tasks;
using Starcade.Core.Runtime.ScriptableObject;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Agate.Starcade.Core.Runtime
{
    public class AssetLibrary : MonoBehaviour
    {
		[Header("Collections")]
		[SerializeField] private GeneralAssetCollection[] _spriteAssetCollections;
		[SerializeField] private GeneralAssetCollection _petAssetCollection;
        [SerializeField] private AssetReference _dummyPetAssetaReference;
		[HideInInspector] public bool isLoaded { get; set; } = false;

        [Header("Dummy")]
		[SerializeField] private Sprite _dummySprite;
		private PetAssets _dummyPetAssets;

		private Dictionary<string, Sprite> _spriteAssets;
        private Dictionary<string, PetAssets> _petAssets;

		public void Init()
        {
            _spriteAssets = new Dictionary<string, Sprite>();
			_petAssets = new Dictionary<string, PetAssets>();
        }

        public async Task LoadAllAssets()
        {
            if (isLoaded) return;
            
            for (int i = 0; i < _spriteAssetCollections.Length; i++)
            {
                GeneralAssetCollection asset = _spriteAssetCollections[i];
                
                await LoadAssets<Sprite>(asset);
            }

            await LoadAssets<PetAssets>(_petAssetCollection);

			_dummyPetAssets = await MainSceneController.Instance.AddressableController.LoadAsset<PetAssets>(_dummyPetAssetaReference);

			isLoaded = true;
        }

        private async Task LoadAssets<T>(GeneralAssetCollection collection)
		{
            for (int i = 0; i < collection.Collection.Length; i++)
			{
                var assetReference = collection.Collection[i];

                T asset = await MainSceneController.Instance.AddressableController.LoadAsset<T>(assetReference.Reference);

                switch (asset)
                {
                    case PetAssets pet:
                        if(_petAssets.ContainsKey(assetReference.Id)) continue;
                        _petAssets.Add(assetReference.Id, pet);
                        break;
                    case Sprite sprite:
                        if(_spriteAssets.ContainsKey(assetReference.Id)) continue;
                        _spriteAssets.Add(assetReference.Id, sprite);
                        break;
                }
            }
		}

		public Sprite GetSpriteAsset(string id)
        {
            if (id == null) return _dummySprite;
            return _spriteAssets.ContainsKey(id) ? _spriteAssets[id] : _dummySprite;
        }

		public PetAssets GetPetObject(string id)
		{
			if (id == null) return _dummyPetAssets;
			return _petAssets.ContainsKey(id) ? _petAssets[id] : _dummyPetAssets;
		}

		public Sprite GetSpriteRewardAsset(RewardBase rewardObjectData)
        {
            if (rewardObjectData == null) return null;

            string iconId;
            switch (rewardObjectData.Type)
            {
                case RewardEnum.GoldCoin:
                    iconId = rewardObjectData.Type.ToString();
                    break;
                case RewardEnum.StarCoin:
                    iconId = rewardObjectData.Type.ToString();
                    break;
                case RewardEnum.StarTicket:
                    iconId = rewardObjectData.Type.ToString();
                    break;
                case RewardEnum.Collectible:
                    iconId = CollectibleItem.FindCollectibleItemById(rewardObjectData.Ref.ToString()).CollectibleItemId;
                    break;
                case RewardEnum.Pet:
                    return GetPetObject(rewardObjectData.Ref).PetSpriteAsset;
                case RewardEnum.PetFragment:
					return GetPetObject(rewardObjectData.Ref).FragmentSpriteAsset;
                case RewardEnum.SpecialBox:
                    iconId = rewardObjectData.Ref.ToString();
                    break;
                case RewardEnum.PetBox:
                    iconId = rewardObjectData.Ref.ToString();
                    break;
                default:
                    iconId = rewardObjectData.Ref;
                    break;
            }

            return _spriteAssets.ContainsKey(iconId) ? _spriteAssets[iconId] : null;
        }
    }
}
