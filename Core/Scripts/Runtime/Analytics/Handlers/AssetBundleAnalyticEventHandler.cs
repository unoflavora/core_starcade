using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class AssetBundleAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{
			public const string MAIN_ASSET_DOWNLOAD_START_EVENT = "main_asset_download_start";
			public const string MAIN_ASSET_DOWNLOAD_COMPLETE_EVENT = "main_asset_download_complete";
			public const string MAIN_ASSET_DOWNLOAD_FAILED_EVENT = "main_asset_download_failed";


		}


		public AssetBundleAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }

		public void TrackMainAssetDownloadStartEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.MAIN_ASSET_DOWNLOAD_START_EVENT);
		}

		public void TrackMainAssetDownloadCompleteEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.MAIN_ASSET_DOWNLOAD_COMPLETE_EVENT);
		}
		public void TrackMainAssetDownloadFailedEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.MAIN_ASSET_DOWNLOAD_FAILED_EVENT);
		}
	}
}
