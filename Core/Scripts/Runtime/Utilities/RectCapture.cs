using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;
public class RectCapture : MonoBehaviour 
{
    public RectTransform _UiToCapture; // Assign the UI element which you wanna capture
    public Image _resultImage;
    
    private int _width; // width of the object to capture
    private int _height; // height of the object to capture
    
    // Update is called once per frame
    void Update () 
    {
        if (Input.GetMouseButtonDown (0)) 
        {
            //StartCoroutine(takeScreenShot ()); // screenshot of a particular UI Element.
        }
        if (Input.GetKeyDown (KeyCode.A)) 
        {
            ScreenCapture.CaptureScreenshot ("FullPageScreenShot.png");
        }
    }
    
    private IEnumerator TakeScreenShot(System.Action<Texture2D> callback = null)
    {
        yield return new WaitForEndOfFrame();
        
        // it must be a coroutine 
        Vector3[] corners = new Vector3[4];
        
        _UiToCapture.GetWorldCorners(corners);
        
        var startX = corners[0].x;
        
        var startY = corners[0].y;

        _width = (int)corners[3].x - (int)corners[0].x;
        
        _height = (int)corners[1].y - (int)corners[0].y;    

        Vector2 temp = _UiToCapture.transform.position;

        var tex = new Texture2D (_width, _height, TextureFormat.RGB24, false);
        
        tex.ReadPixels (new Rect(startX, startY, _width, _height), 0, 0);
        
        tex.Apply ();

        // Encode texture into PNG
        var bytes = tex.EncodeToPNG();
        
        Destroy(tex);

        #if UNITY_EDITOR
        // File.WriteAllBytes(Application.dataPath + "ScreenShot.png", bytes);
        //
        // Debug.Log(Application.dataPath + "ScreenShot.png");
        #endif
        
        string imgsrc = System.Convert.ToBase64String(bytes);
        Texture2D scrnShot = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        if(callback != null) callback(scrnShot);

        scrnShot.LoadImage(System.Convert.FromBase64String(imgsrc));
        
        if (_resultImage != null)
        {
            Sprite sprite = Sprite.Create(scrnShot, new Rect(0, 0, scrnShot.width, scrnShot.height), new Vector2(.5f, .5f));
            _resultImage.sprite = sprite;
        }
    }

    public async Task<Texture2D> Capture(CaptureMode mode)
    {
        Texture2D screenShot = null;

        if (mode == CaptureMode.Rect)
        {
            StartCoroutine(TakeScreenShot((tex) => screenShot = tex));
        }
        
        else
        {
            screenShot = ScreenCapture.CaptureScreenshotAsTexture();
        }
        
        while (screenShot == null) await Task.Yield();

        return screenShot;
    }
    
    public enum CaptureMode 
    {
        FullScreen, Rect
    }
}