using System.Collections;
using Agate.Starcade.Runtime.Lobby;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Lobby.Store
{
    public abstract class LobbyStore : MonoBehaviour
    {
        [SerializeField] public StoreManager.StoreType Type;
    }
}