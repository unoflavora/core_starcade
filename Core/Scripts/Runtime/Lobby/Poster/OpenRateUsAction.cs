using System;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Lobby
{
    public class OpenRateUsAction : IPosterAction
    {
#if UNITY_ANDROID || UNITY_EDITOR
		private Google.Play.Review.ReviewManager _reviewManager;
        private Google.Play.Review.PlayReviewInfo _playReviewInfo;
#endif

        public async Task OnClickAction(PosterData posterData,Action clickAction)
        {
#if UNITY_ANDROID || UNITY_EDITOR
			_reviewManager = new Google.Play.Review.ReviewManager();
            _playReviewInfo = await RequestReview();
			await LaunchReview();
#endif
        }
#if UNITY_ANDROID || UNITY_EDITOR
		private async Task<Google.Play.Review.PlayReviewInfo> RequestReview()
        {
			var requestFlowOperation = _reviewManager.RequestReviewFlow();

            while (!requestFlowOperation.IsDone)
            {
                await Task.Yield();
            }
            
            switch (requestFlowOperation.Error)
            {
                case Google.Play.Review.ReviewErrorCode.ErrorRequestingFlow:
                    Debug.LogError("[RATE US] Request error request flow");
                    return null;
                case Google.Play.Review.ReviewErrorCode.ErrorLaunchingFlow:
                    Debug.LogError("[RATE US] Request error launch flow");
                    return null;
                case Google.Play.Review.ReviewErrorCode.PlayStoreNotFound:
                    Debug.LogError("[RATE US] Play Store not found");
                    return null;
                case Google.Play.Review.ReviewErrorCode.NoError:
                    Debug.Log("[RATE US] Request flow success");
                    return requestFlowOperation.GetResult();
                default:
                    return null;
            }
            return null;
        }

        private async Task LaunchReview()
        {
            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            while (!launchFlowOperation.IsDone)
            {
                await Task.Yield();
            }
            switch (launchFlowOperation.Error)
            {
                case Google.Play.Review.ReviewErrorCode.ErrorRequestingFlow:
                    Debug.LogError("[RATE US] Request error request flow");
                    break;
                case Google.Play.Review.ReviewErrorCode.ErrorLaunchingFlow:
                    Debug.LogError("[RATE US] Request error launch flow");
                    break;
                case Google.Play.Review.ReviewErrorCode.PlayStoreNotFound:
                    Debug.LogError("[RATE US] Play Store not found");
                    break;
            }
            
            Debug.Log("[RATE US] Launch flow success");
        }
#endif

        public void OnClaimAction()
        {
            throw new System.NotImplementedException();
        }
    }
}