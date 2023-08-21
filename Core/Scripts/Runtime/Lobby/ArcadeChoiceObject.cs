using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class ArcadeChoiceObject : MonoBehaviour
    {
        [SerializeField] private Image _machineImage;
        [SerializeField] private Button _playButton;
        [SerializeField] private TMP_Text _playButtonText;
        [SerializeField] private GameObject _cooldownObject;
        [SerializeField] private TMP_Text _cooldownText;


        //[SerializeField] private GameObject _cooldownBlocker;
        //[SerializeField] private TMP_Text _cooldownText;
        private double _cooldown;

        public void InitChoiceObject(Sprite MachineSprite, UnityAction OnPlay, double Cooldown)
        {
			if (_machineImage != null) 
                _machineImage.sprite = MachineSprite;

            _cooldown = Cooldown;
            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(() =>
            {
                OnPlay();
                //MainSceneController.Instance.Audio.PlayFocusSfx(MainSceneController.AUDIO_KEY.BUTTON_PLAY);

			});
            //_cooldownObject.SetActive(false);
            //_playButton.interactable = true;
        }

        private void Update()
        {
            _playButtonText.text = "PLAY";
            if (_cooldown > 0)
            {
                //_cooldownObject.SetActive(true);
                //_playButton.interactable = false;
                _cooldown -= Time.deltaTime;
                TimeSpan time = TimeSpan.FromSeconds(_cooldown);
                //time.ToString(@"hh\:mm\:ss");
                _cooldownText.text = $"Session time : \n{time.ToString(@"hh")} hr {time.ToString(@"mm")} min";
				_cooldownObject.GetComponent<Image>().color = new Color32(220, 38, 38, 225);
			}
            else
            {
                //_cooldownObject.SetActive(false);
				_cooldownText.text = $"Start your session!";
                _cooldownObject.GetComponent<Image>().color = new Color32(19, 123, 210, 225);
				//_cooldownText.text = $"Start your session";
				//_playButton.interactable = true;
			}
        }
    }
}
