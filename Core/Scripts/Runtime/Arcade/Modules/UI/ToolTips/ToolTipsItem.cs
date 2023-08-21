using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules.UI.ToolTips
{
    public class ToolTipsItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _toolTipImage;
        public void SetToolTips(Sprite image, string description)
        {
            _toolTipImage.sprite = image;
            _descriptionText.text = description;
        }
    }
}
