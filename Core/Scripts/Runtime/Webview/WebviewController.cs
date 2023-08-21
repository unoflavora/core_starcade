using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Audio;
using UnityEngine.Events;
using Agate.Starcade.Runtime.Main;

namespace Agate.Starcade
{
    public class WebviewController : MonoBehaviour
    {
        [SerializeField] private WebViewObject _webViewObject;
        [SerializeField] private Button _closeButton;
        public string url { get; set; }
        public UnityAction onClose = () => { };
        private AudioController _audio;
        private GameObject _webview;
        private bool _isWebviewInit;

        private void Awake()
        {
            _closeButton.onClick.AddListener(OnClose);
            _audio = MainSceneController.Instance.Audio;
            SetVisible(false);
        }

        private void OnClose()
        {
            if (_webview != null) _webview.GetComponent<WebViewObject>().SetVisibility(false);
            _webViewObject.SetVisibility(false);
            SetVisible(false);
            gameObject.SetActive(false);
            onClose();
        }


        public void SetVisible(bool visible) 
        {
            _closeButton.gameObject.SetActive(visible);
            var panel = GetComponent<Image>();
            panel.enabled = visible;
        }

        public IEnumerator OpenWebView(string url)
        {
            this.url = url;
            //SetVisible(true);
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_LINUX
            Application.OpenURL(url);
            yield break;
#endif
            if (_isWebviewInit)
            {
                SetVisible(true);
                _webViewObject.SetVisibility(true);
                if (_webview != null) _webview.GetComponent<WebViewObject>().SetVisibility(true);
                _webViewObject.LoadURL(this.url.Replace(" ", "%20"));
                yield break;
            }
            _webViewObject.Init(
                cb: (msg) =>
                {
                    Debug.Log(string.Format("CallFromJS[{0}]", msg));
                    //status.text = msg;
                    //status.GetComponent<Animation>().Play();
                },
                err: (msg) =>
                {
                    Debug.Log(string.Format("CallOnError[{0}]", msg));
                    //status.text = msg;
                    //status.GetComponent<Animation>().Play();
                },
                httpErr: (msg) =>
                {
                    Debug.Log(string.Format("CallOnHttpError[{0}]", msg));
                    //status.text = msg;
                    //status.GetComponent<Animation>().Play();
                },
                started: (msg) =>
                {
                    Debug.Log(string.Format("CallOnStarted[{0}]", msg));
                },
                hooked: (msg) =>
                {
                    Debug.Log(string.Format("CallOnHooked[{0}]", msg));
                },
                ld: (msg) =>
                {
                    Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
#if UNITY_EDITOR_OSX || (!UNITY_ANDROID && !UNITY_WEBPLAYER && !UNITY_WEBGL)
                    _webViewObject.EvaluateJS(@"
                        if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                            window.Unity = {
                                call: function(msg) {
                                    window.webkit.messageHandlers.unityControl.postMessage(msg);
                                }
                            }
                        } else {
                            window.Unity = {
                                call: function(msg) {
                                    window.location = 'unity:' + msg;
                                }
                            }
                        }
                    ");
#elif UNITY_WEBPLAYER || UNITY_WEBGL
                webViewObject.EvaluateJS(
                    "window.Unity = {" +
                    "   call:function(msg) {" +
                    "       parent.unityWebView.sendMessage('WebViewObject', msg)" +
                    "   }" +
                    "};");
#endif
                    _webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
                }
            );
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            //webViewObject.bitmapRefreshCycle = 1;
#endif
            SetVisible(true);
            _webViewObject.SetMargins(
                (int)((Screen.width - 1200) * .5),
                (int)((Screen.height - 657) * .5),
                (int)((Screen.width - 1200) * .5), 
                (int)((Screen.height - 657) * .5)
            );
            _webViewObject.SetTextZoom(100);
            _webViewObject.SetVisibility(true);

#if !UNITY_WEBPLAYER && !UNITY_WEBGL
            if (url.StartsWith("http"))
            {
                _webViewObject.LoadURL(url.Replace(" ", "%20"));
            }
            else
            {
                var exts = new string[]{
                ".jpg",
                ".js",
                ".html"  // should be last
            };
                foreach (var ext in exts)
                {
                    var formatedUrl = url.Replace(".html", ext);
                    var src = System.IO.Path.Combine(Application.streamingAssetsPath, formatedUrl);
                    var dst = System.IO.Path.Combine(Application.persistentDataPath, formatedUrl);
                    byte[] result = null;
                    if (src.Contains("://"))
                    {  // for Android
                        var unityWebRequest = UnityWebRequest.Get(src);
                        yield return unityWebRequest.SendWebRequest();
                        result = unityWebRequest.downloadHandler.data;
                    }
                    else
                    {
                        result = System.IO.File.ReadAllBytes(src);
                    }
                    System.IO.File.WriteAllBytes(dst, result);
                    if (ext == ".html")
                    {
                        _webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
                        break;
                    }
                }
            }
#else
        if (Url.StartsWith("http")) {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
        } else {
            webViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
        }
#endif
            _webview = GameObject.Find("WebViewObject");
            _isWebviewInit = true;
            yield break;
        }
    }
}
