using System;
using System.Collections.Generic;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Lobby;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Agate.Starcade.Runtime.Main.MainSceneController.STATIC_KEY;
using static Agate.Starcade.Runtime.Main.MainSceneController.AUDIO_KEY;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using System.Linq;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;

namespace Agate.Starcade.Scripts.Runtime.Game
{
    public class MailboxController : SceneAdditiveBehaviour
    {

        public enum MenuState
        {
            List,
            Detail,
            Collect,
        };

        [Header("Tab Button")]
        [SerializeField] private Toggle _collectTab;
        [SerializeField] private Image _collectNotification;
        [SerializeField] private Toggle _systemTab;
        [SerializeField] private Image _systemNotification;
        [SerializeField] private Toggle _informationTab;
        [SerializeField] private Image _informationNotification;
        [SerializeField] private Toggle _communityTab;
        [SerializeField] private Image _communityNotification;

        [Header("Menu")]
        [SerializeField] private MailListController _listMenu;
        [SerializeField] private MailDetailController _detailMenu;
        [SerializeField] private MailCollectController _collectMenu;
        [SerializeField] private CollectPopupController _collectSinglePopup;
        [SerializeField] private CollectPopupController _collectMultiPopup;

        [Header("Button")]
        [SerializeField] private Button _refreshButton;

        [Header("Custom Content")]
        [SerializeField] private MailSO _communityMails;

        private MailboxMenuEnum _tabState = MailboxMenuEnum.Collect;
        private MenuState _menuState = MenuState.List;

        private AudioController _audio;
        private GameBackendController _gameBackend;

        private InitAdditiveBaseData _sceneAdditiveData;

        private List<MailboxDataItem> _content;
        private List<MailboxDataItem> _collectData;
        private List<MailboxDataItem> _systemData;
        private List<MailboxDataItem> _informationData;
        private List<MailboxDataItem> _communityData;

		private MailboxAnalyticEventHandler _mailboxAnalyticEvent { get; set; }

		private async void Start()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            await MainSceneController.Instance.AssetLibrary.LoadAllAssets();

            _audio = MainSceneController.Instance.Audio;
            _sceneAdditiveData = LoadSceneHelper.PullData();
            LoadSceneHelper.ClearData(); //HOTFIX

            _mailboxAnalyticEvent = new MailboxAnalyticEventHandler(MainSceneController.Instance.Analytic);

            _collectData = new List<MailboxDataItem>();
            _systemData = new List<MailboxDataItem>();
            _informationData = new List<MailboxDataItem>();
            _communityData = new List<MailboxDataItem>();

            _listMenu.Init();
            _listMenu.OnClaimAllClickEvent.AddListener(ClaimAllMail);
            _listMenu.OnClickEvent.AddListener(OnItemClick);

            _detailMenu.Init();
            _detailMenu.OnCloseEvent.AddListener(() => 
            {
                _listMenu.ResetScroll(_tabState, _content);
                ChangeMenu(MenuState.List);
                _detailMenu.ResetLayout();
            });

            _collectMenu.Init();
            _collectMenu.OnCollectEvent.AddListener(async (id) => 
            {
                MailboxDataItem item = GetItemData(id);
                await ClaimMail(item.MailboxId);
                List<RewardBase> rewards = item.Rewards.ToList();
                ShowMultiPopup(rewards);
                _content.Remove(item);
                UpdateAllMails();
                _listMenu.ResetScroll(_tabState, _content);
                SetNotificationVisible(_tabState);
                ChangeMenu(MenuState.List);
            });
            _collectMenu.OnCloseEvent.AddListener(() => ChangeMenu(MenuState.List));

            _collectSinglePopup.Init();

            _collectMultiPopup.Init();

            MainSceneController.Instance.Loading.DoneLoading();

            _gameBackend = MainSceneController.Instance.GameBackend;
            //await SetContent();
            await SetContent(MainSceneController.Instance.Data.MailData);

            ChangeTab(MailboxMenuEnum.Collect);

            _collectTab.onValueChanged.AddListener(arg0 =>
            {
                ChangeTab(MailboxMenuEnum.Collect);
                _mailboxAnalyticEvent.TrackClickMailboxCollectTabEvent();

			});
            _systemTab.onValueChanged.AddListener(arg0 =>
            {
                ChangeTab(MailboxMenuEnum.System);
                _mailboxAnalyticEvent.TrackClickMailboxSystemTabEvent();

			});
            _informationTab.onValueChanged.AddListener(arg0 =>
            {
                ChangeTab(MailboxMenuEnum.Information);
                _mailboxAnalyticEvent.TrackClickMailboxInformationTabEvent();

			});
            _communityTab.onValueChanged.AddListener(arg0 =>
            {
                ChangeTab(MailboxMenuEnum.Community);
                _mailboxAnalyticEvent.TrackClickMailboxCommunityTabEvent();
			});

