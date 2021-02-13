// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SE.Parsing;

namespace SE.Alchemy
{
    public partial class Preprocessor
    {
        /// <summary>
        /// A collection of reserved macro names that might need to be expanded
        /// implicitly
        /// </summary>
        protected static class ReservedMacros
        {
            public const string VariadicArgumentList = "VA_ARGS__";
            public const string VariadicConditional = "VA_OPT__";
            public const string VariadicConditional_Non_Empty = "VA_OPT__Non_Empty";
            public const string VariadicConditional_Empty = "VA_OPT__Empty";
        }
        
        void ProcessTokenPasting(List<TextProcessorToken> left, List<TextProcessorToken> right, List<TextProcessorToken> result)
        {
            for (int i = 0; i < left.Count - 1; i++)
            {
                result.Add(left[i]);
            }
            {
                TextProcessorToken l = left[left.Count - 1];
                TextProcessorToken r = right[0];
                if(ProcessTokenPasting(l, r, result))
                {
                    for (int i = 0; i < result.Count; i++)
                        if (result[i].Type == Token.Concat)
                        {
                            result[i] = new TextProcessorToken
                            (
                                Token.DoubleHash,
                                result[i].Buffer,
                                result[i].Carret
                            );
                        }
                }
            }
            for (int i = 1; i < right.Count; i++)
            {
                result.Add(right[i]);
            }
        }
        void ProcessTokenPasting(List<TextProcessorToken> result)
        {
            for (int index = 0; result.TryGetIndex(x => x.Type == Token.Concat, out index);)
            {
                List<TextProcessorToken> left = null;
                List<TextProcessorToken> right = null;
                try
                {
                    left = CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Get();
                    for (int i = 0; i < index; i++)
                    {
                        left.Add(result[i]);
                    }
                    if (left.Count > 0)
                    {
                        if (left[left.Count - 1].Type == Token.Whitespace)
                        {
                            if (left.Count == 1)
                            {
                                left[left.Count - 1] = new TextProcessorToken(Token.Whitespace, string.Empty, left[left.Count - 1].Carret);
                            }
                            else left.RemoveAt(left.Count - 1);
                        }
                    }
                    else
                    {
                        result.RemoveAt(index);
                        break;
                    }
                    right = CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Get();
                    for (int i = (index + 1); i < result.Count; i++)
                    {
                        right.Add(result[i]);
                    }
                    if (right.Count > 0)
                    {
                        if (right[0].Type == Token.Whitespace)
                        {
                            if (right.Count == 1)
                            {
                                right[0] = new TextProcessorToken(Token.Whitespace, string.Empty, right[0].Carret);
                            }
                            else right.RemoveAt(0);
                        }
                    }
                    else
                    {
                        result.RemoveAt(index);
                        break;
                    }
                    result.Clear();
                    ProcessTokenPasting(left, right, result);
                }
                finally
                {
                    if (left != null)
                    {
                        CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Return(left);
                    }
                    if (right != null)
                    {
                        CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Return(right);
                    }
                }
            }
        }
        bool ProcessTokenPasting(TextProcessorToken l, TextProcessorToken r, List<TextProcessorToken> result)
        {
            #region Encoding
            Encoding encoding; if (tokenizer.IsUtf8)
            {
                encoding = Encoding.UTF8;
            }
            else encoding = Encoding.ASCII;
            #endregion

            MemoryStream ms = new MemoryStream(encoding.GetBytes(String.Concat(l.Buffer, r.Buffer)));
            Tokenizer t = new Tokenizer(ms, tokenizer.IsUtf8);
            t.State.Set(TokenizerState.AfterWhitespace);

            List<TextProcessorToken> tokens = CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Get();
            TextPointer carret = l.Carret;

            do
            {
                TextPointer tmp = t.Carret;
                Token token = t.Read();
                if (token != Token.NewLine)
                {
                    tokens.Add(new TextProcessorToken
                    (
                        token,
                        t.Buffer,
                        new TextPointer
                        (
                            carret.Line + tmp.Line,
                            carret.Column + tmp.Column
                        )
                    ));
                }
            }
            while (!t.EndOfStream);
            try
            {
                switch (tokens.Count)
                {
                    #region Success
                    case 1:
                        {
                            result.Add(tokens[0]);
                            return true;
                        }
                    #endregion

                    #region Check
                    case 2:
                        {
                            switch (tokens[0].Type)
                            {
                                case Token.Identifier:
                                    {
                                        switch (tokens[1].Type)
                                        {
                                            case Token.Numeric:
                                            case Token.DoubleQuotationLiteral:
                                            case Token.SingleQuotationLiteral:
                                                {
                                                    result.Add(l);
                                                    result.Add(r);
                                                }
                                                return true;
                                        }
                                    }
                                    goto Ill_Formed;
                                default:
                                    goto Ill_Formed;
                            }
                        }
                    #endregion

                    #region Ill-Formed
                    default:
                    Ill_Formed:
                        {
                            errors.AddFormatted(ErrorMessages.InvalidPastingToken, file.FullName, Carret, String.Concat(l.Buffer, r.Buffer));
                            result.Add(l);
                            result.Add(r);
                        }
                        return false;
                    #endregion
                }
            }
            finally
            {
                CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Return(tokens);
            }
        }

