using System;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet;
using Agate.Starcade.Core.Runtime.Pet.MyPet.UI;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Core.Scripts.Runtime.UI;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.Core
{
	public class PetSceneController : MonoBehaviour
	{
		[Header("Interactions")]
		[SerializeField] private Button _closeButton;

		[Header("Tab Button")] 
		[SerializeField] ScrollableTabMenu _tabs;
		[SerializeField] private Toggle _myPetsTab;
		[SerializeField] private Toggle _petFragmentsTab;
		[SerializeField] private Toggle _petAlbumTab;
		[SerializeField] private Toggle _adventureBoxTab;
		[SerializeField] private Toggle _petHouseTab;
		private PetMenuAnalyticEventHandler _petAnalytic;
		private PetSceneTab _currentActiveTab;
		private bool _isLoaded;

		[Header("Additive Data")]
		private static InitAdditiveBaseData _additiveData;
		private static Func<Task> _closeAllAdditiveScene;

		private void Start()
		{
			_additiveData = LoadSceneHelper.PullData();

            MainSceneController.Instance.MainSceneManager.OnCloseScene = _additiveData.OnClose;

            _closeAllAdditiveScene = CloseAllAdditiveScenes;
		
			InitTabAnalytics();
		
			_closeButton.onClick.AddListener(OnCloseClicked);

			LoadAudio();
			
			if(_additiveData != null)
			{
				var subScene = _additiveData.Key;

                switch (subScene)
				{
					case LobbyMenuEnum.MyPets:
                        _tabs.SetActiveContent(_myPetsTab);
                        break;
                    case LobbyMenuEnum.Fragments:
                        break;
                    case LobbyMenuEnum.PetHouse:
                        break;
                    case LobbyMenuEnum.PetAlbum:
                        break;
                    case LobbyMenuEnum.AdventureBox:
						_tabs.SetActiveContent(_adventureBoxTab);
						break;
					default:
                        _tabs.SetActiveContent(_myPetsTab);
                        Debug.LogWarning("Subscene not supported, ignore it");
						break;
                }
            }
			else
			{
				Debug.Log("here");
                _tabs.SetActiveContent(_myPetsTab);
            }

			TriggerTransition();
		}

		private async void TriggerTransition()
		{
			if (_isLoaded) return;
			
			_isLoaded = true;

			while (MainSceneController.Instance.Loading.IsLoading) await Task.Yield();
			
			_tabs.GetComponent<CanvasTransition>().TriggerTransition();
		}

		private async void LoadAudio()
		{
			await MainSceneController.Instance.Audio.LoadAudioData("pethouse_audio");
		}
	
		private void InitTabAnalytics()
		{
			_petAnalytic = new PetMenuAnalyticEventHandler(MainSceneController.Instance.Analytic);
			
			
			_myPetsTab.onValueChanged.AddListener(arg0 =>
			{
				if (arg0)
				{
					_currentActiveTab = PetSceneTab.MyPets;
					_petAnalytic.TrackClickMyPetsTabEvent();
				}
			});
			
			_petFragmentsTab.onValueChanged.AddListener(arg0 =>
			{
				if (arg0)
				{
					_currentActiveTab = PetSceneTab.PetFragments;
					_petAnalytic.TrackClickPetFragmentTabEvent();
				}
			});
			_petAlbumTab.onValueChanged.AddListener(arg0 =>
			{
				if (arg0)
				{
					_currentActiveTab = PetSceneTab.PetAlbum;
					_petAnalytic.TrackClickPetAlbumTabEvent();
				}
			});
			_adventureBoxTab.onValueChanged.AddListener(arg0 =>
			{
				if (arg0)
				{
					_currentActiveTab = PetSceneTab.AdventureBox;
					_petAnalytic.TrackClickAdventureBoxTabEvent();
				}
			});
			_petHouseTab.onValueChanged.AddListener(arg0 =>
			{
				if (arg0)
				{
					_currentActiveTab = PetSceneTab.PetHouse;
					_petAnalytic.TrackClickPetHouseTabEvent();
				}
			});
		}
		
		private void OnCloseClicked()
		{
			BackToLobby(LobbyMenuEnum.Arcade);
		
			_petAnalytic.TrackClickClosePetMenuEvent(Enum.GetName(typeof(PetSceneTab), _currentActiveTab));
		}
	
		public static async void BackToLobby (LobbyMenuEnum menu)
		{
			await _closeAllAdditiveScene();
            
			LoadSceneHelper.CloseSceneAdditive();
            
			_additiveData.OnClose?.Invoke(menu);
		}
	
		private  async Task CloseAllAdditiveScenes()
		{
			var task = new TaskCompletionSource<bool>();
		
			for (int i = 0; i < _tabs.Contents.Count; i++)
			{
				var content = _tabs.Contents[i];
			
				content.SetActive(false);
			
				var additiveScene = content.GetComponent<ScrollableTabItemWithSceneAdditive>();

				if (additiveScene == null || additiveScene.IsSceneLoaded == false)
				{
					if (i  == _tabs.Contents.Count - 1) task.SetResult(true);
				
					continue;
				};

				while (additiveScene.IsSceneLoaded)
				{
					await Task.Yield();
				}
			
				task.SetResult(true);

				break;
			}

			await task.Task;
		}
	}
}