            _closeButton.onClick.AddListener(() =>
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 2));
            });

            _refreshButton.onClick.AddListener(async () => await Refresh());
        }

        private async Task Refresh()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            await MainSceneController.Instance.MainRequestController.Refresh(Core.Runtime.Main.MainRequestController.RequestType.Mailbox);

            _collectData = new List<MailboxDataItem>();
            _systemData = new List<MailboxDataItem>();
            _informationData = new List<MailboxDataItem>();
            //_communityData = new List<MailboxDataItem>();

            //_tabState = MailboxMenuEnum.Collect;
            SetFilterContent(MainSceneController.Instance.Data.MailData);
            SetActiveContent(_tabState);
            ChangeMenu(MenuState.List);
            _listMenu.ResetScroll(_tabState, _content);

            MainSceneController.Instance.Loading.DoneLoading();
        }

        public async void OnItemClick(string id)
        {
            MailboxDataItem item = GetItemData(id);
            if (item == null) return;

            if (item.Header.Category == MailboxMenuEnum.Collect.ToString())
            {
                await OnCollectItemClick(item);
            }
            else if (item.Header.Category == MailboxMenuEnum.System.ToString() || item.Header.Category == MailboxMenuEnum.Information.ToString())
            {
                await OnSystemItemClick(item);
            }
            else if (item.Header.Category == MailboxMenuEnum.Community.ToString())
            {
                await OnCommnunityItemClick(item);
            }

            _mailboxAnalyticEvent.TrackOpenMailboxItem(item.Header.Category.ToString(), id);
        }

        private async Task OnCollectItemClick(MailboxDataItem item)
        {
            DateTime claimDate = DateTime.Parse(item.StatusData.ReadAt).ToUniversalTime();
            if (claimDate <= DateTime.MinValue)
            {
                if (item.Rewards == null) return;
                ChangeMenu(MenuState.Collect);
                _collectMenu.ResetContent(item);
            }
        }

        private async Task OnSystemItemClick(MailboxDataItem item)
        {
            DateTime claimDate = DateTime.Parse(item.StatusData.ReadAt).ToUniversalTime();
            if (claimDate <= DateTime.MinValue)
            {
                await ClaimMail(item.MailboxId);
            }
            ChangeMenu(MenuState.Detail);
            _detailMenu.ResetContent(item);
            UpdateAllMails();
            SetNotificationVisible(_tabState);
        }

        private async Task OnCommnunityItemClick(MailboxDataItem item)
        {
            if (item.Content == null) return;
            if (item.Content.CustomLink == null) return;
            Application.OpenURL(item.Content.CustomLink);
        }

        private void ChangeTab(MailboxMenuEnum target)
        {
            _tabState = target;
            SetActiveContent(target);
            ChangeMenu(MenuState.List);
            _listMenu.ResetScroll(_tabState, _content);
        }

        private void ChangeMenu(MenuState target)
        {
            SetMenuVisible(_menuState, false);
            SetMenuVisible(target, true);
            _menuState = target;
        }

        private void SetMenuVisible(MenuState target, bool visible)
        {
            if (target == MenuState.List)
            {
                _listMenu.gameObject.SetActive(visible);
            }
            else if (target == MenuState.Detail)
            {
                _detailMenu.gameObject.SetActive(visible);
            }
            else if (target == MenuState.Collect)
            {
                _collectMenu.gameObject.SetActive(visible);
            }
        }

        private async Task SetContent(List<MailboxDataItem> data)
        {
            _communityData.AddRange(_communityMails.Mails);
            SetFilterContent(data);

            //var result = await RequestHandler.Request(async () => await _gameBackend.GetMails());
            //if (result.Error != null)
            //{
            //    MainSceneLauncher.Instance.Info.ShowSomethingWrong(result.Error.Code);
            //    LoadSceneHelper.CloseSceneAdditive();
            //}
            //else
            //{
            //    _communityData.AddRange(_communityMails.Mails);
            //    SetFilterContent(result.Data);
            //}
        }

        private async Task ClaimMail(string id)
        {
            var result = await RequestHandler.Request(async () => await _gameBackend.ClaimMail(id));
            if (result.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(result.Error.Code);
                LoadSceneHelper.CloseSceneAdditive();
            }
            else
            {
                MailboxDataItem item = GetItemData(id);
                item.StatusData = result.Data.StatusData;
                if (result.Data.GrantedItems != null)
                {
                    MainSceneController.Instance.Data.ProcessRewards(result.Data.GrantedItems);
                }

			    _mailboxAnalyticEvent.TrackCollectMailboxItemsEvent(result.Data.MailboxId, result.Data.GrantedItems.Select(d => new MailboxAnalyticEventHandler.RewardParameters()
                {
                    Type = d.Type.ToString(),
                    Amount = d.Amount,
                    Ref = d.Ref,
                    
                }).ToList());
            }
		}

        private async void ClaimAllMail()
        {
			var result = await RequestHandler.Request(async () => await _gameBackend.ClaimAllMail(_tabState));
            if (result.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(result.Error.Code);
                LoadSceneHelper.CloseSceneAdditive();
            }
            else
            {
                List<RewardBase> items = new List<RewardBase>();
                for (int i = 0; i < result.Data.Length; i++)
                {
                    RewardBase[] grantedItems = result.Data[i].GrantedItems;
                    if (grantedItems == null) continue;
                    for (int j = 0; j < grantedItems.Length; j++)
                    {
                        items.Add(grantedItems[j]);
                    }

                    _collectData[i].StatusData = result.Data[i].StatusData;
				}

				var mails = MainSceneController.Instance.Data.MailData?.Where(d => d.Header?.Category == _tabState.ToString())?.ToList();
				if (mails != null)
                {
					for (int i = 0; i < mails.Count; i++)
					{
                        var mail = mails[i];
						_mailboxAnalyticEvent.TrackCollectMailboxItemsEvent(mail.MailboxId, mail.Rewards.Select(d => new MailboxAnalyticEventHandler.RewardParameters()
						{
							Type = d.Type.ToString(),
							Amount = d.Amount,
							Ref = d.Ref,
						}).ToList());
					}
				}

                MainSceneController.Instance.Data.ProcessRewards(items.ToArray());
                ShowMultiPopup(items);
                _collectData.Clear();
                SetNotificationVisible(_tabState);
                _listMenu.ResetScroll(_tabState, _content);

			    _mailboxAnalyticEvent.TrackCollectAllMailboxItemsEvent(mails.Select(d => d.MailboxId).ToList());
            }
            UpdateAllMails();
		}

        private void ShowMultiPopup(List<RewardBase> items)
        {
            _collectMultiPopup.SetContent(items);
            _collectMultiPopup.SetVisible(true);
        }

        private void SetFilterContent(List<MailboxDataItem> data)
        {
            foreach (var item in data)
            {
                if (item.Header.Category == MailboxMenuEnum.Collect.ToString())
                {
                    _collectData.Add(item);
                }
                else if (item.Header.Category == MailboxMenuEnum.System.ToString())
                {
                    _systemData.Add(item);
                }
                else if (item.Header.Category == MailboxMenuEnum.Information.ToString())
                {
                    _informationData.Add(item);
                }
                else if (item.Header.Category == MailboxMenuEnum.Community.ToString())
                {
                    _communityData.Add(item);
                }
            }

            SetNotificationVisible(MailboxMenuEnum.Collect);
            SetNotificationVisible(MailboxMenuEnum.System);
            SetNotificationVisible(MailboxMenuEnum.Information);
            //SetNotificationVisible(MailboxMenuEnum.Community);
        }

        private void SetNotificationVisible(MailboxMenuEnum state)
        {
            if (state == MailboxMenuEnum.Collect)
            {
                SetNotificationVisible(_collectData, _collectNotification);
            }
            else if (state == MailboxMenuEnum.System)
            {
                SetNotificationVisible(_systemData, _systemNotification);
            }
            else if (state == MailboxMenuEnum.Information)
            {
                SetNotificationVisible(_informationData, _informationNotification);
            }
            else if (state == MailboxMenuEnum.Community)
            {
                SetNotificationVisible(_communityData, _communityNotification);
            }
        }

        private void SetNotificationVisible(List<MailboxDataItem> data, Image image)
        {
            bool visible = false;
            foreach (var item in data)
            {
                Debug.Log(item.StatusData.ReadAt);
                if (DateTime.Parse(item.StatusData.ReadAt).ToUniversalTime() <= DateTime.MinValue)
                    visible = true; // true
            }
            image.gameObject.SetActive(visible);
        }

        private void SetActiveContent(MailboxMenuEnum state)
        {
            if (state == MailboxMenuEnum.Collect)
            {
                _content = _collectData;
            }
            else if (state == MailboxMenuEnum.System)
            {
                _content = _systemData;
            }
            else if (state == MailboxMenuEnum.Information)
            {
                _content = _informationData;
            }
            else if (state == MailboxMenuEnum.Community)
            {
                _content = _communityData;
            }
        }

        private MailboxDataItem GetItemData(string id)
        {
            MailboxDataItem result = null;
            foreach (var item in _content)
            {
                if (item.MailboxId == id) result = item;
            }
            return result;
        }

        private void UpdateAllMails()
        {
            List<MailboxDataItem> allMail = new List<MailboxDataItem>();
            allMail.AddRange(_collectData);
            allMail.AddRange(_systemData);
            allMail.AddRange(_informationData);
            MainSceneController.Instance.Data.MailData = allMail;
        }

        private void OnDestroy()
        {
            _sceneAdditiveData?.OnClose();
            Debug.Log("MAILBOX DESTROYED");
        }

    }
}