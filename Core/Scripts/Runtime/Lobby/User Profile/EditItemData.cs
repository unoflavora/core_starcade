using Agate.Starcade.Runtime.Enums;
using System;
using UnityEngine.UI;

namespace Agate.Starcade
{
    [Serializable]
    public class EditItemData
    {
        public ItemTypeEnum Type;
        public string Id;
        //public ItemSO DefaultItemSO;
        //public ItemSO ItemsSO;
        public Image PreviewImage;
        public ItemAccessoryData[] Data;
    }
}
