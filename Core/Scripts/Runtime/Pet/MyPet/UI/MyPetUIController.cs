using System.Threading;
using System.Threading.Tasks;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.House.UI;
using Agate.Starcade.Core.Runtime.Pet.MyPet.Modules;
using Agate.Starcade.Core.Runtime.Pet.MyPet.UI.Tooltips;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.UI
{
	public enum PetSceneTab
	{
		MyPets,
		PetFragments,
		PetAlbum,
		AdventureBox,
		PetHouse
	}

	public class MyPetUIController : MonoBehaviour
	{
		[Header("Modules")]
		[SerializeField] private PetTooltipsController _tooltips;
		[SerializeField] private PetStatusView _petStatus;
		[SerializeField] private PetLevelProgressView _petProgress;

		[Header("My Pets UI")] 
		[SerializeField] private TextMeshProUGUI _petName;
		[SerializeField] private Image _petSilhouette;
		[SerializeField] private GameObject _petUis;
		
		[Header("Pet Activity")]
		[SerializeField] private PetActivityView _petActivityView;
		[SerializeField] private Button _cancelAdventureButton;
		[SerializeField] private Button _claimRewardAdventureButton;
		[SerializeField] private Button _startAdventureButton;

		[Header("Interactables")] 
		[SerializeField] private Button _changePetButton;
		[SerializeField] private FadeTween _changePetErrorNotification;

		private bool _errorIsShown;

		#region User Pet View
		public void SetPetData(PetInventoryData pet, int petMaxLevel)
		{
			_petName.text = pet.Name;
			_petProgress.ResetProgress();
			_petProgress.DisplayLevelProgress(pet.ExperienceData, petMaxLevel);
		}
		public void SetUpPetStatus(PetInventoryData data)
		{
			_petStatus.SetupView(data, () => {});
		}

		public void DisplaySilhouette(bool active, Sprite sprite = null)
		{
			_petSilhouette.sprite = sprite;

			_petSilhouette.gameObject.SetActive(active);

			_petUis.SetActive(!active);
		}
		

		#endregion

		#region Pet Tooltip
		public void NextTooltip()
		{
			_tooltips.Next();
		}

		public void ResetTooltip()
		{
			_tooltips.ResetTooltip();
		}
		#endregion

		#region Interactables Event Register
		public void RegisterOnChangePetClick(UnityAction action)
		{
			_changePetButton.onClick.AddListener(action);
		}
		

		public void RegisterOnCancelAdventure(UnityAction action)
		{
			_cancelAdventureButton.onClick.AddListener(action);
		}

		public void RegisterOnClaimAdventureReward(UnityAction action)
		{
			_claimRewardAdventureButton.onClick.AddListener(action);
		}

		public void RegisterOnAdventureClick(UnityAction action)
		{
			_startAdventureButton.onClick.AddListener(action);
		}
		#endregion

		#region Change Pet
		private void EnableChangePet(bool enable)
		{
			foreach (var image in _changePetButton.GetComponentsInChildren<Image>())
			{
				image.color = enable ? _changePetButton.colors.normalColor : _changePetButton.colors.disabledColor;
			}
			
			foreach (var text in _changePetButton.GetComponentsInChildren<TextMeshProUGUI>())
			{
				text.color = enable ? _changePetButton.colors.normalColor : _changePetButton.colors.disabledColor;
			}
			
		}
		
				
		public async void DisplayErrorOnChangePet(string error)
		{
			if (_errorIsShown) return;

			_errorIsShown = true;

			var fadeCancellationToken = new CancellationTokenSource();
			
			_changePetErrorNotification.GetComponentInChildren<TextMeshProUGUI>().SetText(error);
			
			await _changePetErrorNotification.FadeInAsync(fadeCancellationToken.Token);
			
			await Task.Delay(1500);
			
			await _changePetErrorNotification.FadeOutAsync(fadeCancellationToken.Token);

			_errorIsShown = false;
		}
		

		#endregion

		#region Pet Activity
		public void SetCurrentActivity(string petName, PetAdventureData data)
		{
			_petActivityView.SetupCurrentActivity(petName, data);
			
			EnableChangePet(data.IsDispatched == false && data.Rewards == null);
		}

		public void RegisterOnAdventureFinished(UnityAction action)
		{
			_petActivityView.RegisterOnAdventureFinished(action);
		}
		#endregion
	}
}
