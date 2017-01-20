using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TaxonomyWpf
{
	public static class NativeExplorerInterface
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
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
		private const uint SHGFI_USEFILEATTRIBUTES = 0x10;

		private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr SHGetFileInfo(
			string pszPath,
			uint dwFileAttributes,
			ref SHFILEINFO psfi,
			uint cbSizeFileInfo,
			uint uFlags);

		[DllImport("user32.dll")]
		private static extern bool DestroyIcon(IntPtr handle);

		public sealed class HIcon : IDisposable
		{
			public IntPtr IconHandle { get; private set; }

			public void Dispose()
			{
				DestroyIcon(IconHandle);
				IconHandle = IntPtr.Zero;
			}

			public HIcon(IntPtr iconHandle)
			{
				IconHandle = iconHandle;
			}
		}

		public static void OpenContextMenuForFile(string path)
		{
			
		}

		public static HIcon GetGenericFileIcon()
		{
			return GetHIconForFile("generic_file", FILE_ATTRIBUTE_NORMAL, SHGFI_ICON | SHGFI_LARGEICON | SHGFI_USEFILEATTRIBUTES);
		}

		public static HIcon GetHIconForFile(string path)
		{
			return GetHIconForFile(path, 0, SHGFI_ICON | SHGFI_LARGEICON);
		}

		public static HIcon GetHIconForFile(string path, uint fileAttributes, uint flags)
		{
			IntPtr hImg;
			string fName = path;
			SHFILEINFO shinfo = new SHFILEINFO();
			hImg = SHGetFileInfo(
				fName,
				fileAttributes,
				ref shinfo,
				(uint)Marshal.SizeOf(shinfo),
				flags);

			// TODO: "You should call this function from a background thread. Failure to do so could cause the UI to stop responding."

			return new HIcon(shinfo.hIcon);
		}
	}
}
