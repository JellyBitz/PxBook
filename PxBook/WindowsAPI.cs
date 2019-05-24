using System;
using System.Linq;
using System.Runtime.InteropServices;
namespace PxBook
{
	public static class WindowsAPI
	{
		[DllImport("gdi32.dll")]
		public static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
						IntPtr pdv, [In] ref uint pcFonts);
		public static byte[] StringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
											 .Where(x => x % 2 == 0)
											 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
											 .ToArray();
		}
	}
}
