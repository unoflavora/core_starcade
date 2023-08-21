using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate
{
    public class ArcadeMilestoneController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _level;
        [SerializeField] Image[] _rewardIcons;
        [SerializeField] Button _closeButton;

        private void Start()
        {
            _closeButton.onClick.AddListener(() => SetVisible(false));
        }

        public void SetData(int level, string[] ids)
        {
            _level.text = $"LV. {level}";
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == null) continue;
                Sprite sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(ids[i]);
                if (sprite != null)
                {
                    _rewardIcons[i].sprite = sprite;
                    _rewardIcons[i].gameObject.SetActive(true);
                }
                else
                {
                    _rewardIcons[i].gameObject.SetActive(false);
                }
            }
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
