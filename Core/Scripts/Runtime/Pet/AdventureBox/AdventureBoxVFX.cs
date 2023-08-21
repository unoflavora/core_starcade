using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Enum;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Runtime.Main;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.Pet.AdventureBox
{
	public class AdventureBoxVFX : MonoBehaviour
    {
        public static string ADVENTURE_BOX_STAR_APPEAR = "adventure_box_star_appear";
        public static string ADVENTURE_BOX_CHEST_APPEAR = "adventure_box_chest_appear";
        public static string ADVENTURE_BOX_CHEST_START_OPEN = "adventure_box_chest_start_open";
        public static string ADVENTURE_BOX_CHEST_AFTER_OPEN = "adventure_box_chest_after_open";

        [Header("SPINE")]
        [SerializeField] private Vector3 _centerSpinePos;
        [SerializeField] private Vector3 _hiddenSpinePos;
        [SerializeField] private SkeletonGraphic _chestSpine;

        private Animator _animator;
        private CanvasTransition _fadeTransition;

        public string _chestOpenAnimation = "box_open";

        public UnityEvent OnFinishVFX = new UnityEvent();

        private string _chestSkin;

        private CanvasGroup _chestCanvasGroup;

        private async void Start()
        {
            //await MainSceneController.Instance.Audio.LoadAudioData("pet_audio");
            _fadeTransition = gameObject.GetComponent<CanvasTransition>();
            _animator = gameObject.GetComponent<Animator>();
            _chestCanvasGroup = _chestSpine.gameObject.GetComponent<CanvasGroup>();

        }

        public void StartVFX(AdventureBoxTypeEnum type)
        {
            _animator.SetTrigger("Open");

            _chestSkin = type.ToString().ToLower();
        }

        public void PlayOpenChest()
        {
            Debug.Log("OPEN");
            _chestSpine.transform.localPosition = _centerSpinePos;
            _chestCanvasGroup.alpha = 0;
            LeanTween.value(0, 1, 1f).setOnUpdate((float a) => { _chestCanvasGroup.alpha = a; });
            _chestSpine.AnimationState.AddAnimation(0, "empty", false, 0);
            _chestSpine.AnimationState.AddAnimation(0, "empty", false, 0);
            _chestSpine.AnimationState.AddAnimation(0, _chestOpenAnimation, false, 0);
            _chestSpine.AnimationState.AddAnimation(0, "empty", false, 0);

            _chestSpine.Skeleton.SetSkin(_chestSkin);
            _chestSpine.gameObject.SetActive(true);

        }

        public void StopOpenChest()
        {
            Debug.Log("CLOSE");
            OnFinishVFX?.Invoke();
            _animator.SetTrigger("Close");
            //_chestSpine.AnimationState.SetEmptyAnimations(10f);
            //_chestSpine.AnimationState.ClearTracks();
            _chestSpine.transform.localPosition = _hiddenSpinePos;
            //_chestSpine.gameObject.SetActive(false);
        }


        public void OnStarAppear()
        {
            MainSceneController.Instance.Audio.PlaySfx(ADVENTURE_BOX_STAR_APPEAR);
        }

        public void OnChestAppear()
        {
            MainSceneController.Instance.Audio.PlaySfx(ADVENTURE_BOX_CHEST_APPEAR);
        }

        public void OnChestStartOpen()
        {
            MainSceneController.Instance.Audio.PlaySfx(ADVENTURE_BOX_CHEST_START_OPEN);
        }

        public void OnChestAfterOpen()
        {
            MainSceneController.Instance.Audio.PlaySfx(ADVENTURE_BOX_CHEST_AFTER_OPEN);
        }
    }
}