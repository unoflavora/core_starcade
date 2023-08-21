using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Utilities;
using HutongGames.PlayMaker.Actions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Agate.Starcade.Runtime.Helper
{
	public static class LoadSceneHelper
    {
        public static UnityEvent OnBeforeLoad;

        private static SceneData currentActiveSceneData;

        private static List<Scene> listActiveScene;

        private static InitAdditiveBaseData DataPassFromScene;

        private static UnityEvent Event;

        private static List<UnityAction> Action;

        public static void Setup()
        {
            OnBeforeLoad = new UnityEvent();
            listActiveScene = new List<Scene>();
        }
        
        public static async void LoadScene(SceneData sceneData, bool autoCloseLoadingScreen = true, bool waitRotate = true)
        {
            OnBeforeLoad.Invoke();

            if(waitRotate) await CheckAndWaitToRotateAsync(sceneData);

            await MainSceneController.Instance.Loading.StartLoadingInfoDelay(sceneData.SceneKey, sceneData.Background, sceneData.TargetSceneOrientation);
            SceneManager.activeSceneChanged += (scene1, scene2)=>
            {   
                if (autoCloseLoadingScreen)
                {
					MainSceneController.Instance.Loading.DoneLoading();
				}
                OnLoadSceneArcadeComplete(scene1, scene2);
			};
            currentActiveSceneData = sceneData;
			if (sceneData.IsAddressable)
			{
				Debug.Log($"Load Scene via adressable {sceneData.SceneReference}");
				Addressables.LoadSceneAsync(sceneData.SceneReference, LoadSceneMode.Single);
			}
			else
			{
				Debug.Log($"Load Scene normal {sceneData.ScenePath}");
				SceneManager.LoadSceneAsync(sceneData.ScenePath);
			}
		}
        
        public static async void LoadSceneArcade(string slug, SceneData sceneData)
        {
            OnBeforeLoad.Invoke();
			await CheckAndWaitToRotateAsync(sceneData);

			await MainSceneController.Instance.Loading.StartLoadingInfoDelay(slug, sceneData.Background, sceneData.TargetSceneOrientation);
            //await MainSceneLauncher.Instance.Loading.StartLoadingDelay(LoadingScreen.LOADING_TYPE.LoadingInfo, sceneData.Background);
            //SceneManager.activeSceneChanged += OnLoadComplete;
            currentActiveSceneData = sceneData;
			if (sceneData.IsAddressable)
			{
				Debug.Log($"Load Scene via adressable {sceneData.SceneReference}");
				Addressables.LoadSceneAsync(sceneData.SceneReference, LoadSceneMode.Single);
			}
			else
			{
				Debug.Log($"Load Scene normal {sceneData.ScenePath}");
				SceneManager.LoadSceneAsync(sceneData.ScenePath);
			}
		}
        
        
        public static async Task CheckAndWaitToRotateAsync(SceneData sceneData)
        {
			Debug.Log(GetCurrentOrientation());
			if (sceneData.TargetSceneOrientation != GetCurrentOrientation())
			{
				await MainSceneController.Instance.Loading.StartRotate();
				switch (sceneData.TargetSceneOrientation)
				{
					case SceneData.SceneOrientation.Portrait:
						await WaitRotateToPortrait();
						break;
					case SceneData.SceneOrientation.Landscape:
						await WaitRotateToLandscape();
						break;
				}
			}
			else
			{
				if (currentActiveSceneData == null)
				{
					currentActiveSceneData = sceneData;
				}

				if (currentActiveSceneData.TargetSceneOrientation == sceneData.TargetSceneOrientation)
				{
					RotateOrientation(sceneData.TargetSceneOrientation);
				}
				else
				{
					await MainSceneController.Instance.Loading.StartRotate();
					RotateOrientation(sceneData.TargetSceneOrientation);
					await Task.Delay(1000);
					await MainSceneController.Instance.Loading.DoneRotate();
				}
			}
		}
        public static async void LoadScene(string ScenePath)
        {
            await MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.LoadingInfo, null, true);
            SceneManager.activeSceneChanged += OnLoadSceneArcadeComplete;
            SceneManager.LoadSceneAsync(ScenePath);
        }

        public static async void LoadSceneAdditive(string scenePath)
        {
            var loadScene = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            while (!loadScene.isDone) await Task.Yield();
        }

        public static async void LoadSceneAdditive(string scenePath, InitAdditiveBaseData data)
        {
            DataPassFromScene = data;
            var loadScene = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            while (!loadScene.isDone) await Task.Yield();
        }
        
        public static async void LoadSceneAdditive(string scenePath, InitAdditiveBaseData data, UnityAction action1, UnityAction action2)
        {
            Action = new List<UnityAction>();
            DataPassFromScene = data;
            Action.Add(action1);
            Action.Add(action2);
            var loadScene = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            while (!loadScene.isDone) await Task.Yield();
        }

        public static async void LoadSceneAdditive(string scenePath, UnityEvent openEvent)
        {
            Event = openEvent;
            var loadScene = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            while (!loadScene.isDone) await Task.Yield();
        }

        public static async void CloseSceneAdditive()
        {
            Debug.Log("total scene active " + listActiveScene.Count);
            var unloadSceneAsync = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount-1));
            if (unloadSceneAsync == null) return;
            while (!unloadSceneAsync.isDone) await Task.Yield();
        }
        
        public static async Task CloseSceneAdditiveAsync()
        {
	        Debug.Log("total scene active " + listActiveScene.Count);
	        var unloadSceneAsync = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount-1));
	        if (unloadSceneAsync == null) return;
	        while (!unloadSceneAsync.isDone) await Task.Yield();
        }

		public static async Task<Scene> LoadSceneAdditive(AssetReference assetRef)
		{
			var loadScene = Addressables.LoadSceneAsync(assetRef, LoadSceneMode.Additive);
			while (!loadScene.IsDone) await Task.Yield();
            return loadScene.Result.Scene;
		}

		public static async void LoadSceneAdditive(AssetReference assetRef, InitAdditiveBaseData data)
		{
			DataPassFromScene = data;
			var loadScene = Addressables.LoadSceneAsync(assetRef, LoadSceneMode.Additive);
			while (!loadScene.IsDone) await Task.Yield();
		}

		public static async void LoadSceneAdditive(AssetReference assetRef, UnityEvent openEvent)
		{
			Event = openEvent;
			var loadScene = Addressables.LoadSceneAsync(assetRef, LoadSceneMode.Additive);
			while (!loadScene.IsDone) await Task.Yield();
		}

		public static async void LoadSceneAdditive(AssetReference assetRef, InitAdditiveBaseData data, UnityAction action1, UnityAction action2)
		{
			Action = new List<UnityAction>();
			DataPassFromScene = data;
			Action.Add(action1);
			Action.Add(action2);
			var loadScene = Addressables.LoadSceneAsync(assetRef, LoadSceneMode.Additive);
			while (!loadScene.IsDone) await Task.Yield();
		}

		public static void PushData(InitAdditiveBaseData data)
        {
            DataPassFromScene = data;
        }

        public static InitAdditiveBaseData PullData()
        {
            return DataPassFromScene;
        }

        public static UnityEvent PullEvent()
        {
            Debug.Log($"Return Event {Event}");
            return Event;
        }
        
        public static List<UnityAction> PullAction()
        {
            Debug.Log($"Return Event {Event}");
            return Action;
        }

        public static void ClearData()
        {
            DataPassFromScene = default;
        }

        private static void OnLoadSceneArcadeComplete(Scene scene, Scene scene1)
        {
            Debug.Log("KEPANGGIL");

            //MainSceneController.Instance.Loading.DoneLoading();
            SceneManager.activeSceneChanged -= OnLoadSceneArcadeComplete;
        }
        
        private static async Task WaitRotateToPortrait()
        {
			await Task.Delay(1000);
//#if !UNITY_IOS
//			if (Input.deviceOrientation != DeviceOrientation.Unknown)
//            {
//				while (ScreenHelper.GetOrientation() is ScreenOrientation.LandscapeLeft or ScreenOrientation.LandscapeRight)
//				{
//					Debug.Log($"WAIT CHANGE TO PORTRAIT (Screen Orientation: {Screen.orientation} Device Orientation {Input.deviceOrientation})");
//					await Task.Delay(500);
//				}
//			}
//#else
//            await Task.Delay(1000);
//#endif

            RotateOrientation(SceneData.SceneOrientation.Portrait);
            await Task.Delay(1000);
            await MainSceneController.Instance.Loading.DoneRotate();
        }
        
        private static async Task WaitRotateToLandscape()
        {
			await Task.Delay(1000);
//#if !UNITY_IOS
//			if (Input.deviceOrientation != DeviceOrientation.Unknown)
//            {
//			    while (ScreenHelper.GetOrientation() is ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown)
//			    {
//				    Debug.Log($"WAIT CHANGE TO LANDSCAPE (Screen Orientation: {Screen.orientation} Device Orientation {Input.deviceOrientation})");
//				    await Task.Delay(500);
//			    }
//            }
//#else
//            await Task.Delay(1000);
//#endif

			RotateOrientation(SceneData.SceneOrientation.Landscape);
            await Task.Delay(1000);
            await MainSceneController.Instance.Loading.DoneRotate();
        }

        private static void RotateOrientation(SceneData.SceneOrientation sceneOrientation)
        {
            switch (sceneOrientation)
            {
                case SceneData.SceneOrientation.Landscape:
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    ScreenHelper.LastScreenOrientation = ScreenOrientation.LandscapeLeft;
                    // Screen.autorotateToLandscapeLeft = true;
                    // Screen.autorotateToLandscapeRight = true;
                    // Screen.autorotateToPortrait = false;
                    // Screen.autorotateToPortraitUpsideDown = false;
                    break;
                case SceneData.SceneOrientation.Portrait:
                    Screen.orientation = ScreenOrientation.Portrait;
					ScreenHelper.LastScreenOrientation = ScreenOrientation.Portrait;
					// Screen.autorotateToLandscapeLeft = false;
					// Screen.autorotateToLandscapeRight = false;
					// Screen.autorotateToPortrait = true;
					// Screen.autorotateToPortraitUpsideDown = true;
					break;
            }
        }

        public static SceneData.SceneOrientation GetCurrentOrientation()
        {
			switch (ScreenHelper.GetOrientation())
			{
				case ScreenOrientation.LandscapeLeft or ScreenOrientation.LandscapeRight:
					return SceneData.SceneOrientation.Landscape;
				case ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown:
					return SceneData.SceneOrientation.Portrait;
				default:
					return SceneData.SceneOrientation.Landscape;
			}
        }
    }
}
