// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SE.Parsing;

namespace SE.CppLang
{
    /// <summary>
    /// A multidimensional regular expression like rule validator 
    /// </summary>
    public partial class Linter : IDisposable
    {
        const int BackbufferThreshold = 16;

        public delegate bool DependencyResolveCallback(FileDescriptor file, bool relative, ref string path, out Stream stream);
        class LinterPreprocessor : Preprocessor
        {
            Linter parent;

            TokenStream streamBuffer;
            public TokenStream RawDataBuffer
            {
                get { return streamBuffer; }
            }

            public LinterPreprocessor(Linter parent)
            {
                this.streamBuffer = new TokenStream(BackbufferThreshold);
                this.parent = parent;
            }

            protected override bool ResolveFileReference(ParserToken<Token> source, out string path, out Stream stream)
            {
                switch (source.Type)
                {
                    case Token.UnqoutedHeaderName:
                        {
                            path = source.Buffer;
                            if (parent.DependencyResolve != null && parent.DependencyResolve.Invoke(FileDescriptor.Create(File), false, ref path, out stream))
                                return true;
                        }
                        break;
                    case Token.StringLiteral:
                        {
                            path = source.Buffer;
                            if (parent.DependencyResolve != null && parent.DependencyResolve.Invoke(FileDescriptor.Create(File), true, ref path, out stream))
                                return true;
                        }
                        break;
                }
                return base.ResolveFileReference(source, out path, out stream);
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
                            streamBuffer.OnNext(new ParserToken<CppToken>((CppToken)keyword, token.Buffer, token.Carret));
                            break;
                        }
                    }
                    goto default;
                    default: streamBuffer.OnNext(new ParserToken<CppToken>((CppToken)token.Type, token.Buffer, token.Carret));
                    break;
                }
            }
        }

        readonly LinterPreprocessor preprocessor;
        readonly List<ParserRule<CppToken>> rules;

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
        public Dictionary<UInt32, Macro> Defines
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
        /// Occurs when the resolution of an include dependency is needed
        /// </summary>
        public event DependencyResolveCallback DependencyResolve;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public Linter()
        {
            this.preprocessor = new LinterPreprocessor(this);
            this.rules = new List<ParserRule<CppToken>>();
            this.state = new ProcessingState<LinterState>();
        }
        public void Dispose()
        {
            preprocessor.RawDataBuffer.Dispose();
        }

        /// <summary>
        /// Defines a macro for the preprocessor unit
        /// </summary>
        /// <param name="name">The name of the macro to be defined</param>
        /// <param name="replacementList">An optional collection of tokens to add to the source stream</param>
        /// <returns>False if a macro with the same name already exists, true otherwise</returns>
        public bool Define(string name, string replacementList)
        {
            return preprocessor.Define(name, replacementList);
        }
        /// <summary>
        /// Defines a function like macro for the preprocessor unit
        /// </summary>
        /// <param name="name">The name of the macro to be defined</param>
        /// <param name="parameter">A collection of input parameters for this macro</param>
        /// <param name="replacementList">An optional collection of tokens to add to the source stream</param>
        /// <returns>False if a macro with the same name already exists, true otherwise</returns>
        public bool Define(string name, string[] parameter, string replacementList)
        {
            return preprocessor.Define(name, parameter, replacementList);
        }

        /// <summary>
        /// Adds a new linting rule to the collection of rules
        /// </summary>
        /// <param name="rule"></param>
        public void AddRule(ParserRule<CppToken> rule)
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
            foreach (ParserRule<CppToken> rule in rules)
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
