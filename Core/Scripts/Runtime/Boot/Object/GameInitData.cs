using System;
using Agate.Starcade.Scripts.Runtime.Data_Class;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    [Serializable]
    public class GameInitData
    {
        //TODO RENAME
        public UserData data;
        public PlayerBalance balance;
        public LobbyData lobby;
        public LobbyConfigData lobbyConfig;
        public GameVersionData GameVersion;
        public TermsAndCondition TermsAndCondition;
    }
}


