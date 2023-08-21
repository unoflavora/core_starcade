using Agate.Starcade;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
[CreateAssetMenu(menuName = "Asset Library/Add Gameobject Asset Collection")]
public class GameobjectAssetCollection : ScriptableObject
{
    public string Label;
    public BaseAssetReferenceData<AssetReferenceGameObject>[] Collection;
}

