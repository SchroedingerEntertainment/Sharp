﻿#if net40
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace System.Net.Http.Headers
{
    public class RangeItemHeaderValue : ICloneable
    {
        private long? from;
        private long? to;

        public long? From
        {
            get { return from; }
        }

        public long? To
        {
            get { return to; }
        }

        public RangeItemHeaderValue(long? from, long? to)
        {
            if (!from.HasValue && !to.HasValue)
            {
                throw new ArgumentException(ErrorCodes.NetHttpHeadersInvalidRange);
            }
            if (from.HasValue && (from.Value < 0))
            {
                throw new ArgumentOutOfRangeException("from");
            }
            if (to.HasValue && (to.Value < 0))
            {
                throw new ArgumentOutOfRangeException("to");
            }
            if (from.HasValue && to.HasValue && (from.Value > to.Value))
            {
                throw new ArgumentOutOfRangeException("from");
            }

            this.from = from;
            this.to = to;
        }

        private RangeItemHeaderValue(RangeItemHeaderValue source)
        {
            Contract.Requires(source != null);

            this.from = source.from;
            this.to = source.to;
        }

        public override string ToString()
        {
            if (!from.HasValue)
            {
                return "-" + to.Value.ToString(NumberFormatInfo.InvariantInfo);
            }
            else if (!to.HasValue)
            {
                return from.Value.ToString(NumberFormatInfo.InvariantInfo) + "-";
            }
            return from.Value.ToString(NumberFormatInfo.InvariantInfo) + "-" +
                to.Value.ToString(NumberFormatInfo.InvariantInfo);
        }

        public override bool Equals(object obj)
        {
            RangeItemHeaderValue other = obj as RangeItemHeaderValue;

            if (other == null)
            {
                return false;
            }
            return ((from == other.from) && (to == other.to));
        }

        public override int GetHashCode()
        {
            if (!from.HasValue)
            {
                return to.GetHashCode();
            }
            else if (!to.HasValue)
            {
                return from.GetHashCode();
            }
            return from.GetHashCode() ^ to.GetHashCode();
        }

        // Returns the length of a range list. E.g. "1-2, 3-4, 5-6" adds 3 ranges to 'rangeCollection'. Note that empty
        // list segments are allowed, e.g. ",1-2, , 3-4,,".
        internal static int GetRangeItemListLength(string input, int startIndex,
            ICollection<RangeItemHeaderValue> rangeCollection)
        {
            Contract.Requires(rangeCollection != null);
            Contract.Requires(startIndex >= 0);
            Contract.Ensures((Contract.Result<int>() == 0) || (rangeCollection.Count > 0), 
                "If we can parse the string, then we expect to have at least one range item.");

            if ((string.IsNullOrEmpty(input)) || (startIndex >= input.Length))
            {
                return 0;
            }

            // Empty segments are allowed, so skip all delimiter-only segments (e.g. ", ,").
            bool separatorFound = false;
            int current = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex, true, out separatorFound);
            // It's OK if we didn't find leading separator characters. Ignore 'separatorFound'.

            if (current == input.Length)
            {
                return 0;
            }

            RangeItemHeaderValue range = null;
            while (true)
            {
                int rangeLength = GetRangeItemLength(input, current, out range);

                if (rangeLength == 0)
                {
                    return 0;
                }

                rangeCollection.Add(range);

                current = current + rangeLength;
                current = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, current, true, out separatorFound);

                // If the string is not consumed, we must have a delimiter, otherwise the string is not a valid 
                // range list.
                if ((current < input.Length) && !separatorFound)
                {
                    return 0;
                }

                if (current == input.Length)
                {
                    return current - startIndex;
                }
            }
        }
        
        internal static int GetRangeItemLength(string input, int startIndex, out RangeItemHeaderValue parsedValue)
        {
            Contract.Requires(startIndex >= 0);

            // This parser parses number ranges: e.g. '1-2', '1-', '-2'.

            parsedValue = null;

            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }

            // Caller must remove leading whitespaces. If not, we'll return 0.
            int current = startIndex;

            // Try parse the first value of a value pair.
            int fromStartIndex = current;
            int fromLength = HttpRuleParser.GetNumberLength(input, current, false);

            if (fromLength > HttpRuleParser.MaxInt64Digits)
            {
                return 0;
            }

            current = current + fromLength;
            current = current + HttpRuleParser.GetWhitespaceLength(input, current);

            // Afer the first value, the '-' character must follow.
            if ((current == input.Length) || (input[current] != '-'))
            {
                // We need a '-' character otherwise this can't be a valid range.
                return 0;
            }

            current++; // skip the '-' character
            current = current + HttpRuleParser.GetWhitespaceLength(input, current);

            int toStartIndex = current;
            int toLength = 0;

            // If we didn't reach the end of the string, try parse the second value of the range.
            if (current < input.Length)
            {
                toLength = HttpRuleParser.GetNumberLength(input, current, false);
                
                if (toLength > HttpRuleParser.MaxInt64Digits)
                {
                    return 0;
                }
                
                current = current + toLength;
                current = current + HttpRuleParser.GetWhitespaceLength(input, current);
            }

            if ((fromLength == 0) && (toLength == 0))
            {
                return 0; // At least one value must be provided in order to be a valid range.
            }

            // Try convert first value to int64
            long from = 0;
            if ((fromLength > 0) && !HeaderUtilities.TryParseInt64(input.Substring(fromStartIndex, fromLength), out from))
            {
                return 0;
            }

            // Try convert second value to int64
            long to = 0;
            if ((toLength > 0) && !HeaderUtilities.TryParseInt64(input.Substring(toStartIndex, toLength), out to))
            {
                return 0;
            }

            // 'from' must not be greater than 'to'
            if ((fromLength > 0) && (toLength > 0) && (from > to))
            {
                return 0;
            }

            parsedValue = new RangeItemHeaderValue((fromLength == 0 ? (long?)null : (long?)from),
                (toLength == 0 ? (long?)null : (long?)to));
            return current - startIndex;
        }

        object ICloneable.Clone()
        {
            return new RangeItemHeaderValue(this);
        }
    }
}
#endif