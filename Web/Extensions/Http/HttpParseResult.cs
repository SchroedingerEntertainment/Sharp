#if net40
using System;
using System.Text;

namespace System.Net.Http
{
    internal enum HttpParseResult
    {
        Parsed,
        NotParsed,
        InvalidFormat,
    }
}
#endif