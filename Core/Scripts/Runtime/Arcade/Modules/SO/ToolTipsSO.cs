using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules.SO
{
    [CreateAssetMenu(menuName = "Starcade/Arcade/ToolTips")]
    public class ToolTipsSO : UnityEngine.ScriptableObject
    {
        public List<ToolTipsPageData> Page;
    }

    [Serializable]
    public class ToolTipsData
    {
        public Sprite ToolTipSprite;
        [Multiline(40)]
        public string Description;
    }

    [Serializable]
    public class ToolTipsPageData
    {
        public string PageTitle;
        public List<ToolTipsData> ToolTips;
    }
}
