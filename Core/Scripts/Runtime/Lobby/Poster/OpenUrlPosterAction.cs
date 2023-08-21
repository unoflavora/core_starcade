using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Lobby;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Info;
using Newtonsoft.Json;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Lobby
{
    public class OpenUrlPosterAction: IPosterAction
    {
        public async Task OnClickAction(PosterData posterData,Action clickAction)
        {
            Application.OpenURL(posterData.PosterActionData.PosterActionStringData);
            
            string data = PlayerPrefs.GetString("PosterState");
            List<PosterStateReward> list = new List<PosterStateReward>();
            list = JsonConvert.DeserializeObject<List<PosterStateReward>>(data);
            list.Find(reward => reward.RewardId == posterData.PosterId).IsDone = true;
            PlayerPrefs.SetString("PosterState", JsonConvert.SerializeObject(list));
            PlayerPrefs.Save();
            Debug.Log("SAVED "+ JsonConvert.SerializeObject(list));
            
            MainSceneController.Instance.Info.Show(posterData.InfoPopUpType, new InfoAction("Close", null), null);
        }

        public void OnClaimAction()
        {
            throw new System.NotImplementedException();
        }
    }
}