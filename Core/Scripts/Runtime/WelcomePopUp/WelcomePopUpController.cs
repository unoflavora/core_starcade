using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Castle.Core.Internal;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Scripts.Runtime.WelcomePopUp
{
    public class WelcomePopUpController : MonoBehaviour
    {
        private const string WELCOME_POP_UP_DO_NOT_SHOW_FLAG = "WelcomePopUpDoNotShowFlag";
        private const string WELCOME_POP_UP_LATEST_VIEWED = "WelcomePopUpLatestViewed";
        private const string WELCOME_POP_UP_VIEW_COUNT = "WelcomePopUpViewCount";

        [SerializeField] private Canvas _welcomePopUpCanvas;
        [SerializeField] private Transform _containerPopUp;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Toggle _dontShowAgainToggle;
        [SerializeField] private Image _welcomePopUpImage;
        [SerializeField] private int _closeButtonDelay;

		[Header("Collections")]
		[SerializeField] private WelcomePopUpScriptableObject _collections;

        [Header("Debug Mode")]
        [SerializeField] private bool _forceShow;


        private List<WelcomePopUpData> _listActiveWelcomeSprite = new List<WelcomePopUpData>();

        private List<WelcomePopUpViewCount> _listWelcomePopUpViewCounts = new List<WelcomePopUpViewCount>(); 

        private List<string> _listHiddenWelcomePopUp = new List<string>();

        private int _currentIndexPopUp = 0;

        private void Start()
        {
            _welcomePopUpCanvas.worldCamera = Camera.main;

			SetupContent(_collections.ListActiveWelcomePopUpData);
        }

        public void SetupContent(List<WelcomePopUpData> welcomePopUpDatas)
        {
            if(!CheckWelcomePopUpStatus())
            {
                Debug.Log("[WELCOME POP UP] Already viewed, skip...");
                ClosePopUp();
                return;
            }
            else
            {
                Debug.Log("[WELCOME POP UP] Today is new day, open...");
            }

            _closeButton.onClick.AddListener(OnClose);

            _listActiveWelcomeSprite.Clear();

            InitDontShowData();

            foreach (var data in welcomePopUpDatas)
            {
                if (CheckWelcomePopUpIsActive(data) && !CheckWelcomePopIsHidden(data))
                {
                    _listActiveWelcomeSprite.Add(data);
                }
            }

            if(_listActiveWelcomeSprite.Count == 0)
            {
                Debug.Log("[WELCOME POP UP] Nothing to show, skip...");
                ClosePopUp();
            }
            else
            {
                ShowPopUp(_listActiveWelcomeSprite[0]);
            }
        }

        public void OnClose()
        {
            if (_dontShowAgainToggle.isOn)
            {
                AddHiddenWelcomePopData(_listActiveWelcomeSprite[_currentIndexPopUp]);
            }

            AddViewCount(_listActiveWelcomeSprite[_currentIndexPopUp]);

            _currentIndexPopUp++;

            if(_currentIndexPopUp > _listActiveWelcomeSprite?.Count - 1)
            {
                ClosePopUp();
            }
            else
            {
                DelayButton();
                ShowPopUp(_listActiveWelcomeSprite[_currentIndexPopUp]);
            }
        }

        private async void DelayButton()
        {
            _closeButton.interactable = false;
            await Task.Delay(_closeButtonDelay * 1000);
            _closeButton.interactable = true;
        }

        private void ShowPopUp(WelcomePopUpData welcomePopUpData)
        {
            _welcomePopUpImage.sprite = welcomePopUpData.dataSprite;
        }

        private void ClosePopUp()
        {
            _containerPopUp.gameObject.SetActive(false);
            LoadSceneHelper.CloseSceneAdditive();
        }

        private void HidePopUp()
        {
            _containerPopUp.gameObject.SetActive(false);
        }

        private void InitDontShowData()
        {
            if (PlayerPrefs.HasKey(WELCOME_POP_UP_DO_NOT_SHOW_FLAG))
            {
                string data = PlayerPrefs.GetString(WELCOME_POP_UP_DO_NOT_SHOW_FLAG);
                _listHiddenWelcomePopUp = JsonConvert.DeserializeObject<List<string>>(data);
            }
            else
            {
                List<string> listHidden = new List<string>();
                string data = JsonConvert.SerializeObject(listHidden);
                PlayerPrefs.SetString(WELCOME_POP_UP_DO_NOT_SHOW_FLAG, data);
                _listHiddenWelcomePopUp = listHidden;
                PlayerPrefs.Save();
            }
        }

        private bool CheckWelcomePopUpStatus()
        {
            if (PlayerPrefs.HasKey(WELCOME_POP_UP_LATEST_VIEWED))
            {
                bool status = false;
                string lastViewedStringData = PlayerPrefs.GetString(WELCOME_POP_UP_LATEST_VIEWED);
                DateTime lastViewedData = DateTime.Parse(lastViewedStringData);

                if (DateTime.Compare(lastViewedData.Date, DateTime.Now.Date) == 0)
                {
                    Debug.Log("[WELCOME POP UP] Last view = " + lastViewedData);
                    Debug.Log("[WELCOME POP UP] Now = " + DateTime.Now);

                    Debug.Log("[WELCOME POP UP] Is same day");
                    status = false;
                }

                if (DateTime.Compare(lastViewedData.Date, DateTime.Now.Date) == 1)
                {
                    Debug.Log("[WELCOME POP UP] Last view = " + lastViewedData);
                    Debug.Log("[WELCOME POP UP] Now = " + DateTime.Now);

                    Debug.Log("[WELCOME POP UP] Is Yesterday day");
                    status = false;
                }
                if (DateTime.Compare(lastViewedData.Date, DateTime.Now.Date) == -1)
                {
                    Debug.Log("[WELCOME POP UP] Last view = " + lastViewedData);
                    Debug.Log("[WELCOME POP UP] Now = " + DateTime.Now);

                    Debug.Log("[WELCOME POP UP] Is tommorow day");
                    status = true;
                }


                string newData = DateTime.Now.ToString();
                PlayerPrefs.SetString(WELCOME_POP_UP_LATEST_VIEWED, newData);
                PlayerPrefs.Save();

                return status;
            }
            else
            {
                Debug.Log("[WELCOME POP UP] Welcome pop status not found, creating one...");
                string dateTimeNow = DateTime.Now.ToString();
                PlayerPrefs.SetString(WELCOME_POP_UP_LATEST_VIEWED, dateTimeNow);
                PlayerPrefs.Save();
                return true;
            }
        }

        private bool CheckWelcomePopUpIsActive(WelcomePopUpData welcomePopUpData)
        {
            if (PlayerPrefs.HasKey(WELCOME_POP_UP_VIEW_COUNT))
            {
                string lastViewedStringData = PlayerPrefs.GetString(WELCOME_POP_UP_VIEW_COUNT);
                _listWelcomePopUpViewCounts = JsonConvert.DeserializeObject<List<WelcomePopUpViewCount>>(lastViewedStringData);
            }
            else
            {
                Debug.Log("[WELCOME POP UP] Welcome pop view count not found, creating one...");
                string data = JsonConvert.SerializeObject(_listWelcomePopUpViewCounts);
                PlayerPrefs.SetString(WELCOME_POP_UP_VIEW_COUNT, data);
                PlayerPrefs.Save();
            }

			DateTime dateTimeNow = DateTime.Now;

			DateTime? startDate = (!string.IsNullOrEmpty(welcomePopUpData.StartDate)) ? DateTime.ParseExact(welcomePopUpData.StartDate, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-US")) : null;
			DateTime? endDate = (!string.IsNullOrEmpty(welcomePopUpData.EndDate)) ? DateTime.ParseExact(welcomePopUpData.EndDate, "dd/MM/yyyy", new System.Globalization.CultureInfo("en-US")) : null;

			bool isValidStartDate = startDate == null || DateTime.Compare(startDate.Value, dateTimeNow) == -1;
            bool IsValidEndDate = endDate == null || DateTime.Compare(endDate.Value, dateTimeNow) == 1;

			bool isInRangeDate = isValidStartDate && IsValidEndDate;

            if (!isInRangeDate) 
            {
                Debug.Log("[WELCOME POP UP] Welcome pop view data for id " + welcomePopUpData.dataId + " is expired, don't show it...");  
                return false; //THIS WELCOME POP UP DATA IS EXPIRED, DONT SHOW AGAIN
            }

            if (_listWelcomePopUpViewCounts.Exists(data => data.dataId == welcomePopUpData.dataId))
            {
                var viewData = _listWelcomePopUpViewCounts.Find(data => data.dataId == welcomePopUpData.dataId).viewCount;
                var viewMaxCountData = welcomePopUpData.ViewedMaxCount;

                if (welcomePopUpData.ViewedMaxCount == 0)
                {
                    Debug.Log("[WELCOME POP UP] Welcome pop view data for id " + welcomePopUpData.dataId + " dont have max view, show it...");
                    return true; //THIS WELCOME POP UP DONT HAVE VIEW MAX, ALWAYS SHOW IF STILL IN RANGE DATE
                }

                if (viewMaxCountData >= viewData)
                {
                    Debug.Log("[WELCOME POP UP] Welcome pop view data for id " + welcomePopUpData.dataId + " view count is not maxed (" + viewMaxCountData +" >= "+ viewData + "), show it...");
                    return true; //THIS WELCOME POP UP VIEW COUNT STILL NOT MAXED, SHOW IT
                }
                else
                {
                    Debug.Log("[WELCOME POP UP] Welcome pop view data for id " + welcomePopUpData.dataId + " view count is maxed (" + viewMaxCountData + " >= " + viewData + "), don't show it...");
                    return false; //THIS WELCOME POP UP VIEW COUNT STILL MAXED, DON'T SHOW IT
                }
            }

            else
            {
                WelcomePopUpViewCount tempData = new WelcomePopUpViewCount();
                tempData.dataId = welcomePopUpData.dataId;
                tempData.viewCount = 0;
                _listWelcomePopUpViewCounts.Add(tempData);

                string data = JsonConvert.SerializeObject(_listWelcomePopUpViewCounts);
                PlayerPrefs.SetString(WELCOME_POP_UP_VIEW_COUNT, data);
                PlayerPrefs.Save();

                Debug.Log("[WELCOME POP UP] Welcome pop view data for id " + welcomePopUpData.dataId + " not found, creating one and view count is not maxed because data created new, show it......");
                return true; //THIS WELCOME POP UP DATA PLAYER PREF NEED TO BE CREATED, SHOW IT BECAUSE NEW DATA ALWAYS VIEW COUNT 0
            }
        }

        private bool CheckWelcomePopIsHidden(WelcomePopUpData welcomePopUpData)
        {
            return _listHiddenWelcomePopUp.Contains(welcomePopUpData.dataId);
        }


        private void AddHiddenWelcomePopData(WelcomePopUpData welcomePopUpData)
        {
            _listHiddenWelcomePopUp.Add(welcomePopUpData.dataId);
            string data = JsonConvert.SerializeObject(_listHiddenWelcomePopUp);
            PlayerPrefs.SetString(WELCOME_POP_UP_DO_NOT_SHOW_FLAG, data);
            PlayerPrefs.Save();
            Debug.Log("[WELCOME POP UP] Hide this poster with id " + welcomePopUpData.dataId);
        }

        private void AddViewCount(WelcomePopUpData welcomePopUpData)
        {
            var data = _listWelcomePopUpViewCounts.FirstOrDefault(data => data.dataId == welcomePopUpData.dataId);
            data.viewCount++;
            string newData = JsonConvert.SerializeObject(_listWelcomePopUpViewCounts);
            PlayerPrefs.SetString(WELCOME_POP_UP_VIEW_COUNT, newData);
            PlayerPrefs.Save();

        }
    }

    public enum WelcomePopUpTypeEnum
    {
        ViewByDate,
        ViewByCount
    }

    [Serializable]
    public class WelcomePopUpData
    {
        public string dataId;
        public Sprite dataSprite;
        public string StartDate;
        public string EndDate;
        public int ViewedMaxCount;
    }

    public class WelcomePopUpStatus
    {
        public string latestTimeViewed;
    }

    public class HiddenWelcomePopData
    {
        public string dataId;
    }

    [Serializable]
    public class WelcomePopUpViewCount
    {
        public string dataId;
        public int viewCount;
    }
}