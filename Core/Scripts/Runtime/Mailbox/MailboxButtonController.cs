using Agate.Starcade.Runtime.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate
{
    public class MailboxButtonController : MonoBehaviour
    {
        [SerializeField] private GameObject _notification;

        public void SetNotificationVisible(List<MailboxDataItem> data)
        {
            bool visible = false;
            foreach (var item in data)
            {
                DateTime readAt = DateTime.Parse(item.StatusData.ReadAt).ToUniversalTime();
                if (readAt <= DateTime.MinValue) visible = true;
            }
            _notification.gameObject.SetActive(visible);
        }
    }
}
