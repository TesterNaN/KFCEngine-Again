using System;
using System.Runtime.InteropServices;
using System.Text;

namespace KidFansChannel.GalCore
{
	internal class NativeMethods
	{
		internal const int PATH_MAX = 260;

		internal const int CSIDL_PROFILE = 40;

		[DllImport("shfolder.dll", CharSet = CharSet.Auto)]
		internal static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);
	}
}
