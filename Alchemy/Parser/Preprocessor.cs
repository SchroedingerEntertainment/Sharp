// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SE.Parsing;

namespace SE.Alchemy
{
    /// <summary>
    /// Performs preprocessing of the input text data based on the Alchemy Preprocessor Rules
    /// </summary>
    public partial class Preprocessor : TreeBuilder<Token, TokenizerState, TextPreprocessorState>
    {
        private readonly static string[] VaParameter = new string[]
        {
            "n"
        };

        IParserContext context;
        HashSet<UInt32> resolverStack;
        Stream outputStream;

        Dictionary<UInt32, Macro> defines;
        /// <summary>
        /// A collection of macros defined for this preprocessor unit
        /// </summary>
        public Dictionary<UInt32, Macro> Defines
        {
            get { return defines; }
        }

        List<string> errors;
        /// <summary>
        /// A collection of error messages occurred during build
        /// </summary>
        public List<string> Errors
        {
            get { return errors; }
        }

        FileDescriptor file;
        /// <summary>
        /// The file currently processed
        /// </summary>
        public string File
        {
            get { return file.GetAbsolutePath(); }
        }

        /// <summary>
        /// Determines if the current open stream is an UTF8 or ASCII stream
        /// </summary>
        public bool IsUtf8
        {
            get 
            {
                if (tokenizer != null)
                {
                    return tokenizer.IsUtf8;
                }
                else return false;
            }
        }

        /// <summary>
        /// Creates a new preprocessor instance
        /// <paramref name="stream">The output data stream to write the converted text to</paramref>
        /// </summary>
        public Preprocessor(Stream stream, IParserContext context)
        {
            this.outputStream = stream;
            this.context = context;
            this.defines = new Dictionary<UInt32, Macro>();
            this.resolverStack = new HashSet<uint>();
            this.errors = new List<string>();
            this.discardNonControlTokens = false;
            this.discardWhitespaceToken = true;
            this.discardNewLineToken = true;
            this.enabled = true;

            Define(ReservedMacros.VariadicConditional_Non_Empty, VaParameter, "n");
            Define(ReservedMacros.VariadicConditional_Empty, VaParameter);
        }

