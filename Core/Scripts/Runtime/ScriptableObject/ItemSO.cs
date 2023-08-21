using System;
using UnityEngine;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade;
using UnityEngine.AddressableAssets;

[Serializable]
[CreateAssetMenu(menuName = "Asset Library/Add New Asset Collection")]
public class ItemSO : ScriptableObject
{
    public ItemTypeEnum Type;
    public BaseAssetReferenceData<AssetReference>[] Data;
}
