// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Parsing
{
    /// <summary>
    /// Wraps an underlying text stream to produce a stream of tree-builder tokens 
    /// </summary>
    public class ParserStream<TokenId> : IDisposable where TokenId : struct, IConvertible, IComparable
    {
        ITokenFormatter formatter;

        List<ParserToken<TokenId>> buffer;
        /// <summary>
        /// The underlaying data buffer
        /// </summary>
        public List<ParserToken<TokenId>> Buffer
        {
            get { return buffer; }
        }
        public long Length
        {
            get { return buffer.Count; }
        }

        long position;
        public long Position
        {
            get { return position; }
            set
            {
                if (value < 0) position = 0;
                else if (value > buffer.Count) position = buffer.Count;
                else position = value;
            }
        }

        /// <summary>
        /// An element considered as currently pointing to
        /// </summary>
        public ParserToken<TokenId> Current
        {
            get
            {
                if (position >= buffer.Count) return default(ParserToken<TokenId>);
                else return buffer[(int)position];
            }
            set { buffer[(int)position] = value; }
        }

        /// <summary>
        /// The head element of the buffer
        /// </summary>
        public ParserToken<TokenId> Head
        {
            get
            {
                if (buffer.Count == 0) return default(ParserToken<TokenId>);
                else return buffer[buffer.Count - 1];
            }
        }

        bool result;
        /// <summary>
        /// 
        /// </summary>
        public bool Result
        {
            get { return result; }
            set { result = value; }
        }

        /// <summary>
        /// Creates a new buffer with provided capacity
        /// </summary>
        /// <param name="capacity">The capacity in elements the buffer should manage</param>
        public ParserStream(ITokenFormatter formatter, int capacity)
        {
            this.formatter = formatter;
            this.buffer = new List<ParserToken<TokenId>>(capacity);
            this.result = true;
        }
        /// <summary>
        /// Creates a new buffer instance
        /// </summary>
        public ParserStream(ITokenFormatter formatter)
            :this(formatter, 16)
        { }
        public void Dispose()
        {
            buffer.Clear();
        }

        /// <summary>
        /// Determines if this Stream is at the end of data
        /// </summary>
        /// <returns>True if this Streams end is reached, false otherwise</returns>
        public bool Eof()
        {
            while (Length <= Position && !formatter.EndOfStream && result)
            {
                result = formatter.ParseNext();
            }
            return (Length <= Position);
        }

        /// <summary>
        /// Returns the next token from this Stream
        /// </summary>
        /// <returns>The token read from the stream or default</returns>
        public ParserToken<TokenId> Get()
        {
            if (!Eof())
            {
                ParserToken<TokenId> result = buffer[(int)(position)];
                Position++;

                return result;
            }
            else return default(ParserToken<TokenId>);
        }

        /// <summary>
        /// Returns the next token from this Stream without processing it
        /// </summary>
        /// <returns>The token read from the stream or default</returns>
        public ParserToken<TokenId> Peek()
        {
            if (!Eof())
            {
                return buffer[(int)(position)];
            }
            else return default(ParserToken<TokenId>);
        }

        /// <summary>
        /// Replaces the last item added to the buffer
        /// </summary>
        /// <param name="value">The value to replace the item with</param>
        /// <returns>The value that has been replaced</returns>
        public ParserToken<TokenId> Replace(ParserToken<TokenId> value)
        {
            if (buffer.Count > 0)
            {
                ParserToken<TokenId> result = buffer[buffer.Count - 1];
                buffer[buffer.Count - 1] = value;

                return result;
            }
            else
            {
                buffer.Add(value);
                position++;

                return default(ParserToken<TokenId>);
            }
        }

        /// <summary>
        /// Discards items from the end of this buffer
        /// </summary>
        /// <param name="count">An amount of items to discard</param>
        public void Discard(int count)
        {
            buffer.RemoveRange(buffer.Count - count, count);
            position = buffer.Count;
        }

        public void Flush()
        {
            buffer.RemoveRange(0, (int)position);
            position = 0;
        }
    }
}
