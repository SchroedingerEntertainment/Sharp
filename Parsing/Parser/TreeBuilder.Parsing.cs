// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SE.Parsing
{
    public abstract partial class TreeBuilder<TokenId, TokenizerStateId, ParserStateId> where TokenId : struct, IConvertible, IComparable
                                                                                        where TokenizerStateId : struct, IConvertible, IComparable
                                                                                        where ParserStateId : struct, IConvertible, IComparable
    {
        /// <summary>
        /// Maintains the processing state within the parser
        /// </summary>
        public struct ParserContext : IDisposable
        {
            TreeBuilder<TokenId, TokenizerStateId, ParserStateId> root;
            StreamTokenizer<TokenId, TokenizerStateId> defaultTokenizer;
            List<ValueTuple<TokenId, string, TextPointer>> defaultQueue;
            object defaultContext;

            /// <summary>
            /// Gets a value that indicates whether the current stream position is at the end
            /// of the stream
            /// </summary>
            public bool EndOfStream
            {
                get { return root.EndOfStream; }
            }

            bool result;
            /// <summary>
            /// The temporary processing state
            /// </summary>
            public bool Result
            {
                get { return result; }
            }

            /// <summary>
            /// Creates a new parsing instance from the defined parser
            /// </summary>
            /// <param name="stream">An ASCII or UTF8 text stream to process</param>
            /// <param name="discardStream">determines if the stream should be closed after processing ends</param>
            public ParserContext(TreeBuilder<TokenId, TokenizerStateId, ParserStateId> root, Stream stream, Encoding encoding, bool discardStream, object context = null)
            {
                result = true;
                if (root.BuilderState.Count == 0)
                    root.BuilderState.Set(default(ParserStateId));

                if (encoding == null)
                {
                    encoding = stream.GetEncoding();
                }
                if (encoding.Equals(Encoding.ASCII) && encoding.Equals(Encoding.UTF8))
                    throw new FormatException();

                defaultContext = root.currentContext;
                root.currentContext = context;

                defaultQueue = root.preservationQueue;
                root.preservationQueue = CollectionPool<List<ValueTuple<TokenId, string, TextPointer>>, ValueTuple<TokenId, string, TextPointer>>.Get();

                defaultTokenizer = root.tokenizer;
                root.tokenizer = root.Begin(stream, encoding == Encoding.UTF8);
                root.tokenizer.Discard = discardStream;

                this.root = root;
            }
            public void Dispose()
            {
                try
                {
                    result = root.Finalize(result, root.currentContext);
                    if (root.tokenizer != null)
                    {
                        root.tokenizer.Dispose();
                    }
                }
                finally
                {
                    CollectionPool<List<ValueTuple<TokenId, string, TextPointer>>, ValueTuple<TokenId, string, TextPointer>>.Return(root.preservationQueue);

                    root.preservationQueue = defaultQueue;
                    root.currentContext = defaultContext;
                    root.tokenizer = defaultTokenizer;
                }
            }

            /// <summary>
            /// Creates the next item in current tree of tokens from the provided text stream
            /// </summary>
            /// <returns>The temporary processing state</returns>
            public bool ParseNext()
            {
                TokenId token;
                if (root.preservationQueue.Count > 0)
                {
                    ValueTuple<TokenId, string, TextPointer> item = root.preservationQueue[0];
                    root.preservationQueue.RemoveAt(0);

                    root.current = item.Item2;
                    root.textPointer = item.Item3;
                    token = item.Item1;
                }
                else
                {
                    root.textPointer = root.tokenizer.Carret;
                    token = root.tokenizer.Read(root.currentContext);
                    root.current = null;
                }

                if (!root.DiscardToken(token, root.currentContext))
                    result &= root.ProcessToken(token, root.currentContext);

                return result;
            }
        }

        /// <summary>
        /// Starts creating the tree of tokens from the provided text stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        /// <param name="discardStream">determines if the stream should be closed after processing ends</param>
        public ParserContext BeginParse(Stream stream, Encoding encoding, bool discardStream, object context = null)
        {
            return new ParserContext(this, stream, encoding, discardStream, context);
        }
        /// <summary>
        /// Starts creating the tree of tokens from the provided text stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        public ParserContext BeginParse(Stream stream, bool discardStream, object context = null)
        {
            return BeginParse(stream, null, discardStream, context);
        }
        /// <summary>
        /// Starts creating the tree of tokens from the provided text stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        public ParserContext BeginParse(Stream stream, Encoding encoding, object context = null)
        {
            return BeginParse(stream, encoding, true, context);
        }
        /// <summary>
        /// Starts creating the tree of tokens from the provided text stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        public ParserContext BeginParse(Stream stream, object context = null)
        {
            return BeginParse(stream, null, true, context);
        }

        /// <summary>
        /// Creates the tree of tokens from the provided text stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        /// <param name="discardStream">determines if the stream should be closed after processing ends</param>
        /// <returns>True if successfully processed the stream, false otherwise</returns>
        public bool Parse(Stream stream, Encoding encoding, bool discardStream, object context = null)
        {
            bool result = true;
            using (ParserContext ctx = BeginParse(stream, encoding, discardStream, context))
                {
                    while (!EndOfStream)
                    {
                        result = ctx.ParseNext();
                    }
                }
            return result;
        }
        /// <summary>
        /// Creates the tree of tokens from the provided text stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        /// <returns>True if successfully processed the stream, false otherwise</returns>
        public bool Parse(Stream stream, bool discardStream, object context = null)
        {
            return Parse(stream, null, discardStream, context);
        }
        /// <summary>
        /// Creates the tree of tokens from the provided text stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        /// <returns>True if successfully processed the stream, false otherwise</returns>
        public bool Parse(Stream stream, Encoding encoding, object context = null)
        {
            return Parse(stream, encoding, true, context);
        }
        /// <summary>
        /// Creates the tree of tokens from the provided text stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        /// <returns>True if successfully processed the stream, false otherwise</returns>
        public bool Parse(Stream stream, object context = null)
        {
            return Parse(stream, null, true, context);
        }

        /// <summary>
        /// Prepares the parser for a clean run and returns an instance of the desired tokenizer
        /// </summary>
        protected abstract StreamTokenizer<TokenId, TokenizerStateId> Begin(Stream stream, bool isUtf8);
        /// <summary>
        /// Finalizes the parser after a run and returns the final result
        /// </summary>
        protected abstract bool Finalize(bool result, object context);

        /// <summary>
        /// Checks current token for being discarded
        /// </summary>
        /// <returns>True if discarded, false otherwise</returns>
        protected virtual bool DiscardToken(TokenId token, object context)
        {
            return false;
        }
        /// <summary>
        /// Processes current token and returns success or failure. The returned state doesn't has
        /// any impact on the processing loop itself. Set the stream to the end to cancel further
        /// processing
        /// </summary>
        /// <returns>True if successfully handled, false otherwise</returns>
        protected abstract bool ProcessToken(TokenId token, object context);
    }
}
