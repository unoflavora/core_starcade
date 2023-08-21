using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agate.Modules.Hexa.Pathfinding;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend.Data;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Newtonsoft.Json;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace Starcade.Arcades.MT02.Scripts.Runtime
{
    public class MonstamatchBackendController : IMonstamatchBackend
    {
        private WebRequestHelper _webRequestHelper;
        private string _gameBaseUrl;
        private UnityAction _quitGame;

        public MonstamatchBackendController(UnityAction quitGame)
        {
            _webRequestHelper = new WebRequestHelper();
            _gameBaseUrl = MainSceneController.Instance.EnvironmentConfig.GameBaseUrl;
            _quitGame = quitGame;
        }

        public async Task<MonstamatchInitData> GetInitData(GameModeEnum mode)
        {
            var bodyData = new Dictionary<string, string>();
            bodyData["mode"] = mode.ToString();
            var body = JsonConvert.SerializeObject(bodyData);
            var data = await Request<MonstamatchInitData>(body, "init");
            return data;
        }
        public async Task<MonstamatchSpinData> Spin(string sessionId)
        {  
            var bodyData = new Dictionary<string, string>();
            bodyData["sessionId"] = sessionId;
            var body = JsonConvert.SerializeObject(bodyData);

            return await Request<MonstamatchSpinData>(body, "spin");
        }
        
        public async Task<MonstamatchJackpotData> UseJackpot(string sessionId)
        {
            var bodyData = new Dictionary<string, string>();
            bodyData["sessionId"] = sessionId;
                
            var body = JsonConvert.SerializeObject(bodyData);

            return await Request<MonstamatchJackpotData>(body, "use-jackpot");
        }
        
        public async Task<MonstamatchMatchData> Match(string sessionId, List<Coordinate> paths)
        {
            var bodyData = new MonstamatchMatchMessageData();
            bodyData.SessionId = sessionId;
            bodyData.Path = paths.Select(s => new CoordinatePath(s)).ToList();
            
            var body = JsonConvert.SerializeObject(bodyData);
            Debug.Log("Sending this data: " + body);
           
            return await Request<MonstamatchMatchData>(body, "match");
        }

        public async Task<MonstamatchUseBulletData> UseBullet(string sessionId, int hitIndex)
        {
            var bodyData = new Dictionary<string, string>();
            bodyData["sessionId"] = sessionId;
            bodyData["hitIndex"] = "" + hitIndex;
            var body = JsonConvert.SerializeObject(bodyData);
            
            return await Request<MonstamatchUseBulletData>(body, "use-bullet");
        }
        
        public async Task BuyBullet(string sessionId, int index)
        {
            var bodyData = new Dictionary<string, string>();
            bodyData["sessionId"] = sessionId;
            bodyData["index"] = "" + index;
            var body = JsonConvert.SerializeObject(bodyData);
            
            await Request<object>(body, "buy-bullet");
        }

        public async Task<MonstamatchUsePuzzleData> UsePuzzle(string sessionId)
        {
            var bodyData = new Dictionary<string, string>();
            bodyData["sessionId"] = sessionId;
            var body = JsonConvert.SerializeObject(bodyData);
            
            return await Request<MonstamatchUsePuzzleData>(body, "use-puzzle-jackpot");
        }
        
        public async Task<MonstamatchClaimRewardData> ClaimReward(string sessionId)
        {
            var bodyData = new Dictionary<string, string>();
            bodyData["sessionId"] = sessionId;
            var body = JsonConvert.SerializeObject(bodyData);
            return await Request<MonstamatchClaimRewardData>(body, "claim-minigame-reward");
        }
        
        private Dictionary<string, string> CreateHeader()
        {
            var header = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + MainSceneController.Token },
                {
                    "Content-Type", "application/json; charset=utf-8"
                }
            };
            return header;
        }
        private async Task<T> Request<T>(string body, string endpoint)
        {
            try
            {
                var header = CreateHeader();
                var response = await RequestHandler.Request(async () =>
                    await _webRequestHelper.PostRequest<T>(_gameBaseUrl + "monsta-match/" + endpoint, header, body));
                
                if (response.Error != null)  throw new Exception(response.Error.Message);
                
                return response.Data;

            }
            catch (Exception error)
            {
                var isFinish = false;

                MainSceneController.Instance.Info.Show(error.Message, "Error", InfoIconTypeEnum.Error, new InfoAction("Quit Game", () => 
                {
                    isFinish = true;
                    _quitGame();
                }), null);

                await AsyncUtility.WaitUntil(() => isFinish);
                await Task.Delay(100);
                return default;
            }
        }
    }
}