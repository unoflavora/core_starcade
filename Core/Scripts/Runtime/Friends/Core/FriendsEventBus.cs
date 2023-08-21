using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.Core
{
    public enum FriendsEvent
    {
        OnSendFriendRequest,
        OnUnfriendFriend,
        OnFriendRequestAction,
        OnSendGiftToFriend,
        OnFriendStatusChanged
    }
    
    public class FriendsEventBus
    {
        private static FriendsEventBus _instance;
        
        private readonly Dictionary<FriendsEvent, Action<object>> _eventHandlers;

        public static FriendsEventBus Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FriendsEventBus();
                return _instance;
            }
        }
        
        private FriendsEventBus()
        {
            _eventHandlers = new Dictionary<FriendsEvent, Action<object>>();
        }
        
        public void Subscribe(FriendsEvent eventName, Action<object> handler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers.Add(eventName, handler);
            }
            else
            {
                _eventHandlers[eventName] += handler;
            }
        }
        
        public void Unsubscribe(FriendsEvent eventName, Action<object> handler)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] -= handler;
            }
        }
        
        public void UnsubscribeAll(FriendsEvent eventName)
        {
            
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = null;
            }
        }
        
        public void Publish(FriendsEvent eventName, object data)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                Debug.Log(eventName);
                
                _eventHandlers[eventName]?.Invoke(data);
            }
        }
        
    }
}