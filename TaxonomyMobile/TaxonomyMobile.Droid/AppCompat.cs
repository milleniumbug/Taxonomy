using System;

namespace TaxonomyMobile
{
	public static class AppCompat
	{
		// Returns the location of the external SD Card
		// If the phone doesn't support the external SD card, this returns null
		// External SD Card access is possibly broken on 4.4 and newer, blame Google
		public static string GetExternalSdCardLocation()
		{
			var colonSeparated = Environment.GetEnvironmentVariable("SECONDARY_STORAGE");
			if(colonSeparated == null)
				return null;
			var directories = colonSeparated.Split(':');
			return directories[0];
		}
	}
}