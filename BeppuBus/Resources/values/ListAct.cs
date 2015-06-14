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
using System.IO.IsolatedStorage;
using System.IO;

using Android.Database.Sqlite;

namespace BeppuBus
{
	[Activity ( Theme = "@style/AppTheme", /*ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, */Label = "BeppuBus", MainLauncher = false)]		
	public class ListAct : Activity 
	{
		protected override void OnResume()
		{
			base.OnResume(); 
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);


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


			SetContentView (Resource.Layout.Table);


			var prefs = this.GetSharedPreferences("setting", FileCreationMode.Private);
			bool romaji = prefs.GetBoolean ("romaji", true);


			ListView allList = FindViewById<ListView> (Resource.Id.listView2);
			allList.FastScrollEnabled = true;

			allList.Adapter = new ListAdapter1 (this, stopCollection, romaji );
			allList.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				string position = (e.Position + 1).ToString();

				var toMain = new Intent(this, typeof(Activity1));
				toMain.PutExtra("PlaceToMain", position);
				if (romaji == false){
					toMain.PutExtra("PlaceName", stopCollection[e.Position].stop_namej);
				}else{
					toMain.PutExtra("PlaceName", stopCollection[e.Position].stop_name);
				}
				toMain.PutExtra("TypeToMain", "nogps");
				StartActivity(toMain);


			};

			//Check GPS
			string gpsStopName = "Nearest: ";
			if (Intent.GetStringExtra ("FoundPlace") == null) {
				gpsStopName += "No GPS";
			} else {
				//Get id of stop found by GPS
				int position = stopCollection.FindIndex(s => s.stop_name == Intent.GetStringExtra("FoundPlace"));



				if (position > 3) {
					allList.SetSelection (position - 3);
				}

				gpsStopName += Intent.GetStringExtra ("FoundPlace");
				string[] test = new string[] { gpsStopName };
				ListView gpsList = FindViewById<ListView> (Resource.Id.listView1);
				gpsList.Adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItem1, test);



				gpsList.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
				{
					var toMain = new Intent(this, typeof(Activity1));
					toMain.PutExtra("PlaceToMain", (position+1).ToString());

					if (romaji == false){
						toMain.PutExtra("PlaceName", stopCollection[position].stop_namej);
					}else{
						toMain.PutExtra("PlaceName", stopCollection[position].stop_name);
					}

					toMain.PutExtra("TypeToMain", "gps");
					StartActivity(toMain);
				};
			}

		}

		// Menu Creation
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			menu.Add(0,0,0,"Settings");
			menu.Add(0,1,1,"About");
			return true;
		}
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case 0:
				StartActivity (typeof(SettingAct));
				return true;
				case 1: StartActivity (typeof(AboutActivity));
				return true;
				default:
				return base.OnOptionsItemSelected(item);
			}
		}

		
		/*
		protected override void OnListItemClick(ListView l, View v, int position, long id)
		{

			allInstances trans = new allInstances ();
			var t = placeAList[position];
			if (position == 0 && Intent.GetStringExtra ("PlaceType")=="gps") { //gps locat
				var toMain = new Intent(this, typeof(Activity1));
				toMain.PutExtra("PlaceToMain", trans.toFile(Intent.GetStringExtra ("FoundPlace"),1));
				toMain.PutExtra("TypeToMain", "gps");

				StartActivity (toMain);
			//	Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();
				//gpsWorker ();
				//StartActivity (typeof(RealMainAct));
			}
			else{
				var toMain = new Intent(this, typeof(Activity1));
				toMain.PutExtra("PlaceToMain", trans.toFile(t,1));
				toMain.PutExtra("TypeToMain", "nogps");
			
				StartActivity (toMain);
			}

		}
		*/




	}
}

