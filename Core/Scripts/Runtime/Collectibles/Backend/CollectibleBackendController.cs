using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles.Response;
using Agate.Starcade.Scripts.Runtime.Model;
using Agate.Starcade.Scripts.Runtime.Utilities;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Backend
{
    public class CollectibleBackendController
    {
        private readonly GameBackendController _backendController;
        private readonly MainModel _mainModel;
        

        public CollectibleBackendController()
        {
            _backendController = MainSceneController.Instance.GameBackend;
            _mainModel = MainSceneController.Instance.Data;
        }
        
        public static void InitDummyData()
        {
            List<CollectibleSetData> dummyData = new List<CollectibleSetData>()
            {
               new CollectibleSetData()
               {
                   CollectibleSetName = "Default Set",
                   CollectibleSetId = "DummySet1",
                   CollectibleItems = new List<CollectibleItem>()
                   {
                       new CollectibleItem()
                       {
                           CollectibleItemId = "Dummy1",
                           CollectibleItemName = "Dummy Name 1",
                           Amount = 1,
                           Rarity = 3
                       },
                       new CollectibleItem()
                       {
                           CollectibleItemId = "Dummy2",
                           CollectibleItemName = "Dummy Name 2",
                           Amount = 1,
                           Rarity = 2
                       },
                       new CollectibleItem()
                       {
                           CollectibleItemId = "Dummy3",
                           CollectibleItemName = "Dummy Name 3",
                           Amount = 1,
                           Rarity = 1
                       }
                   }
               },
               new CollectibleSetData()
               {
                   CollectibleSetName = "Default Set 2",
                   CollectibleSetId = "DummySet2",
                   CollectibleItems = new List<CollectibleItem>()
                   {
                       new CollectibleItem()
                       {
                           CollectibleItemId = "SecondDummy1",
                           CollectibleItemName = "Dummy Second 1",
                           Amount = 2,
                           Rarity = 3
                       },
                       new CollectibleItem()
                       {
                           CollectibleItemId = "SecondDummy2",
                           CollectibleItemName = "Dummy Second 2",
                           Amount = 2,
                           Rarity = 2
                       },
                       new CollectibleItem()
                       {
                           CollectibleItemId = "SecondDummy3",
                           CollectibleItemName = "Dummy Second 3",
                           Amount = 2,
                           Rarity = 1
                       }
                   }
               },
               new CollectibleSetData()
               {
                   CollectibleSetName = "Default Set 3",
                   CollectibleSetId = "DummySet3",
                   CollectibleItems = new List<CollectibleItem>()
                   {
                       new CollectibleItem()
                       {
                           CollectibleItemId = "ThirdDummy1",
                           CollectibleItemName = "Dummy Third 1",
                           Amount = 0,
                           Rarity = 3
                       },
                       new CollectibleItem()
                       {
                           CollectibleItemId = "ThirdDummy2",
                           CollectibleItemName = "Dummy Third 2",
                           Amount = 0,
                           Rarity = 2
                       },
                       new CollectibleItem()
                       {
                           CollectibleItemId = "ThirdDummy3",
                           CollectibleItemName = "Dummy Third 3",
                           Amount = 0,
                           Rarity = 1
                       }
                   }
               }
            };
            
            MainSceneController.Instance.Data.CollectiblesData = dummyData;
            
            MainSceneController.Instance.Loading.DoneLoading();
        }
        
        public async Task<CollectibleUserRewardData> ClaimReward(string setId)
        {
            GenericResponseData<CollectibleUserRewardData> claimRewardResponse = null;
            
            AsyncUtility.StartLoadingSeq(() => claimRewardResponse != null);

            claimRewardResponse = await RequestHandler.Request(async () => await _backendController.ClaimReward(setId));
            

            if (claimRewardResponse.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(claimRewardResponse.Error.Code);
                return null;
            }

            return claimRewardResponse.Data;
        }

        public async Task<CollectibleItem> CombineCollectible(List<CollectibleItem> itemsToCombine, string itemsSetId)
        {
            GenericResponseData<CollectiblesCombineResponseData> combineResponse = null;
            
            AsyncUtility.StartLoadingSeq(() => combineResponse != null);
            
            var itemNames =  itemsToCombine.ConvertAll(itemToCombine => itemToCombine.GetItemId());
            
            string setId = itemsSetId;

            combineResponse = await RequestHandler.Request(async () => await _backendController.CombineCollectible(itemNames, setId));
            
            if (combineResponse.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(combineResponse.Error.Code);
                return null;
            }
            
            var userCollectibles = _mainModel.CollectiblesData;
            
            var resultItem = 
                userCollectibles
                .Find(setData => setData.CollectibleSetId == itemsSetId).CollectibleItems
                .Find(i => i.GetItemId() == combineResponse.Data.CollectibleItemId);
            
            return resultItem;
        }


        public async Task<List<CollectibleSetData>> AddCollectibles()
        {
            var addCollectiblesResponse = await _backendController.AddCollectibles();
            
            if (addCollectiblesResponse.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(addCollectiblesResponse.Error.Code);
                return null;
            }

            return addCollectiblesResponse.Data;
        }
    }
}