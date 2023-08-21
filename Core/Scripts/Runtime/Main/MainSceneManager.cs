using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Agate.Starcade.Runtime.Lobby.StoreManager;

namespace Agate.Starcade.Core.Runtime.Main
{
    public class MainSceneManager : MonoBehaviour
    {
        public enum MainMenuEnum
        {
            Lobby,
            EntryPoint,
            Mailbox,
            UserProfile,
            Pet
        }

        [SerializeField] private SceneData _lobbySceneData;
        private List<LobbyMenuEnum> _lobbyValidMenu = new List<LobbyMenuEnum>()
        { LobbyMenuEnum.Arcade, LobbyMenuEnum.Store, LobbyMenuEnum.Poster, LobbyMenuEnum.Pets , LobbyMenuEnum.StoreLootbox, LobbyMenuEnum.StorePetbox};

        [SerializeField] private AssetReference _userProfileScene;
        private List<LobbyMenuEnum> _userProfileValidMenu = new List<LobbyMenuEnum>()
        { LobbyMenuEnum.MyProfile, LobbyMenuEnum.Collection, LobbyMenuEnum.Friend, LobbyMenuEnum.Statistic };

        [SerializeField] private AssetReference _mailBoxScene;
        private List<LobbyMenuEnum> _mailBoxValidMenu = new List<LobbyMenuEnum>()
        { LobbyMenuEnum.Collect, LobbyMenuEnum.System, LobbyMenuEnum.Information, LobbyMenuEnum.Community };

        [SerializeField] private AssetReference _pet;
        private List<LobbyMenuEnum> _petValidMenu = new List<LobbyMenuEnum>()
        { LobbyMenuEnum.MyPets, LobbyMenuEnum.Fragments, LobbyMenuEnum.PetHouse, LobbyMenuEnum.PetAlbum, LobbyMenuEnum.AdventureBox };

        [SerializeField] private AssetReference _entryPoint;
        private List<LobbyMenuEnum> _entryPointValidMenu = new List<LobbyMenuEnum>()
        { LobbyMenuEnum.Achivement, LobbyMenuEnum.DailyMission, LobbyMenuEnum.DailyMission, LobbyMenuEnum.Setting };

        public OnCloseAdditive OnCloseScene;

        public void MoveToLobby(LobbyMenuEnum lobbyMenuEnum = LobbyMenuEnum.Arcade)
        {
            if (CheckMenuIsValid(MainMenuEnum.Lobby, lobbyMenuEnum))
            {
                LoadSceneHelper.PushData(new InitAdditiveBaseData { Key = lobbyMenuEnum });
                LoadSceneHelper.LoadScene(_lobbySceneData);
            }
            else
            {
                Debug.LogError("Lobby Menu Enum is invalid!");
            }
        }

        public void MoveToUserProfile(LobbyMenuEnum lobbyMenuEnum = LobbyMenuEnum.MyProfile)
        {
            if (CheckMenuIsValid(MainMenuEnum.UserProfile, lobbyMenuEnum))
            {
                LoadSceneHelper.LoadSceneAdditive(_userProfileScene, new InitAdditiveBaseData { Key = lobbyMenuEnum });
            }
            else
            {
                Debug.LogError("Lobby Menu Enum is invalid!");
            }
        }

        public async void MoveToPet(LobbyMenuEnum lobbyMenuEnum = LobbyMenuEnum.MyPets, bool isSameScene = false, UnityAction action = null)
        {
            Debug.Log("clicked");

            if (isSameScene) 
            {
                await LoadSceneHelper.CloseSceneAdditiveAsync();
                await LoadSceneHelper.CloseSceneAdditiveAsync();
            }

            if (CheckMenuIsValid(MainMenuEnum.Pet, lobbyMenuEnum))
            {
                LoadSceneHelper.LoadSceneAdditive(_pet, new InitAdditiveBaseData 
                { 
                    Key = lobbyMenuEnum,
                    OnClose = OnCloseScene
                });
            }
            else
            {
                Debug.LogError("Lobby Menu Enum is invalid!");
            }
        }

        public void MoveToStore(StoreType storeCategoryTypeEnum = StoreType.General)
        {
            LoadSceneHelper.PushData(new InitAdditiveBaseData { Key = LobbyMenuEnum.Store, Data = storeCategoryTypeEnum });
            LoadSceneHelper.LoadScene(_lobbySceneData);
        }


        bool CheckMenuIsValid(MainMenuEnum mainMenuEnum, LobbyMenuEnum lobbyMenuEnum)
        {
            switch (mainMenuEnum)
            {
                case MainMenuEnum.Lobby:
                    return _lobbyValidMenu.Contains(lobbyMenuEnum);
                case MainMenuEnum.EntryPoint:
                    return _entryPointValidMenu.Contains(lobbyMenuEnum);
                case MainMenuEnum.Mailbox:
                    return _mailBoxValidMenu.Contains(lobbyMenuEnum);
                case MainMenuEnum.UserProfile:
                    return _userProfileValidMenu.Contains(lobbyMenuEnum);
                case MainMenuEnum.Pet:
                    return _petValidMenu.Contains(lobbyMenuEnum);
                default: 
                    return false;
            }
        }
    }
}