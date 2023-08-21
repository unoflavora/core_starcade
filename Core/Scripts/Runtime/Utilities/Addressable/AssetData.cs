using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Agate
{
    public class AssetData
    {
        public enum DownloadState
        {
            None,
            Downloading,
            Finish,
            Failed,
            Missing,
        }

        public AddressableController.AssetID Id;
        public string[] Keys;
        public long Size;
        public long[] Sizes;
        public DownloadState State;
        public UnityEvent OnStart;
        public UnityEvent<float, float> OnProgress;
        public UnityEvent OnFailed;
        public UnityEvent OnComplete;
    }
}
