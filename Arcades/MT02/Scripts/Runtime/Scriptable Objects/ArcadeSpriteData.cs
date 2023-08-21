using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Data.Enums;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Starcade/Arcade/Monstamatch/ArcadeSpriteData")]
    public class ArcadeSpriteData : UnityEngine.ScriptableObject
    {
        public Sprite NightBackground;
        public Sprite DayBackground;
        public Sprite NightBackgroundCurtain;
        public Sprite DayBackgroundCurtain;
        public Sprite NightMovingCurtain;
        public Sprite DayMovingCurtain;
        public Sprite DayPuzzleBackground;
        public Sprite NightPuzzleBackground;
        public Sprite DayReachCoinBackground;
        public Sprite NightReachCoinBackground;

        public Sprite GetBackgroundSprite(DaytimeEnums time)
        {
            var sprite = time switch
            {
                DaytimeEnums.Day => DayBackground,
                DaytimeEnums.Night => NightBackground,
                _ => DayBackground
            };
            return sprite;
        }
        
        public Sprite GetBackgroundCurtain(DaytimeEnums time)
        {
            var sprite = time switch
            {
                DaytimeEnums.Day => DayBackgroundCurtain,
                DaytimeEnums.Night => NightBackgroundCurtain,
                _ => DayBackgroundCurtain
            };
            return sprite;
        }
        
        public Sprite GetMovingCurtain(DaytimeEnums time)
        {
            var sprite = time switch
            {
                DaytimeEnums.Day => DayMovingCurtain,
                DaytimeEnums.Night => NightMovingCurtain,
                _ => DayMovingCurtain
            };
            return sprite;
        }

        public Sprite GetPuzzleBackground(DaytimeEnums time)
        {
            return time == DaytimeEnums.Day ? DayPuzzleBackground : NightPuzzleBackground;
        }

        public Sprite GetReachCoinBackground(DaytimeEnums time)
        {
            return time == DaytimeEnums.Day ? DayReachCoinBackground : NightReachCoinBackground;
        }


    }
}
