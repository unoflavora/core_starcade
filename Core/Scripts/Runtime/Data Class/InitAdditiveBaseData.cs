using System;
using Agate.Starcade.Runtime.Enums;

namespace Agate.Starcade.Scripts.Runtime.Data_Class
{
    public delegate void OnCloseAdditive(LobbyMenuEnum menu = LobbyMenuEnum.Arcade); // After closing scene, where to go?

    public class InitAdditiveBaseData
    {
        public string SceneCallee;
        public OnCloseAdditive OnClose;
        public LobbyMenuEnum Key;
        public object Data;
    }
    
}