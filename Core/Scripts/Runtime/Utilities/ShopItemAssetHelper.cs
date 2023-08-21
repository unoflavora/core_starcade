using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using System.Collections;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Agate.Starcade
{
    public class ShopItemAssetHelper
    {
        public static Sprite GetAsset(RewardBase item)
        {
            string goldCoinId = "currency_default_goldcoin";
            string starCoinId = "currency_default_starcoin";
            string starTicketId = "currency_default_starticket";

            Sprite sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(item.Ref);
            if (sprite != null) return sprite;
            if (item.Type == RewardEnum.GoldCoin)
            {
                sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(goldCoinId);
            }
            else if (item.Type == RewardEnum.StarCoin)
            {
                sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(starCoinId);
            }
            else if (item.Type == RewardEnum.StarTicket)
            {
                sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(starTicketId);
            }
            else if (item.Type == RewardEnum.Pet)
            {
                var pet = JsonConvert.DeserializeObject<PetInventoryData>(item.RefObject.ToString());
                
                sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(pet.Id);
            }
            return sprite;
        }
    }
}
