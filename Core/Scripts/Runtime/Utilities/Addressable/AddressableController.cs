using Agate.Starcade;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using System;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Runtime.Audio;

namespace Agate
{
    public class AddressableController : MonoBehaviour
    {
        public enum AssetID
        {
            None,
            Main,
            Starmatch,
            Plinko,
            Monstarmatch,
            CoinPusher,
            CP02
        }

        [SerializeField] private AssetSO[] _assetList;
        private Dictionary<string, AssetData> _assetDictionary;

        private Queue<AssetData> _addressableQueue = new Queue<AssetData>();
        private AssetData _current;
        private bool _isDownloadCheckDone = false;

        public void Init()
        {
            _assetDictionary = new Dictionary<string, AssetData>();
            foreach (AssetSO asset in _assetList)
            {

                string[] keys = CreateNonEmptyArray(asset.SceneKey, asset.ActiveKey, asset.AssetKeys);

                AssetData assetData = new AssetData()
                {
                    Id = asset.Id,
                    State = AssetData.DownloadState.None,
                    Keys = keys
                };
                assetData.OnStart = new UnityEvent();
                assetData.OnProgress = new UnityEvent<float, float>();
                assetData.OnFailed = new UnityEvent();
                assetData.OnComplete = new UnityEvent();
                _assetDictionary.Add(asset.Id.ToString(), assetData);
            }
        }

        public void CheckDownloadSize(UnityAction OnComplete)
        {
			if (_isDownloadCheckDone)
            {
                OnComplete();
                return;
            }
            _isDownloadCheckDone = true;
            InitSingleDownloadSize(0, OnComplete);
        }

		public async Task<bool> CheckDownloadSize()
		{
			var taskCompletionSource = new TaskCompletionSource<bool>();
			var task = taskCompletionSource.Task;
			Action<bool> callback = taskCompletionSource.SetResult;

			if (_isDownloadCheckDone)
			{
                return true;
			}

			InitSingleDownloadSize(0, () =>
            {
                callback(true);
			});

			_isDownloadCheckDone = true;
			return await task;
		}

		public void SetOnStartEvent(AssetID id, UnityAction OnStart)
        {
            if (id == AssetID.None) return;
            _assetDictionary[id.ToString()].OnStart.AddListener(OnStart);
        }

        public void SetOnProgressEvent(AssetID id, UnityAction<float, float> OnProgress)
        {
            if (id == AssetID.None) return;
            _assetDictionary[id.ToString()].OnProgress.AddListener(OnProgress);
        }

		public void SetOnFailedEvent(AssetID id, UnityAction onFailed)
		{
			if (id == AssetID.None) return;
			_assetDictionary[id.ToString()].OnFailed.AddListener(onFailed);
		}

		public void SetOnCompleteEvent(AssetID id, UnityAction OnComplete)
        {
            if (id == AssetID.None) return;
            _assetDictionary[id.ToString()].OnComplete.AddListener(OnComplete);
        }

        public void InvokeOnStartEvent(AssetID id)
        {
            if (id == AssetID.None) return;
            _assetDictionary[id.ToString()].OnStart.Invoke();
        }

        public void RemoveAllListeners()
        {
            Dictionary<string, AssetData>.ValueCollection assets = _assetDictionary.Values;
            foreach (AssetData asset in assets)
            {
                if (asset.OnComplete != null) asset.OnComplete.RemoveAllListeners();
                if (asset.OnProgress != null) asset.OnProgress.RemoveAllListeners();
                if (asset.OnStart != null) asset.OnStart.RemoveAllListeners();
                if (asset.OnFailed != null) asset.OnFailed.RemoveAllListeners();
            }
        }

        public AssetData.DownloadState GetDownloadStatus(AssetID id)
        {
            if (id == AssetID.None) return AssetData.DownloadState.Missing;
            return _assetDictionary[id.ToString()].State;
        }

        public long GetDownloadSize(AssetID id)
        {
            if (id == AssetID.None) return 0;
            return _assetDictionary[id.ToString()].Size;
        }

        public void AddDownload(AssetID id)
        {
            _addressableQueue.Enqueue(_assetDictionary[id.ToString()]);
        }

        public void StartDownload()
        {
            if (_addressableQueue.Count <= 0)
            {
                _current = null;
                return;
            }
            _current = _addressableQueue.Dequeue();
            DownloadAssets();
        }

        public bool IsDownloading()
        {
            return _current != null;
        }

        public void DownloadAssets()
        {
            int index = 0;
            if (_current.Keys == null)
            {
                _current.State = AssetData.DownloadState.Finish;
                _current.OnComplete.Invoke();
                return;
            }
            GetDownloadSize(_current.Keys, (size, sizes) => {
                _current.Size = size;
                _current.Sizes = sizes.ToArray();
                Debug.Log($"Start Download Size : {size}");
                StartCoroutine(DownloadAsset(index, 0));
            });
        }

        public void GetDownloadSize(string[] keys, UnityAction<long, List<long>> OnComplete)
        {
            if (keys == null)
            {
                OnComplete(0, new List<long>());
                return;
            }
            StartCoroutine(GetSingleDownloadSize(0, keys, 0, new List<long>(), OnComplete));
        }

        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(AssetReference sceneReference, LoadSceneMode mode)
        {
            return Addressables.LoadSceneAsync(sceneReference, mode);
        }

