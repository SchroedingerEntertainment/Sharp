// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;
using SE.Reactive;

namespace SE.SharpLang
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenStream : StreamedReceiver<ParserToken<SharpToken>>
    {
        TreeBuilder<Token, TokenizerState, PreprocessorStates>.ParserContext tokenSource;
        /// <summary>
        /// 
        /// </summary>
        public TreeBuilder<Token, TokenizerState, PreprocessorStates>.ParserContext TokenSource
        {
            get { return tokenSource; }
            set { tokenSource = value; }
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
        /// 
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <param name="capacity"></param>
        public TokenStream(int capacity)
            : base(capacity)
        {
            this.result = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenSource"></param>
        public TokenStream()
            : this(16)
        { }

        public override bool Eof()
        {
            while (Length <= Position && !tokenSource.EndOfStream && result)
            {
                result &= tokenSource.ParseNext();
            }
            return base.Eof();
        }
    }
}
