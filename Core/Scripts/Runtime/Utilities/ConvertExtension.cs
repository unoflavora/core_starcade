using System.Text;
using System;

namespace Agate.Starcade.Scripts.Runtime.Utilities
{
    public static class ConvertExtension
    {
		public static string ToBase64(this string value)
		{
			var bytesText = Encoding.UTF8.GetBytes(value);
			return Convert.ToBase64String(bytesText);
		}

		public static string FromBase64(this string base64EncodedData)
		{
			var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(base64EncodedBytes);
		}
	}
}
