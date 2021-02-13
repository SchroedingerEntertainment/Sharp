// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

#if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462
using System;
using System.Collections.Generic;

namespace System
{
    internal class ErrorCodes
    {
        public const string ValueTupleIncorrectType = "The parameter should be a ValueTuple type of appropriate arity.";
        public const string ValueTupleLastArgumentNotAValueTuple = "The TRest type argument of ValueTuple`8 must be a ValueTuple.";
    }
}
#endif