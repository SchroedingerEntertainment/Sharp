// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SE.Parsing;

namespace SE.SharpLang
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Linter : IDisposable
    {
        const int BackbufferThreshold = 16;

        class LinterPreprocessor : Preprocessor
        {
            TokenStream streamBuffer;
            public TokenStream RawDataBuffer
            {
                get { return streamBuffer; }
            }

            public LinterPreprocessor()
            {
                this.streamBuffer = new TokenStream(BackbufferThreshold);
            }

            protected override void OnNextParserToken(ParserToken<Token> token)
            {
                switch (token.Type)
                {
                    case Token.Whitespace:
                    case Token.NewLine: return;
                    case Token.Identifier:
                    {
                        Keywords keyword; if (token.Buffer.IsLowerCase() && Enum.TryParse(token.Buffer.ToTitleCase(), out keyword))
                        {
                            streamBuffer.OnNext(new ParserToken<SharpToken>((SharpToken)keyword, token.Buffer, token.Carret));
                            break;
                        }
                    }
                    goto default;
                    default: streamBuffer.OnNext(new ParserToken<SharpToken>((SharpToken)token.Type, token.Buffer, token.Carret));
                    break;
                }
            }
        }

        readonly LinterPreprocessor preprocessor;
        readonly List<ParserRule<SharpToken>> rules;

        /// <summary>
        /// Returns the underlaying raw data buffer
        /// </summary>
        public TokenStream RawDataBuffer
        {
            get { return preprocessor.RawDataBuffer; }
        }

        /// <summary>
        /// Gets a value that indicates whether the current stream position is at the end
        /// of the stream
        /// </summary>
        public bool EndOfStream
        {
            get { return preprocessor.RawDataBuffer.Eof(); }
        }

        /// <summary>
        /// Returns the read position used in a rule
        /// </summary>
        public long Position
        {
            get { return preprocessor.RawDataBuffer.Position; }
            set { preprocessor.RawDataBuffer.Position = value; }
        }

        ProcessingState<LinterState> state;
        /// <summary>
        /// Returns the linter state used while processing
        /// </summary>
        public ProcessingState<LinterState> State
        {
            get { return state; }
        }

        TextPointer textPointer;
        /// <summary>
        /// Returns current file position
        /// </summary>
        public TextPointer Carret
        {
            get { return textPointer; }
            set { textPointer = value; }
        }

        string textBuffer;
        /// <summary>
        /// Returns current buffer content for further use
        /// </summary>
        public string Buffer
        {
            get
            {
                if (textBuffer == null) return string.Empty;
                else return textBuffer;
            }
        }

        int scopeId;
        /// <summary>
        /// Returns current code scope position
        /// </summary>
        public int Scope
        {
            get { return scopeId; }
        }

        /// <summary>
        /// A collection of symbols defined for this preprocessor unit
        /// </summary>
        public List<UInt32> Defines
        {
            get { return preprocessor.Defines; }
        }

        /// <summary>
        /// A collection of error messages accrued during build
        /// </summary>
        public List<string> Errors
        {
            get { return preprocessor.Errors; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Linter()
        {
            this.preprocessor = new LinterPreprocessor();
            this.rules = new List<ParserRule<SharpToken>>();
            this.state = new ProcessingState<LinterState>();
        }
        public void Dispose()
        {
            preprocessor.RawDataBuffer.Dispose();
        }

        /// <summary>
        /// Defines a preprocessor symbol
        /// </summary>
        /// <param name="name">The symbol to be defined</param>
        /// <returns>False if the named symbol already exists, true otherwise</returns>
        public bool Define(string name)
        {
            UInt32 id = name.Fnv32();
            if (!preprocessor.Defines.Contains(id))
            {
                preprocessor.Defines.Add(id);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Adds a new linting rule to the collection of rules
        /// </summary>
        /// <param name="rule"></param>
        public void AddRule(ParserRule<SharpToken> rule)
        {
            rules.Add(rule);
        }

        /// <summary>
        /// Creates the tree of tokens from the provided text stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        /// <param name="discardStream">determines if the stream should be closed after processing ends</param>
        /// <returns>True if successfully processed the stream, false otherwise</returns>
        public bool Parse(Stream stream, Encoding encoding, bool discardStream, object context = null)
        {
            foreach (ParserRule<SharpToken> rule in rules)
            {
                rule.OnReset();
            }
            RawDataBuffer.Result = true;
            bool result = true;

            using (TreeBuilder<Token, TokenizerState, PreprocessorStates>.ParserContext ctx = preprocessor.BeginParse(stream, encoding, discardStream, context))
            {
                RawDataBuffer.TokenSource = ctx;
                while (!RawDataBuffer.Eof() && RawDataBuffer.Result)
                {
                    result |= Verify(GetToken());
                }
            }
            return (RawDataBuffer.Result & result);
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
    }
}
