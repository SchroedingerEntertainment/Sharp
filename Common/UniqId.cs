// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System
{
    /// <summary>
    /// Provides runtime unique ID numbers
    /// </summary>
    public static class UniqueId
    {
        class IdGenerator
        {
            readonly long timeMask;
            readonly long sequenceMask;
            readonly int idUniqueBits;
            readonly long generatorid;
            
            long currentTimestamp = -1;
            int currentSequence = 0;

            public IdGenerator(int generatorId)
            {
                this.timeMask = GetBitMask(timestampBits);
                this.sequenceMask = GetBitMask(sequenceBits);
                this.idUniqueBits = threadIdBits + sequenceBits;
                this.generatorid = generatorId;
            }

            public bool TryNext(out long id)
            {
                long ticks = clockCallback();
                long timestamp = ticks & timeMask;
                if (timestamp < currentTimestamp || ticks < 0)
                {
                    id = 0;
                    return false;
                }
                else if (timestamp == currentTimestamp)
                {
                    if (currentSequence >= sequenceMask)
                    {
                        do
                        {
                            Thread.Sleep(0);
                        }
                        while(currentTimestamp == clockCallback());
                    }
                    currentSequence++;
                }
                else
                {
                    currentSequence = 0;
                    currentTimestamp = timestamp;
                }
                unchecked
                {
                    id = (timestamp << idUniqueBits) + (generatorid << sequenceBits) + currentSequence;
                }
                return true;
            }
        }

        private readonly static ThreadLocal<IdGenerator> generator;
        private static atomic_int nextId;

        private static byte timestampBits = 41;
        /// <summary>
        /// Gets or sets the number of bits used to store the timestamp
        /// </summary>
        public static byte TimestampBits
        {
            get { return timestampBits; } 
            set { timestampBits = value; }
        }
        /// <summary>
        /// Gets the limit of intervals the generator can run
        /// </summary>
        public static long MaxIntervals
        {
            get { return (1L << timestampBits); }
        }

        private static byte threadIdBits = 10;
        /// <summary>
        /// Gets or sets the number of bits used to store the thread ID
        /// </summary>
        public static byte ThreadIdBits
        {
            get { return threadIdBits; } 
            set { threadIdBits = value; }
        }
        /// <summary>
        /// Gets the limit of threads which can query IDs
        /// </summary>
        public static int MaxThreads
        {
            get { return (1 << threadIdBits); }
        }

        private static byte sequenceBits = 12;
        /// <summary>
        /// Gets or sets the number of bits used to store the ID sequence
        /// </summary>
        public static byte SequenceBits
        {
            get { return sequenceBits; } 
            set { sequenceBits = value; }
        }
        /// <summary>
        /// Gets the limit of sequential IDs for a determined time period
        /// </summary>
        public static int MaxSequenceces
        {
            get { return (1 << sequenceBits); }
        }

        private static Func<long> clockCallback = Stopwatch.GetTimestamp;
        /// <summary>
        /// Gets or sets the tick clock callback used to obtain the timestamp needed to generate unique IDs
        /// </summary>
        public static Func<long> ClockCallback
        {
            get { return clockCallback; } 
            set { clockCallback = value; }
        }

        static UniqueId()
        {
            generator = new ThreadLocal<IdGenerator>(CreateNewGenerator);
        }

        /// <summary>
        /// Tries to obtain a new unique ID from the generator
        /// </summary>
        /// <returns>True if the attempt was successful, false otherwise</returns>
        public static bool TryNext64(out long id)
        {
            return generator.Value.TryNext(out id);
        }
        /// <summary>
        /// Obtains a new unique ID from the generator
        /// </summary>
        public static long Next64()
        {
            long id; if (!generator.Value.TryNext(out id))
            {
                throw new ArgumentOutOfRangeException();
            }
            else return id;
        }
        
        /// <summary>
        /// Tries to obtain a new unique ID from the generator
        /// </summary>
        /// <returns>True if the attempt was successful, false otherwise</returns>
        public static bool TryNext32(out int id)
        {
            long tmp; if (generator.Value.TryNext(out tmp))
            {
                id = tmp.GetHashCode();
                return true;
            }
            else
            {
                id = 0;
                return false;
            }
        }
        /// <summary>
        /// Obtains a new unique ID from the generator
        /// </summary>
        public static int Next32()
        {
            int id; if (!TryNext32(out id))
            {
                throw new ArgumentOutOfRangeException();
            }
            else return id;
        }

        private static IdGenerator CreateNewGenerator()
        {
            return new IdGenerator(nextId.Increment());
        }
        private static long GetBitMask(byte bits)
        {
            return (1L << bits) - 1;
        }
    }
}