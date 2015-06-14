using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
//using Xamarin.Geolocation;
using System.Threading;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;
using System.Collections;
using Android.Database.Sqlite;
using System.Linq;
using System.Drawing;
using Android.Gms.Ads;  
using admobDemo;  
using admobDemo.AndroidPhone.ad; 

namespace BeppuBus
{
	[Activity (Theme = "@style/AppTheme", Label = "Timetable"/*, ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait*/)]
	public class Activity1 : Activity 
	{
		private int stop;
		private List<Timetable> timetableCollection;
		private List<DateTime> spSchoolDayCollection;
		private List<DateTime> spNonSchoolDayCollection;

		private List<Timetable> toAPU;
		private List<Timetable> toBep;
		private List<Timetable> toKT;
		private List<Timetable> toBep51;
		private List<Timetable> toAPU51;
		private List<Timetable> toExp55;

		private List<DateTime> toAPUD = new List<DateTime>();
		private List<DateTime> toBepD = new List<DateTime> ();
		private List<DateTime> toKTD = new List<DateTime>();
		private List<DateTime> toBep51D = new List<DateTime>();
		private List<DateTime> toAPU51D = new List<DateTime> ();
		private List<DateTime> toExp55D = new List<DateTime> ();

		private int isSchoolDay;

		//Countdown timer for refresher
		private int startin;
		System.Threading.Timer timer;

		//Resources declaration
		private LinearLayout lay1, lay2, lay3, lay0, closer, gpsicon;
		private TextView placeTBX, o7t, o8t, o9t, o10t, o11t, o12t, o13t, o14t, o15t, o16t, o17t, o18t, o19t, o20t, o21t, o22t, o23t, placeinthebox;
		private TextView timeAPUText,timeBepText,timeKTText,currentAPUText,currentBepText,currentKTText;

		//To Beppu51
		private LinearLayout lay4;
		private TextView timeBep51T, currentBep51T;

		//To APU51
		private LinearLayout lay5;
		private TextView timeAPU51T, currentAPU51T;

		//For Expresss55
		private LinearLayout lay6;
		private TextView timeExp55, currentExp55;

