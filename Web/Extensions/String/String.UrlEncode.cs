// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SE.Web
{
	public static partial class StringExtension
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static string UrlEncode(this string s, IEnumerable<KeyValuePair<string, string>> parameter)
		{
			if (parameter != null && parameter.Any())
			{
				StringBuilder stringBuilder = new StringBuilder(s);
				IEnumerator<KeyValuePair<string, string>> iterator = parameter.GetEnumerator();
				for (int i = 0; iterator.MoveNext(); i++)
				{
					stringBuilder.Append(i == 0 ? "?" : "&");
					stringBuilder.Append(iterator.Current.Key);
					stringBuilder.Append("=");
					stringBuilder.Append(iterator.Current.Value);
				}
				return Uri.EscapeDataString(stringBuilder.ToString());
			}
			else return Uri.EscapeDataString(s);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string UrlEncode(this string s)
		{
			return UrlEncode(s, null);
		}
	}
}
