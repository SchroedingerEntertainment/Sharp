// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SE.Alchemy
{
    /// <summary>
    /// A formatted text stream that will preprocess the udnerlaying input on demand
    /// </summary>
    public partial class ProcessorStream : MemoryStream, IParserContext
    {
        Preprocessor parser;
        Preprocessor.ParserContext context;
        bool isParsing;

        /// <summary>
        /// Determines this stream's encoding
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                if (parser.IsUtf8)
                {
                    return Encoding.UTF8;
                }
                else return Encoding.ASCII;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the current stream position is at the end
        /// of the stream
        /// </summary>
        public bool EndOfStream
        {
            get { return parser.EndOfStream; }
        }

        /// <summary>
        /// A collection of error messages occurred during read
        /// </summary>
        public List<string> Errors
        {
            get { return parser.Errors; }
        }

        public override long Length
        {
            get
            {
                if (base.Position == base.Length && !isParsing)
                {
                    UpdateBuffer(8);
                }
                return base.Length;
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return true; }
        }
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a new stream wrapper around the input data
        /// </summary>
        public ProcessorStream(IParserContext parserContext, Stream stream, Encoding encoding, object context = null)
        {
            if (parserContext == null)
            {
                parserContext = this;
            }
            this.parser = new Preprocessor(this, parserContext);
            this.context = parser.BeginParse(stream, encoding, false, context);
        }
        /// <summary>
        /// Creates a new stream wrapper around the input data
        /// </summary>
        public ProcessorStream(IParserContext parserContext, Stream stream, object context = null)
            : this(parserContext, stream, null, context)
        { }
        /// <summary>
        /// Creates a new stream wrapper around the input data
        /// </summary>
        public ProcessorStream(Stream stream, Encoding encoding, object context = null)
            : this(null, stream, null, context)
        { }
        /// <summary>
        /// Creates a new stream wrapper around the input data
        /// </summary>
        public ProcessorStream(Stream stream, object context = null)
            : this(null, stream, null, context)
        { }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Defines a macro for this preprocessor unit
        /// </summary>
        /// <param name="name">The name of the macro to be defined</param>
        /// <param name="replacementList">An optional collection of tokens to add to the source stream</param>
        /// <returns>False if a macro with the same name already exists, true otherwise</returns>
        public bool Define(string name, string replacementList)
        {
            return parser.Define(name, replacementList);
        }

        public bool AddModule(string id)
        {
            return false;
        }
        public bool ResolveFileReference(object context, ref string path, ref string prefix, out Stream stream)
        {
            stream = null;
            return false;
        }
        public string Transform(Token token, string input)
        {
            return input;
        }

        /// <summary>
        /// Reads data from the underlaying stream to fill the stream buffer
        /// </summary>
        /// <param name="count">The amount of data that should be read</param>
        /// <returns>The amount of data that has been read</returns>
        public int Fill(int count = 128)
        {
            long length = base.Length;
            UpdateBuffer(count);

            return (int)(base.Length - length);
        }
        bool UpdateBuffer(int count)
        {
            isParsing = true;
            try
            {
                if (EndOfStream)
                {
                    return false;
                }
                int remaining = (int)(base.Length - base.Position);
                if (remaining > 0)
                {
                    byte[] buffer = GetBuffer();
                    Buffer.BlockCopy(buffer, count, buffer, 0, (int)(base.Length - count));
                    SetLength(base.Length - count);
                }
                else SetLength(0);
                while (!parser.EndOfStream && base.Length < count)
                {
                    if (!context.ParseNext())
                        return false;
                }
                return (base.Length > 0);
            }
            finally
            {
                isParsing = false;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int total = 0;
            do
            {
                int read = base.Read(buffer, offset, count);
                total += read;

                if (read < count && !UpdateBuffer(count))
                {
                    return total;
                }
                offset = total;
                count -= read;
            }
            while (count > 0);
            return total;
        }
        public override int ReadByte()
        {
            int bt = base.ReadByte();
            if (bt == -1 && UpdateBuffer(1))
            {
                bt = base.ReadByte();
            }
            return bt;
        }
    }
}
