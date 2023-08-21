using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.UI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleWithCustomValue : MonoBehaviour
    {
        [SerializeField] private int _value;
        public int Value => _value;

        public void SetValue(string title, int value)
        {
            _value = value;
        }
    }
}
