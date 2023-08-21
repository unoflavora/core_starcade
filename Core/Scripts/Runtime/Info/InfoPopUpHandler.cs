using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Agate.Starcade.Runtime.Main.MainSceneController.AUDIO_KEY;
using Agate.Starcade.Runtime.Main;

namespace Agate.Starcade.Scripts.Runtime.Info
{
    public class InfoPopUpHandler : MonoBehaviour
    {
        public static string REWARD_POP_UP = "reward_pop_up";

        [Header("Panel")]
        [SerializeField] private GameObject _infoPanel;
        [SerializeField] private GameObject _rewardPanel;
        [SerializeField] private GameObject _confirmPanel;

        [Header("Information Icon")]
        [SerializeField] private Image _iconInfo;

        [Header("Information Text")]
        [SerializeField] private TMP_Text _descInfo;
        [SerializeField] private TMP_Text _titleInfo;

        [Header("Button")]
        [SerializeField] private GameObject _buttonContainer;

        [SerializeField] private Button _positiveActionButton;
        [SerializeField] private TMP_Text _positiveActionButtonText;
        [SerializeField] private Button _negativeActionButton;
        [SerializeField] private TMP_Text _negativeActionButtonText;

        //[SerializeField] private Button _closeButton;

        [Header("Data")]
        [SerializeField] private InfoDataScriptableObject _infoDataScriptableObject;
        [SerializeField] private List<Sprite> _iconList;

        [Header("Reward Panel")]
        [SerializeField] private RectTransform _rewardPanelRect;
 
        [Header("Reward Text")]
        [SerializeField] private TMP_Text _titleRewardText;
        [SerializeField] private TMP_Text _captionRewardText;

        [Header("Reward Icon")]
        [SerializeField] private RewardIconObject[] _rewardIconObjects;

        [Header("Reward Button")]
        [SerializeField] private Button _positiveRewardActionButton;
        [SerializeField] private TMP_Text _positiveRewardActionButtonText;
        [SerializeField] private Button _negativeRewardActionButton;
        [SerializeField] private TMP_Text _negativeRewardActionButtonText;

        private Dictionary<ErrorType, InfoData<ErrorType>> _errorDatas;
        private Dictionary<WarningType, InfoData<WarningType>> _warningDatas;
        private Dictionary<InfoType, InfoData<InfoType>> _infoDatas;



        [Header("Confirm Panel")]
        [SerializeField] private TMP_Text _titleConfirmText;
        [SerializeField] private TMP_Text _subTitleConfirmText;
        [SerializeField] private Transform _iconConfirmContainer;
        [SerializeField] private UnityEvent _onConfirmEvent;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_Text _confirmButtonText;
        [SerializeField] private Button _cancelConfirmButton;
        [SerializeField] private Button _closeConfirmButton;
        [SerializeField] private CanvasTransition _confirmPanelTransition;

        private AudioController _audio;

        private UnityAction _onPositiveCustomAction;
        private bool _isHavePositive;
        private UnityAction _onNegativeCustomAction;
        private bool _isHaveNegative;

        public void Awake()
        {
            _positiveActionButton.onClick.AddListener(OnAction);
            _negativeActionButton.onClick.AddListener(OnAction);
            _negativeActionButton.onClick.AddListener(() => _infoPanel.SetActive(false));
            PrepareData();
        }

        private void Reset()
        {
            _iconInfo.gameObject.SetActive(true);
            _descInfo.gameObject.SetActive(true);
            _titleInfo.gameObject.SetActive(true);
            _buttonContainer.gameObject.SetActive(true);
            _positiveActionButton.gameObject.SetActive(true);
            _negativeActionButton.gameObject.SetActive(true);
            _positiveActionButton.onClick.RemoveAllListeners();
            _negativeActionButton.onClick.RemoveAllListeners();
            _positiveActionButton.onClick.AddListener(OnAction);
            _negativeActionButton.onClick.AddListener(OnAction);
        }


        public void Show(ErrorType errorType, InfoAction positiveAction, InfoAction negativeAction)
        {
            Reset();
            var data = _errorDatas[errorType];
            SetInfo(data.Icon, data.Desc, data.Title, data.IsCloseable, positiveAction, negativeAction);
            _infoPanel.SetActive(true);
        }

        public void Show(WarningType warningType, InfoAction positiveAction, InfoAction negativeAction)
        {
            Reset();
            var data = _warningDatas[warningType];
            SetInfo(data.Icon, data.Desc, data.Title, data.IsCloseable, positiveAction, negativeAction);
            _infoPanel.SetActive(true);
        }

