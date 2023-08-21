using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.Arcade.Modules.SO;
using TMPro;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules.UI.ToolTips
{
    public class ToolTipsPage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _pageTitle;
        [SerializeField] private ToolTipsItem _toolTipsItemsPrefab;

        public void SetToolTipsPage(ToolTipsPageData toolTipsPage)
        {
            _pageTitle.text = toolTipsPage.PageTitle;
            InstantiateToolTips(toolTipsPage.ToolTips);
        }

        public void InstantiateToolTips(List<ToolTipsData> toolTipsItems)
        {
            foreach(var toolTips in toolTipsItems)
            {
                var toolTipsItem = Instantiate(_toolTipsItemsPrefab);
                toolTipsItem.SetToolTips(toolTips.ToolTipSprite , toolTips.Description);
            }
        }
    }
}
