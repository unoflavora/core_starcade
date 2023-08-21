using System;
using System.Threading.Tasks;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Core.Runtime.Lobby;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Agate.Starcade.Core.Runtime.Lobby
{
    [CreateAssetMenu(fileName = "Poster", menuName = "Poster/OpenAdditiveScenePoster", order = 1)]
    [Serializable]
    public class OpenAdditiveScenePosterAction : ScriptableObject, IPosterAction
    {
        public bool IsComingSoon;
        public Sprite PosterSprite;
        public Object PosterActionInterface;
        public PosterActionEnum PosterAction;
        public PosterActionData PosterActionData;
        public Component Component;
        
        public async Task OnClickAction(PosterData posterData, Action clickAction)
        {
            InitAdditiveBaseData initAdditiveBaseData = new InitAdditiveBaseData()
            {
                Key = LobbyMenuEnum.Poster,
                OnClose = (s) => clickAction(),
            };
            LoadSceneHelper.LoadSceneAdditive(posterData.PosterActionData.PosterActionAssetData, initAdditiveBaseData);
        }

        public void OnClaimAction()
        {
            throw new NotImplementedException();
        }
    }
}