		protected override void OnCreate (Bundle bundle)
		{
			//Use Main as contentview
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			//Define components
			this.lay1 = FindViewById<LinearLayout> (Resource.Id.box1);
			this.lay2 = FindViewById<LinearLayout> (Resource.Id.box2);
			this.lay3 = FindViewById<LinearLayout> (Resource.Id.box3);
			this.lay0 = FindViewById<LinearLayout> (Resource.Id.box0);
			this.lay4 = FindViewById<LinearLayout> (Resource.Id.boxBep51);
			this.lay5 = FindViewById<LinearLayout> (Resource.Id.boxAPU51);
			this.lay6 = FindViewById<LinearLayout> (Resource.Id.boxExp55);

			this.lay1.Click += (object sender, EventArgs e) => { tablecreator(1);};
			this.lay2.Click += (object sender, EventArgs e) => { tablecreator(2);};
			this.lay3.Click += (object sender, EventArgs e) => { tablecreator(3);};
			this.lay4.Click += (object sender, EventArgs e) => { tablecreator(4);};
			this.lay5.Click += (object sender, EventArgs e) => { tablecreator(5);};
			this.lay6.Click += (object sender, EventArgs e) => { tablecreator(6);};

			this.gpsicon = FindViewById<LinearLayout> (Resource.Id.gpsframe);
			this.placeinthebox = FindViewById<TextView> (Resource.Id.timetableTex);
			this.o7t = FindViewById<TextView> (Resource.Id.SevenCon);
			this.o8t = FindViewById<TextView> (Resource.Id.EightCon);
			this.o9t = FindViewById<TextView> (Resource.Id.NineCon);
			this.o10t = FindViewById<TextView> (Resource.Id.TenCon);
			this.o11t = FindViewById<TextView> (Resource.Id.Ten1Con);
			this.o12t = FindViewById<TextView> (Resource.Id.Ten2Con);
			this.o13t = FindViewById<TextView> (Resource.Id.Ten3Con);
			this.o14t = FindViewById<TextView> (Resource.Id.Ten4Con);
			this.o15t = FindViewById<TextView> (Resource.Id.Ten5Con);
			this.o16t = FindViewById<TextView> (Resource.Id.Ten6Con);
			this.o17t = FindViewById<TextView> (Resource.Id.Ten7Con);
			this.o18t = FindViewById<TextView> (Resource.Id.Ten8Con);
			this.o19t = FindViewById<TextView> (Resource.Id.Ten9Con);
			this.o20t = FindViewById<TextView> (Resource.Id.Ten20Con);
			this.o21t = FindViewById<TextView> (Resource.Id.Ten21Con);
			this.o22t = FindViewById<TextView> (Resource.Id.Ten22Con);
			this.o23t = FindViewById<TextView> (Resource.Id.Ten23Con);

			this.closer = FindViewById<LinearLayout> (Resource.Id.linearLayout5);
			this.closer.Click += (object sender, EventArgs e) => {lay0.Visibility = ViewStates.Gone;};

			this.timeAPUText = FindViewById<TextView> (Resource.Id.timeAPUText);
			this.timeBepText = FindViewById<TextView> (Resource.Id.timeBepText);
			this.timeKTText = FindViewById<TextView> (Resource.Id.timeKTText);
			this.timeBep51T = FindViewById<TextView> (Resource.Id.timeBep51);
			this.timeAPU51T = FindViewById<TextView> (Resource.Id.timeAPU51);
			this.timeExp55 = FindViewById<TextView> (Resource.Id.timeExp55);

			this.currentAPUText = FindViewById<TextView> (Resource.Id.currentAPUText);
			this.currentBepText = FindViewById<TextView> (Resource.Id.currentBepText);
			this.currentKTText = FindViewById<TextView> (Resource.Id.currentKTText);
			this.currentBep51T = FindViewById<TextView> (Resource.Id.currentBep51);
			this.currentAPU51T = FindViewById<TextView> (Resource.Id.currentAPU51);
			this.currentExp55 = FindViewById<TextView> (Resource.Id.currentExp55);

			this.placeTBX = FindViewById<TextView> (Resource.Id.placeTB);
			this.placeTBX.Click += (object sender, EventArgs e) => {
				Finish();
			};  


			//Check whether there is gps intent or not?
			if (Intent.GetStringExtra ("TypeToMain") == "gps") 
			{
				gpsicon.Visibility = ViewStates.Visible;
				placeTBX.Text=string.Format("{0}", Intent.GetStringExtra("PlaceName") ?? "Data not available");
			}
			else
			{
				gpsicon.Visibility = ViewStates.Gone;
				placeTBX.Text=Intent.GetStringExtra("PlaceName") ?? "Data not available";
			}



			getData();



			LinearLayout schoolFrame = FindViewById<LinearLayout> (Resource.Id.schoolframe);
			TextView schoolTxt = FindViewById<TextView> (Resource.Id.txtSchoolDay);



			if (Intent.GetStringExtra ("ShowAnother") == "true") {

				if (Schoolday () == 0) {isSchoolDay = 1;}
				else {isSchoolDay = 0;}

				schoolFrame.Click += (object sender, EventArgs e) => {
					Finish();
				};
			} else {
				isSchoolDay = Schoolday ();
				schoolFrame.Click += (object sender, EventArgs e) => {
					Intent toAnother = new Intent (this, typeof(Activity1));
					toAnother.PutExtra ("PlaceToMain", Intent.GetStringExtra ("PlaceToMain"));
					toAnother.PutExtra ("TypeToMain", Intent.GetStringExtra ("TypeToMain"));
					toAnother.PutExtra("PlaceName", Intent.GetStringExtra("PlaceName"));
					toAnother.PutExtra ("ShowAnother", "true");
					StartActivity (toAnother);
				};

			}


			if (isSchoolDay == 0) {

				schoolFrame.SetBackgroundColor (Android.Graphics.Color.ParseColor ("#FF5252"));
				schoolTxt.Text = "NO SCHOOL";
			} else {
				//should be
				schoolFrame.SetBackgroundColor (Android.Graphics.Color.ParseColor ("#4CAF50"));
			}













			try{

					toAPU = timetableCollection.Where (t => t.bus == 50 || t.bus == 52 || t.bus == 53 || t.bus == 54).Where(t => t.type == 0).Where(t => t.school == 2 || t.school == isSchoolDay).ToList ();
					toBep = timetableCollection.Where (t => t.bus == 50).Where(t => t.type == 1).Where(t => t.school ==2 || t.school == isSchoolDay).ToList ();
					toKT = timetableCollection.Where (t => t.bus == 52).Where(t => t.type == 1).Where(t => t.school == 2 || t.school == isSchoolDay).ToList ();
					toBep51 = timetableCollection.Where(t=> t.bus==51).Where(t=>t.type == 1).Where(t => t.school == 2 || t.school == isSchoolDay).ToList();	
					toAPU51 = timetableCollection.Where(t=> t.bus==51).Where(t=>t.type == 0).Where(t => t.school == 2 || t.school == isSchoolDay).ToList();	
					toExp55 = timetableCollection.Where(t => t.bus == 55).Where(t => t.school == 2 || t.school == isSchoolDay).ToList();



					foreach (Timetable t in toAPU) {toAPUD.Add (t.time);}

					foreach (Timetable t in toBep) {toBepD.Add (t.time);}

					foreach (Timetable t in toKT) {toKTD.Add (t.time);}
					
					if (stop >= 25 && stop <= 30){foreach (Timetable t in toBep51){toBepD.Add(t.time);}}
					else{foreach (Timetable t in toBep51){toBep51D.Add(t.time);}}

					if (stop <= 19){foreach (Timetable t in toAPU51){toAPUD.Add(t.time);}}
					else {foreach (Timetable t in toAPU51){toAPU51D.Add(t.time);}}
					
					foreach (Timetable t in toExp55){toExp55D.Add(t.time);	}

				}catch{}



			//Hide some layouts
			lay0.Visibility = ViewStates.Gone;
			if (toAPUD.Count == 0) {lay1.Visibility = ViewStates.Gone;}
			if (toKTD.Count == 0) {lay2.Visibility = ViewStates.Gone;}
			if (toBepD.Count == 0) {lay3.Visibility = ViewStates.Gone;}
			if (toBep51D.Count == 0) {lay4.Visibility = ViewStates.Gone;}
			if (toAPU51D.Count == 0) {lay5.Visibility = ViewStates.Gone;}
			if (toExp55D.Count == 0) {lay6.Visibility = ViewStates.Gone;}

			//Refresher done at OnResume
		}

