using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Main;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Task = System.Threading.Tasks.Task;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.Interaction
{
    public class PetInteractionController : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] ParticleSystem _interactionVfx;
        [SerializeField] GameObject _touchVfx;
        [SerializeField] private Camera _camera;
        [SerializeField] [Range(0, 5)] private float _betweenInteractionDelay;
        
        private PetSpineController _pet;
        private bool _dragging;
        
        private UnityEvent _onPetInteract;
        private UnityEvent _onUserTouchPet;
        private bool _isDragging;
        private bool _playingSfx;

        private void Start()
        {
            _onUserTouchPet = new UnityEvent();
        }

        private void Update()
        {
            if (_isDragging && _touchVfx != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    transform.parent as RectTransform,
                    Input.mousePosition,
                    _camera,
                    out Vector2 canvasMousePos);
                
                InteractPet();
                

                // Set the position of the GameObject to follow the mouse position
                _touchVfx.transform.localPosition = canvasMousePos;
            }
        }

        private async void TouchSfx()
        {
            if (_playingSfx) return;
            _playingSfx = true;
            MainSceneController.Instance.Audio.PlaySfx(PetAudioKeys.SFX_PET001);
            await Task.Delay(500);
            _playingSfx = false;
        }

        public void Handle(PetSpineController pet)
        {
            _pet = pet;

            if (MainSceneController.Instance == null) return;
            
            _onPetInteract = new UnityEvent();

            _interactionVfx.Stop();
            
            _pet.OnPetIdle();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            _onUserTouchPet.Invoke();
            
            InteractPet();
        }

        private void InteractPet()
        {
            if (_pet == null || _interactionVfx.isPlaying) return;
            
            TouchSfx();
            
            PlayOnClickVFX();

            _pet.OnPetInteract();

            _onPetInteract.Invoke();
        }


        public void RegisterOnPetInteract(UnityAction action)
        {
            _onPetInteract.RemoveAllListeners();
            _onPetInteract.AddListener(action);
        }

        public void RegisterOnPetTouched(UnityAction action)
        {
            _onUserTouchPet = new UnityEvent();
            _onUserTouchPet.AddListener(action);
        }

        private void PlayOnClickVFX()
        {
            _interactionVfx.gameObject.SetActive(true);
            _interactionVfx.Play();
            MainSceneController.Instance.Audio.PlaySfx(PetAudioKeys.SFX_PET002);

            // TODO change this into async
            Invoke(nameof(StopInteractionVfx), _betweenInteractionDelay);
        }
        
        private void StopInteractionVfx()
        {
            _interactionVfx.Stop();
            _interactionVfx.gameObject.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_touchVfx == null) return;

            _isDragging = false;
            _touchVfx.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_touchVfx == null) return;

            _isDragging = true; 
            _touchVfx.SetActive(true);
        }
    }
}