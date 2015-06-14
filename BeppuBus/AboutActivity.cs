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

namespace BeppuBus
{
	[Activity (Theme = "@style/AppTheme", /*ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, */Label = "About")]			
	public class AboutActivity : Activity
	{
		Button feedbackbutton;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.About);

			PackageInfo pkgInfo = this.PackageManager.GetPackageInfo (this.PackageName, 0);
			TextView versionNumber = FindViewById<TextView> (Resource.Id.v127);
			versionNumber.Text = "v" + pkgInfo.VersionName.ToString ();  

			this.feedbackbutton = FindViewById<Button> (Resource.Id.feedbackBtn);
			this.feedbackbutton.Click += Feed;
			//var likeBtn = FindViewById (Resource.Id.likeBtn);
			//likeBtn.Click += (sender, e) => {
			//};
		}
		private void Feed(object sender, EventArgs e)
		{
			var uri = Android.Net.Uri.Parse ("mailto: apudevx@gmail.com?subject=BeppuBus Feedback");
			var intent = new Intent (Intent.ActionView, uri); 
			StartActivity (intent);  
		}



	}
}

