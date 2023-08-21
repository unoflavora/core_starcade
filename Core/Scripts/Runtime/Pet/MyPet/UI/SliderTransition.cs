using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.UI
{

    public class SliderTransition : Slider
    {
        public float TransitionTime = 1f;
        public void ValueTransition(float targetValue)
        {
            LeanTween.value(0, targetValue, 1f).setOnUpdate((float v) =>
            {
                value = v;
            }).setEaseOutSine();
        }

        public void ValueTransition(float min, float max ,float targetValue)
        {
            minValue = min;
            maxValue = max;
            LeanTween.value(min, targetValue, 1f).setOnUpdate((float v) =>
            {
                value = v;
            }).setEaseOutSine();
        }
    }
}