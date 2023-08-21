using System;
using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Core
{
    public static class CollectibleActionController
    {
        public static Action OnConvertPinClicked;
        
        public static Action<List<CollectibleItem>> OnConvertPinPopupClicked;

        public static Action<Transform> OnPinClicked;

        public static Action OnClaimReward;
        
        public static Action<List<CollectibleItem>> OnConvertPinConfirmed;

        public static Action<List<CollectibleItem>> OnShareClicked;
        
        public static Action<CollectibleItem> OnSendPinClicked;
    }
}