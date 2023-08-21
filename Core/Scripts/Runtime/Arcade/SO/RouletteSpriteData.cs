using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Enums;
using UnityEngine;

[CreateAssetMenu(menuName = "Starcade Scriptable Object/ Roulette SO Background")]
public class RouletteSpriteData : ScriptableObject
{
    [Header("WheelBackground")]
    [Header("Gold")]
    public Sprite WheelGold;
    public Sprite OuterGlowGold;
    public Sprite InnerGlowGold;
    [Header("Star")]
    public Sprite WheelStar;
    public Sprite OuterGlowStar;
    public Sprite InnerGlowStar;

    [Header("Background")] [Header("Gold")]
    public Sprite[] BackgroundGold;
    [Header("Star")] 
    public Sprite[] BackgroundStar;
    
    [Header("Coin")] 
    [Header("Gold")] 
    public Sprite[] IconGold;
    [Header("Star")] 
    public Sprite[] IconStar;
    [Header("StarTicket")] 
    public Sprite[] IconTicket;
    public Sprite SkipStep;

    public Sprite GetWheelBackground(GameModeEnum type)
    {
        var sprite = type switch
        {
            GameModeEnum.Gold => WheelGold,
            GameModeEnum.Star => WheelStar,
            _ => null
        };
        return sprite;
    }
    
    public Sprite GetInnerGlow(GameModeEnum type)
    {
        var sprite = type switch
        {
            GameModeEnum.Gold => InnerGlowGold,
            GameModeEnum.Star => InnerGlowStar,
            _ => null
        };
        return sprite;
    }
    
    public Sprite GetOuterGlow(GameModeEnum type)
    {
        var sprite = type switch
        {
            GameModeEnum.Gold => OuterGlowGold,
            GameModeEnum.Star => OuterGlowStar,
            _ => null
        };
        return sprite;
    }
    
    //TODO: use enumerator
    public Sprite GetRewardIcon(string type, int tier)
    {
        Sprite sprite = null;
        switch (type)
        {
            case "GoldCoin":
                sprite = IconGold[tier-1];
                break;
            case "StarCoin":
                sprite = IconStar[tier-1];
                break;
            case "StarTicket":
                sprite = IconTicket[tier-1];
                break;
            default:
                return null;
        }
        return sprite;
    }
    
    public Sprite GetBackgroundIcon(string type, int tier)
    {
        Sprite sprite = null;
        switch (type)
        {
            case "GoldCoin":
                sprite = BackgroundGold[tier-1];
                break;
            case "StarCoin" :
            case "StarTicket":
                sprite = BackgroundStar[tier-1];
                break;
            default:
                return null;
        }
        return sprite;
    }
    
    
}
