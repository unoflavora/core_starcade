using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Core.Runtime.Lobby.Lootbox.VFX.Animators;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;
using Agate.Starcade.Core.Runtime.Lobby.Lootbox;
using Agate.Starcade.Core.Runtime.Lobby.Lootbox.Animators;
using Starcade.Core.Runtime.Lobby.Script.Lootbox.VFX;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Lobby.Lootbox.VFX
{

    public class GachaLootboxVFXController : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private Camera _camera;
        
        [Header("VFX Objects")]
        [SerializeField] private GameObject _sceneWorld;
        [SerializeField] private StarIconAnimator _starIcon;
        [SerializeField] private FadeTween _fade;
        [SerializeField] private PinVFXAnimator _pinsVFX;
        [SerializeField] private List<CollectibleSlot> _collectiblePins;
        [SerializeField] private List<LootboxObject> _activeLootbox;
        [SerializeField] private TextMeshProUGUI _tapText;
        [SerializeField] private Button _tapButton;
        [SerializeField] private Button _skipButton;
        
        [Header("Animators")] 
        [SerializeField] private Animator _pins;
        [SerializeField] private Animator _cameraAnimator;
        [SerializeField] private Animator _tap;

        public Camera SceneCamera => _camera;
        #region Data
        private GachaVFXData _data;
        [SerializeField] private Animator _chest;
        private AudioController _audioController;
        private bool _isSkip;
        private Action _onFinished;
        
        #endregion
        
        #region Animation Events
        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int Appear = Animator.StringToHash("Appear");
        private InitAdditiveBaseData _lobbyData;

        #endregion
        private void Start()
        {
            _skipButton.onClick.AddListener(SkipAnimation);
        }

        public void PlayVFX(GachaVFXData data, Action onFinished)
        {
            _data = data;
            _onFinished = onFinished;
            
            InitPins(_data.CollectibleItems);
            
            _skipButton.gameObject.SetActive(false);
            
            _audioController = MainSceneController.Instance.Audio;

            GameObject lootboxObject = _activeLootbox.Find(x => x.Rarity == _data.LootboxType).ObjectPrefab;
            
            _chest = lootboxObject.GetComponent<Animator>();
            
            lootboxObject.SetActive(false);
            
            PlaySequence();
        }

        private async void PlaySequence()
        {
            if(!_isSkip) await StartAnimation();

            if (!_isSkip) await ShowLootbox(_data != null ? _data.LootboxType : LootboxRarityEnum.Silver);
        }

        private async Task StartAnimation()
        {
            _fade.FadeIn();
            
            _audioController.PlayBgm(GachaLootboxAudioKeys.BGM_LOOTBOX);
            
            if(MainSceneController.Instance.EnvironmentConfig.IsSkipLootboxEnabled) _skipButton.gameObject.SetActive(true);

            await Task.Delay(1000);

            _sceneWorld.SetActive(true);
            
            await _fade.FadeOutAsync();
            
            _starIcon.gameObject.SetActive(true);
            
            await _starIcon.Open(_data != null ? _data.LootboxType : LootboxRarityEnum.Bronze);

            await _starIcon.Close();
            
            _starIcon.gameObject.SetActive(false);
        }

        private async Task Stop()
        {

            _fade.FadeIn();

            await Task.Delay(1000);
            
            _sceneWorld.SetActive(false);
            
            _chest.gameObject.SetActive(false);

            await _fade.FadeOutAsync();

            //if(_data != null)
            //    _data.Camera.GetUniversalAdditionalCameraData().cameraStack.Remove(_camera);

            // _otherCamera.enabled = true;

            _onFinished();

        }
        private async Task ShowLootbox(LootboxRarityEnum lootboxTypeEnum)
        {
            //_fade.FadeIn();
            //await Task.Delay(1000);
            //_sceneWorld.SetActive(true);
            //await _fade.FadeOutAsync();
            //_starIcon.gameObject.SetActive(true);
            //_audioController.PlaySfx(GachaLootboxAudioKeys.intro);
            //await _starIcon.Open(_data != null ? _data.LootboxType : LootboxRarityEnum.Bronze);
            //await _starIcon.Close();

            _starIcon.gameObject.SetActive(false);

            _chest.gameObject.SetActive(true);
            _chest.SetTrigger(Appear);
            _cameraAnimator.SetTrigger(Appear);
            await Task.Delay(1500);
            if (!_isSkip) OpenTap();
        }

        private async void OpenLootbox()
        {
            _skipButton.gameObject.SetActive(false); 

            _chest.SetTrigger(Open);
            _cameraAnimator.SetTrigger(Open);
            _pins.SetTrigger(Appear);
            await Task.Delay(2500);
            ContinueTap();
        }

        // wait for user input, if user clicked the screen, return true, else block the program
        private async Task WaitForInput(string prompt)
        {
            _tapText.SetText(prompt);
            _tap.gameObject.SetActive(true);
            
            while (!Input.GetMouseButtonDown(0))
            {
                await Task.Yield();
            }
            _tap.gameObject.SetActive(false);
        }

        private void OpenTap()
        {
            _tapButton.gameObject.SetActive(true);
            _tap.gameObject.SetActive(true);
            _tapButton.onClick.AddListener(() =>
            {
                MainSceneController.Instance.Audio.PlaySfx(GachaLootboxAudioKeys.SFX_LOOTBOX_RUMBLE);
                OpenLootbox();
                _tapButton.gameObject.SetActive(false);
                _tap.gameObject.SetActive(false);
                _tapButton.onClick.RemoveAllListeners();
            });
            _tapText.SetText("Tap to Open");
        }

        private void ContinueTap()
        {
            _tapButton.gameObject.SetActive(true);
            _tap.gameObject.SetActive(true);
            _skipButton.gameObject.SetActive(false);
            _tapButton.onClick.AddListener(async () =>
            {
                _tapButton.gameObject.SetActive(false);
                _tap.gameObject.SetActive(false);
                _tapButton.onClick.RemoveAllListeners();
                _pinsVFX.HidePinsVFX();
                _pins.SetTrigger(Open);
                await Stop();
            });
            _tapText.SetText("Tap to Continue");
        }

        public void InitPins(List<CollectibleItem> items)
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

        private void SkipAnimation()
        {
            Debug.Log("skip");
            _sceneWorld.SetActive(true);
            _starIcon.gameObject.GetComponent<MeshRenderer>().enabled = false;
            _chest.gameObject.SetActive(true);
            _isSkip = true;
            _starIcon.ForceClose();
            OpenLootbox();
        }

        private void OnDisable()
        {
            _audioController.PlayBgm("bgm_lobby");

        }
    }

    
    [Serializable]
    public class LootboxObject
    {
        public enum LootboxObjectType
        {
            Bronze,
            Silver,
            Gold,
            SpecialBronze,
            SpecialSilver,
            SpecialGold
        }

        public LootboxRarityEnum Rarity;
        public GameObject ObjectPrefab;
    }
}