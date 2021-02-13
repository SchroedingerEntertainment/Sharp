// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SE.Config
{
    /// <summary>
    /// Can be filled with a formatted list of properties to build a manual
    /// </summary>
    public class PropertyPage : FinalizerObject
    {
        class ArgumentComparer : IComparer<PropertyPageRow>
        {
            public static bool IsPropertyCharacter(char c)
            {
                switch (c)
                {
                    case '-':
                    case '/': return true;
                    default: return false;
                }
            }

            public int Compare(PropertyPageRow x, PropertyPageRow y)
            {
                string kx = x.Keys[0].Split(',').First().Trim();
                string ky = y.Keys[0].Split(',').First().Trim();

                if (IsPropertyCharacter(kx[0]) && IsPropertyCharacter(ky[0]))
                {
                    bool xIsLetter = false;
                    foreach (char c in kx)
                        if (!IsPropertyCharacter(c))
                        {
                            xIsLetter = char.IsLetter(c);
                            break;
                        }
                    bool yIsLetter = false;
                    foreach (char c in ky)
                        if (!IsPropertyCharacter(c))
                        {
                            yIsLetter = char.IsLetter(c);
                            break;
                        }

                    if((xIsLetter && yIsLetter) || (!xIsLetter && !yIsLetter)) return string.Compare(kx, ky);
                    else if(xIsLetter) return -1;
                    else return 1;
                }
                else if (IsPropertyCharacter(kx[0])) return 1;
                else if (IsPropertyCharacter(ky[0])) return -1;
                else
                {
                    PropertyPageKeyValueRow xi = x as PropertyPageKeyValueRow;
                    PropertyPageKeyValueRow yi = y as PropertyPageKeyValueRow;
                    if (xi != null && yi != null)
                    {
                        return xi.VerbIndex.CompareTo(yi.VerbIndex);
                    }
                    else return 0;
                }
            }
        }
        private readonly static ArgumentComparer Comparer = new ArgumentComparer();

        List<PropertyPageRow> rows;
        /// <summary>
        /// The rows contained in this page
        /// </summary>
        public List<PropertyPageRow> Rows
        {
            get { return rows; }
        }

        StringBuilder result;

        protected string separator = " ";
        /// <summary>
        /// The separator chars used for printing
        /// </summary>
        public string Separator
        {
            get { return separator; }
            set { separator = value; }
        }

        PropertyPageFlags flags;
        /// <summary>
        /// A set of configuration flags passed to the formatter
        /// </summary>
        public PropertyPageFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        /// <summary>
        /// Creates a new page instance with provided options
        /// </summary>
        public PropertyPage(PropertyPageFlags flags)
        {
            this.flags = flags;
            this.rows = new List<PropertyPageRow>();
            this.result = new StringBuilder();
        }
        /// <summary>
        /// Creates a new page instance with default options
        /// </summary>
        public PropertyPage()
            : this(PropertyPageFlags.HarmonizeFlags)
        { }
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                foreach (PropertyPageRow row in rows)
                row.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Transforms a string into the coresponding property key, based on local
        /// configuration settings
        /// </summary>
        public string MakeKey(string name)
        {
            if ((flags & PropertyPageFlags.DashNotation) == PropertyPageFlags.DashNotation)
            {
                switch (name.Length)
                {
                    case 1: return string.Concat("-", name);
                    default: return string.Concat("--", name);
                }
            }
            else if ((flags & PropertyPageFlags.SlashNotation) == PropertyPageFlags.SlashNotation)
            {
                return string.Concat("/", name);
            }
            else return name;
        }

        /// <summary>
        /// Appends a new row at the end of this page
        /// </summary>
        public PropertyPageKeyValueRow AddRow(string key, PropertyDescriptionAttribute description)
        {
            result.Clear();

            PropertyPageKeyValueRow row; if (description != null)
            {
                row = new PropertyPageKeyValueRow(description.Text);
                row.Type = description.Type;
            }
            else row = new PropertyPageKeyValueRow(string.Empty);
            row.Keys.Add(key);
            rows.Add(row);
            return row;
        }
        /// <summary>
        /// Appends a new row at the end of this page
        /// </summary>
        public PropertyPageRow AddRow(string key)
        {
            result.Clear();

            PropertyPageRow row = new PropertyPageRow();
            row.Keys.Add(key);
            rows.Add(row);
            return row;
        }

        /// <summary>
        /// Appends a property key to the last row contained in this page
        /// </summary>
        /// <param name="key"></param>
        public void AppendKey(string key)
        {
            result.Clear();

            if (rows.Count != 0)
            {
                PropertyPageRow row = rows[rows.Count - 1];
                row.Keys.Add(key);
            }
            else AddRow(key, null);
        }

        /// <summary>
        /// Sorts rows by their key
        /// </summary>
        public void Sort()
        {
            rows.Sort(Comparer);
            result.Clear();
        }

        /// <summary>
        /// Creates a single line string describing the usage of the properties contained in this page.
        /// This is usually displayed in the command line
        /// </summary>
        public string GetUsageLine()
        {
            StringBuilder sb = new StringBuilder();
            foreach (PropertyPageRow row in rows)
            {
                PropertyPageKeyValueRow kv = row as PropertyPageKeyValueRow;
                if (kv != null)
                {
                    for (int i = 0; i < kv.Keys.Count; i++)
                    {
                        foreach (string key in kv.Keys[i].Split(','))
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append(' ');
                            }
                            switch (kv.Type)
                            {
                                case PropertyType.Optional:
                                    {
                                        sb.Append('[');
                                        sb.Append(key.Trim());
                                        sb.Append(']');
                                    }
                                    break;
                                case PropertyType.Required:
                                    {
                                        sb.Append('<');
                                        sb.Append(key.Trim());
                                        sb.Append('>');
                                    }
                                    break;
                                default: sb.Append(key.Trim());
                                    break;
                            }
                            if (!string.IsNullOrEmpty(kv.DefaultValue))
                            {
                                sb.Append(':');
                                sb.Append('{');
                                sb.Append(kv.DefaultValue);
                                sb.Append('}');
                            }
                        }
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Turns this page into a string representation which is formatted at a certain
        /// line boundary
        /// </summary>
        /// <param name="lineWidth">The maximum number of characters per line</param>
        public string ToString(int lineWidth)
        {
            if (result.Length == 0 && rows.Count > 0)
            {
                IEnumerable<PropertyPageRow> iterator = rows.Where(x => x is PropertyPageKeyValueRow);
                
                int columnWidth = iterator.Max(x => (x as PropertyPageKeyValueRow).Width);
                lineWidth = (lineWidth - (columnWidth + separator.Length));
                if (lineWidth <= 0)
                {
                    lineWidth = 1;
                }
                bool hasKeyPairs = iterator.SelectMany(x => (x as PropertyPageKeyValueRow).Keys).Any(x => x.IndexOf(',') != -1);
                bool additionalSpaceBetween = ((flags & PropertyPageFlags.AdditionalLineBreaks) == PropertyPageFlags.AdditionalLineBreaks);
                bool hasHadOptionKey = false;
                for (int t = 0; t < rows.Count; t++)
                {
                    PropertyPageRow row = rows[t];
                    PropertyPageKeyValueRow kv = row as PropertyPageKeyValueRow;
                    bool isOption = ArgumentComparer.IsPropertyCharacter(row.Keys[0][0]);
                    if (t > 0)
                    {
                        if ((hasHadOptionKey && !isOption) || (!hasHadOptionKey && isOption))
                        {
                            result.AppendLine();
                        }
                        else if (additionalSpaceBetween && ArgumentComparer.IsPropertyCharacter(row.Keys[0][0]))
                        {
                            result.AppendLine();
                        }
                    }
                    hasHadOptionKey = false;
                    if (kv != null)
                    {
                        int index = 0;
                        string[] description = kv.Description.Trim().Split(' ');
                        for (int i = 0; i < kv.Keys.Count; i++)
                        {
                            hasHadOptionKey |= ArgumentComparer.IsPropertyCharacter(kv.Keys[i][0]);
                            if (isOption && hasKeyPairs && row.Keys[i].IndexOf(',') == -1 && kv.Keys[i][1] == '-')
                            {
                                result.Append(' ', 4);
                                AppendLine(kv.Keys[i], description, ref index, columnWidth - 4, lineWidth);
                            }
                            else AppendLine(kv.Keys[i], description, ref index, columnWidth, lineWidth);
                        }
                        for (; index < description.Length;)
                        {
                            AppendLine(string.Empty, description, ref index, columnWidth, lineWidth);
                        }
                    }
                    else for(int i = 0; i < row.Keys.Count; i++)
                    {
                        result.AppendLine();
                        result.Append(row.Keys[i].PadRight(columnWidth));
                        result.AppendLine(separator);
                    }
                }
            }
            return result.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }
        public override string ToString()
        {
            return ToString(int.MaxValue);
        }

        void AppendLine(string key, string[] description, ref int index, int columnWidth, int lineWidth)
        {
            result.Append(key.PadRight(columnWidth));
            if (index == 0 && description[0].Length > 0)
            {
                result.Append(separator);
            }
            for (int n = 0, length = lineWidth; index < description.Length && length > 0; length -= description[index].Length, index++, n++)
            {
                if (length - description[index].Length < 0)
                    break;

                if (n > 0)
                {
                    result.Append(' ');
                }
                result.Append(description[index]);
            }
            result.AppendLine();
        }
    }
}
