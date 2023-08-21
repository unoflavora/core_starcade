using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Development;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TestingGround : MonoBehaviour
{
    public SimpleWebRequest WebRequest;
    public TMP_InputField UrlInputField;
    public Button GetSubmitWebRequest;

    public LoadingScreen LoadingScreen;

    private void Start()
    {
        GetSubmitWebRequest.onClick.AddListener(async () => await TestingLoadingGetApi());
    }
    
    async Task TestingLoadingGetApi()
    {
        MainSceneController.Instance.Loading.StartLoading();
        await WebRequest.CallGet(UrlInputField.text, null);
        var rand = Random.Range(0, 10);
        await Task.Delay(rand * 1000);
        if (rand >= 5)
        {
            //throw new SimpleErrorException();
        }
        MainSceneController.Instance.Loading.DoneLoading();
        
        
    }
}
