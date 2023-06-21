using System;
using System.Diagnostics.CodeAnalysis;

namespace ginsjuan.Extensions
{
    [ExcludeFromCodeCoverage]
	public static class StringExtensions
	{
		/// <summary>
		/// Encodes a plain text to 64 base string
		/// </summary>
		/// <param Name="plainText"></param>
		/// <returns>64 bits encoded</returns>
		public static string Base64Encode(this string plainText)
		{
			if (!string.IsNullOrEmpty(plainText))
			{
				var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
				return Convert.ToBase64String(plainTextBytes);
			}

			return string.Empty;

		}
	}
}
