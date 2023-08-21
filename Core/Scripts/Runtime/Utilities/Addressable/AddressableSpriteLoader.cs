using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Agate
{
    public class AddressableSpriteLoader : MonoBehaviour
    {
        // Assign in Editor
        public AssetReference reference;

        // Start the load operation on start
        void Start()
        {
            AsyncOperationHandle handle = reference.LoadAssetAsync<Sprite>();
            handle.Completed += Handle_Completed;
        }

        // Instantiate the loaded prefab on complete
        private void Handle_Completed(AsyncOperationHandle obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                Image image = GetComponent<Image>();
                image.sprite = (Sprite) obj.Result;
            }
            else
            {
                Debug.LogError($"AssetReference {reference.RuntimeKey} failed to load.");
            }
        }

        // Release asset when parent object is destroyed
        public void ReleaseAsset()
        {
            reference.ReleaseAsset();
        }
    }
}
