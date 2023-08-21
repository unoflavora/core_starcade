using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/StringVariable")]
public class StringVariable : ScriptableObject
{
    public string Value;

    public void SetValue(string newValue)
    {
        Value = newValue;
    }

}