		private void getData()
		{
			List<Timetable> timetableCollection = new List<Timetable> ();

			DatabaseHelper dbHelper = new DatabaseHelper (this);
			SQLiteDatabase db = dbHelper.ReadableDatabase;
			string stop = Intent.GetStringExtra ("PlaceToMain") ?? "Data not Available";
			this.stop = int.Parse (stop);
			string query = "SELECT * FROM Timetable WHERE stop = " + stop;
			var cursor = db.RawQuery (query, null);
			if(cursor.MoveToFirst())
			{
				do {
					Timetable timetable = new Timetable();
					timetable._id = cursor.GetInt(cursor.GetColumnIndex("_id"));
					timetable.stop = cursor.GetInt(cursor.GetColumnIndex("stop"));
					timetable.type = cursor.GetInt(cursor.GetColumnIndex("type"));
					timetable.bus = cursor.GetInt(cursor.GetColumnIndex("bus"));
					timetable.school = cursor.GetInt(cursor.GetColumnIndex("school"));
					timetable.time = DateTime.Parse(cursor.GetString(cursor.GetColumnIndex("time")));

					timetableCollection.Add(timetable);
				} while(cursor.MoveToNext ());
				this.timetableCollection = timetableCollection;
			}

			List<DateTime> spNonSchoolDayCollection = new List<DateTime> ();

			query = "SELECT date FROM SchoolDay WHERE school = 0";
			cursor = db.RawQuery (query, null);
			if(cursor.MoveToFirst())
			{
				do {
					spNonSchoolDayCollection.Add(DateTime.Parse(cursor.GetString(cursor.GetColumnIndex("date"))));
				} while(cursor.MoveToNext ());
			}
			this.spNonSchoolDayCollection = spNonSchoolDayCollection;


			List<DateTime> spSchoolDayCollection = new List<DateTime> ();

			query = "SELECT date FROM SchoolDay WHERE school = 1";
			cursor = db.RawQuery (query, null);
			if(cursor.MoveToFirst())
			{
				do {
					spSchoolDayCollection.Add(DateTime.Parse(cursor.GetString(cursor.GetColumnIndex("date"))));
				} while(cursor.MoveToNext ());
			}
			this.spSchoolDayCollection = spSchoolDayCollection;
		}

