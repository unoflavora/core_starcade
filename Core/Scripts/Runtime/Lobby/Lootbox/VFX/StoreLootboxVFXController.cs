using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Lobby.Lootbox.VFX;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;
using Starcade.Core.Runtime.Lobby.Lootbox.VFX;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Starcade.Core.Runtime.Lobby.Script.Lootbox.VFX
{
    public enum GachaType
    {
        Regular, Premium
    }
    public class GachaVFXData
    {
        public Camera Camera;
        public LootboxRarityEnum LootboxType;
        public List<CollectibleItem> CollectibleItems;
        public GachaType GachaType;
    }
    
    public class StoreLootboxVFXController : MonoBehaviour
    {
        [SerializeField] private PremiumGachaLootboxVFXController _premium;
        [SerializeField] private GachaLootboxVFXController _regular;
        [SerializeField] private GachaType _type;
        private InitAdditiveBaseData _lobbyData;

        private GachaVFXData _data;
        private Camera _mainCamera;

        async void Start()
        {
            _data = SetupDummyData();
            
            await MainSceneController.Instance.Audio.LoadAudioData(key: "gacha_audio");

            GetLobbyData();
            
            _regular.gameObject.SetActive(_type == GachaType.Regular);
            
            _premium.gameObject.SetActive(_type == GachaType.Premium);

            if (_type == GachaType.Regular)
            {
                _regular.PlayVFX(_data, () => _lobbyData?.OnClose());
            }
            else
            {
                _premium.PlayVFX(_data, () => _lobbyData?.OnClose());
            }
            
            MainSceneController.Instance.Loading.DoneLoading();
        }

        private void GetLobbyData()
        {
            _lobbyData = LoadSceneHelper.PullData();
            
            if (_lobbyData == null) return;
            
            _data = (GachaVFXData) _lobbyData.Data;

            _type = _data.GachaType;

            _regular.InitPins(_data.CollectibleItems);

            _data.Camera.GetUniversalAdditionalCameraData().cameraStack
                .Add(_type == GachaType.Premium ? _premium.SceneCamera : _regular.SceneCamera);
        }

        private GachaVFXData SetupDummyData()
        {
            return new GachaVFXData() {
                Camera = null,
                LootboxType = LootboxRarityEnum.Bronze,
                CollectibleItems = new List<CollectibleItem>()
                {
                    new()
                    {
                        CollectibleItemId = "CLB_TheCarnivalSet_04", CollectibleItemName = "From Me, To You",
                        Amount = 1, Rarity = 2
                    },
                    new()
                    {
                        CollectibleItemId = "CLB_TheCarnivalSet_04", CollectibleItemName = "From Me, To You",
                        Amount = 1, Rarity = 2
                    },
                    new()
                    {
                        CollectibleItemId = "CLB_TheCarnivalSet_04", CollectibleItemName = "From Me, To You",
                        Amount = 1, Rarity = 2
                    },
                    new()
                    {
                        CollectibleItemId = "CLB_TheCarnivalSet_04", CollectibleItemName = "From Me, To You",
                        Amount = 1, Rarity = 2
                    },
                },
                GachaType = _type
            };
        }
    }
}
