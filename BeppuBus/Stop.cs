using System;

namespace BeppuBus
{
	public class Stop
	{

		public int _id {get; set;}
		public string stop_name {get; set;}
		public string stop_namej {get; set;}
		public float stop_lat {get; set;}
		public float stop_lon {get; set;}
	}

	public class Timetable
	{

		public int _id {get; set;}
		public int stop{get; set;}
		public int type {get; set;}
		public int bus{get; set;}
		public int school {get; set;}
		public DateTime time { get; set; }

	}
	
}