        public void Show(InfoType infoType, InfoAction positiveAction, InfoAction negativeAction)
        {
            Reset();
            var data = _infoDatas[infoType];
            SetInfo(data.Icon, data.Desc, data.Title, data.IsCloseable, positiveAction, negativeAction);
            _infoPanel.SetActive(true);
        }

        public void Show(string desc, string title, InfoIconTypeEnum iconType, InfoAction positiveAction, InfoAction negativeAction)
        {
            Reset();
            SetInfo(GetIcon(iconType), desc, title, true, positiveAction, negativeAction);
            _infoPanel.SetActive(true);
        }

        public void Show(string desc, string title, Sprite sprite, InfoAction positiveAction, InfoAction negativeAction)
        {
            Reset();
            SetInfo(sprite, desc, title, true, positiveAction, negativeAction);
            _infoPanel.SetActive(true);
        }

        public void ShowSomethingWrong(string code)
        {
            Reset();
            if (code == "0") return;

            var data = _errorDatas[ErrorType.SomethingWentWrong];
            string title = code.ToString();
            InfoAction CloseGameAction = new InfoAction()
            {
                ActionName = "Quit Game",
                Action = Application.Quit
            };
            SetInfo(data.Icon, data.Desc, title, data.IsCloseable, null, CloseGameAction);
            _infoPanel.SetActive(true);
        }

        public async Task ShowDelay(InfoType infoType, InfoAction positiveAction, InfoAction negativeAction, int delay)
        {
            Reset();
            var data = _infoDatas[infoType];
            SetInfo(data.Icon, data.Desc, data.Title, data.IsCloseable, positiveAction, negativeAction);
            //_closeButton.gameObject.SetActive(false);
            _infoPanel.SetActive(true);
            await DelayedClose(delay);
        }

        private void SetInfo(Sprite icon, string desc, string title, bool isCloseable, InfoAction positiveAction, InfoAction negativeAction)
        {
            _iconInfo.sprite = icon;

            if (desc == string.Empty) _descInfo.gameObject.SetActive(false);
            else _descInfo.text = desc;

            if (title == string.Empty) _titleInfo.gameObject.SetActive(false);
            else _titleInfo.text = title;

            //_closeButton.gameObject.SetActive(isCloseable);

            _buttonContainer.gameObject.SetActive(true);
            if (positiveAction != null)
            {
                _positiveActionButton.gameObject.SetActive(true);

                if (positiveAction.Action == null) _onPositiveCustomAction += () => _infoPanel.SetActive(false);
                else _onPositiveCustomAction += positiveAction.Action;

                _positiveActionButton.onClick.AddListener(_onPositiveCustomAction);
                _isHavePositive = true;
                _positiveActionButtonText.text = positiveAction.ActionName;
            }
            else
            {
                _positiveActionButton.gameObject.SetActive(false);
            }

            if (negativeAction != null)
            {
                _negativeActionButton.gameObject.SetActive(true);

                if (negativeAction.Action == null) _onNegativeCustomAction += () => _infoPanel.SetActive(false);
                else _onNegativeCustomAction += negativeAction.Action;

                _negativeActionButton.onClick.AddListener(_onNegativeCustomAction);
                _isHaveNegative = true;
                _negativeActionButtonText.text = negativeAction.ActionName;
            }
            else
            {
                _negativeActionButton.gameObject.SetActive(false);
            }

            if (!_isHaveNegative && !_isHavePositive)
            {
                _buttonContainer.SetActive(false);
            }

        }

        private Sprite GetIcon(InfoIconTypeEnum infoIconTypeEnum)
        {
            return _iconList[(int)infoIconTypeEnum] ? _iconList[(int)infoIconTypeEnum] : _iconList[(int)InfoIconTypeEnum.Default];
        }

        private void OnAction()
        {
            if (_isHavePositive)
            {
                Debug.Log("REMOVE POSITIVE");
                _positiveActionButton.onClick.RemoveListener(_onPositiveCustomAction);
                _onPositiveCustomAction = null;
                _isHavePositive = false;
            }

            if (_isHaveNegative)
            {
                _negativeActionButton.onClick.RemoveListener(_onNegativeCustomAction);
                _onNegativeCustomAction = null;
                _isHaveNegative = false;
            }
            _infoPanel.SetActive(false);
        }

        private void PrepareData()
        {
            _infoDatas = _infoDataScriptableObject.ListInfoDatas.ToDictionary(keySelector: m => m.Type,
                elementSelector: m => m);

            _errorDatas =
                _infoDataScriptableObject.ListInfoErrorDatas.ToDictionary(keySelector: m => m.Type,
                    elementSelector: m => m);

            _warningDatas =
                _infoDataScriptableObject.ListInfoWarningDatas.ToDictionary(keySelector: m => m.Type,
                    elementSelector: m => m);
        }

