using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Agate.Starcade
{
    [Serializable]
    public class BaseAssetReferenceData<T>
    {
        public string Id;
        public T Reference;
    }
}