		private void refresher()
		{
			//get current time
			DateTime currentTime = DateTime.Now;

			//try for force close preventation
			try
			{
				string format = "HH:mm";
				try { 
					DateTime closestTimeAPU = Convert.ToDateTime(FindClosestDate(toAPUD, currentTime).ToString()); 
					TimeSpan differentAPU = closestTimeAPU - currentTime; //カウントダウン専用
					timeAPUText.Text = string.Format("{0} Mins", Convert.ToInt32(differentAPU.TotalMinutes));
					currentAPUText.Text = string.Format("{0}", closestTimeAPU.ToString(format));
				}
				catch {
					timeAPUText.Text = "No Bus";
				}

				try
				{
					DateTime closestTimeBep = Convert.ToDateTime(FindClosestDate(toBepD, currentTime).ToString());
					TimeSpan differentBep = closestTimeBep - currentTime;　//カウントダウン専用
					timeBepText.Text = string.Format("{0} Mins", Convert.ToInt32(differentBep.TotalMinutes));
					currentBepText.Text = string.Format("{0}", closestTimeBep.ToString(format));
				}
				catch {
					timeBepText.Text ="No Bus";
				}

				try
				{
					DateTime closestTimeKT = Convert.ToDateTime(FindClosestDate(toKTD, currentTime).ToString());
					TimeSpan differentKT = closestTimeKT - currentTime;　//カウントダウン専用
					timeKTText.Text = string.Format("{0} Mins", Convert.ToInt32(differentKT.TotalMinutes));
					currentKTText.Text = string.Format("{0}", closestTimeKT.ToString(format));
				}
				catch {
					timeKTText.Text ="No Bus";
				}

				try
				{
					DateTime closestTimeB51 = Convert.ToDateTime(FindClosestDate(toBep51D, currentTime).ToString());
					TimeSpan differentB51 = closestTimeB51 - currentTime;　//カウントダウン専用
					timeBep51T.Text = string.Format("{0} Mins", Convert.ToInt32(differentB51.TotalMinutes));
					currentBep51T.Text = string.Format("{0}", closestTimeB51.ToString(format));
				}
				catch {
					timeBep51T.Text ="No Bus";
				}

				try
				{
					DateTime closestTimeA51 = Convert.ToDateTime(FindClosestDate(toAPU51D, currentTime).ToString());
					TimeSpan differentA51 = closestTimeA51 - currentTime;　//カウントダウン専用
					timeAPU51T.Text = string.Format("{0} Mins", Convert.ToInt32(differentA51.TotalMinutes));
					currentAPU51T.Text = string.Format("{0}", closestTimeA51.ToString(format));
				}
				catch {
					timeAPU51T.Text ="No Bus";
				}

				try
				{
					DateTime closestTimeE55 = Convert.ToDateTime(FindClosestDate(toExp55D, currentTime).ToString());
					TimeSpan differentE55 = closestTimeE55 - currentTime;　//カウントダウン専用
					timeExp55.Text = string.Format("{0} Mins", Convert.ToInt32(differentE55.TotalMinutes));
					currentExp55.Text = string.Format("{0}", closestTimeE55.ToString(format));
				}
				catch {
					timeExp55.Text ="No Bus";
				}

			}
			catch (System.Exception)
			{

			}
		}



