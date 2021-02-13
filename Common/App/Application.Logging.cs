// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime
{
    public static partial class Application
    {
        private static Spinlockʾ logLock;

        private static SeverityFlags logSeverity;
        /// <summary>
        /// A user defined log level
        /// </summary>
        public static SeverityFlags LogSeverity
        {
            get { return logSeverity; }
            set { logSeverity = value; }
        }

        private static ILogSystem logSystem;
        /// <summary>
        /// Gets or sets the log system used to process messages
        /// </summary>
        public static ILogSystem LogSystem
        {
            get { return logSystem; }
            set { logSystem = value; }
        }

        /// <summary>
        /// Writes a message to local declared log output. The default level
        /// is Full locking
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Log(string message, params object[] args)
        {
            Log(SeverityFlags.Full, message, args);
        }
        /// <summary>
        /// Writes a message to local declared log output and level
        /// </summary>
        /// <param name="severity">The log level to use</param>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Log(SeverityFlags severity, string message, params object[] args)
        {
            if (severity > logSeverity)
                return;

            logLock.Lock();
            try
            {
                logSystem.Log(string.Format(message, args));
            }
            finally
            {
                logLock.Release();
            }
        }

        /// <summary>
        /// Writes a message to local declared warn output. The default level
        /// is Full locking
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Warning(string message, params object[] args)
        {
            Warning(SeverityFlags.Full, message, args);
        }
        /// <summary>
        /// Writes a message to local declared warn output and level
        /// </summary>
        /// <param name="severity">The log level to use</param>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Warning(SeverityFlags severity, string message, params object[] args)
        {
            if (severity > logSeverity)
                return;

            logLock.Lock();
            try
            {
                logSystem.Warning(string.Format(message, args));
            }
            finally
            {
                logLock.Release();
            }
        }

        /// <summary>
        /// Writes a message to local declared error output. The default level
        /// is Full locking
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Error(string message, params object[] args)
        {
            Error(SeverityFlags.Full, message, args);
        }
        /// <summary>
        /// Writes a message to local declared error output and level
        /// </summary>
        /// <param name="severity">The log level to use</param>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Error(SeverityFlags severity, string message, params object[] args)
        {
            if (severity > logSeverity)
                return;

            hasErrors.Exchange(true);
            logLock.Lock();
            try
            {
                logSystem.Error(string.Format(message, args));
            }
            finally
            {
                logLock.Release();
            }
        }
        /// <summary>
        /// Logs an Exception to the default error channel
        /// </summary>
        /// <param name="error">The Exception to log</param>
        public static void Error(Exception error)
        {
            bool detailed = (logSeverity > SeverityFlags.Minimal);

            AggregateException errorSet = error as AggregateException;
            string message; if (errorSet != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(error.Message);
                foreach (Exception e in errorSet.InnerExceptions)
                {
                    if (detailed)
                    {
                        sb.AppendLine(error.Message);
                        sb.AppendLine(error.StackTrace);
                    }
                    else sb.AppendLine(error.Message);
                }
                message = sb.ToString();
                detailed = false;
            }
            else message = error.Message;
            if (detailed)
            {
                Error(SeverityFlags.None, "{0}{1}{2}", error.Message, Environment.NewLine, error.StackTrace);
            }
            else Error(SeverityFlags.None, error.Message);
        }
    }
}
