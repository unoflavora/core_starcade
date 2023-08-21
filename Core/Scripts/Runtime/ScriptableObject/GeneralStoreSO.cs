using System;
using UnityEngine;
using Agate.Starcade.Core.Runtime.Lobby.Store;

[Serializable]
[CreateAssetMenu(menuName = "Starcade Scriptable Object/ General Store SO")]
public class GeneralStoreSO : ScriptableObject
{
    public GeneralStoreModel[] items;
    public GameObject itemPrefab;
}
