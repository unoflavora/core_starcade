using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agate.Starcade;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(FadeTween))]
public class ButtonNotificationPopup : MonoBehaviour
{
    private FadeTween _fade;
    private bool _errorIsShown;

    // Start is called before the first frame update
    void Start()
    {
        _fade = GetComponent<FadeTween>();
    }

    public async void DisplayError(string error)
    {
        if (_errorIsShown) return;

        _errorIsShown = true;

        var fadeCancellationToken = new CancellationTokenSource();
			
        _fade.GetComponentInChildren<TextMeshProUGUI>().SetText(error);
			
        await _fade.FadeInAsync(fadeCancellationToken.Token);
			
        await Task.Delay(1500);
			
        await _fade.FadeOutAsync(fadeCancellationToken.Token);
        
        fadeCancellationToken.Dispose();

        _errorIsShown = false;
    }
}
