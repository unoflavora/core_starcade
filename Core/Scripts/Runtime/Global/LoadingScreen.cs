using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate;
using Agate.Starcade;
using Agate.Starcade.Arcade.Plinko;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Helper;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using ScreenOrientation = Agate.SceneData.SceneOrientation;

//TODO FIX NAMESPACE

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Sprite _defaultLoadingInfoBackground;
    [SerializeField] private List<LoadingInfoData> _loadingInfoDatas;
    [SerializeField] private List<LoadingInfoCollectionSO> _loadingInfoCollection;
    
    [SerializeField] private LoadingInfoController _loadingInfoController;

    [SerializeField] private GameObject _titleScreenLoading;
    [SerializeField] private RectTransform _titleLoadingCircle;

    [SerializeField] private GameObject _rotateScreen;
    [SerializeField] private TMP_Text _currentOrientation;

    [SerializeField,Tooltip("In Millisecond")] private int _loadingDelay;
    private bool _isLoading;
    public bool IsLoading => _isLoading;

    public enum LOADING_TYPE
    {
        MiniLoading,
        LoadingInfo,
    }

    private void Update()
    {
        _currentOrientation.text = "Current = " +  Input.deviceOrientation;
    }

    public void StartLoading()
    {
        try
        {
            gameObject.SetActive(true);
            _loadingInfoController.gameObject.SetActive(true);
        }
        catch(StarcadeErrorException errorException)
        {
            Debug.LogError(errorException);
        }
    }

    //public void StartLoading(LOADING_TYPE loadingType)
    //{
    //    gameObject.SetActive(true);
    //    switch (loadingType)
    //    {
    //        case LOADING_TYPE.MiniLoading:
				//_titleScreenLoading.SetActive(true);
    //            break;
    //        case LOADING_TYPE.LoadingInfo:
    //            if (_loadingInfoController.isActiveAndEnabled) return;

    //            _loadingInfoController.gameObject.SetActive(true);
    //            _loadingInfoController.SetUp(_loadingInfoDatas[Random.Range(0,_loadingInfoDatas.Count)], _defaultLoadingInfoBackground, LoadSceneHelper.GetCurrentOrientation());
    //            break;
    //        default:
    //            _titleScreenLoading.SetActive(true);
    //            break;
    //    }
    //}

    public void StartLoadingInfo(string label)
    {
        var list = _loadingInfoCollection.Find(collection => collection.CollectionLabel == label).LoadingInfoDatas;
        _loadingInfoController.SetUp(list[Random.Range(0,list.Count)],null, LoadSceneHelper.GetCurrentOrientation());
    }
    
    public async Task StartLoadingInfoDelay(string label, Sprite loadingBackground, ScreenOrientation screenOrientation)
    {
		//if (_loadingInfoController.isActiveAndEnabled) return;

		gameObject.SetActive(true);
        _loadingInfoController.gameObject.SetActive(true);
        if (_loadingInfoCollection.Exists(collection => collection.CollectionLabel == label))
        {
            Debug.Log("loading data found "  + label);
            var list = _loadingInfoCollection.Find(collection => collection.CollectionLabel == label).LoadingInfoDatas;
            _loadingInfoController.SetUp(list[Random.Range(0,list.Count)],loadingBackground, screenOrientation);
            await Delay();
        }
        else
        {
            Debug.Log("loading data not found " + label);
            var list = _loadingInfoCollection.Find(collection => collection.CollectionLabel == "general").LoadingInfoDatas;
            _loadingInfoController.SetUp(list[Random.Range(0,list.Count)],loadingBackground, screenOrientation);
            await Delay();
        }
    }

    public void StartTitleLoading(LOADING_TYPE loadingType)
    {
        _titleLoadingCircle.anchoredPosition = new Vector3(0f, -130f, 0f);
        _titleLoadingCircle.localScale = new Vector3(0.8f, 0.8f, 1f);
        StartLoading(loadingType);
    }

    public async Task StartLoading(LOADING_TYPE loadingType, Sprite loadingBackground = null, bool useDelay = false)
    {
        gameObject.SetActive(true);
        
        _isLoading = true;
        
        switch (loadingType)
        {
            case LOADING_TYPE.MiniLoading:
                _titleScreenLoading.SetActive(true);
                break;
            case LOADING_TYPE.LoadingInfo:

				_loadingInfoController.gameObject.SetActive(true);
                _loadingInfoController.SetUp(_loadingInfoDatas[Random.Range(0, _loadingInfoDatas.Count)],loadingBackground, LoadSceneHelper.GetCurrentOrientation());
                break;
            default:
                _titleScreenLoading.SetActive(true);
                break;
        }

        if(useDelay) await Delay();
    }

    public void StartLoadingDownload()
    {
        if (_loadingInfoController.isActiveAndEnabled) return;
        _loadingInfoController.gameObject.SetActive(true);
        _loadingInfoController.SetUp(_loadingInfoDatas[Random.Range(0, _loadingInfoDatas.Count)], null, LoadSceneHelper.GetCurrentOrientation(),true);
    }

    public void UpdateLoadingDownloadIndicatorText(string text)
    {
        _loadingInfoController.UpdateDownloadIndicator(text);
    }

    public async Task StartRotate()
    {
        gameObject.SetActive(true);
        _rotateScreen.gameObject.SetActive(true);
    }
    
    public async Task DoneRotate()
    {
        _rotateScreen.gameObject.SetActive(false);
    }

    public void DoneLoading()
    {
        Debug.Log("done loading");
        _titleLoadingCircle.anchoredPosition = Vector3.zero;
        _titleLoadingCircle.localScale = Vector3.one;
        gameObject.SetActive(false);
        _titleScreenLoading.SetActive(false);
        _loadingInfoController.gameObject.SetActive(false);
        _rotateScreen.gameObject.SetActive(false);
        _isLoading = false;
    }

    private async Task Delay()
    {
        await Task.Delay(_loadingDelay);
    }
}
public static class LoadingCollectionLabel
{
    public const string General = "general";
    public const string PL01 = "plinko";
    public const string PL02 = "pl02";
    public const string MT01 = "match3";
    public const string MT02 = "monstamatch";
    public const string CP01 = "cp01";
    public const string CP02 = "cp02";
}
