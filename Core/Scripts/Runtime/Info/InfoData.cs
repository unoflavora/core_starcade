using System;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Info
{
    [Serializable]
    public class InfoData<T>
    {
        public T Type;
        public string Desc;
        public string Title;
        public Sprite Icon;
        public bool IsCloseable;
    }
}