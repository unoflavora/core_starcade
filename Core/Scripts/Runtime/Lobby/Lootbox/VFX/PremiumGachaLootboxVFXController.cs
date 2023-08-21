using System;
using System.Collections.Generic;
using System.Linq;
using Agate.Starcade.Core.Runtime.Lobby.Lootbox.VFX;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;
using Starcade.Core.Runtime.Lobby.Script.Lootbox.VFX;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Starcade.Core.Runtime.Lobby.Lootbox.VFX
{
    [Serializable]
    public struct LootboxType
    {
        public LootboxRarityEnum type;
        public GameObject prefab;
    }
    
    public class PremiumGachaLootboxVFXController : MonoBehaviour
    {
        [Header("Timeline Modules")]
        [SerializeField] private ParticleSystem _hexagonRay;
        [SerializeField] private ParticleSystem _boxRay;
        [SerializeField] private PlayableDirector _director;
        [SerializeField] private PlayableDirector _postInputDirector;
        [SerializeField] private GameObject _pleaseTapInfo;
        [SerializeField] private Camera _camera;
        [SerializeField] private Button _skipButton;
        
        [Header("Lootbox Config")]
        [SerializeField] private LootboxVFXController _lootbox;
        [SerializeField] private LootboxRarityEnum _currentType;
        [SerializeField] private List<LootboxType> _types;
        [SerializeField] private List<CollectibleSlot> _collectiblePins;
        
        public bool _waitForInput;
        public Camera SceneCamera => _camera;
        private Action _onFinished;

        public void PlayVFX(GachaVFXData data, Action onFinished)
        {
            MainSceneController.Instance.Audio.PlayBgm(GachaLootboxAudioKeys.BGM_LOOTBOX);
            
            _pleaseTapInfo.SetActive(false);
            
            _currentType = data.LootboxType;
            
            _onFinished = onFinished;
            
            var output = _director.playableAsset.outputs.First(i => i.streamName == "Sfx");
            _director.SetGenericBinding(output.sourceObject, MainSceneController.Instance.Audio.AudioSourceSfx);

            output = _postInputDirector.playableAsset.outputs.First(i => i.streamName == "Sfx");
            _postInputDirector.SetGenericBinding(output.sourceObject, MainSceneController.Instance.Audio.AudioSourceSfx);

            InitPins(data.CollectibleItems);
            
            _types.ForEach(i => {
                i.prefab.SetActive(i.type == _currentType);
            });

            _director.enabled = true;
            _director.Play();
        }

        private void Awake()
        {
            _director.enabled = false;
            _postInputDirector.enabled = false;
            _waitForInput = false;
            _skipButton.gameObject.SetActive(MainSceneController.Instance.EnvironmentConfig.IsSkipLootboxEnabled);
            _skipButton.onClick.AddListener(Skip);

            _types.ForEach(i => {
                i.prefab.SetActive(i.type == _currentType);
             });
        }

        private void Update()
        {
            if (!isActiveAndEnabled) return;
            
            if (_waitForInput && Input.GetMouseButtonDown(0))
            {
                GoToNextSequence();
            }
        }

        public void WaitForInput()
        {
            _hexagonRay.Play();
            _boxRay.Play();
            _waitForInput = true;
            _pleaseTapInfo.SetActive(true);
        }

        public void GoToNextSequence()
        {

            _waitForInput = false;
            
            _pleaseTapInfo.SetActive(false);

            _director.enabled = false;
            _director.Stop();
            
            _postInputDirector.enabled = true;
            _postInputDirector.Play();
        }

        public void OnFinishedVFX()
        {
            _onFinished.Invoke();
        }

        public void Skip()
        {
            GoToNextSequence();
            
            _skipButton.interactable = false;
            
            _postInputDirector.time = 5.8f;
        }
        
        private void InitPins(List<CollectibleItem> items)
        {
            for (int i = 0; i < _collectiblePins.Count; i++)
            {
                if (i < items.Count)
                {
                    _collectiblePins[i].SetupData(items[i], i, new GridUIOptions()
                    {
                        ShowItemCount = false,
                        ShowQuestionMark = false,
                        ShowItemName = false
                    });
                    _collectiblePins[i].gameObject.SetActive(true);
                }
                else
                {
                    _collectiblePins[i].gameObject.SetActive(false);
                }
            }
        }

        private void OnDisable()
        {
            MainSceneController.Instance.Audio.PlayBgm("bgm_lobby");
        }
    }
}