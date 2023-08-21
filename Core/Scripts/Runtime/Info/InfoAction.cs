using System;
using UnityEngine.Events;

namespace Agate.Starcade.Scripts.Runtime.Info
{
    public class InfoAction
    {
        public string ActionName;
        public UnityAction Action;

        public InfoAction()
        {
            
        }
        
        public InfoAction(string actionName, UnityAction action)
        {
            ActionName = actionName;
            Action = action;
        }
    }
}