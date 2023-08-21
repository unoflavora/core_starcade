using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.SO
{
    [CreateAssetMenu(menuName = "Starcade Scriptable Object/ Roulette SO")]
    public class ImageKeys : UnityEngine.ScriptableObject
    {
        //TODO: Use RewardSpriteData Instead
        [Header("Gold")]
        public Sprite GoldCoinTier1;
        public Sprite GoldCoinTier2;
        public Sprite GoldCoinTier3;
        public Sprite GoldCoinTier4;
        [Header("Star")]
        public Sprite StarCoinTier1;
        public Sprite StarCoinTier2;
        public Sprite StarCoinTier3;
        public Sprite StarCoinTier4;
        [Header("StarTicket")] 
        public Sprite StarTicketTier1;
        public Sprite StarTicketTier2;
        public Sprite StarTicketTier3;
        public Sprite StarTicketTier4;
        public Sprite SkipStep;
    }
}