        bool ExpandFunctionMacro(Macro macro, bool flush)
        {
            if (resolverStack.Contains(macro.Id))
                return false;

            List<List<TextProcessorToken>> parameters; if (ReadParameterList(macro, out parameters))
            {
                List<TextProcessorToken> tokens = CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Get();
                tokens.AddRange(macro.ReplacementList);
                resolverStack.Add(macro.Id);

                int variadicCommaExtension = 0;
                if (parameters.Count > 0)
                {
                    for (int i = 0; i < tokens.Count; i++)
                        switch (tokens[i].Type)
                        {
                            #region Whitespace
                            case Token.Whitespace:
                            case Token.Comment:
                            case Token.NewLine:
                                break;
                            #endregion

                            #region Identifier
                            case Token.Identifier:
                                {
                                    int index;
                                    switch (tokens[i].Buffer)
                                    {
                                        #region __VA_ARGS__
                                        case ReservedMacros.VariadicArgumentList:
                                            {
                                                if (macro.IsVariadic)
                                                {
                                                    index = macro.Parameter.Count;

                                                    #region Extension
                                                    /**
                                                      A compiler extension (, ## __VA_ARGS__) that removes the leading comma if
                                                     __VA_ARGS__ is empty
                                                    */
                                                    if (variadicCommaExtension == 2)
                                                    {
                                                        while (i - 1 > 0 && tokens[i - 1].Type != Token.Comma)
                                                        {
                                                            tokens.RemoveAt(i - 1);
                                                            i--;
                                                        }
                                                        if (parameters[index].Count == 0)
                                                        {
                                                            tokens.RemoveAt(i - 1);
                                                            i--;
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else index = -1;
                                            }
                                            break;
                                        #endregion

                                        #region __VA_OPT__
                                        case ReservedMacros.VariadicConditional:
                                            {
                                                if (macro.IsVariadic)
                                                {
                                                    string name; if (parameters[macro.Parameter.Count].Count > 0)
                                                    {
                                                        name = ReservedMacros.VariadicConditional_Non_Empty;
                                                    }
                                                    else name = ReservedMacros.VariadicConditional_Empty;
                                                    if (ExpandMacro(name, tokens, false, false, false))
                                                    {
                                                        continue;
                                                    }
                                                }
                                                index = -1;
                                            }
                                            break;
                                        #endregion

                                        #region Anything else
                                        default:
                                            {
                                                if (!macro.Parameter.TryGetIndex(tokens[i].Buffer.Fnv32(), out index))
                                                {
                                                    index = -1;
                                                }
                                            }
                                            break;
                                        #endregion
                                    }
                                    if (index >= 0)
                                    {
                                        List<TextProcessorToken> parameter = parameters[index];

                                        tokens.RemoveAt(i);
                                        tokens.InsertRange(i, parameter);
                                        i += (parameter.Count - 1);
                                    }
                                }
                                goto default;
                            #endregion

                            #region Stringify
                            case Token.Stringify:
                                {
                                    tokens.RemoveAt(i);
                                    while (tokens[i].Type == Token.Whitespace)
                                    {
                                        tokens.RemoveAt(i);
                                    }
                                    int index; 
                                    switch (tokens[i].Buffer)
                                    {
                                        #region __VA_ARGS__
                                        case ReservedMacros.VariadicArgumentList:
                                            {
                                                if (macro.IsVariadic)
                                                {
                                                    index = macro.Parameter.Count;
                                                }
                                                else index = -1;
                                            }
                                            break;
                                        #endregion

                                        #region __VA_OPT__
                                        case ReservedMacros.VariadicConditional:
                                            {
                                                if (macro.IsVariadic)
                                                {
                                                    string name; if (parameters[macro.Parameter.Count].Count > 0)
                                                    {
                                                        name = ReservedMacros.VariadicConditional_Non_Empty;
                                                    }
                                                    else name = ReservedMacros.VariadicConditional_Empty;
                                                    if (ExpandMacro(name, tokens, false, false, false))
                                                    {
                                                        continue;
                                                    }
                                                }
                                                index = -1;
                                            }
                                            break;
                                        #endregion

                                        #region Anything else
                                        default:
                                            {
                                                if (!macro.Parameter.TryGetIndex(tokens[i].Buffer.Fnv32(), out index))
                                                {
                                                    index = -1;
                                                }
                                            }
                                            break;
                                        #endregion
                                    }
                                    if (index >= 0)
                                    {
                                        List<TextProcessorToken> parameter = parameters[index];
                                        StringBuilder sb = new StringBuilder();
                                        sb.Append('\"');
                                        foreach (TextProcessorToken t in parameter)
                                        {
                                            foreach (char c in t.Buffer)
                                            {
                                                switch (c)
                                                {
                                                    case '\"':
                                                    case '\\':
                                                        {
                                                            sb.Append('\\');
                                                        }
                                                        goto default;
                                                    default:
                                                        {
                                                            sb.Append(c);
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        sb.Append('\"');
                                        tokens[i] = new TextProcessorToken
                                        (
                                            Token.DoubleQuotationLiteral,
                                            sb.ToString(),
                                            (parameter.Count > 0) ? parameter[0].Carret : tokens[i].Carret
                                        );
                                    }
                                    else tokens[i] = new TextProcessorToken(Token.DoubleQuotationLiteral, tokens[i].Buffer, tokens[i].Carret);
                                }
                                goto default;
                            #endregion

                            #region (,)
                            case Token.Comma:
                                {
                                    variadicCommaExtension = 1;
                                }
                                break;
                            #endregion

                            #region (##)
                            case Token.Concat:
                                {
                                    variadicCommaExtension <<= 1;
                                }
                                break;
                            #endregion

                            #region Anything else
                            default:
                                {
                                    variadicCommaExtension = 0;
                                }
                                break;
                            #endregion
                        }
                }
                foreach (List<TextProcessorToken> parameter in parameters)
                {
                    CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Return(parameter);
                }
                CollectionPool<List<List<TextProcessorToken>>, List<TextProcessorToken>>.Return(parameters);

                ProcessTokenPasting(tokens);
                int count = 0;
                for (int i = tokens.Count - 1; i >= 0; i--)
                {
                    TextProcessorToken token = tokens[i];
                    tokens.RemoveAt(i);

                    if (token.Type == Token.Identifier && ExpandMacro(token.Buffer, null, false, false, false))
                    {
                        continue;
                    }
                    switch (token.Type)
                    {
                        case Token.Whitespace:
                            {
                                if (!PushToken(ValueTuple.Create(token.Type, token.Buffer, Carret), Token.Whitespace))
                                    continue;
                            }
                            break;
                        default:
                            {
                                PushToken(ValueTuple.Create(token.Type, token.Buffer, Carret));
                            }
                            break;
                    }
                    count++;
                }
                CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Return(tokens);
                if (flush)
                {
                    Flush(ProcessToken, count);
                }
                resolverStack.Remove(macro.Id);
                return true;
            }
            else return false;
        }
        bool ReadParameterList(Macro macro, out List<List<TextProcessorToken>> parameters)
        {
            List<TextProcessorToken> preserved = CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Get();
            preserved.Add(new TextProcessorToken(Token.Identifier, Current, Carret));
            parameters = null;
            try
            {
                #region Detect Parameter List Start
                while (!EndOfStream)
                {
                    Token token = MoveNext();
                    switch (token)
                    {
                        case Token.Whitespace:
                        case Token.Comment:
                        case Token.NewLine:
                            {
                                preserved.Add(new TextProcessorToken(token, Current, Carret));
                            }
                            break;
                        case Token.RoundBracketOpen:
                            {
                                preserved.Add(new TextProcessorToken(token, Current, Carret));
                            }
                            goto ParameterList;
                        default:
                            {
                                for (int i = preserved.Count - 1; i >= 0; i--)
                                {
                                    TextProcessorToken ptoken = preserved[i];
                                    PushToken(ValueTuple.Create(ptoken.Type, ptoken.Buffer, ptoken.Carret));
                                }
                                PushToken(ValueTuple.Create(token, Current, Carret));
                            }
                            return false;
                    }
                }
                #endregion

                #region Parameter List Mismatch
            Error:

                for (int i = preserved.Count - 1; i >= 0; i--)
                {
                    TextProcessorToken ptoken = preserved[i];
                    PushToken(ValueTuple.Create(ptoken.Type, ptoken.Buffer, ptoken.Carret));
                }
                MoveNext();
                return false;
                #endregion

                #region Read Parameter List
            ParameterList:

                List<TextProcessorToken> parameter = CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Get();
                parameters = CollectionPool<List<List<TextProcessorToken>>, List<TextProcessorToken>>.Get();
                parameters.Add(parameter);

                bool variableArgs = (macro.IsVariadic && macro.Parameter.Count == 0);
                int scope = 0;

                while (!EndOfStream)
                {
                    Token token = MoveNext();
                    switch (token)
                    {
                        case Token.NewLine:
                        case Token.Whitespace:
                        case Token.Comment:
                            {
                                preserved.Add(new TextProcessorToken(token, Current, Carret));
                                if (parameter.Count > 0 && parameter[parameter.Count - 1].Type != Token.Whitespace)
                                {
                                    parameter.Add(new TextProcessorToken(Token.Whitespace, Current, Carret));
                                }
                            }
                            break;
                        case Token.Comma:
                            {
                                if (!variableArgs)
                                {
                                    preserved.Add(new TextProcessorToken(token, Current, Carret));
                                    while (parameter.Count > 0 && parameter[parameter.Count - 1].Type == Token.Whitespace)
                                    {
                                        parameter.RemoveAt(parameter.Count - 1);
                                    }
                                    parameter = CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Get();
                                    parameters.Add(parameter);

                                    variableArgs = (macro.IsVariadic && macro.Parameter.Count < parameters.Count);
                                }
                                else goto default;
                            }
                            break;
                        case Token.RoundBracketOpen:
                            {
                                scope++;
                            }
                            goto default;
                        case Token.RoundBracketClose:
                            {
                                if (scope == 0)
                                {
                                    preserved.Add(new TextProcessorToken(token, Current, Carret));
                                    if (parameters.Count == 1 && parameter.Count == 0)
                                    {
                                        parameters.RemoveAt(0);
                                        CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Return(parameter);
                                    }
                                    goto Success;
                                }
                                else scope--;
                            }
                            goto default;
                        case Token.Concat:
                            {
                                preserved.Add(new TextProcessorToken(token, Current, Carret));
                                parameter.Add(new TextProcessorToken(Token.DoubleHash, Current, Carret));
                            }
                            break;
                        case Token.Stringify:
                            {
                                preserved.Add(new TextProcessorToken(token, Current, Carret));
                                parameter.Add(new TextProcessorToken(Token.Hash, Current, Carret));
                            }
                            break;
                        default:
                            {
                                preserved.Add(new TextProcessorToken(token, Current, Carret));
                                parameter.Add(new TextProcessorToken(token, Current, Carret));
                            }
                            break;
                    }
                }
                errors.AddFormatted(ErrorMessages.ParameterTerminatorMissing, file.FullName, Carret);
                #endregion

                #region Validate Parameter List
            Success:

                bool success = (macro.Parameter.Count == parameters.Count);
                if (macro.IsVariadic)
                {
                    if (parameters.Count > macro.Parameter.Count)
                    {
                        success = true;
                    }
                    else if (success)
                    {
                        while (parameter.Count > 0 && parameter[parameter.Count - 1].Type == Token.Whitespace)
                        {
                            parameter.RemoveAt(parameter.Count - 1);
                        }
                        parameter = CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Get();
                        parameters.Add(parameter);
                    }
                }
                if (!success)
                {
                    errors.AddFormatted(ErrorMessages.ParameterMismatch, file.FullName, Carret, macro.Name);
                    goto Error;
                }
                else
                {
                    while (parameter.Count > 0 && parameter[parameter.Count - 1].Type == Token.Whitespace)
                    {
                        parameter.RemoveAt(parameter.Count - 1);
                    }
                    return true;
                }
                #endregion
            }
            finally
            {
                CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Return(preserved);
            }
        }

        bool ExpandObjectMacro(Macro macro, List<TextProcessorToken> resolverList, bool flush)
        {
            if (resolverStack.Contains(macro.Id))
                return false;

            List<TextProcessorToken> tokens = CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Get();
            tokens.AddRange(macro.ReplacementList);
            resolverStack.Add(macro.Id);

            ProcessTokenPasting(tokens);
            if (resolverList == null)
            {
                int count = tokens.Count;
                for (int i = tokens.Count - 1; i >= 0; i--)
                {
                    TextProcessorToken token = tokens[i];
                    PushToken(ValueTuple.Create(token.Type, token.Buffer, Carret));
                }
                CollectionPool<List<TextProcessorToken>, TextProcessorToken>.Return(tokens);
                if (flush)
                {
                    Flush(ProcessToken, count);
                }
            }
            else resolverList.AddRange(tokens);
            resolverStack.Remove(macro.Id);
            return true;
        }

        bool ExpandMacro(string name, List<TextProcessorToken> resolverList, bool flushObjectMacro, bool expandFunctionStyleMacro, bool flushfunctionStyleMacro)
        {
            Macro macro; if (defines.TryGetValue(name.Fnv32(), out macro))
            {
                if (macro.Parameter.Count > 0 || macro.IsVariadic)
                {
                    if (expandFunctionStyleMacro)
                    {
                        return ExpandFunctionMacro(macro, flushfunctionStyleMacro);
                    }
                    else return false;
                }
                else return ExpandObjectMacro(macro, resolverList, flushObjectMacro);
            }
            else return false;
        }
    }
}
