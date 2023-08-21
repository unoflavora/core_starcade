using TMPro;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.UI.Tooltips
{
    public class PetTooltip : MonoBehaviour
    {
        [SerializeField] private FadeTween _tween;
        [SerializeField] private TextMeshProUGUI _text;

        public void Show(string text)
        {
            
            _text.text = text;
            _tween.FadeIn();

        }
        
        public void Hide()
        {
            _tween.FadeOut();
        }
    }
}