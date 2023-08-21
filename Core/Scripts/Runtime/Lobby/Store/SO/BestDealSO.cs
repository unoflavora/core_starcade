using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Starcade Scriptable Object/BestDeal SO")]
public class BestDealSO : ScriptableObject
{
    public string ItemId;

    public string CooldownText;
        public float Interval;

        public Color TextColor;
        public double Cost;
        public string CostAssetId;
        [HideInInspector] public Sprite CostSprite;

        public string BackgroundAssetId;
        [HideInInspector] public Sprite Background;
        public string LabelAssetId;
        [HideInInspector] public Sprite Label;
}