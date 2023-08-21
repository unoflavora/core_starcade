using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.OnBoarding;
using Agate.Starcade.Scripts.Runtime.Utilities;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class OnBoardingController : MonoBehaviour
    {
        [SerializeField] private List<OnBoardingEvent> _onBoardingEvents;
        [SerializeField] private List<GameObject> _onBoardingObject;

        [Header("Buttons")]
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _skipButton;
        [SerializeField] private Button _openSkipOnBoardingButton;

        public UnityEvent OnStartOnBoarding { get; set; }
        public UnityEvent<int> OnCompleteOnBoardingEvent { get; set; }
        public UnityEvent OnEndOnBoarding { get; set; }
        public UnityEvent<int> OnSkipOnboarding { get; set; }
        

        private bool NextByDelay;
        private bool NextByTrigger;
        private bool IsDoingTask = false;
        
        private string onBoardingField;
        private int totalTaskDone;
        private bool IsEventDone;
        private bool IsOnBoardingFinish;
        private int currentState;
        private OnBoardingStateData currentStateData;

        private UnityAction onFinishTask;
        private UnityAction onNextTaskAction;

        private string _rewardId;
        private bool isTutorialFinish;
        private bool isHaveReward;

        private UnityAction _onCompleteReward;
        private void Awake()
        {
            _nextButton.onClick.AddListener(OnNextTask);
            onFinishTask += OnFinishTask;
            onNextTaskAction += OnNextTask;
        }

        private void Start()
        {
            //Skip OnBoarding Prompt
            _openSkipOnBoardingButton?.onClick.AddListener(() =>
            {
				//To-Do : Create SO InfoType for skip prompt
				MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.BUTTON_GENERAL);
				MainSceneController.Instance.Info.Show("You can't replay this tutorial. Are you sure you want to skip the tutorial?", 
                    string.Empty, InfoIconTypeEnum.Alert, 
                    new InfoAction("Skip", SkipOnBoarding), 
                    new InfoAction("Cancel", null)
                );
            });

            //Skip Current OnBoarding Task
            _skipButton.onClick.AddListener(OnSkipTask);
        }

        private void OnEnable()
        {
            //Change to Skip On Boarding Function
            DebugLogConsole.AddCommand("skipFTUE", "Force skip FTUE", SkipOnBoarding);
        }

        private void OnDisable()
        {
            DebugLogConsole.RemoveCommand("skipFTUE");
        }

        private void Update()
        {
            // if (!IsOnBoardingFinish)
            // {
            //     _nextButton.gameObject.SetActive(IsEventDone);
            // }
            // else
            // {
            //     _nextButton.gameObject.SetActive(false);
            // }
            //
        }

        /// <summary>
        /// Function to skip onboarding 
        /// </summary>
        private async void SkipOnBoarding()
        {
            _openSkipOnBoardingButton.gameObject.SetActive(false);

            IsOnBoardingFinish = true;
            Debug.Log("ON BOARDING DONE!");

            OnSkipOnboarding.Invoke(currentState);

            currentStateData = new OnBoardingStateData()
            {
                State = currentState,
                IsComplete = true,
                Field = onBoardingField,
            };

            var response = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.OnBoardingCompletedEvent(currentStateData));
			if (response.Error != null)
			{
				//MainSceneController.Instance.Info.ShowSomethingWrong(response.Error.Code);
				return;
			}

            TutorialChecker.SetTutorialFinish(onBoardingField);

			await FinishOnBoarding();
		}

        public void InitOnboarding(string field, int state, string rewardId = null, UnityAction onCompleteReward = null)
        {
            OnStartOnBoarding = new UnityEvent();
            OnEndOnBoarding = new UnityEvent();
            OnCompleteOnBoardingEvent = new UnityEvent<int>();
            OnSkipOnboarding = new UnityEvent<int>();

            currentState = state;
            onBoardingField = field;

            isHaveReward = true;

            if(rewardId != null) _rewardId = rewardId;
            if(_onCompleteReward != null) _onCompleteReward = onCompleteReward;
        }

        public void StartOnboarding()
        {
			if (currentState > _onBoardingEvents.Count)
			{
				return;
			}
			OnStartOnBoarding.Invoke();

			DoTask(_onBoardingEvents[currentState]);
		}

        private void DoTask(object onBoardingEvent)
        {
            _nextButton.gameObject.SetActive(false);
            IsEventDone = false;
            foreach (var onBoardingObject in _onBoardingObject)
            {
                onBoardingObject.GetComponent<IOnBoardingTask>().StartTask(onBoardingEvent,onFinishTask, onNextTaskAction);
            }

            OnBoardingEvent currentEvent = (OnBoardingEvent)onBoardingEvent;
            NextByDelay = currentEvent.Delay > 0;
            NextByTrigger = currentEvent.InteractType is Highlight.InteractType.ButtonTrigger or Highlight.InteractType.Custom or Highlight.InteractType.Activate;

            //Next by trigger cannot be skipped
            if (!NextByTrigger && !NextByDelay)
            {
                _skipButton.gameObject.SetActive(true);
            }

            IsDoingTask = true;
        }

        private async void OnNextTask()
        {
            if (totalTaskDone != _onBoardingObject.Count) return; //SPAM PREVENTIVE
            
            totalTaskDone = 0;
            currentState++;
            

            if (currentState == _onBoardingEvents.Count)
            {
                IsOnBoardingFinish = true;
                Debug.Log("ON BOARDING DONE!");
                
                currentStateData = new OnBoardingStateData()
                {
                    State = currentState,
                    IsComplete = true,
                    Field = onBoardingField,
                };
                Debug.Log("state saved on " + currentState);
                var response = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.OnBoardingCompletedEvent(currentStateData));
                if (response.Error != null)
                {
                    //MainSceneController.Instance.Info.ShowSomethingWrong(response.Error.Code);
                    return;
                }

				await FinishOnBoarding();
				return;
            }
            currentStateData = new OnBoardingStateData()
            {
                State = currentState,
                IsComplete = false,
                Field = onBoardingField,
            };
            Debug.Log("state saved on " + currentState);
            // TODO uncomment after backend is ready
            // await MainSceneLauncher.Instance.GameBackendController.OnBoardingCompletedEvent(currentStateData);
            OnCompleteOnBoardingEvent.Invoke(currentState);
            DoTask(_onBoardingEvents[currentState]);

			MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.BUTTON_GENERAL);
		}

        private void OnSkipTask()
        {
            if (totalTaskDone >= _onBoardingObject.Count) { return; } //If all of the task is already finished, no need to skip
            foreach (var onBoardingObject in _onBoardingObject)
            {
                //Call OnSkipTask on task that are currently doing task
                IOnBoardingTask task = onBoardingObject.GetComponent<IOnBoardingTask>();
                
                if (task.IsDoingTask)
                {
                    task.OnSkipTask();
                }
            }

		}
        
        private void OnFinishTask()
        {
            totalTaskDone++;
            if (totalTaskDone == _onBoardingObject.Count)
            {
                //Disable skip button on task finish
                _skipButton.gameObject.SetActive(false);

                foreach (var onBoardingObject in _onBoardingObject)
                {
                    onBoardingObject.GetComponent<IOnBoardingTask>().OnTaskFinish();
                }
                Debug.Log("Event Done");
                IsDoingTask = false;
                if (NextByDelay || NextByTrigger)
                {
                    IsEventDone = true;
                    _nextButton.gameObject.SetActive(true);
                    OnNextTask();
                }
                else
                {
                    IsEventDone = true;
                    _nextButton.gameObject.SetActive(true);
                }
            }
        }

        private async Task FinishOnBoarding()
        {
            foreach (var onBoardingObject in _onBoardingObject)
            {
                onBoardingObject.GetComponent<IOnBoardingTask>().OnBoardingFinish();
            }
            
            //await Task.Delay(1000);
            OnEndOnBoarding.Invoke();
            gameObject.SetActive(false);
        }
    }
}
