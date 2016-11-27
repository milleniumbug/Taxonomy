using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TaxonomyWpf
{
	public static class NativeExplorerInterface
	{
		[StructLayout(LayoutKind.Sequential)]
		private struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

		private const uint SHGFI_ICON = 0x100;
		private const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
		private const uint SHGFI_SMALLICON = 0x1;    // 'Small icon

		[DllImport("shell32.dll")]
		private static extern IntPtr SHGetFileInfo(
			string pszPath,
			uint dwFileAttributes,
			ref SHFILEINFO psfi,
			uint cbSizeFileInfo,
			uint uFlags);

		[DllImport("user32.dll")]
		private static extern bool DestroyIcon(IntPtr handle);

		public static Icon GetIconForFile(string path)
		{
			IntPtr hImg;
			string fName = path;
			SHFILEINFO shinfo = new SHFILEINFO();
			hImg = SHGetFileInfo(
				fName,
				0,
				ref shinfo,
				(uint)Marshal.SizeOf(shinfo),
				SHGFI_ICON | SHGFI_LARGEICON);

			// TODO: "You should call this function from a background thread. Failure to do so could cause the UI to stop responding."
			// TODO: If SHGetFileInfo returns an icon handle in the hIcon member of the SHFILEINFO structure pointed to by psfi, you are responsible for freeing it with DestroyIcon when you no longer need it.

			Icon myIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon);

			//DestroyIcon(shinfo.hIcon);
			return myIcon;
		}
	}
}
