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
using System.IO.IsolatedStorage;
using System.IO;

namespace BeppuBus
{
	[Activity (NoHistory = true, Theme = "@android:style/Theme.DeviceDefault.Light.Dialog", /*ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, */Label = "Settings")]		
	public class SettingAct : Activity
	{
		private Button save;
		private Switch gpstog;
		private Switch romajitog;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Setting);
			this.gpstog = FindViewById<Switch> (Resource.Id.gpstoggle);
			this.romajitog = FindViewById<Switch> (Resource.Id.romajitoggle);
			this.save = FindViewById<Button> (Resource.Id.saveSet);
			this.save.Click += saveSetting;

			var prefs = this.GetSharedPreferences("setting", FileCreationMode.Private);
			this.gpstog.Checked = prefs.GetBoolean("gps",true);
			this.romajitog.Checked = prefs.GetBoolean("romaji",true);
		}

		private void saveSetting(object sender, EventArgs e)
		{
			var prefs = this.GetSharedPreferences("setting", FileCreationMode.Private);
			var editor = prefs.Edit();
			if (gpstog.Checked == true) 
			{
				editor.PutBoolean ("gps", true);
			} 
			else 
			{
				editor.PutBoolean ("gps", false);
			}

			if (romajitog.Checked == true) {
				editor.PutBoolean ("romaji", true);
			} 
			else 
			{
				editor.PutBoolean ("romaji", false);
			}
			editor.Commit();
			Finish ();
			StartActivity (typeof(SplashAct));
		}
	}
}

