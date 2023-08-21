using System;
using UnityEngine;

namespace Agate.Starcade.Boot
{
    [Serializable]
    public class StarcadeError
    {
        public string ErrorName;
        public int ErrorCode;
        public string ErrorMessage;
        public Sprite ErrorIcon;
        public ErrorScreen.ErrorType ErrorType;
    }
}
