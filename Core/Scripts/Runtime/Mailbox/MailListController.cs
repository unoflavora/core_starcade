using Agate.Starcade;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Game;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Agate.Starcade.Runtime.Main.MainSceneController.AUDIO_KEY;


namespace Agate.Starcade
{
    public class MailListController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _empty;
        [SerializeField] private ScrollablePoolController _scroll;
        [SerializeField] private Button _claimAllButton;

        [Header("Empty Text")]
        [SerializeField] private string _emptyCollect;
        [SerializeField] private string _emptySystem;
        [SerializeField] private string _emptyInformation;
        [SerializeField] private string _emptyCommunity;

        private AudioController _audio;

        private MailboxMenuEnum _state;
        private List<MailboxDataItem> _data;
        private MailboxDataItem[] _currentPool;

        [HideInInspector] public UnityEvent<string> OnClickEvent;
        [HideInInspector] public UnityEvent OnClaimAllClickEvent;

        public void Init()
        {
            _audio = MainSceneController.Instance.Audio;
            _claimAllButton.onClick.AddListener(() => {
                _audio.PlaySfx(MAILBOX_CLAIM);
                OnClaimAllClickEvent.Invoke();
            });
            InitScroll();
        }

        public void InitScroll()
        {
            _scroll.Init(0);
            _scroll.OnResetScroll.AddListener((direction, startIndex, endIndex) => 
            {
                MailboxDataItem[] pool = new MailboxDataItem[endIndex - startIndex];
                for (int i = startIndex; i < endIndex; i++)
                {
                    pool[i - startIndex] = _data[i];
                }
                _currentPool = pool;
            });
            _scroll.OnItemActivated.AddListener((index, gameObject) =>
            {
                MailItemController mailController = gameObject.GetComponent<MailItemController>();
                if (mailController == null || _currentPool[index] == null) return;
                mailController.Init(_state);
                mailController.SetContent(_currentPool[index].MailboxId, _currentPool[index].Header, _currentPool[index].StatusData);
                mailController.SetOnclickListener((id) =>
                {
                    OnClickEvent.Invoke(id);
                });
            });
            _scroll.OnItemDeactivated.AddListener((index, gameObject) =>
            {
                MailItemController mailController = gameObject.GetComponent<MailItemController>();
                if (mailController == null) return;

                mailController.RemoveAllListeners();
            });
            _scroll.ResetScroll(ScrollablePoolController.ScrollDirection.Down);
        }

        
        public void ResetScroll(MailboxMenuEnum state, List<MailboxDataItem> data)
        {
            _state = state;
            SetContentVisible(_state, data.Count <= 0);
            _data = data;
            _scroll.ResetPosition(data.Count);
            _scroll.ResetScroll(ScrollablePoolController.ScrollDirection.Down);
        }

        public void SetContentVisible(MailboxMenuEnum state, bool isEmpty)
        {
            _empty.gameObject.SetActive(isEmpty);
            _scroll.gameObject.SetActive(!isEmpty);
            _claimAllButton.gameObject.SetActive(!isEmpty && state == MailboxMenuEnum.Collect);
            if (state == MailboxMenuEnum.Collect)
            {
                _empty.text = _emptyCollect;
            }
            else if (state == MailboxMenuEnum.System)
            {
                _empty.text = _emptySystem;
            }
            else if (state == MailboxMenuEnum.Information)
            {
                _empty.text = _emptyInformation;
            }
            else if (state == MailboxMenuEnum.Community)
            {
                _empty.text = _emptyCommunity;
            }
        }
    }
}
