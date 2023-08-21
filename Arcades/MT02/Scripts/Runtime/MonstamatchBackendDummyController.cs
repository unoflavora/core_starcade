﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Modules.Hexa.Pathfinding;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Starcade.Arcades.MT02.Scripts.Runtime
{
    public class MonstamatchBackendDummyController : IMonstamatchBackend
    {
        public async Task<MonstamatchInitData> GetInitData(GameModeEnum mode)
        {
            try
            {
                await Task.Delay(50);
                const string stringData = "{\"sessionId\":\"Spin0\",\"mode\":\"Gold\",\"startDate\":\"2023-02-07T03:06:49.259Z\",\"endDate\":\"2023-02-08T03:06:49.259Z\",\"isStarted\":true,\"game\":{\"jackpotProgress\":52,\"jackpotCount\":0,\"killMonsterCount\":0,\"remainingSpinChance\":0,\"jackpotsQueue\":null,\"currentPuzzleJackpotProgress\":[1,0,0,0,0,0],\"killMonsterProgress\":{\"state\":false,\"currentHealth\":20000,\"bulletsLeft\":10},\"spinSession\":{\"lcgKey\":1946844085,\"spinCount\":1,\"matchCount\":1,\"remainingMatchChance\":9,\"instantJackpotProgress\":{\"totalCollected\":0,\"claimedRewardIndex\":0,\"rewards\":[]},\"boardState\":[{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false}]}},\"balance\":{\"goldCoin\":813250,\"starCoin\":6000,\"starTicket\":0},\"config\":{\"cost\":0,\"sessionDuration\":1440,\"costCurrency\":\"GoldCoin\",\"rewardCurrency\":\"GoldCoin\",\"paytableConfig\":{\"width\":9,\"height\":7,\"maxMatchPerSpin\":10},\"symbols\":[{\"id\":\"Puzzle\",\"index\":0,\"percentage\":50,\"type\":0,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false}],\"specialCoinConfig\":[{\"amount\":0,\"percentage\":3.13},{\"amount\":1,\"percentage\":15.63},{\"amount\":2,\"percentage\":15.63},{\"amount\":3,\"percentage\":15.63},{\"amount\":4,\"percentage\":15.63},{\"amount\":5,\"percentage\":6.25},{\"amount\":6,\"percentage\":6.25},{\"amount\":7,\"percentage\":6.25},{\"amount\":8,\"percentage\":3.13},{\"amount\":9,\"percentage\":3.13},{\"amount\":10,\"percentage\":3.13},{\"amount\":11,\"percentage\":3.13},{\"amount\":12,\"percentage\":1003.13}],\"rewardSymbols\":[{\"type\":0,\"baseReward\":500,\"baseExpJackpot\":8},{\"type\":1,\"baseReward\":250,\"baseExpJackpot\":4}],\"jackpotConfig\":{\"totalProgress\":500,\"bonusExp\":50,\"roulettes\":[{\"index\":0,\"rewardType\":0,\"rewardCurrency\":\"GoldCoin\",\"rewardAmount\":50000,\"tier\":1,\"percentage\":0},{\"index\":1,\"rewardType\":0,\"rewardCurrency\":\"GoldCoin\",\"rewardAmount\":100000,\"tier\":1,\"percentage\":0},{\"index\":2,\"rewardType\":0,\"rewardCurrency\":\"GoldCoin\",\"rewardAmount\":150000,\"tier\":1,\"percentage\":0},{\"index\":3,\"rewardType\":0,\"rewardCurrency\":\"GoldCoin\",\"rewardAmount\":250000,\"tier\":1,\"percentage\":0},{\"index\":4,\"rewardType\":0,\"rewardCurrency\":\"GoldCoin\",\"rewardAmount\":500000,\"tier\":1,\"percentage\":0},{\"index\":5,\"rewardType\":1,\"rewardCurrency\":\"GoldCoin\",\"rewardAmount\":0,\"tier\":2,\"percentage\":500}]},\"instantJackpotConfig\":[{\"index\":1,\"target\":5,\"rewardType\":\"Mini\",\"rewardCurrency\":\"GoldCoin\",\"rewardAmount\":250000},{\"index\":2,\"target\":9,\"rewardType\":\"Minor\",\"rewardCurrency\":\"GoldCoin\",\"rewardAmount\":500000},{\"index\":3,\"target\":12,\"rewardType\":\"Mayor\",\"rewardCurrency\":\"GoldCoin\",\"rewardAmount\":1000000}],\"puzzleJackpotConfig\":{\"maxPuzzlePieces\":6,\"rewardAmount\":100000,\"rewardCurrency\":\"GoldCoin\"},\"killMonsterConfig\":{\"defaultBulletAmount\":10,\"health\":20000,\"parts\":[{\"index\":-1,\"type\":-1,\"damageAmount\":0},{\"index\":0,\"type\":0,\"damageAmount\":1000},{\"index\":1,\"type\":1,\"damageAmount\":750},{\"index\":2,\"type\":2,\"damageAmount\":750},{\"index\":3,\"type\":3,\"damageAmount\":500}],\"additionalBulletConfig\":[{\"index\":0,\"bulletAmount\":2,\"cost\":0,\"costCurrency\":\"GoldCoin\"},{\"index\":1,\"bulletAmount\":5,\"cost\":0,\"costCurrency\":\"GoldCoin\"},{\"index\":2,\"bulletAmount\":10,\"cost\":0,\"costCurrency\":\"GoldCoin\"}],\"rewards\":[{\"target\":20000,\"rewardAmount\":500000,\"rewardCurrency\":\"GoldCoin\",\"tier\":1},{\"target\":17999,\"rewardAmount\":750000,\"rewardCurrency\":\"GoldCoin\",\"tier\":2},{\"target\":14999,\"rewardAmount\":1000000,\"rewardCurrency\":\"GoldCoin\",\"tier\":3},{\"target\":4999,\"rewardAmount\":1500000,\"rewardCurrency\":\"GoldCoin\",\"tier\":4},{\"target\":0,\"rewardAmount\":2000000,\"rewardCurrency\":\"GoldCoin\",\"tier\":5}]},\"id\":\"default_gold\",\"mode\":\"Gold\"}}";
                var data = JsonConvert.DeserializeObject<MonstamatchInitData>(stringData);
                data.IsTutorial = true;
                Debug.Log($"anu [{data.SessionId}]");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public async Task<MonstamatchSpinData> Spin(string sessionId)
        {
            await Task.Delay(50);
            var stringData = sessionId switch
            {
                "Spin0" => "{\"startDate\":\"2023-02-07T03:06:49.259Z\",\"endDate\":\"2023-02-08T03:06:49.259Z\",\"isStarted\":true,\"spinSession\":{\"lcgKey\":207151741760,\"spinCount\":1,\"matchCount\":0,\"remainingMatchChance\":10,\"instantJackpotProgress\":{\"totalCollected\":0,\"claimedRewardIndex\":0,\"rewards\":[]},\"boardState\":[{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false}]},\"balance\":{\"goldCoin\":804000.0,\"starCoin\":6000.0,\"starTicket\":0.0},\"pay\":{\"type\":\"GoldCoin\",\"amount\":10000.0}}",
                
                "Spin1" => "{\"startDate\":\"2023-02-07T03:06:49.259Z\",\"endDate\":\"2023-02-08T03:06:49.259Z\",\"isStarted\":true,\"spinSession\":{\"lcgKey\":207151928756,\"spinCount\":1,\"matchCount\":0,\"remainingMatchChance\":10,\"instantJackpotProgress\":{\"totalCollected\":0,\"claimedRewardIndex\":0,\"rewards\":[]},\"boardState\":[{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Puzzle\",\"index\":0,\"percentage\":18.18,\"type\":0,\"isSpecial\":false},{\"id\":\"Puzzle\",\"index\":0,\"percentage\":18.18,\"type\":0,\"isSpecial\":false},{\"id\":\"Puzzle\",\"index\":0,\"percentage\":18.18,\"type\":0,\"isSpecial\":false},{\"id\":\"Puzzle\",\"index\":0,\"percentage\":18.18,\"type\":0,\"isSpecial\":false},{\"id\":\"Puzzle\",\"index\":0,\"percentage\":18.18,\"type\":0,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false}]},\"balance\":{\"goldCoin\":794000.0,\"starCoin\":6000.0,\"starTicket\":0.0},\"pay\":{\"type\":\"GoldCoin\",\"amount\":10000.0}}",
                
                "Spin2" => "{\"startDate\":\"2023-02-07T03:06:49.259Z\",\"endDate\":\"2023-02-08T03:06:49.259Z\",\"isStarted\":true,\"spinSession\":{\"lcgKey\":207152332572,\"spinCount\":1,\"matchCount\":0,\"remainingMatchChance\":10,\"instantJackpotProgress\":{\"totalCollected\":0,\"claimedRewardIndex\":0,\"rewards\":[]},\"boardState\":[{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Puzzle\",\"index\":0,\"percentage\":18.18,\"type\":0,\"isSpecial\":false},{\"id\":\"Puzzle\",\"index\":0,\"percentage\":18.18,\"type\":0,\"isSpecial\":false},{\"id\":\"Puzzle\",\"index\":0,\"percentage\":18.18,\"type\":0,\"isSpecial\":false},{\"id\":\"Puzzle\",\"index\":0,\"percentage\":18.18,\"type\":0,\"isSpecial\":false},{\"id\":\"Puzzle\",\"index\":0,\"percentage\":18.18,\"type\":0,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object2\",\"index\":2,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":true},{\"id\":\"Object5\",\"index\":5,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object5\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object1\",\"index\":1,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object4\",\"index\":4,\"percentage\":18.18,\"type\":1,\"isSpecial\":false},{\"id\":\"Object3\",\"index\":3,\"percentage\":18.18,\"type\":1,\"isSpecial\":false}]},\"balance\":{\"goldCoin\":784000.0,\"starCoin\":6000.0,\"starTicket\":0.0},\"pay\":{\"type\":\"GoldCoin\",\"amount\":10000.0}}",
                _ => null
            };
            var data = JsonConvert.DeserializeObject<MonstamatchSpinData>(stringData);
            return data;
        }

        public async Task<MonstamatchJackpotData> UseJackpot(string sessionId)
        {
            await Task.Delay(50);
            var stringData =
                "{\"index\":4,\"rewardType\":1,\"rewardCurrency\":\"GoldCoin\",\"rewardAmount\":0,\"tier\":2,\"percentage\":50}";
            var data = JsonConvert.DeserializeObject<MonstamatchJackpotData>(stringData);
            return data;
        }

        public async Task<MonstamatchMatchData> Match(string sessionId, List<Coordinate> paths)
        {
            await Task.Delay(50);
            return new MonstamatchMatchData();
        }

        public async Task<MonstamatchUseBulletData> UseBullet(string sessionId, int hitIndex)
        {
            await Task.Delay(50);
            return new MonstamatchUseBulletData();
        }

        public async Task BuyBullet(string sessionId, int amount)
        {
            await Task.Delay(50);
        }

        public async Task<MonstamatchUsePuzzleData> UsePuzzle(string sessionId)
        {
            return new MonstamatchUsePuzzleData();
        }

        public async Task<MonstamatchClaimRewardData> ClaimReward(string sessionId)
        {
            return new MonstamatchClaimRewardData();
        }
    }
}