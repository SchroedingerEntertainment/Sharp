// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime
{
    public static partial class Application
    {
        private static int[] errorCodes = null;

        /// <summary>
        /// Obtains a final success return code
        /// </summary>
        public static int SuccessReturnCode
        {
            get { return 0; }
        }
        /// <summary>
        /// Obtains a final general failure return code
        /// </summary>
        public static int FailureReturnCode
        {
            get { return 1; }
        }

        /// <summary>
        /// Combines a set of values into certain return code
        /// </summary>
        /// <param name="category">The category of the return code</param>
        /// <param name="subCategory">The sub category of the return code</param>
        /// <param name="code">The ID value describing the result</param>
        /// <returns>The final return code</returns>
        public static int GetResultFromValues(int category, int subCategory, byte code)
        {
            return (((byte)category << 16) | ((byte)subCategory << 8) | code);
        }
        /// <summary>
        /// Tries to map a given error or return code into a known final return code
        /// </summary>
        /// <param name="code">The code to map</param>
        /// <returns>The final return code</returns>
        public static int GetResultFromKnownError(int code)
        {
            if (code < 0 || code > errorCodes.Length)
                throw new InvalidCastException();

            return errorCodes[(int)code];
        }

        /// <summary>
        /// Obtains a final failure return code if an error has been logged or else
        /// a success return code
        /// </summary>
        /// <returns></returns>
        public static int GetReturnCode()
        {
            if (hasErrors.UnsafeValue)
            {
                return FailureReturnCode;
            }
            else return SuccessReturnCode;
        }
    }
}