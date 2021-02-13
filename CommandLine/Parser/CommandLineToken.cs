// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.CommandLine
{
    /// <summary>
    /// A parsed command line argument
    /// </summary>
    public class CommandLineToken
    {
        CommandLineOptions owner;

        protected CommandLineTokenType type;
        /// <summary>
        /// The type of data this token stands for
        /// </summary>
        public CommandLineTokenType Type
        {
            get 
            {
                return type;
            }
        }

        protected object data;
        /// <summary>
        /// A data value object for this token
        /// </summary>
        public object Value
        {
            get 
            {
                return data;
            }
            set 
            {
                object tmp = data;
                data = value;

                if (!data.Equals(tmp))
                {
                    owner.Invalidate(this, tmp);
                }
            }
        }

        protected CommandLineToken next;
        /// <summary>
        /// A connection to the next sibling token
        /// </summary>
        public CommandLineToken Next
        {
            get 
            {
                return next;
            }
            internal set 
            {
                next = value;
            }
        }

        /// <summary>
        /// Determines a value token
        /// </summary>
        public bool IsValue
        {
            get { return (type >= CommandLineTokenType.Bool && type <= CommandLineTokenType.String); }
        }

        /// <summary>
        /// Creates a new empty command line argument
        /// </summary>
        internal CommandLineToken(CommandLineOptions owner, CommandLineTokenType type)
        {
            this.owner = owner;
            this.type = type;
        }

        /// <summary>
        /// Returns the contained raw data as bool value
        /// </summary>
        public bool ToBoolean()
        {
            if (type != CommandLineTokenType.Bool)
                throw new InvalidCastException();

            return Convert.ToBoolean(data);
        }

        /// <summary>
        /// Returns the contained raw data as 16 bit integer value
        /// </summary>
        public Int16 ToInt16()
        {
            if (type != CommandLineTokenType.Integer)
                throw new InvalidCastException();

            return Decimal.ToInt16(Convert.ToDecimal(data));
        }
        /// <summary>
        /// Returns the contained raw data as 16 bit unsigned integer value
        /// </summary>
        public UInt16 ToUInt16()
        {
            if (type != CommandLineTokenType.Integer)
                throw new InvalidCastException();

            return Decimal.ToUInt16(Convert.ToDecimal(data));
        }
        /// <summary>
        /// Returns the contained raw data as 32 bit integer value
        /// </summary>
        public Int32 ToInt32()
        {
            if (type != CommandLineTokenType.Integer)
                throw new InvalidCastException();

            return Decimal.ToInt32(Convert.ToDecimal(data));
        }
        /// <summary>
        /// Returns the contained raw data as 32 bit unsigned integer value
        /// </summary>
        public UInt32 ToUInt32()
        {
            if (type != CommandLineTokenType.Integer)
                throw new InvalidCastException();

            return Decimal.ToUInt32(Convert.ToDecimal(data));
        }
        /// <summary>
        /// Returns the contained raw data as 64 bit unsigned integer value
        /// </summary>
        public UInt64 ToUInt64()
        {
            if (type != CommandLineTokenType.Integer)
                throw new InvalidCastException();

            return Decimal.ToUInt64(Convert.ToDecimal(data));
        }
        /// <summary>
        /// Returns the contained raw data as 32 bit integer value
        /// </summary>
        public Int64 ToInt64()
        {
            if (type != CommandLineTokenType.Integer)
                throw new InvalidCastException();

            return Decimal.ToInt64(Convert.ToDecimal(data));
        }

        /// <summary>
        /// Returns the contained raw data as 32 bit floating point value
        /// </summary>
        public Single ToSingle()
        {
            if (type != CommandLineTokenType.Decimal)
                throw new InvalidCastException();

            return Decimal.ToSingle(Convert.ToDecimal(data));
        }
        /// <summary>
        /// Returns the contained raw data as 64 bit floating point value
        /// </summary>
        public Double ToDouble()
        {
            if (type != CommandLineTokenType.Decimal)
                throw new InvalidCastException();

            return Decimal.ToDouble(Convert.ToDecimal(data));
        }

        /// <summary>
        /// Returns the contained raw data as .NET decimal value
        /// </summary>
        public Decimal ToInteger()
        {
            if (type != CommandLineTokenType.Integer)
                throw new InvalidCastException();

            return Decimal.Truncate(Convert.ToDecimal(data));
        }
        /// <summary>
        /// Returns the contained raw data as .NET decimal value
        /// </summary>
        public Decimal ToDecimal()
        {
            if (type != CommandLineTokenType.Decimal)
                throw new InvalidCastException();

            return Convert.ToDecimal(data);
        }

        /// <summary>
        /// Returns the contained raw data as .NET string value
        /// </summary>
        public override string ToString()
        {
            switch(type)
            {
                case CommandLineTokenType.Bool:
                case CommandLineTokenType.Integer:
                case CommandLineTokenType.Decimal:
                case CommandLineTokenType.String: return data.ToString();
                default: return type.ToString();
            }
        }
    }
}