		//timetable generator
		private void tablecreator(int input)
		{
			string format = "mm";
			//declare stringbuilders
			StringBuilder o7 = new StringBuilder();
			StringBuilder o8 = new StringBuilder();
			StringBuilder o9 = new StringBuilder();
			StringBuilder o10 = new StringBuilder();
			StringBuilder o11 = new StringBuilder();
			StringBuilder o12 = new StringBuilder();
			StringBuilder o13 = new StringBuilder();
			StringBuilder o14 = new StringBuilder();
			StringBuilder o15 = new StringBuilder();
			StringBuilder o16 = new StringBuilder();
			StringBuilder o17 = new StringBuilder();
			StringBuilder o18 = new StringBuilder();
			StringBuilder o19 = new StringBuilder();
			StringBuilder o20 = new StringBuilder();
			StringBuilder o21 = new StringBuilder();
			StringBuilder o22 = new StringBuilder();
			StringBuilder o23 = new StringBuilder();
			//load static list to bearchanger
			List<DateTime> bearchanger = new List<DateTime> ();

			switch(input)
			{
				case 1:
					bearchanger = toAPUD.OrderBy(x=>x.TimeOfDay).ToList();
					placeinthebox.Text="To APU";
					break;

				case 2:
					bearchanger = toKTD.OrderBy(x=>x.TimeOfDay).ToList();
					placeinthebox.Text="To Beppu Kotsu Center";
					break;
			
				case 3:
					placeinthebox.Text="To Beppu Station";
					bearchanger = toBepD.OrderBy(x=>x.TimeOfDay).ToList();
					break;
			
				case 4:
					placeinthebox.Text = "To Beppu Station (51)";
					bearchanger = toBep51D.OrderBy (x => x.TimeOfDay).ToList ();
					break;

				case 5:
					placeinthebox.Text = "To APU (51)";
					bearchanger = toAPU51D.OrderBy (x => x.TimeOfDay).ToList ();
					break;

				case 6:
					placeinthebox.Text = "APU Express";
					bearchanger = toExp55D.OrderBy (x => x.TimeOfDay).ToList ();
					break;
			}

			//store each hour into stringbuilders
			foreach (DateTime item in bearchanger) {
				if (item.Hour == 7) {
					o7.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 8) {
					o8.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 9) {
					o9.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 10) {
					o10.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 11) {
					o11.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 12) {
					o12.Append (string.Format("{0}  ",item.ToString(format)));
				}//test
				else if (item.Hour == 13) {
					o13.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 14) {
					o14.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 15) {
					o15.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 16) {
					o16.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 17) {
					o17.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 18) {
					o18.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 19) {
					o19.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 20) {
					o20.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 21) {
					o21.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 22) {
					o22.Append (string.Format("{0}  ",item.ToString(format)));
				}
				else if (item.Hour == 23) {
					o23.Append (string.Format("{0}  ",item.ToString(format)));
				}
			} 
			//display stringbuilders
			o7t.Text = o7.ToString ();
			o8t.Text = o8.ToString ();
			o9t.Text = o9.ToString ();
			o10t.Text = o10.ToString ();
			o11t.Text = o11.ToString ();
			o12t.Text = o12.ToString ();
			o13t.Text = o13.ToString ();
			o14t.Text = o14.ToString ();
			o15t.Text = o15.ToString ();
			o16t.Text = o16.ToString ();
			o17t.Text = o17.ToString ();
			o18t.Text = o18.ToString ();
			o19t.Text = o19.ToString ();
			o20t.Text = o20.ToString ();
			o21t.Text = o21.ToString ();
			o22t.Text = o22.ToString ();
			o23t.Text = o23.ToString ();
			//show the table panel
			lay0.Visibility = ViewStates.Visible;

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


		public int Schoolday()
		{
			DateTime date = new DateTime();
			date = DateTime.Now;

			//First check if date is in SP SchoolDay
			if (spSchoolDayCollection.Any(d => d.Date == date.Date))
			{
				return 1;
			}

			//Then, check if date is in SP nonSchool
			if (spNonSchoolDayCollection.Any (d => d.Date == date.Date)) {
				return 0;
			}

			//Return non-school on Aug, Sep, Feb, Mar
			if (date.Month == 2 ||date.Month == 3 ||date.Month == 8 ||date.Month == 9 ) {
				return 0;
			}

			//Finally check if it is weekday or weekend
			if (date.DayOfWeek != DayOfWeek.Saturday &&
			    date.DayOfWeek != DayOfWeek.Sunday)
			{
				return 1;
			}
			return 0;
		}

		public static DateTime? FindClosestDate(IEnumerable<DateTime> source, DateTime target)
		{
			DateTime? result = null;
			var lowestDifference = TimeSpan.MaxValue;

			foreach (var date in source)
			{
				if (date <= target)
					continue;

				var difference = date - target;

				if (difference < lowestDifference)
				{
					lowestDifference = difference;
					result = date;
				}
			}
			return result;
		}


		protected override void OnResume()
		{
			base.OnResume ();
			//start the timer
			refresher ();
			startin = 60 - DateTime.Now.Second;
			timer  = new System.Threading.Timer ((o) => {RunOnUiThread(()=> refresher());}, null, startin * 1000, 60000);
		}

		protected override void OnPause()
		{
			base.OnPause ();
			//Dispose old timer
			timer.Dispose ();
		}
	}
}


