using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class InteractTask : MonoBehaviour, IOnBoardingTask
    {
        public enum Interaction
        {
            Basic,
            CallAPI
        }
        
        [SerializeField] private GameObject[] _listInteractObject;
        [SerializeField] private Button _interactButton;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private GameObject _container;

        [SerializeField] private Image _shadowBackground;
        private Color _currentShadowColor;
        private Color _invisShadowColor;
        
        private Button _sceneButtonObject;
        private UnityAction _onFinishInteract;
        private UnityAction _onNextTaskAction;
        public bool IsDoingTask { get; set; }
        private Transform _onBoardingTransform;

        private void Awake()
        {
            _onBoardingTransform = gameObject.transform.parent;
            _currentShadowColor = _shadowBackground.color;
            _invisShadowColor = new Color(_currentShadowColor.r, _currentShadowColor.g, _currentShadowColor.b, 0f);
        }
        

        private async Task SetUp(int objectIndex)
        {
            if(objectIndex == -1)
            {
                HideInteract();
                return;
            }
            if (_listInteractObject[objectIndex].GetComponent<Button>())
            {
                await Task.Delay(300);
                ShowInteract();
                GameObject interactObject = _listInteractObject[objectIndex];
                
                _interactButton.gameObject.transform.SetParent(interactObject.transform);
                //gameObject.transform.SetParent(interactObject.transform);
                
                _interactButton.gameObject.GetComponent<RectTransform>().sizeDelta =
                    interactObject.GetComponent<RectTransform>().sizeDelta + _offset;
                gameObject.GetComponent<RectTransform>().sizeDelta =
                    interactObject.GetComponent<RectTransform>().sizeDelta + _offset;
                
                _interactButton.gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localPosition = Vector3.zero;
                
                _interactButton.gameObject.transform.SetParent(_container.gameObject.transform);
                _sceneButtonObject = _listInteractObject[objectIndex].GetComponent<Button>();
                _interactButton.onClick.AddListener(StartInteract);
            }
            else
            {
                _interactButton.onClick.AddListener(OnInteraction);
            }
        }

        private void ShowInteract()
        {
            _container.SetActive(true);
        }
        
        private void HideInteract()
        {
            _container.SetActive(false);
        }

        private async void StartInteract()
        {
            await Interact();
        }
        
        private async Task Interact()
        {
            try
            {
                _shadowBackground.color = _invisShadowColor;
                Debug.Log("Interact with Object");
                _sceneButtonObject.onClick.Invoke();
            }
            catch
            {
                Debug.Log("Cant Interact with Object");
            }

            if (_onNextTaskAction == null)
            {
                Debug.Log("Action null, check here");
            }
            else
            {
                Debug.Log("task interact finish");
                _interactButton.onClick.RemoveAllListeners();
                _onNextTaskAction.Invoke();
            }
        }

        private void OnInteraction()
        {
            
        }
        
        public async void StartTask(object onBoardingEvent, UnityAction onFinishTask, UnityAction onNextTask)
        {
            //OnBoardingEvent currentEvent = (OnBoardingEvent)onBoardingEvent;
            //if (currentEvent.InteractObjectIndex == -1)
            //{
                //onFinishTask.Invoke();
                //HideInteract();
                //return;
            //}
            //await SetUp(currentEvent.InteractObjectIndex);
            //_onNextTaskAction = onNextTask;
        }

        public void OnSkipTask()
        {
            HideInteract();
        }

        public void OnTaskFinish()
        {
            HideInteract();
        }

        public void OnBoardingFinish()
        {
            Debug.Log("Hide interact");
            HideInteract();
        }
    }
}
