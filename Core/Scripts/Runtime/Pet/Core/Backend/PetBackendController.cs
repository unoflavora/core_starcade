using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Newtonsoft.Json;
using static Agate.Starcade.Runtime.Main.MainSceneController;

namespace Agate.Starcade.Core.Runtime.Pet.Core.Backend
{
    public class PetBackendController
    {
        private WebRequestHelper _webRequestHelper { get; set; }
        
        private readonly string _baseUrl;
        
        private static PetBackendController _instance;
        public static PetBackendController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PetBackendController();
                }

                return _instance;
            }
        }
        
        
        private PetBackendController()
        {
            _webRequestHelper = new WebRequestHelper();
            _baseUrl = MainSceneController.Instance.EnvironmentConfig.GameBaseUrl;
        }

        public async Task<GenericResponseData<SwitchData>> SwitchPet(string petUniqueId)
        {
            StartLoading();

            Dictionary<string, string> body = new Dictionary<string, string>()
            {
                { "petUniqueId", petUniqueId }
            };

            var result =
                 await HandleRequest(() => _webRequestHelper.PostRequest<SwitchData>(_baseUrl + "user-pet/switch", GetHeader(), JsonConvert.SerializeObject(body)));
            
            return result;
        }

        public async Task<GenericResponseData<PetAdventureData>> Dispatch(string adventureId)
        {
            StartLoading();
            
            Dictionary<string, string> body = new Dictionary<string, string>()
            {
                { "adventureId", adventureId }
            };
            
            var result = await HandleRequest(() => _webRequestHelper.PostRequest<PetAdventureData>(_baseUrl + "pet-adventure", GetHeader(), JsonConvert.SerializeObject(body)));
            
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }
        
        public async Task<GenericResponseData<PetCancelAdventureData>> CancelDispatch()
        {
            StartLoading();
            
            var result = await HandleRequest(() => _webRequestHelper.PostRequest<PetCancelAdventureData>(_baseUrl + "pet-adventure/cancel", GetHeader(), ""));
            
            MainSceneController.Instance.Loading.DoneLoading();
            return result;
        }

        public async Task<GenericResponseData<List<RewardBase>>> ClaimReward()
        {
            StartLoading();

            var result = await HandleRequest(() => _webRequestHelper.GetRequest<List<RewardBase>>(_baseUrl + "pet-adventure/claim-reward", GetHeader()));
            
            MainSceneController.Instance.Loading.DoneLoading();
            
            return result;
        }

        public async Task SkipAdventure()
        {
            StartLoading();
            var result = await HandleRequest(() => _webRequestHelper.PostRequest<GenericResponseData<object>>(_baseUrl + "pet-adventure/leap", GetHeader(), ""));
            
            MainSceneController.Instance.Loading.DoneLoading();
        }
        
        public async Task AddPet(string petId)
        {
            StartLoading();
            
            var result = await HandleRequest(() => _webRequestHelper.PostRequest<GenericResponseData<object>>(_baseUrl + "user-pet/grant-pet/" + petId, GetHeader(), ""));
            
            MainSceneController.Instance.Loading.DoneLoading();
        }

        public async Task<GenericResponseData<PetLevelUpData>> LevelUp(string petUniqueId, List<string> collectibleItemIds)
        {
            StartLoading();

            var data = new PetLevelUpBody()
            {
                CollectibleItems = collectibleItemIds,
                PetUniqueId = petUniqueId
            };
            
            var body = JsonConvert.SerializeObject(data);

            var result = await HandleRequest(() => _webRequestHelper.PostRequest<PetLevelUpData>(_baseUrl + "user-pet/level-up", GetHeader(),body ));

            
            MainSceneController.Instance.Loading.DoneLoading();
            
            return result;
        }
        
        public async Task<GenericResponseData<GiftPetData>> GiftPet(long friendCode, string petUniqueId)
        {
            StartLoading();
            var data = new GiftPetData()
            {
                FriendCode = friendCode,
                PetUniqueId = petUniqueId
            };
            var body = JsonConvert.SerializeObject(data);

            var result = await HandleRequest(() => _webRequestHelper.PostRequest<GiftPetData>(_baseUrl + "user-pet/gift", GetHeader(),body ));
            
            MainSceneController.Instance.Loading.DoneLoading();
            
            return result;

            
        }
        
        private static Dictionary<string, string> GetHeader()
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + Token },
                { "Content-Type", "application/json" }
            };
            return header;
        }

        private void StartLoading()
        {
            MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
        }

        public PetBackendData GetDummyPetData()
        {
            var json =
                "{\"data\":{\"pets\":[{\"id\":\"pet_default\",\"name\":\"Pokemon\",\"owned\":1,\"basicSkill\":{\"type\":\"AdventureGoldCoin\",\"amount\":1000}},{\"owned\":1,\"id\":\"pet_default_dummy\",\"name\":\"Pokemen\",\"basicSkill\":{\"type\":\"AdventureGoldCoin\",\"amount\":5000}}],\"activePet\":{\"uniqueId\":\"420b79cb-059a-4c60-b61d-713677993f3c\",\"adventureData\":{\"isDispatched\":false,\"type\":\"3Hour\",\"startDate\":\"2023-07-13T06:31:50.704Z\",\"endDate\":\"2023-07-13T09:31:50.696Z\",\"rewards\":[{\"type\":\"GoldCoin\",\"ref\":null,\"amount\":1000},{\"type\":\"SpecialBox\",\"ref\":\"abx_common\",\"amount\":1}]}},\"petInventory\":[{\"id\":\"pet_default\",\"uniqueId\":\"ef700106-0311-42d1-9ddb-03df2e969ad8\",\"isActive\":false,\"experienceData\":{\"currentExp\":0,\"level\":1,\"nextExp\":30,\"bottomExp\":0},\"obtainedDate\":\"2023-07-13T06:45:20.651Z\",\"basicSkill\":{\"type\":\"AdventureGoldCoin\",\"amount\":1000},\"subSkills\":[{\"id\":\"abx_rare\",\"level\":1,\"tier\":1,\"value\":4}]},{\"id\":\"pet_default_dummy\",\"uniqueId\":\"420b79cb-059a-4c60-b61d-713677993f3c\",\"isActive\":false,\"experienceData\":{\"currentExp\":0,\"level\":1,\"nextExp\":30,\"bottomExp\":0},\"obtainedDate\":\"2023-07-13T06:45:20.651Z\",\"basicSkill\":{\"type\":\"AdventureGoldCoin\",\"amount\":1000},\"subSkills\":[{\"id\":\"abx_rare\",\"level\":1,\"tier\":1,\"value\":4}]}],\"petConfigs\":{\"levelUpConfig\":{\"maxLevel\":50,\"expGainFormulas\":[{\"collectibleRarity\":1,\"expAmount\":1},{\"collectibleRarity\":2,\"expAmount\":2},{\"collectibleRarity\":3,\"expAmount\":5},{\"collectibleRarity\":4,\"expAmount\":10}],\"levelUpFormula\":{\"x\":10,\"y\":20}},\"fragmentConfig\":{\"maxPetCombined\":10},\"adventureConfigs\":[{\"id\":\"3Hour\",\"tier\":1,\"time\":180,\"possibleRewards\":[\"abx_common\",\"abx_rare\"]},{\"id\":\"6Hour\",\"tier\":2,\"time\":360,\"possibleRewards\":[\"abx_common\",\"abx_rare\",\"abx_epic\"]},{\"id\":\"9Hour\",\"tier\":3,\"time\":560,\"possibleRewards\":[\"abx_common\",\"abx_rare\",\"abx_epic\",\"abx_legendary\"]}],\"subSkillConfigs\":[{\"tier\":1,\"baseMultiplier\":4,\"id\":\"abx_rare\",\"levelupChanceRate\":70},{\"tier\":2,\"baseMultiplier\":2,\"id\":\"abx_epic\",\"levelupChanceRate\":25},{\"tier\":3,\"baseMultiplier\":1,\"id\":\"abx_legendary\",\"levelupChanceRate\":5}]},\"adventureData\":{\"isDispatched\":true,\"type\":\"3Hour\",\"startDate\":\"2023-07-13T06:31:50.704Z\",\"endDate\":\"2023-07-13T09:31:50.696Z\",\"rewards\":[{\"type\":\"GoldCoin\",\"ref\":null,\"amount\":1000},{\"type\":\"SpecialBox\",\"ref\":\"abx_common\",\"amount\":1}]},\"petFragmentInventory\":[{\"petId\":\"pet_default\",\"owned\":50,\"obtainedDate\":\"2023-07-13T06:45:16.85Z\",\"requirementAmount\":50}]},\"meta\":null,\"error\":null,\"message\":\"ok\",\"statusCode\":200}";
            
            var data = JsonConvert.DeserializeObject<GenericResponseData<PetBackendData>>(json);

            return data.Data;
        }

        public PetDispatchData DummyDispatch()
        {
            var json =
                "{\"data\":{\"petId\":\"pet_default\",\"uniqueId\":\"420b79cb-059a-4c60-b61d-713677993f3c\",\"startDate\":\"2023-07-14T06:31:50.704Z\",\"endDate\":\"2023-07-14T09:31:50.696Z\"}}";
            
            var data = JsonConvert.DeserializeObject<GenericResponseData<PetDispatchData>>(json);

            return data.Data;

        }

        private async Task<GenericResponseData<T>> HandleRequest<T>(Func<Task<GenericResponseData<T>>> func)
        {
            var res = await RequestHandler.Request(async () => await func());
            return res;
        }


    }
}