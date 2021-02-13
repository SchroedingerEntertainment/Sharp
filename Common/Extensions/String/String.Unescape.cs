// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static partial class StringExtension
    {
        /// <summary>
        /// Undos any escape sequences in this string
        /// </summary>
        public static string Unescape(this string str)
        {
            if (string.IsNullOrEmpty(str)) 
            { 
                return str;
            }
            StringBuilder sb = new StringBuilder(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                switch (c)
                {
                    case '\\':
                        {
                            if (i + 1 < str.Length)
                            {
                                switch (str[i + 1])
                                {
                                    #region \t
                                    case 't':
                                        {
                                            sb.Append('\t');
                                            i++;
                                        }
                                        break;
                                    #endregion

                                    #region \r
                                    case 'r':
                                        {
                                            sb.Append('\r');
                                            i++;
                                        }
                                        break;
                                    #endregion

                                    #region \n
                                    case 'n':
                                        {
                                            sb.Append('\n');
                                            i++;
                                        }
                                        break;
                                    #endregion

                                    default: sb.Append(c);
                                        break;
                                }
                            }
                            else goto default;
                        }
                        break;
                    default: sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}