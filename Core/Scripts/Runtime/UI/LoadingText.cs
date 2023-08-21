using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Agate
{
    public class LoadingText : MonoBehaviour
    {
        private TMP_Text loadingText;
        private int max = 5;

        private async void Awake()
        {
            loadingText = gameObject.GetComponent<TMP_Text>();
            loadingText.text = "Loading";
            await Loading();
        }

        private async Task Loading()
        {
            while (true)
            {
                if (loadingText.text.Length - 7 >= max)
                {
                    loadingText.text = "Loading";
                    await Task.Delay(300);
                }
                else
                {
                    loadingText.text += ".";
                    await Task.Delay(300);
                }
            }
        }
    }
}