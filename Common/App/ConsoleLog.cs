// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Runtime
{
    /// <summary>
    /// Logs to the default console
    /// </summary>
    public class ConsoleLog : ILogSystem
    {
        const ConsoleColor WarnColor = ConsoleColor.Yellow;
        const ConsoleColor ErrorColor = ConsoleColor.Red;

        public readonly static ConsoleLog Instance = new ConsoleLog();
        private static Spinlockʾ accessLock;

        static ConsoleLog()
        {
            accessLock = new Spinlockʾ();
        }
        private ConsoleLog()
        { }

        public void Log(string message)
        {
            accessLock.Lock();
            try
            {
                Console.WriteLine(message);
            }
            finally
            {
                accessLock.Release();
            }
        }
        public void Warning(string message)
        {
            accessLock.Lock();
            try
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = WarnColor;
                Console.WriteLine(string.Concat("[WARNING] ", message));
                Console.ForegroundColor = color;
            }
            finally
            {
                accessLock.Release();
            }
        }
        public void Error(string message)
        {
            accessLock.Lock();
            try
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ErrorColor;
                Console.WriteLine(string.Concat("[ERROR] ", message));
                Console.ForegroundColor = color;
            }
            finally
            {
                accessLock.Release();
            }
        }
    }
}