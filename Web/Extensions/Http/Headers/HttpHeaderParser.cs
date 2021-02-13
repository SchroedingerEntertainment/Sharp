﻿#if net40
using System.Collections;
using System.Diagnostics.Contracts;

namespace System.Net.Http.Headers
{
#if DEBUG
    [ContractClass(typeof(HttpHeaderParserContract))]
#endif
    internal abstract class HttpHeaderParser
    {
        internal const string DefaultSeparator = ", ";

        private bool supportsMultipleValues;
        private string separator;

        public bool SupportsMultipleValues 
        {
            get { return supportsMultipleValues; }
        }

        public string Separator
        {
            get 
            {
                Contract.Assert(this.supportsMultipleValues);
                return separator; 
            }
        }

        // If ValueType implements Equals() as required, there is no need to provide a comparer. A comparer is needed
        // e.g. if we want to compare strings using case-insensitive comparison.
        public virtual IEqualityComparer Comparer 
        {
            get { return null; }
        }

        protected HttpHeaderParser(bool supportsMultipleValues)
        {
            this.supportsMultipleValues = supportsMultipleValues;

            if (supportsMultipleValues)
            {
                this.separator = DefaultSeparator;
            }
        }

        protected HttpHeaderParser(bool supportsMultipleValues, string separator)
        {
            Contract.Requires(!string.IsNullOrEmpty(separator));

            this.supportsMultipleValues = supportsMultipleValues;
            this.separator = separator;
        }

        // If a parser supports multiple values, a call to ParseValue/TryParseValue should return a value for 'index'
        // pointing to the next non-whitespace character after a delimiter. E.g. if called with a start index of 0
        // for string "value , second_value", then after the call completes, 'index' must point to 's', i.e. the first
        // non-whitespace after the separator ','.
        public abstract bool TryParseValue(string value, object storeValue, ref int index, out object parsedValue);

        public object ParseValue(string value, object storeValue, ref int index)
        {
            // Index may be value.Length (e.g. both 0). This may be allowed for some headers (e.g. Accept but not
            // allowed by others (e.g. Content-Length). The parser has to decide if this is valid or not.
            Contract.Requires((value == null) || ((index >= 0) && (index <= value.Length)));

            // If a parser returns 'null', it means there was no value, but that's valid (e.g. "Accept: "). The caller
            // can ignore the value.
            object result = null;
            if (!TryParseValue(value, storeValue, ref index, out result))
            {
                throw new FormatException(string.Format(System.Globalization.CultureInfo.InvariantCulture, ErrorCodes.NetHttpHeadersInvalidValue, 
                    value == null ? "<null>" : value.Substring(index)));
            }
            return result;
        }

        // If ValueType is a custom header value type (e.g. NameValueHeaderValue) it implements ToString() correctly.
        // However for existing types like int, byte[], DateTimeOffset we can't override ToString(). Therefore the 
        // parser provides a ToString() virtual method that can be overridden by derived types to correctly serialize 
        // values (e.g. byte[] to Base64 encoded string).
        // The default implementation is to just call ToString() on the value itself which is the right thing to do
        // for most headers (custom types, string, etc.).
        public virtual string ToString(object value)
        {
            Contract.Requires(value != null);

            return value.ToString();
        }
    }

#if DEBUG
    [ContractClassFor(typeof(HttpHeaderParser))]
    internal abstract class HttpHeaderParserContract : HttpHeaderParser
    {
        public HttpHeaderParserContract()
            : base(false)
        {
        }

        public override bool TryParseValue(string value, object storeValue, ref int index, out object parsedValue)
        {
            // Index may be value.Length (e.g. both 0). This may be allowed for some headers (e.g. Accept but not
            // allowed by others (e.g. Content-Length). The parser has to decide if this is valid or not.
            Contract.Requires((value == null) || ((index >= 0) && (index <= value.Length)));

            parsedValue = null;
            return false;
        }
    }
#endif

}
#endif