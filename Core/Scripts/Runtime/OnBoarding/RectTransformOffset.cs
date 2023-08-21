using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade
{
    [Serializable]
    public class RectTransformOffset : MonoBehaviour
    {
        public float Left { set; get; }
        public float Right { set; get; }
        public float Top { set; get; }
        public float Bottom { set; get; }
    }
}
