using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Data.Enums;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Scriptable_Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Utility
{
    public class DaytimeTransitionHelper : MonoBehaviour
    {
        [SerializeField] private bool _currentDay;
        
        [SerializeField] private Image _background;
        [SerializeField] private Image _curtainBackgroundL;
        [SerializeField] private Image _curtainBackgroundR;
        [SerializeField] private Image _animatedCurtainL;
        [SerializeField] private Image _animatedCurtainR;
        [SerializeField] private Image _puzzleBackground;
        [SerializeField] private Image _reachCoinBackground;
        [SerializeField] private ArcadeSpriteData _arcadeSpriteData;
        [SerializeField] private Animator _DayAnimator;
        [SerializeField] private Animator _NightAnimator;

        public void SetTimeOfDay(DaytimeEnums daytimeEnums)
        {
            SetCurtains(daytimeEnums);
            SetObjects(daytimeEnums);
            SetCloud(daytimeEnums);
        }

        private void SetObjects(DaytimeEnums daytimeEnums)
        {
            _background.sprite = _arcadeSpriteData.GetBackgroundSprite(daytimeEnums);
            _puzzleBackground.sprite = _arcadeSpriteData.GetPuzzleBackground(daytimeEnums);
            _reachCoinBackground.sprite = _arcadeSpriteData.GetReachCoinBackground(daytimeEnums);
        }

        private void SetCurtains(DaytimeEnums daytimeEnums)
        {
            var curtain = _arcadeSpriteData.GetBackgroundCurtain(daytimeEnums);
            _curtainBackgroundL.sprite = curtain;
            _curtainBackgroundR.sprite = curtain;

            var movingCurtain = _arcadeSpriteData.GetMovingCurtain(daytimeEnums);
            _animatedCurtainL.sprite = movingCurtain;
            _animatedCurtainR.sprite = movingCurtain;
        }

        private void SetCloud(DaytimeEnums daytimeEnums)
        {
            _DayAnimator.gameObject.SetActive(daytimeEnums == DaytimeEnums.Day);
            _NightAnimator.gameObject.SetActive(daytimeEnums != DaytimeEnums.Day);

            if (daytimeEnums == DaytimeEnums.Day)
            {
                _DayAnimator.SetTrigger("Day");
                _NightAnimator.ResetTrigger("Night");
            }
            else
            {
                _DayAnimator.ResetTrigger("Day");
                _NightAnimator.SetTrigger("Night");

            }
        }
    }
}
