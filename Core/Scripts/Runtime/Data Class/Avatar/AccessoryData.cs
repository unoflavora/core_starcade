using Agate.Starcade.Runtime.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade
{
    public class AccessoryData 
    {
        public ItemTypeEnum Type;
        public string Id;
        public object Data;
        public UnityEvent OnAccessoryChanged;
    }
}