        private async Task DelayedClose(int delay)
        {
            await Task.Delay(delay * 1000);
            _infoPanel.SetActive(false);
        }


        public void ShowReward(string title, string caption, RewardBase[] reward, InfoAction positiveAction, InfoAction negativeAction, string panelPopUpSfx = null, string rewardPopUpSfx = null)
        {
            if (panelPopUpSfx != null) MainSceneController.Instance.Audio.PlaySfx(panelPopUpSfx);
            else MainSceneController.Instance.Audio.PlaySfx(REWARD_POP_UP);

            _rewardPanel.SetActive(true);

            SetRewardPanelSize(reward.Length > 3);

            _titleRewardText.text = title;
            _captionRewardText.text = caption;

            foreach (RewardIconObject rewardIconObject in _rewardIconObjects)
            {
                rewardIconObject.gameObject.SetActive(false);
            }

            for (int i = 0; i < reward.Length; i++)
            {
                _rewardIconObjects[i].gameObject.SetActive(true);
                _rewardIconObjects[i].SetContent(reward[i], rewardPopUpSfx);
            }

            if (positiveAction != null)
            {
                _positiveRewardActionButton.gameObject.SetActive(true);

                if (positiveAction.Action != null)
                {
                    _positiveRewardActionButton.onClick.AddListener(positiveAction.Action);
                }

                _positiveRewardActionButton.onClick.AddListener(() =>
                {
                    _rewardPanel.SetActive(false);
                    _negativeRewardActionButton.onClick.RemoveAllListeners();
                    _positiveRewardActionButton.onClick.RemoveAllListeners();
                });

                _positiveRewardActionButtonText.text = positiveAction.ActionName;
            }
            else
            {
                _positiveRewardActionButton.gameObject.SetActive(false);
            }

            if (negativeAction != null)
            {
                _negativeRewardActionButton.gameObject.SetActive(true);

                if (negativeAction.Action != null)
                {
                    _negativeRewardActionButton.onClick.AddListener(negativeAction.Action);
                }

                _negativeRewardActionButton.onClick.AddListener(() =>
                {
                    _rewardPanel.SetActive(false);
                    _negativeRewardActionButton.onClick.RemoveAllListeners();
                    _positiveRewardActionButton.onClick.RemoveAllListeners();
                });

                _negativeRewardActionButtonText.text = negativeAction.ActionName;
            }
            else
            {
                _negativeRewardActionButton.gameObject.SetActive(false);
            }
        }

        public void ShowReward(string title, string caption, Sprite[] sprite, long[] amountReward, InfoAction positiveAction, InfoAction negativeAction)
        {
            _rewardPanel.SetActive(true);

            SetRewardPanelSize(amountReward.Length > 3);

            _titleRewardText.text = title;
            _captionRewardText.text = caption;

            foreach (RewardIconObject rewardIconObject in _rewardIconObjects)
            {
                rewardIconObject.gameObject.SetActive(false);
            }

            for (int i = 0; i < sprite.Length; i++)
            {
                _rewardIconObjects[i].gameObject.SetActive(true);
                _rewardIconObjects[i].SetContent(sprite[i], amountReward[i]);
            }

            if (positiveAction != null)
            {
                _positiveRewardActionButton.gameObject.SetActive(true);

                if (positiveAction.Action != null)
                {
                    _positiveRewardActionButton.onClick.AddListener(positiveAction.Action);
                }

                _positiveRewardActionButton.onClick.AddListener(() =>
                {
                    _rewardPanel.SetActive(false);
                    _positiveRewardActionButton.onClick.RemoveAllListeners();
                });

                _positiveRewardActionButtonText.text = positiveAction.ActionName;
            }
            else
            {
                _positiveRewardActionButton.gameObject.SetActive(false);
            }

            if (negativeAction != null)
            {
                _negativeRewardActionButton.gameObject.SetActive(true);

                if (negativeAction.Action != null)
                {
                    _negativeRewardActionButton.onClick.AddListener(negativeAction.Action);
                }

                _negativeRewardActionButton.onClick.AddListener(() =>
                {
                    _rewardPanel.SetActive(false);
                    _negativeRewardActionButton.onClick.RemoveAllListeners();
                });

                _negativeRewardActionButtonText.text = negativeAction.ActionName;
            }
            else
            {
                _negativeRewardActionButton.gameObject.SetActive(false);
            }
        }

