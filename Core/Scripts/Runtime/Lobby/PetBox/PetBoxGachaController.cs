using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Core.Runtime.Lobby.Lootbox.Animators;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Agate.Starcade.Runtime.Audio;

namespace Agate.Starcade.Core.Runtime.Lobby.PetBox
{
    public class PetBoxGachaController : MonoBehaviour
    {
        public static string BGM_LOOTBOX = "bgm_lootbox";
        public static string STAR_AMBIENCE = "star_ambience";
        public static string PETBOX_STAR_APPEAR = "petbox_star_appear";
        public static string LOOTBOX_PORTAL_APPEAR = "lootbox_portal_appear";
        public static string LOOTBOX_PORTAL_CHARGE = "lootbox_portal_charge";
        public static string LOOTBOX_BOX_APPEAR = "lootbox_box_appear";
        public static string LOOTBOX_BOX_POPOUP = "lootbox_box_popout";
        public static string PETBOX_BOX_HYPE = "petbox_box_hype";
        public static string LOOTBOX_BOX_OPEN = "lootbox_box_open";

        [SerializeField] private Animator _cameraAnimator;
        [SerializeField] private Animator _sceneAnimator;
        [SerializeField] private Animator _eggAnimator;
        [SerializeField] private Animator _starAnimator;
        [SerializeField] private StarIconAnimator _starIcon;
        [SerializeField] private Button _openEgg;
        [SerializeField] private GameObject _curtain;
        [SerializeField] private GameObject _curtainExplosion;

        [SerializeField] private PetBoxGachaEvent _gachaEvent;


        private AudioController _audio;
        private Scene _currentScene;
        private UnityAction _afterGacha;

        private async void Start()
        {
            _audio = MainSceneController.Instance.Audio;
            InitEvent();
            var data = LoadSceneHelper.PullData();

            _afterGacha = (UnityAction)data.Data;

            MainSceneController.Instance.Loading.DoneLoading();
            StartStarVFX();
            _starIcon.OnFinishStar.AddListener(StartEggGacha);
            _openEgg.onClick.AddListener(Open);
        }

        public async void StartStarVFX()
        {
            _audio.StopBgm();
            _audio.PlayBgm(BGM_LOOTBOX);
            _sceneAnimator.gameObject.SetActive(false);
            _eggAnimator.gameObject.SetActive(false);
            _cameraAnimator.enabled = false;

            await Task.Delay(2500);
            _starAnimator.enabled = true;
            _audio.PlaySfx(PETBOX_STAR_APPEAR);
        }

        public void InitEvent()
        {
            _gachaEvent.StarAppearEvent.AddListener(() =>
            {
                _audio.PlaySfx(PETBOX_STAR_APPEAR);
            });

            _gachaEvent.PortalActivationEvent.AddListener(() =>
            {
                _audio.PlaySfx(LOOTBOX_PORTAL_APPEAR);
            });

            _gachaEvent.BoxAppearEvent.AddListener(() =>
            {
                _audio.PlaySfx(LOOTBOX_BOX_APPEAR);
            });

            _gachaEvent.BoxPopUpEvent.AddListener(() =>
            {
                _audio.PlaySfx(LOOTBOX_BOX_POPOUP);
            });

            _gachaEvent.BoxHypeEvent.AddListener(() =>
            {
                _audio.PlaySfx(PETBOX_BOX_HYPE);
            });

            _gachaEvent.BoxOpenEvent.AddListener(() =>
            {
                _audio.PlaySfx(LOOTBOX_BOX_OPEN);
            });
        }

        public async void StartEggGacha()
        {
            
            var mat = _curtain.GetComponent<MeshRenderer>().material;
            var color = mat.color;

            _cameraAnimator.gameObject.SetActive(true);
            _cameraAnimator.enabled = false;
            _sceneAnimator.gameObject.SetActive(true);
            _sceneAnimator.enabled = false;
            //_eggAnimator.enabled = false;

            LeanTween.value(1, 0, 0.5f).setOnUpdate((float value) =>
            {
                color.a = value;
                mat.color = color;
            });

            await Task.Delay(1000);

            _cameraAnimator.enabled = true;
            _sceneAnimator.enabled = true;
            _eggAnimator.gameObject.SetActive(true);
            _eggAnimator.enabled = true;

            Appear();
        }

        private async void StartGachaDebug()
        {
            Appear();
            await Task.Delay(3000);
            Idle();
            await Task.Delay(5000);
            Open();
            await Task.Delay(3000);
        }

        private async void Appear()
        {
            _cameraAnimator.SetTrigger("Appear");
            _sceneAnimator.SetTrigger("Appear");
            _eggAnimator.SetTrigger("Appear");

            await Task.Delay(2500);

            _openEgg.gameObject.SetActive(true);
        }

        private void Idle()
        {
            _cameraAnimator.SetTrigger("Idle");
            _sceneAnimator.SetTrigger("Idle");
            _eggAnimator.SetTrigger("Idle");
        }

        private async void Open()
        {
            _openEgg.gameObject.SetActive(false);
            _cameraAnimator.SetTrigger("Open");
            _sceneAnimator.SetTrigger("Open");
            _eggAnimator.SetTrigger("Open");

            await Task.Delay(4000);

            _curtainExplosion.SetActive(true);

            var mat = _curtainExplosion.GetComponent<MeshRenderer>().material;
            var color = mat.color;

            LeanTween.value(0, 1, 0.3f).setOnUpdate((float value) =>
            {
                color.a = value;
                mat.color = color;
            });

            await Task.Delay(1000);

            _sceneAnimator.gameObject.SetActive(false);
            _eggAnimator.gameObject.SetActive(false);

            LeanTween.value(1, 0, 0.3f).setOnUpdate((float value) =>
            {
                color.a = value;
                mat.color = color;
            });

            await Task.Delay(1000);

            _afterGacha.Invoke();

            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount-1));
            _audio.PlayBgm("bgm_lobby");
        }
    }
}