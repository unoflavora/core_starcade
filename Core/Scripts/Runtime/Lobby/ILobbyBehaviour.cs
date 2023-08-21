using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Agate.Starcade
{
    public interface ILobbyBehaviour
    {
        public ToggleButton ToggleButton { get; set; }
        public void OnOpen();
        public void OnClose();
        
        public UnityEvent CheckNotificationEvent { get; set; }
    }
}