        public void ShowReward(string title, string caption, Sprite[] sprite, string[] amountReward, InfoAction positiveAction, InfoAction negativeAction)
        {
            _rewardPanel.SetActive(true);

            SetRewardPanelSize(amountReward.Length > 3);

            _titleRewardText.text = title;
            _captionRewardText.text = caption;

            foreach (RewardIconObject rewardIconObject in _rewardIconObjects)
            {
                rewardIconObject.gameObject.SetActive(false);
            }

            for (int i = 0; i < sprite.Length; i++)
            {
                _rewardIconObjects[i].gameObject.SetActive(true);
                _rewardIconObjects[i].SetContent(sprite[i], amountReward[i]);
            }

            if (positiveAction != null)
            {
                _positiveRewardActionButton.gameObject.SetActive(true);

                if (positiveAction.Action != null)
                {
                    _positiveRewardActionButton.onClick.AddListener(positiveAction.Action);
                }

                _positiveRewardActionButton.onClick.AddListener(() =>
                {
                    _rewardPanel.SetActive(false);
                    _positiveRewardActionButton.onClick.RemoveAllListeners();
                });

                _positiveRewardActionButtonText.text = positiveAction.ActionName;
            }
            else
            {
                _positiveRewardActionButton.gameObject.SetActive(false);
            }

            if (negativeAction != null)
            {
                _negativeRewardActionButton.gameObject.SetActive(true);

                if (negativeAction.Action != null)
                {
                    _negativeRewardActionButton.onClick.AddListener(negativeAction.Action);
                }

                _negativeRewardActionButton.onClick.AddListener(() =>
                {
                    _rewardPanel.SetActive(false);
                    _negativeRewardActionButton.onClick.RemoveAllListeners();
                });

                _negativeRewardActionButtonText.text = negativeAction.ActionName;
            }
            else
            {
                _negativeRewardActionButton.gameObject.SetActive(false);
            }
        }

        private void SetRewardPanelSize(bool isMore)
        {
            if (isMore)
            {
                _rewardPanelRect.offsetMin = new Vector2(-800, _rewardPanelRect.offsetMin.y);
                _rewardPanelRect.offsetMax = new Vector2(800, _rewardPanelRect.offsetMax.y);
            }
            else
            {
                _rewardPanelRect.offsetMin = new Vector2(-524, _rewardPanelRect.offsetMin.y);
                _rewardPanelRect.offsetMax = new Vector2(524, _rewardPanelRect.offsetMax.y);
            }
        }


        public void ShowConfirmation(TitleLabel ConfirmationLabel, GameObject ConfirmationIcon, UnityAction ConfirmationAction)
        {
            _confirmPanel.gameObject.SetActive(true);

            //set title
            _titleConfirmText.text = ConfirmationLabel.Title;
            _subTitleConfirmText.text = ConfirmationLabel.SubTitle;

            ConfirmationIcon?.transform.SetParent(_iconConfirmContainer, false);

            //set button
            _confirmButton.gameObject.SetActive(true);
            _confirmButton.onClick.AddListener(ConfirmationAction);
            _confirmButton.onClick.AddListener(ResetConfirmPanel);

            _cancelConfirmButton.gameObject.SetActive(true);
            _cancelConfirmButton.onClick.AddListener(ResetConfirmPanel);
        }

        public void ShowSuccessConfirmation(TitleLabel ConfirmationLabel, GameObject ConfirmationIcon, InfoAction customAfterConfirmAction = null)
        {
            Debug.Log("show success");
            _confirmPanel.gameObject.SetActive(true);

            //set title
            _titleConfirmText.text = ConfirmationLabel.Title;
            _subTitleConfirmText.text = ConfirmationLabel.SubTitle;

            ConfirmationIcon?.transform.SetParent(_iconConfirmContainer, false);

            //set button
            if (customAfterConfirmAction != null)
            {
                _confirmButton.gameObject.SetActive(true);
                _confirmButtonText.text = customAfterConfirmAction.ActionName;
                _confirmButton.onClick.AddListener(customAfterConfirmAction.Action);
                _confirmButton.onClick.AddListener(ResetConfirmPanel);
            }
            
            _closeConfirmButton.gameObject.SetActive(true);
            _closeConfirmButton.onClick.AddListener(ResetConfirmPanel);
        }

        private void ResetConfirmPanel()
        {
            _confirmButton.onClick.RemoveAllListeners();
            _closeConfirmButton.onClick.RemoveAllListeners();
            _cancelConfirmButton.onClick.RemoveAllListeners();

            foreach (Transform child in _iconConfirmContainer)
            {
                Destroy(child.gameObject);
            }

            _confirmButton.gameObject.SetActive(false);
            _closeConfirmButton.gameObject.SetActive(false);
            _cancelConfirmButton.gameObject.SetActive(false);
            _confirmPanel.gameObject.SetActive(false);
        }
    }

    public class TitleLabel
    {
        public string Title;
        public string SubTitle;

        public TitleLabel(string title, string subtitle)
        {
            Title = title;
            SubTitle = subtitle;
        }
    }
}
