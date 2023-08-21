using System.Threading.Tasks;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules.UI
{
    public class HalfRadialProgressBarController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _vfx;
        [SerializeField] private bool _isUI;

        private float _previousAngle;

        private void Rotate(float angle)
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 180, angle);
        }

        public async Task FillProgressBar(float current, float maximum)
        {
            if (maximum > 0)
            {
                var o = gameObject;
                var previousAngle = o.transform.localEulerAngles.z;
                var nextAngle = current / maximum * 180;
                LeanTween.value(o, Rotate, previousAngle, nextAngle, 0.5f);
                if (!_isUI)
                {
                    await Fade();
                }
            }
        }
        
        public async Task FillProgressBar(float current, float maximum, float time)
        {
            if (maximum > 0)
            {
                var o = gameObject;
                var previousAngle = o.transform.localEulerAngles.z;
                var nextAngle = current / maximum * 180;
                LeanTween.value(o, Rotate, previousAngle, nextAngle, time);
                if (!_isUI)
                {
                    await Fade();
                }
            }
        }

        private async Task Fade()
        {
            SetSpriteAlpha(0);
            await Task.Delay(500);
            SetSpriteAlpha(1);
        }

        private void SetSpriteAlpha(float val)
        {
            if (_vfx != null)
                _vfx.color = new Color(1f, 1f, 1f, val);
        }
    }
}