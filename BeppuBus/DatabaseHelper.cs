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
using System.IO;
using Android.Database.Sqlite;
using Android.Database;
using Android.Preferences;

namespace BeppuBus
{
	public class DatabaseHelper : SQLiteOpenHelper 
	{
		public static string dbPath = "/data/data/com.apudevx.beppubusb/databases/";
		private static string dbName = "db.sqlite";
		private static string SP_KEY_DB_VER = "db_ver";
		public static int dbVersion = 9;

		private SQLiteDatabase myDataBase;
		private Context myContext;

		public DatabaseHelper(Context context)
			:base(context, dbName, null, dbVersion)
		{
			this.myContext = context;

		}

		public void createDataBase()
		{
			bool dbExist = checkDatabase (dbName);
			if (dbExist) 
			{
				//Check version
				var prefs = PreferenceManager.GetDefaultSharedPreferences (myContext);
				int oldDBVersion = prefs.GetInt (SP_KEY_DB_VER, 1);
				if (dbVersion != oldDBVersion) {
					//dbFile = myContext.GetDatabasePath (dbName);
					File.Delete (dbPath + dbName);
					dbExist = checkDatabase (dbName);
				} else {
					//Android.Widget.Toast.MakeText (myContext, "Test", Android.Widget.ToastLength.Short).Show ();
				}


			} 


			if (!dbExist)
			{
				this.ReadableDatabase.AcquireReference ();
				try{
					copyDataBase();
				}
				catch(IOException e){
					throw new IOException ("Unable to create dtb");
				}
			}


		}

		private bool checkDatabase(String db)
		{
			SQLiteDatabase checkDB = null;
			try 
			{
				string myPath = dbPath + db;
				checkDB = SQLiteDatabase.OpenDatabase(myPath, null, DatabaseOpenFlags.OpenReadonly);

			}
			catch(SQLiteException e){}


			if (checkDB != null) {
				checkDB.Close ();

			}

			return checkDB != null ? true : false;
		}

		private void copyDataBase()
		{
			var myInput = myContext.Assets.Open(dbName);
			string outFileName = dbPath + dbName;
			var myOutput = new FileStream(outFileName, FileMode.OpenOrCreate);

			byte[] buffer = new byte[1024];
			int length;
			while ((length = myInput.Read(buffer, 0, 1024)) > 0)
				myOutput.Write(buffer, 0, length);
			myOutput.Flush();

			//Add DB Version
			var prefs = PreferenceManager.GetDefaultSharedPreferences (myContext);
			var editor = prefs.Edit ();
			editor.PutInt (SP_KEY_DB_VER, dbVersion);
			editor.Commit ();


			myOutput.Close();
			myInput.Close();
		}

		public void openDataBase()
		{
			string myPath = dbPath + dbName;
			myDataBase = SQLiteDatabase.OpenDatabase (myPath, null, DatabaseOpenFlags.OpenReadonly);
		}

		public void close()
		{
			if (myDataBase != null) {
				myDataBase.Close ();
			}
			base.Close ();
		}


		#region implemented abstract members of SQLiteOpenHelper
		public override void OnCreate (SQLiteDatabase db)
		{
			//throw new NotImplementedException ();a
		}
		public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
		{
			//throw new NotImplementedException ();
		}
		#endregion
	}
}
