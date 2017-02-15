using System;

namespace TaxonomyMobile
{
	public static class AppCompat
	{
		// Returns the location of the external SD Card
		// If the phone doesn't support the external SD card, this returns null
		// Direct external SD Card write access to files is broken on 4.4 and newer unless rooted, blame Google
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