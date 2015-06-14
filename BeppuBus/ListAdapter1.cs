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

namespace BeppuBus
{
	public class ListAdapter1 : BaseAdapter<string>
	{

		Activity context;
		List<Stop> stopCollection;
		bool romaji;

		/*
		List<Timetable> timetableCollection;
		public ListAdapter1(Activity context, List<Stop> stopList, List<Timetable> timetableList) : base()
		{
			this.context = context;
			this.stopCollection = stopList;
			this.timetableCollection = timetableList;
		}*/

		public ListAdapter1(Activity context, List<Stop> stopList) : base()
		{
			this.context = context;
			this.stopCollection = stopList;
		}

		public ListAdapter1(Activity context, List<Stop> stopList, bool romaji) : base()
		{
			this.context = context;
			this.stopCollection = stopList;
			this.romaji = romaji;
		}

		public override long GetItemId(int position){return position;}

		//public override string this[int position]{get {return timetableCollection[position].time.ToString();}}
		public override string this[int position]{get {return stopCollection[position].stop_name;}}

		public override int Count { get { return stopCollection.Count; } }

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView;
			if (view == null) {view = context.LayoutInflater.Inflate (Android.Resource.Layout.SimpleListItem2, null);}

			if (this.romaji == true) {
				view.FindViewById<TextView> (Android.Resource.Id.Text1).Text = stopCollection [position].stop_name;
				view.FindViewById<TextView> (Android.Resource.Id.Text2).Text = stopCollection [position].stop_namej;
			} else {
				view.FindViewById<TextView> (Android.Resource.Id.Text1).Text = stopCollection [position].stop_namej;
				view.FindViewById<TextView> (Android.Resource.Id.Text2).Text = stopCollection [position].stop_name;
			}

			return view;
		}
	}

}