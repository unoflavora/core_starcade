using System.Collections;
using System.Collections.Generic;
using Agate.Starcade;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Main;
using DanielLochner.Assets.SimpleScrollSnap;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ArcadeChoiceController : MonoBehaviour
{
    //[SerializeField] private TMP_Text _choiceName;
    [SerializeField] private Image _choicePoster;
    [SerializeField] private Button _choiceButton;
    [SerializeField] private Button _scrollButton;
    [SerializeField] private Image _playIcon;

    [SerializeField] private Image _downloadIcon;
    [SerializeField] private GameObject _download;
    [SerializeField] private Slider _downloadBar;
    [SerializeField] private TextMeshProUGUI _downloadText;


    public Button ChoiceButton => _choiceButton;
    public Button ScrollButton => _scrollButton;
    
    [SerializeField] private GameObject _lockedPanel;
    [SerializeField] private GameObject _checkMark;

    [SerializeField] private FadeTween _checkMarkFadeTween;

    [SerializeField] private SlideInTween _hoverTween;

    private SimpleScrollSnap _simpleScrollSnap;
    public bool isComingSoon { get; set; }

    public void InitChoice(int indexChoice,ArcadeSO arcadeSo,SimpleScrollSnap scrollSnap)
    {
        if (arcadeSo.ArcadeIcon == null)
        {
            _lockedPanel.SetActive(true);
            _choicePoster.enabled = false; //COMING SOON POSTER
        }
        else
        {
            _choicePoster.sprite = arcadeSo.ArcadeIcon;
        }
        _choiceButton.interactable = false;
        _simpleScrollSnap = scrollSnap;
        _scrollButton.onClick.AddListener(() => MoveChoice(indexChoice));

        isComingSoon = false;
    }

    public void InitComingSoon(int indexChoice, SimpleScrollSnap scrollSnap)
    {
        _lockedPanel.SetActive(true);
        _choicePoster.enabled = false;
        _simpleScrollSnap = scrollSnap;
        _scrollButton.onClick.AddListener(() => MoveChoice(indexChoice));
    }

    public void Hover(bool choice)
    {
        if (choice)
        {
            _hoverTween.SlideIn();
            _checkMarkFadeTween.FadeIn();
            _choiceButton.interactable = true;
            _scrollButton.gameObject.SetActive(false);
            if (isComingSoon) _playIcon.enabled = false;
            else _playIcon.enabled = true;
        }
        else
        {
            _hoverTween.SlideOut();
            _checkMarkFadeTween.FadeOut();
            _choiceButton.interactable = false;
            _scrollButton.gameObject.SetActive(true);
            _playIcon.enabled = false;
        }
    }

    public void SetDownloadIconVisible(bool visible)
    {
        _downloadIcon.gameObject.SetActive(visible);
    }

    public void SetDownloadVisible(bool visible)
    {
        _download.SetActive(visible);
    }

    public void SetDownloadBar(float current, float total)
    {
        float progress = current / total;
        _downloadBar.value = progress;
        float roundCurrent = Mathf.Round(current / Mathf.Pow(10f, 5));
        float roundTotal = Mathf.Round(total / Mathf.Pow(10f, 5));
        _downloadText.text = $"{roundCurrent / 10f}MB / {roundTotal / 10f}MB";
    }

    private void MoveChoice(int index)
    {
        MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.BUTTON_GENERAL);
        _simpleScrollSnap.GoToPanel(index);
    }
}
