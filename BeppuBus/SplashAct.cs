using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
//using Xamarin.Geolocation;
using System.Threading;
using Android.Locations;
using System.IO;
using Android.Util;
using Android.Database.Sqlite;

namespace BeppuBus
{
	[Activity (NoHistory = true, Theme = "@android:style/Theme.DeviceDefault.Light.Dialog.NoActionBar", /*ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, */Label = "BeppuBus", MainLauncher = true)]					
	public class SplashAct : Activity, ILocationListener
	{
	
		private LocationManager _locMgr;
		private bool romanji;

		public void OnLocationChanged(Location location)
		{

			//Android.Widget.Toast.MakeText (this, String.Format ("Latitude = {0}, Longitude = {1}", location.Latitude, location.Longitude), Android.Widget.ToastLength.Short).Show ();	
			
			try{
				Double latGPS = location.Latitude;
				Double lonGPS = location.Longitude;


				DatabaseHelper dbHelper = new DatabaseHelper (this);
				try{
					dbHelper.createDataBase();
				}catch(IOException ioe) {
					throw new IOException ("Unable to create database \n" + ioe.Message.ToString() );
				}



				SQLiteDatabase db = dbHelper.ReadableDatabase;

				List<Stop> stopCollection = new List<Stop> ();

				string query = "SELECT * FROM Stop";
				var cursor = db.RawQuery (query, null);
				//StartManagingCursor (cursor);

				if (cursor.MoveToFirst ()) {
					do {
						Stop stop = new Stop();
						stop._id = cursor.GetInt(cursor.GetColumnIndex("_id"));
						stop.stop_name = cursor.GetString(cursor.GetColumnIndex("stop_name"));
						stop.stop_namej = cursor.GetString(cursor.GetColumnIndex("stop_namej"));
						stop.stop_lat = cursor.GetFloat(cursor.GetColumnIndex("stop_lat"));
						stop.stop_lon = cursor.GetFloat(cursor.GetColumnIndex("stop_lon"));

						stopCollection.Add(stop);
					} while(cursor.MoveToNext ());
				}


				List<getSetPos> stoplistPos = new List<getSetPos>();

				foreach (Stop s in stopCollection)
				{
					stoplistPos.Add(new getSetPos{place = s.stop_name, lat = s.stop_lat, lon = s.stop_lon, placej=s.stop_namej });
				}

				/*
				stoplistPos.Add(new getSetPos { place = "APU", lat = 33.33777, lon = 131.468594 }); //stop lists
				stoplistPos.Add(new getSetPos { place = "KamegawaEki", lat = 33.331773, lon = 131.493311 });
				stoplistPos.Add(new getSetPos { place = "Minamisugairiguchi", lat = 33.302935, lon = 131.502013 });
				stoplistPos.Add(new getSetPos { place = "Mochigahama", lat = 33.293629, lon = 131.502719 });
				stoplistPos.Add(new getSetPos { place = "BeppuKitahama", lat = 33.279528, lon = 131.50553 });
				stoplistPos.Add(new getSetPos { place = "BeppuEkimae", lat = 33.279120, lon = 131.500288 });
				*/

				//Mocking Locat: getSetPos currentPlace = new getSetPos { lat = latF, lon = lonF, place = "current" }; //create object from lonF latF (can mock locat here too)
				getSetPos currentPlace = new getSetPos { lat = latGPS, lon = lonGPS, place = "current" };
				List<tobeSorted> sortedPlace = new List<tobeSorted>();
				foreach (var dist in stoplistPos)
				{
					double container = distFinder.distanceWorker(dist, currentPlace);
					sortedPlace.Add(new tobeSorted { distanceKM = container, placeName = dist.place, placeNameJ=dist.placej});
				}
				IComparer<tobeSorted> reangleaw = new NoLambdahere ();
				sortedPlace.Sort (reangleaw);
				//textCheck.Text = string.Format ("{0}", sortedPlace [0].placeName);

				//Android.Widget.Toast.MakeText(this, string.Format("{0} is found nearby",sortedPlace[0].placeName), Android.Widget.ToastLength.Short).Show();
				var sendIntent1 = new Intent(this, typeof(ListAct));
				if (romanji == false){
					sendIntent1.PutExtra("FoundPlace", sortedPlace[0].placeNameJ);
				} else{
					sendIntent1.PutExtra("FoundPlace", sortedPlace[0].placeName);
				}
				sendIntent1.PutExtra("PlaceType", "gps");
				StartActivity (sendIntent1);
			}
			catch{
				Android.Widget.Toast.MakeText(this, "GPS is unavailable", Android.Widget.ToastLength.Short).Show();
				var sendIntent = new Intent(this, typeof(ListAct));
				sendIntent.PutExtra("PlaceType", "nogps");
				StartActivity (sendIntent);
			}
		
		
		}

		public void OnProviderDisabled(string provider)
		{
		}

		public void OnProviderEnabled(string provider)
		{
		}

		public void OnStatusChanged(string provider, Availability status, Bundle extras)
		{
	
		}


		protected override void OnPause()
		{
			base.OnPause();

			_locMgr.RemoveUpdates(this);
		}

		protected override void OnResume()
		{
			base.OnResume();

			Criteria locationCriteria = new Criteria();
			locationCriteria.Accuracy = Accuracy.Coarse;
			locationCriteria.PowerRequirement = Power.NoRequirement;

			string locationProvider = _locMgr.GetBestProvider(locationCriteria, true);

			if (!String.IsNullOrEmpty(locationProvider))
			{
				_locMgr.RequestLocationUpdates(locationProvider, 2000, 1, this);
			}
			else
			{
				Log.Warn("LocationDemo", "Could not determine a location provider.");
			}
		}

		private Button skipping;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView(Resource.Layout.Splash);
			this.skipping = FindViewById<Button> (Resource.Id.skipBTN);
			this.skipping.Click += Hello;


			var prefs = this.GetSharedPreferences("setting", FileCreationMode.Private);

			if(prefs.GetBoolean("romaji", true) == false ){
				this.romanji = false;
			}else{
				this.romanji = true;
			}

			if (prefs.GetBoolean ("gps", true) == true) {
				// use location service directly       
				_locMgr = GetSystemService (LocationService) as LocationManager;
			} else {
				StartActivity (typeof(ListAct));
			}





		}
		private void Hello(object sender, EventArgs e)
		{

			StartActivity (typeof(ListAct));

		}


		public class NoLambdahere : IComparer<tobeSorted>
		{
			public int Compare(tobeSorted x, tobeSorted y)
			{
				int compareDate = x.distanceKM.CompareTo(y.distanceKM);
				if (compareDate == 0)
				{
					return x.distanceKM.CompareTo(y.distanceKM);
				}
				return compareDate;
			}
		}

	











		public void gpsWorker()
		{


		}
		
}
}
	


