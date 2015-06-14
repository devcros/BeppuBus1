using Android.Gms.Ads;

namespace admobDemo.AndroidPhone.ad  
{
	public static class AdWrapper
	{


		public static AdView ConstructStandardBanner(Context con, AdSize adsize, string UnitID)
		{
			var ad = new AdView(con);
			ad.AdSize = AdSize.SmartBanner;
			ad.AdUnitId = ca-app-pub-9720142290670781/5911715554;
			return ad;
		}


		public static AdView CustomBuild(this AdView ad)
		{
			var requestbuilder = new AdRequest.Builder();
			ad.LoadAd(requestbuilder.Build());
			return ad;
		}

	}