		public async Task<Scene> LoadSceneAsset(string sceneKey)
		{
			Scene scene = await Addressables.LoadAssetAsync<Scene>(sceneKey).Task;
			return scene;
		}

		public void unloadAsset(AssetReference assetRef)
		{
            assetRef.ReleaseAsset();
		}
        
        public async Task<Sprite> LoadSpriteAsset(AssetReference reference)
        {
            Sprite sprite = await Addressables.LoadAssetAsync<Sprite>(reference).Task;
            return sprite;
        }

		public async Task<T> LoadAsset<T>(AssetReference reference)
		{
            T item = await Addressables.LoadAssetAsync<T>(reference).Task;
			return item;
		}

		public void UnloadBundledSpriteAssets(ItemAccessoryData[] data)
        {
            foreach (ItemAccessoryData item in data)
            {
                item.Reference.ReleaseAsset();
            }
        }

        public async Task<AudioData> LoadAudioAsset(RawAudioData rawAudioData)
        {
            AudioClip audioClip = await Addressables.LoadAssetAsync<AudioClip>(rawAudioData.audioReference).Task;
            AudioData audioData = new AudioData(rawAudioData.id, audioClip, rawAudioData.label);
            return audioData;
        }

        public void UnloadAudioAssets(string label, AudioCollection audioCollection)
        {
            foreach (RawAudioData rawAudioData in audioCollection.NewBgmCollection)
            {
                if (rawAudioData.label == label)
                {
                    rawAudioData.audioReference.ReleaseAsset();
                }
            }

            foreach (RawAudioData rawAudioData in audioCollection.NewSfxCollection)
            {
                if (rawAudioData.label == label)
                {
                    rawAudioData.audioReference.ReleaseAsset();
                }
            }
        }

        #region Recursive Functions
        private void InitSingleDownloadSize(int index, UnityAction OnComplete)
        {
            if (_assetList.Length <= index)
            {
                OnComplete();
                return;
            }
            AssetSO assetSO = _assetList[index];
            AssetData assetData = _assetDictionary[assetSO.Id.ToString()];

            GetDownloadSize(assetData.Keys, (size, sizes) =>
            {
                assetData.Size = size;
                assetData.Sizes = sizes.ToArray();
                if (size <= 0) assetData.State = AssetData.DownloadState.Finish;
                InitSingleDownloadSize(index + 1, OnComplete);
            });
        }

        private IEnumerator GetSingleDownloadSize(int index, string[] keys, long total, List<long> totals, UnityAction<long, List<long>> OnComplete)
        {
            if (keys.Length <= index)
            {
                OnComplete(total, totals);
                yield break;
            }
            if (keys[index] == null || keys[index] == "")
            {
                StartCoroutine(GetSingleDownloadSize(index + 1, keys, total, totals, OnComplete));
            }
            AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(keys[index]);
            yield return getDownloadSize;
            long currentTotal = total + getDownloadSize.Result;
            totals.Add(getDownloadSize.Result);
            StartCoroutine(GetSingleDownloadSize(index + 1, keys, currentTotal, totals, OnComplete));
        }

        private IEnumerator DownloadAsset(int index, float prevProgress)
        {
            Debug.Log($"Download Asset : {index}");
            if (_current.Keys.Length <= index)
            {
                _current.OnComplete.Invoke();
                StartDownload();
                yield break;
            }

            Debug.Log($"Download Asset : {index} {_current.Keys[index]}");
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(_current.Keys[index], false);
            _current.State = AssetData.DownloadState.Downloading;
            float progress = 0;
            float currentProgress = prevProgress;

            Debug.Log($"Downloading...");
            while (downloadHandle.Status == AsyncOperationStatus.None)
            {
                float percentageComplete = downloadHandle.GetDownloadStatus().Percent;
                if (percentageComplete > progress * 1.1)
                {
                    progress = percentageComplete;
                    Debug.Log($"Download : {(progress * (float)_current.Sizes[index]) + currentProgress} / {(float)_current.Size}");
                    _current.OnProgress.Invoke((progress * (float)_current.Sizes[index]) + currentProgress, (float)_current.Size);
                }
                yield return null;
            }

            _current.State = downloadHandle.Status == AsyncOperationStatus.Succeeded ? AssetData.DownloadState.Finish : AssetData.DownloadState.Failed;

            Debug.Log($"Download Finish {_current.State}");

            if (_current.State == AssetData.DownloadState.Failed)
            {
                Debug.Log($"Download FAILED");
                MainSceneController.Instance.Info.Show(Starcade.Scripts.Runtime.Info.ErrorType.SomethingWentWrong, new Starcade.Scripts.Runtime.Info.InfoAction("OK", null), null);
                _current.State = AssetData.DownloadState.None;
                _current.OnFailed.Invoke();
            }

            currentProgress += _current.Sizes[index];
            Addressables.Release(downloadHandle);
            StartCoroutine(DownloadAsset(index + 1, currentProgress));
        }
        #endregion

        #region Private Functions
        private string[] CreateNonEmptyArray(string string1, string string2, string[] strings)
        {
            List<string> stringList = new List<string>();
            if (!IsStringEmpty(string1)) stringList.Add(string1);
            if (!IsStringEmpty(string2)) stringList.Add(string2);
            foreach(string item in strings)
            {
                stringList.Add(item);
            }
            string[] result = stringList.ToArray();
            return result;
        }

        private bool IsStringEmpty(string text)
        {
            return text == null || text == "";
        }
        #endregion
    }
}
