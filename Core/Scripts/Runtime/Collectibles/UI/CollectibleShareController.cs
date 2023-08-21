using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Core.Runtime.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI
{
    public class CollectibleShareController : MonoBehaviour
    {
        [SerializeField] private GameObject _sharePreview;
        [SerializeField] private GameObject _sharedObject;
        [SerializeField] private TextMeshProUGUI _setTitle;
        [SerializeField] private PoolableGridUI _screenshotGrid;
        [SerializeField] private ConfirmationPopup _previewScreen;
        public async Task Capture(List<CollectibleItem> data, string setName, UnityAction onFinishInteraction)
        {
            _sharedObject.SetActive(true);
            
            _setTitle.SetText(setName);
            
            _screenshotGrid.Draw(data);

            var screenshotImage = await GetComponent<RectCapture>().Capture(RectCapture.CaptureMode.Rect);

            await Task.Delay(1000);

            _sharedObject.SetActive(false);

            _sharePreview.SetActive(true);

            _previewScreen.AddListenerOnConfirmation(() =>
            {
                ShareSocial(screenshotImage, setName);
                onFinishInteraction?.Invoke();
            });
            
            _previewScreen.AddListenerOnCancel(() =>
            {
                _sharePreview.SetActive(false);
                onFinishInteraction?.Invoke();
            });
        }
        
        private void ShareSocial(Texture2D screenshotImage, string setName)
        {
            if (screenshotImage != null)
            {
                NativeShare nativeShare = new NativeShare();

                // Add the file to share
                nativeShare.AddFile(screenshotImage);

                // Set the share title and message (optional)
                nativeShare.SetTitle("Check out my image!");

                nativeShare.SetText($"Look at my incredible {setName} collections! Play Starcade so you can get them too!!");

                nativeShare.SetCallback((NativeShare.ShareResult result, string shareTarget) =>
                {
                    Debug.Log(Enum.GetName(result.GetType(), result));
                    Debug.Log(shareTarget);
                    _sharePreview.SetActive(false);
                });

                // Share the file
                nativeShare.Share();
            }
        }
    }
}