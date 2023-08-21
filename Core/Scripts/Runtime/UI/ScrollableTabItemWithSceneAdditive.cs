 using System;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Scripts.Runtime.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
 using UnityEngine.UI;

 namespace Agate.Starcade.Core.Scripts.Runtime.UI
{
    public class ScrollableTabItemWithSceneAdditive : MonoBehaviour
    {
        [SerializeField] private AssetReference _sceneToLoad;
        [SerializeField] private Toggle _toggle;
        private bool _isLoaded { get; set; } = false;
        private Scene _scene { get; set; }

        public bool IsSceneLoaded => _isLoaded;

        private async void OnEnable()
        {
            if (_isLoaded) return;
            
            _isLoaded = true;
            
            _toggle.interactable = false;
            
            _scene = await LoadSceneHelper.LoadSceneAdditive(_sceneToLoad);
            
            if (!isActiveAndEnabled)
            {
                UnloadScene();
                return;
            }
            
            _toggle.interactable = true;
        }

        private void OnDisable()
        {
            UnloadScene();
        }

        private async void UnloadScene()
        {
            if (!_isLoaded || !_scene.isLoaded) return;
            
            _toggle.interactable = false;

			var unloadSceneAsync = SceneManager.UnloadSceneAsync(_scene);
            
			while (unloadSceneAsync != null && !unloadSceneAsync.isDone) await Task.Yield();
            
            _isLoaded = false;

            _toggle.interactable = true;
        }
    }
}