        /// <summary>
        /// Defines a macro for this preprocessor unit
        /// </summary>
        /// <param name="name">The name of the macro to be defined</param>
        /// <param name="replacementList">An optional collection of tokens to add to the source stream</param>
        /// <returns>False if a macro with the same name already exists, true otherwise</returns>
        public bool Define(string name, params TextProcessorToken[] replacementList)
        {
            UInt32 id = name.Fnv32();
            if (!defines.ContainsKey(id))
            {
                Macro macro = new Macro(id, name);
                macro.ReplacementList.AddRange(replacementList);
                defines.Add(macro.Id, macro);

                return true;
            }
            else return false;
        }
        /// <summary>
        /// Defines a macro for this preprocessor unit
        /// </summary>
        /// <param name="name">The name of the macro to be defined</param>
        /// <param name="replacementList">An optional collection of tokens to add to the source stream</param>
        /// <returns>False if a macro with the same name already exists, true otherwise</returns>
        public bool Define(string name, string replacementList)
        {
            if (!defines.ContainsKey(name.Fnv32()))
            {
                MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(replacementList));
                Tokenizer t = new Tokenizer(ms, false);
                t.State.Set(TokenizerState.AfterWhitespace);

                List<TextProcessorToken> tokens = CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Get();
                try
                {
                    TextPointer carret = t.Carret;
                    Token token = t.Read();

                    while (!t.EndOfStream && token != Token.NewLine)
                    {
                        tokens.Add(new TextProcessorToken
                        (
                            token,
                            t.Buffer,
                            carret
                        ));
                        carret = t.Carret;
                        token = t.Read();
                    }
                    return Define(name, tokens.ToArray());
                }
                finally
                {
                    CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Return(tokens);
                }
            }
            else return false;
        }
        /// <summary>
        /// Defines a function like macro for this preprocessor unit
        /// </summary>
        /// <param name="name">The name of the macro to be defined</param>
        /// <param name="parameter">A collection of input parameters for this macro</param>
        /// <param name="replacementList">An optional collection of tokens to add to the source stream</param>
        /// <returns>False if a macro with the same name already exists, true otherwise</returns>
        public bool Define(string name, string[] parameter, params TextProcessorToken[] replacementList)
        {
            UInt32 id = name.Fnv32();
            if (!defines.ContainsKey(id))
            {
                Macro macro = new Macro(id, name);
                foreach (string p in parameter)
                {
                    macro.Parameter.Add(p.Fnv32());
                }
                macro.HasParameter = true;
                macro.ReplacementList.AddRange(replacementList);
                defines.Add(macro.Id, macro);

                return true;
            }
            else return false;
        }
        /// <summary>
        /// Defines a function like macro for this preprocessor unit
        /// </summary>
        /// <param name="name">The name of the macro to be defined</param>
        /// <param name="parameter">A collection of input parameters for this macro</param>
        /// <param name="replacementList">An optional collection of tokens to add to the source stream</param>
        /// <returns>False if a macro with the same name already exists, true otherwise</returns>
        public bool Define(string name, string[] parameter, string replacementList)
        {
            if (!defines.ContainsKey(name.Fnv32()))
            {
                MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(replacementList));
                Tokenizer t = new Tokenizer(ms, false);
                t.State.Set(TokenizerState.AfterWhitespace);

                List<TextProcessorToken> tokens = CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Get();
                try
                {
                    TextPointer carret = t.Carret;
                    Token token = t.Read();

                    while (!t.EndOfStream && token != Token.NewLine)
                    {
                        tokens.Add(new TextProcessorToken
                        (
                            token,
                            t.Buffer,
                            carret
                        ));
                        carret = t.Carret;
                        token = t.Read();
                    }
                    return Define(name, parameter, tokens.ToArray());
                }
                finally
                {
                    CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Return(tokens);
                }
            }
            else return false;
        }

        protected override StreamTokenizer<Token, TokenizerState> Begin(Stream stream, bool isUtf8)
        {
            BuilderState.Reset();
            errors.Clear();
            
            this.discardNonControlTokens = false;
            this.discardWhitespaceToken = true;
            this.discardNewLineToken = true;

            return new Tokenizer(stream, isUtf8);
        }

        protected override bool DiscardToken(Token token, object context)
        {
            if (!enabled)
            {
                switch (token)
                {
                    case Token.EnableDirective:
                        {
                            while (!tokenizer.EndOfStream && token != Token.NewLine)
                            {
                                token = MoveNext();
                            }
                            enabled = true;
                        }
                        break;
                    default:
                        {
                            WriteToStream(token);
                        }
                        break;
                }
                return true;
            }
            else switch (token)
            {
                case Token.IfdefDirective:
                case Token.IfndefDirective:
                case Token.IfDirective:
                case Token.ElifDirective:
                case Token.ElseDirective:
                case Token.EndifDirective:
                    {
                        discardNonControlTokens = false;
                        return false;
                    }
                case Token.NewLine: switch (BuilderState.Current)
                        {
                            case TextPreprocessorState.Ifdef:
                            case TextPreprocessorState.Ifndef:
                            case TextPreprocessorState.If:
                            case TextPreprocessorState.Import:
                            case TextPreprocessorState.Elif:
                            case TextPreprocessorState.Else:
                            case TextPreprocessorState.Endif:
                            case TextPreprocessorState.Error:
                            case TextPreprocessorState.Define:
                                { }
                                return false;
                        default:
                            {
                                if (discardNewLineToken && BuilderState.Current == TextPreprocessorState.Master)
                                {
                                    WriteToStream(token);
                                }
                                return discardNewLineToken;
                            }
                        }
                case Token.Whitespace:
                    {
                        if (discardWhitespaceToken && BuilderState.Current == TextPreprocessorState.Master)
                        {
                            WriteToStream(token);
                        }
                        return discardWhitespaceToken;
                    }
                case Token.Comment:
                    {
                        if (BuilderState.Current == TextPreprocessorState.Master)
                        { }
                        return true;
                    }
                default:
                    {
                        return discardNonControlTokens;
                    }
            }
        }

        protected override bool ProcessToken(Token token, object context)
        {
            file = context as FileDescriptor;

        Head:
            switch (BuilderState.Current)
            {
                #region Master
                case TextPreprocessorState.Master:
                    {
                        if (Master(ref token)) break;
                        else goto Head;
                    }
                #endregion

                #region #ifdef
                case TextPreprocessorState.Ifdef:
                    {
                        if (Ifdef(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #ifndef
                case TextPreprocessorState.Ifndef:
                    {
                        if (Ifndef(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #if
                case TextPreprocessorState.If:
                    {
                        if (If(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #import
                case TextPreprocessorState.Import:
                    {
                        if (Import(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #elif
                case TextPreprocessorState.Elif:
                    {
                        if (Elif(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #else
                case TextPreprocessorState.Else:
                    {
                        if (Else(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #endif
                case TextPreprocessorState.Endif:
                    {
                        if (Endif(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #error
                case TextPreprocessorState.Error:
                    {
                        if (Error(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #define
                case TextPreprocessorState.Define:
                    {
                        if (Define(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #undef
                case TextPreprocessorState.Undefine:
                    {
                        if (Undefine(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #warn
                case TextPreprocessorState.Warning:
                    {
                        if (Warning(token)) break;
                        else goto Head;
                    }
                #endregion

                #region FunctionMacro = '(' Identifier (',' Identifier)* ')';
                case TextPreprocessorState.FunctionMacro:
                    {
                        if (FunctionMacro(token)) break;
                        else goto Head;
                    }
                #endregion

                #region Failure
                case TextPreprocessorState.Failure:
                    {
                        MoveToEnd();
                    }
                    return false;
                #endregion

                default: throw new ArgumentException(BuilderState.Current.ToString());
            }
            return true;
        }

        #region Master
        protected virtual ProductionState Master(ref int state, ref Token token)
        {
            switch (token)
            {
                #region #ifdef
                case Token.IfdefDirective:
                    {
                        BuilderState.Set(TextPreprocessorState.Ifdef);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #ifndef
                case Token.IfndefDirective:
                    {
                        BuilderState.Set(TextPreprocessorState.Ifndef);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #if
                case Token.IfDirective:
                    {
                        BuilderState.Set(TextPreprocessorState.If);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #import
                case Token.ImportDirective:
                    {
                        BuilderState.Set(TextPreprocessorState.Import);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #elif
                case Token.ElifDirective:
                    {
                        BuilderState.Set(TextPreprocessorState.Elif);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #else
                case Token.ElseDirective:
                    {
                        BuilderState.Set(TextPreprocessorState.Else);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #endif
                case Token.EndifDirective:
                    {
                        BuilderState.Set(TextPreprocessorState.Endif);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #error
                case Token.Error:
                    {
                        BuilderState.Set(TextPreprocessorState.Error);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #define
                case Token.DefineDirective:
                    {
                        BuilderState.Set(TextPreprocessorState.Define);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #disable
                case Token.DisableDirective:
                    {
                        while (!tokenizer.EndOfStream && token != Token.NewLine)
                        {
                            token = MoveNext();
                        }
                        enabled = false;
                    }
                    break;
                #endregion

                #region #undef
                case Token.UndefDirective:
                    {
                        BuilderState.Set(TextPreprocessorState.Undefine);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #warn
                case Token.Warning:
                    {
                        BuilderState.Set(TextPreprocessorState.Warning);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region Macro
                case Token.Identifier:
                    {
                        if (!ExpandMacro(Current))
                            goto default;
                    }
                    break;
                #endregion

                #region Translate Concat (##) => DoubleHash (##)
                case Token.Concat:
                    {
                        token = Token.DoubleHash;
                    }
                    return ProductionState.Preserve;
                #endregion

                #region Translate Stringify (#) => Hash (#)
                case Token.Stringify:
                    {
                        token = Token.Hash;
                    }
                    return ProductionState.Preserve;
                #endregion

                default:
                    {
                        WriteToStream(token);
                    }
                    break;
            }
            return ProductionState.Success;
        }
        bool Master(ref Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Master(ref state, ref token), state);
        }
        #endregion

        #region #ifdef
        protected virtual ProductionState Ifdef(ref int state, Token token)
        {
            if (GetConditionalScope(false))
            {
                switch (token)
                {
                    case Token.Identifier:
                        {
                            BeginConditional(Token.IfdefDirective, defines.ContainsKey(Current.Fnv32()));
                        }
                        break;
                    default:
                        {
                            errors.AddFormatted(ErrorMessages.InvalidMacroName, file.FullName, Carret);
                        }
                        return ProductionState.Failure;
                }
            }
            else BeginConditional(Token.IfDirective, false);
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Ifdef(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Ifdef(ref state, token), state);
        }
        #endregion

        #region #ifndef
        protected virtual ProductionState Ifndef(ref int state, Token token)
        {
            if (GetConditionalScope(false))
            {
                switch (token)
                {
                    case Token.Identifier:
                        {
                            BeginConditional(Token.IfndefDirective, !defines.ContainsKey(Current.Fnv32()));
                        }
                        break;
                    default:
                        {
                            errors.AddFormatted(ErrorMessages.InvalidMacroName, file.FullName, Carret);
                        }
                        return ProductionState.Failure;
                }
            }
            else BeginConditional(Token.IfDirective, false);
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Ifndef(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Ifndef(ref state, token), state);
        }
        #endregion

        #region #if
        protected virtual ProductionState If(ref int state, Token token)
        {
            if (GetConditionalScope(false))
            {
                switch (token)
                {
                    default:
                        {
                            PushToken(ValueTuple.Create(token, Current, Carret));
                            bool result; if (EvaluateExpression(out result))
                            {
                                BeginConditional(Token.IfDirective, result);
                            }
                            else BeginConditional(Token.IfDirective, false);
                        }
                        break;
                    case Token.NewLine:
                        {
                            errors.AddFormatted(ErrorMessages.MissingExpressionValue, file.FullName, Carret);
                            BeginConditional(Token.IfDirective, false);
                        }
                        break;
                }
            }
            else BeginConditional(Token.IfDirective, false);
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool If(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(If(ref state, token), state);
        }
        #endregion

        #region #import
        protected virtual ProductionState Import(ref int state, Token token)
        {
            switch (token)
            {
                #region Macro
                case Token.Identifier:
                    {
                        if (!ExpandMacro(Current))
                        {
                            if (!context.AddModule(Current))
                            {
                                errors.AddFormatted(ErrorMessages.ModuleNotFound, file.FullName, Carret, Current);
                                return ProductionState.Failure;
                            }
                            else break;
                        }
                    }
                    return ProductionState.Next;
                #endregion

                #region Literal
                case Token.SingleQuotationLiteral:
                case Token.DoubleQuotationLiteral:
                    {
                        string path = Current;
                        string prefix = string.Empty;
                        switch ((token = PreserveToken()))
                        {
                            case Token.SingleQuotationLiteral:
                            case Token.DoubleQuotationLiteral:
                                {
                                    prefix = Current.Substring(1, Current.Length - 2)
                                                    .Unescape();
                                }
                                break;
                        }
                        Stream stream; if (context.ResolveFileReference(file, ref path, ref prefix, out stream))
                        {
                            if (path.StartsWith("\"?"))
                            {
                                path = Path.Combine(file.Location.GetAbsolutePath(), "*");
                            }
                            if (!string.IsNullOrWhiteSpace(prefix))
                            {
                                MemoryStream ms = MemoryPool<MemoryStream>.Get();
                                try
                                {
                                    if(IsUtf8) ms.Write(Encoding.UTF8.GetBytes(prefix));
                                    else ms.Write(Encoding.ASCII.GetBytes(prefix));
                                    ms.Position = 0;

                                    if (!ExpandFileReference(path, ms))
                                    {
                                        return ProductionState.Failure;
                                    }
                                }
                                finally
                                {
                                    MemoryPool<MemoryStream>.Return(ms);
                                }
                            }
                            if (!ExpandFileReference(path, stream))
                            {
                                return ProductionState.Failure;
                            }
                            else break;
                        }
                        else
                        {
                            errors.AddFormatted(ErrorMessages.FileNotFound, file.FullName, Carret, path);
                            return ProductionState.Failure;
                        }
                    }
                #endregion

                #region BogusLiteral
                case Token.BogusSingleQuotationLiteral:
                case Token.BogusDoubleQuotationLiteral:
                    {
                        errors.AddFormatted(ErrorMessages.UnterminatedFileName, file.FullName, Carret, "#import");
                    }
                    break;
                #endregion

                default:
                    {
                        errors.AddFormatted(ErrorMessages.InvalidFileName, file.FullName, Carret, "#import");
                    }
                    break;
            }
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Import(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Import(ref state, token), state);
        }
        #endregion

        #region #elif
        protected virtual ProductionState Elif(ref int state, Token token)
        {
            if (scopeStack.Count > 0)
            {
                if (scopeStack.Peek().Item1 != Token.ElseDirective)
                {
                    if (GetConditionalScope(true) && !GetConditionalState())
                    {
                        switch (token)
                        {
                            default:
                                {
                                    PushToken(ValueTuple.Create(token, Current, Carret));
                                    bool result; if (EvaluateExpression(out result))
                                    {
                                        BeginConditional(Token.ElifDirective, result);
                                    }
                                    else BeginConditional(Token.ElifDirective, false);
                                }
                                break;
                            case Token.NewLine:
                                {
                                    errors.AddFormatted(ErrorMessages.MissingExpressionValue, file.FullName, Carret);
                                    BeginConditional(Token.ElifDirective, false);
                                }
                                break;
                        }
                    }
                    else BeginConditional(Token.ElifDirective, false);
                }
                else errors.AddFormatted(ErrorMessages.UnexpectedElifConditional, file.FullName, new TextPointer(Carret.Line, 0));
            }
            else errors.AddFormatted(ErrorMessages.UnexpectedElifConditional, file.FullName, new TextPointer(Carret.Line, 0));
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Elif(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Elif(ref state, token), state);
        }
        #endregion

        #region #else
        protected virtual ProductionState Else(ref int state, Token token)
        {
            if (scopeStack.Count > 0)
            {
                ValueTuple<Token, TextPointer, bool> scope = scopeStack.Peek();
                if (scope.Item1 == Token.ElseDirective)
                {
                    errors.AddFormatted(ErrorMessages.UnexpectedElseConditional, file.FullName, new TextPointer(Carret.Line, 0));
                    BeginConditional(Token.ElseDirective, false);
                }
                else BeginConditional(Token.ElseDirective, !GetConditionalState());
            }
            else errors.AddFormatted(ErrorMessages.UnexpectedElseConditional, file.FullName, new TextPointer(Carret.Line, 0));
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Else(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Else(ref state, token), state);
        }
        #endregion

        #region #endif
        protected virtual ProductionState Endif(ref int state, Token token)
        {
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            EndConditional();
            return ProductionState.Success;
        }
        bool Endif(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Endif(ref state, token), state);
        }
        #endregion

        #region #error
        protected virtual ProductionState Error(ref int state, Token token)
        {
            StringBuilder sb = new StringBuilder();
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                sb.Append(Current);
                token = MoveNext();
            }
            errors.AddFormatted(ErrorMessages.PreprocessorError, file.FullName, Carret, sb.ToString());
            scopeStack.Clear();
            return ProductionState.Failure;
        }
        bool Error(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Error(ref state, token), state);
        }
        #endregion

        #region #define
        protected virtual ProductionState Define(ref int state, Token token)
        {
            switch (state)
            {
                case 0: switch (token)
                    {
                        case Token.Identifier:
                            {
                                id = Current.Fnv32();
                                if (defines.ContainsKey(id))
                                {
                                    errors.AddFormatted(ErrorMessages.MacroRedefinition, file.FullName, Carret, Current);
                                    goto default;
                                }
                                else
                                {
                                    discardNewLineToken = false;
                                    discardWhitespaceToken = false;
                                    defines.Add(id, new Macro(id, Current));
                                }
                            }
                            break;
                        default:
                            {
                                errors.AddFormatted(ErrorMessages.InvalidMacroName, file.FullName, Carret);
                            }
                            return ProductionState.Failure;
                    }
                    return ProductionState.Shift;

                case 1:
                    {
                        discardWhitespaceToken = true;
                        switch (token)
                        {
                            case Token.NewLine:
                                {
                                    state = 3;
                                }
                                break;
                            case Token.RoundBracketOpen:
                                {
                                    BuilderState.Add(TextPreprocessorState.FunctionMacro);
                                }
                                return ProductionState.Shift | ProductionState.Reduce;
                            default:
                                {
                                    return ProductionState.Shift | ProductionState.Preserve;
                                }
                        }
                    }
                    goto default;

                case 2:
                    {
                        Macro macro; if (defines.TryGetValue(id, out macro))
                        {
                            List<TextProcessorToken> replacementList = macro.ReplacementList;
                            bool expectsDefinedParameter = false;
                            while (!tokenizer.EndOfStream && token != Token.NewLine)
                            {
                                switch (token)
                                {
                                    case Token.Whitespace:
                                    case Token.Comment:
                                        {
                                            if (replacementList.Count > 0 && replacementList[replacementList.Count - 1].Type != Token.Whitespace)
                                            {
                                                replacementList.Add(new TextProcessorToken(Token.Whitespace, Current, Carret));
                                            }
                                        }
                                        break;
                                    case Token.Identifier:
                                        {
                                            expectsDefinedParameter = false;
                                        }
                                        goto default;
                                    case Token.Stringify:
                                        {
                                            if (macro.HasParameter)
                                            {
                                                replacementList.Add(new TextProcessorToken(token, Current, Carret));
                                                expectsDefinedParameter = true;
                                            }
                                            else replacementList.Add(new TextProcessorToken(Token.Hash, Current, Carret));
                                        }
                                        break;
                                    default:
                                        {
                                            if (expectsDefinedParameter)
                                            {
                                                errors.AddFormatted(ErrorMessages.InvalidStringificationToken, file.FullName, Carret);
                                                return ProductionState.Failure;
                                            }
                                            replacementList.Add(new TextProcessorToken(token, Current, Carret));
                                        }
                                        break;
                                }
                                token = MoveNext();
                            }
                            while (replacementList.Count > 0 && replacementList[replacementList.Count - 1].Type == Token.Whitespace)
                            {
                                replacementList.RemoveAt(replacementList.Count - 1);
                            }
                            if (replacementList.Count > 0)
                            {
                                if (replacementList[0].Type == Token.Concat)
                                {
                                    errors.AddFormatted(ErrorMessages.InvalidPastingOperatorStart, file.FullName, replacementList[0].Carret);
                                    defines.Remove(id);
                                }
                                else if (replacementList[replacementList.Count - 1].Type == Token.Concat)
                                {
                                    errors.AddFormatted(ErrorMessages.InvalidPastingOperatorEnd, file.FullName, replacementList[replacementList.Count - 1].Carret);
                                    defines.Remove(id);
                                }
                            }
                        }
                    }
                    goto default;

                default:
                    {
                        while (!tokenizer.EndOfStream && token != Token.NewLine)
                        {
                            token = MoveNext();
                        }
                        discardWhitespaceToken = true;
                        discardNewLineToken = true;
                    }
                    return ProductionState.Success;
            }
        }
        bool Define(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Define(ref state, token), state);
        }
        #endregion

        #region #undef
        protected virtual ProductionState Undefine(ref int state, Token token)
        {
            switch (token)
            {
                case Token.Identifier:
                    {
                        if (!defines.Remove(Current.Fnv32()))
                        {
                            errors.AddFormatted(ErrorMessages.MacroUndefined, file.FullName, Carret, Current);
                        }
                    }
                    break;
                default:
                    {
                        errors.AddFormatted(ErrorMessages.InvalidMacroName, file.FullName, Carret);
                    }
                    return ProductionState.Failure;
            }
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Undefine(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Undefine(ref state, token), state);
        }
        #endregion

        #region #warn
        protected virtual ProductionState Warning(ref int state, Token token)
        {
            StringBuilder sb = new StringBuilder();
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                sb.Append(Current);
                token = MoveNext();
            }
            errors.AddFormatted(ErrorMessages.PreprocessorError, file.FullName, Carret, sb.ToString());
            return ProductionState.Success;
        }
        bool Warning(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Warning(ref state, token), state);
        }
        #endregion

        #region FunctionMacro = '(' (Identifier (',' Identifier)* )? ((',' VariadicArguments) | VariadicArguments)? ')';
        protected virtual ProductionState FunctionMacro(ref int state, Token token)
        {
            switch (state)
            {
                #region (Identifier ... )? (... | VariadicArguments)? ')'
                case 0: switch (token)
                    {
                        case Token.RoundBracketClose:
                            {
                                defines[id].HasParameter = true;
                                discardWhitespaceToken = false;
                            }
                            return ProductionState.Success;
                        case Token.Identifier:
                            {
                                defines[id].Parameter.Add(Current.Fnv32());
                            }
                            return ProductionState.Shift;
                        case Token.VariableArgs:
                            {
                                defines[id].IsVariadic = true;
                                state = 3;
                            }
                            return ProductionState.Next;
                        case Token.NewLine:
                            {
                                errors.AddFormatted(ErrorMessages.MissingParameterListTerminator, file.FullName, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                        default:
                            {
                                errors.AddFormatted(ErrorMessages.InvalidParameter, file.FullName, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                    }
                #endregion

                #region ... ( ... (',' ...)* )? ...
                case 1: switch (token)
                    {
                        case Token.RoundBracketClose:
                            {
                                defines[id].HasParameter = true;
                                discardWhitespaceToken = false;
                            }
                            return ProductionState.Success;
                        case Token.Comma:
                            return ProductionState.Shift;
                        case Token.NewLine:
                            {
                                errors.AddFormatted(ErrorMessages.MissingParameterListTerminator, file.FullName, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                        default:
                            {
                                errors.AddFormatted(ErrorMessages.MissingParameterListSeparator, file.FullName, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                    }
                #endregion

                #region ... ( ... (... Identifier)* )? ((... VariadicArgs) | ...)? ...
                case 2: switch (token)
                    {
                        case Token.Identifier:
                            {
                                defines[id].Parameter.Add(Current.Fnv32());
                                state = 1;
                            }
                            return ProductionState.Next;
                        case Token.VariableArgs:
                            {
                                defines[id].IsVariadic = true;
                            }
                            return ProductionState.Shift;
                        case Token.NewLine:
                            {
                                errors.AddFormatted(ErrorMessages.MissingParameterListTerminator, file.FullName, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                        default:
                            {
                                errors.AddFormatted(ErrorMessages.InvalidParameter, file.FullName, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                    }
                #endregion

                #region ')'
                case 3: switch (token)
                    {
                        case Token.RoundBracketClose:
                            {
                                defines[id].HasParameter = true;
                                discardWhitespaceToken = false;
                            }
                            return ProductionState.Success;
                        default:
                            {
                                errors.AddFormatted(ErrorMessages.MissingParameterListTerminator, file.FullName, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                    }
                #endregion

                default: return ProductionState.Failure;
            }
        }
        bool FunctionMacro(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(FunctionMacro(ref state, token), state);
        }
        #endregion

        /// <summary>
        /// Tries to evaluate upcoming tokens into a logical state
        /// </summary>
        /// <param name="result">The result of the evaluation</param>
        /// <returns>True if the line was read entirely, false otherwise</returns>
        protected bool EvaluateExpression(out bool result)
        {
            try
            {
                discardNewLineToken = false;
                decimal tmp; if (LogicalOr(out tmp))
                {
                    if (EndOfStream || PreserveToken() == Token.NewLine)
                    {
                        result = (tmp != 0);
                        return true;
                    }
                    else errors.AddFormatted(ErrorMessages.InvalidExpressionValue, file.FullName, Carret);
                }
                result = false;
                return false;
            }
            finally
            {
                discardNewLineToken = true;
            }
        }

        /// <summary>
        /// Tries to expand the given macro into the stream
        /// </summary>
        /// <param name="name">The name of any defined macro in this preprocessor</param>
        /// <returns>True if the macro could be expanded, false otherwise</returns>
        protected bool ExpandMacro(string name)
        {
            return ExpandMacro(name, null, true, true, true);
        }

        /// <summary>
        /// Preserves current state and parses an embedded file reference into the 
        /// current source stream
        /// </summary>
        /// <param name="path">A path the source data is located at</param>
        /// <param name="stream">A stream of source data to be processed</param>
        /// <returns>False if a critical error occurred during parsing, true otherwise</returns>
        protected bool ExpandFileReference(string path, Stream stream)
        {
            List<bool> states = CollectionPool<List<bool>, bool>.Get();
            states.Add(discardNonControlTokens);
            states.Add(discardWhitespaceToken);
            states.Add(discardNewLineToken);
            FileDescriptor currentFile = file;

            Stack<int> tmpProductionStates = productionStates;

            productionStates = StackPool<int>.Get();
            

            try
            {
                if (Parse(stream, true, FileDescriptor.Create(path)))
                {
                    return true;
                }
                else return false;
            }
            finally
            {
                BuilderState.Add(TextPreprocessorState.Import);

                StackPool<int>.Return(productionStates);

                productionStates = tmpProductionStates;

                file = currentFile;
                discardNewLineToken = states[2];
                discardWhitespaceToken = states[1];
                discardNonControlTokens = states[0];
                CollectionPool<List<bool>, bool>.Return(states);
            }
        }

        /// <summary>
        /// Transforms the current token's text data based on the context and writes
        /// it to the underlaying output stream
        /// </summary>
        /// <param name="token">The token to control transformation of the text</param>
        protected void WriteToStream(Token token)
        {
            long origin = outputStream.Position;
            outputStream.Position = outputStream.Length;
            if (tokenizer.IsUtf8)
            {
                outputStream.Write(Encoding.UTF8.GetBytes(context.Transform(token, Current)));
            }
            else outputStream.Write(Encoding.ASCII.GetBytes(context.Transform(token, Current)));
            outputStream.Position = origin;
        }

        protected override bool Finalize(bool result, object context)
        {
            if (BuilderState.Current == TextPreprocessorState.FunctionMacro)
            {
                errors.AddFormatted(ErrorMessages.MissingParameterListTerminator, file.FullName, Carret);
                return false;
            }
            else
            {
                foreach (ValueTuple<Token, TextPointer, bool> scope in scopeStack)
                {
                    errors.AddFormatted(ErrorMessages.UnterminatedDirective, file.FullName, new TextPointer(scope.Item2.Line, 0));
                }
                return result;
            }
        }
    }
}
