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
	public struct tobeSorted
	{
		public double distanceKM { get; set; }
		public string placeName { get; set; }
		public string placeNameJ {get; set;}
	}

	public struct getSetPos
	{
		public string place { get; set; }
		public string placej { get; set; }
		public double lat { get; set; }
		public double lon { get; set; }
	}

	static class distFinder
	{
		const double PIx = Math.PI;
		const double RADIO = 6378.16;

		public static double radians(double x)
		{
			return x * PIx / 180;
		}


		public static double distanceWorker(getSetPos busStop, getSetPos currentPlace)
		{
			double R = 6371; 

			double sLat1 = Math.Sin(radians(busStop.lat));
			double sLat2 = Math.Sin(radians(currentPlace.lat));
			double cLat1 = Math.Cos(radians(busStop.lat));
			double cLat2 = Math.Cos(radians(currentPlace.lat));
			double cLon = Math.Cos(radians(busStop.lon) - radians(currentPlace.lon));

			double cosD = sLat1 * sLat2 + cLat1 * cLat2 * cLon;

			double d = Math.Acos(cosD);

			double dist = R * d;

			return dist;
		}
	}
}

