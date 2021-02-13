// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using SE.Parsing;

namespace SE.Json
{
    /// <summary>
    /// JSON DOM Assembler
    /// </summary>
    public partial class Parser : TreeBuilder<Token, TokenizerState, ParserState>
    {
        List<JsonNode> nodeStack;
        JsonDocument document;

        List<string> errors;
        /// <summary>
        /// A collection of error messages occurred during build
        /// </summary>
        public List<string> Errors
        {
            get { return errors; }
        }

        bool endOfStream;
        public override bool EndOfStream
        {
            get { return base.EndOfStream | endOfStream; }
        }

        /// <summary>
        /// Creates a new JSON DOM assembler
        /// </summary>
        /// <param name="document">A document hosting the desired DOM</param>
        public Parser(JsonDocument document)
        {
            this.nodeStack = new List<JsonNode>();
            this.errors = new List<string>();
            this.document = document;
        }

        protected override StreamTokenizer<Token, TokenizerState> Begin(Stream stream, bool isUtf8)
        {
            endOfStream = false;
            nodeStack.Clear();
            errors.Clear();

            return new Tokenizer(stream, isUtf8);
        }

        protected override bool DiscardToken(Token token, object context)
        {
            return (token == Token.Whitespace);
        }
        protected override bool ProcessToken(Token token, object context)
        {

        Head:
            switch (BuilderState.Current)
            {
                #region Master
                case ParserState.Master:
                    {
                        if (Master(ref token)) break;
                        else goto Head;
                    }
                #endregion

                #region Object = '{' (Property | (Property ',')+)? '}';
                case ParserState.Object:
                    {
                        if (Object(ref token)) break;
                        else goto Head;
                    }
                #endregion

                #region Array = '[' (Value | (Value ',')+)? ']';
                case ParserState.Array:
                    {
                        if (Array(ref token)) break;
                        else goto Head;
                    }
                #endregion

                #region Property = String ':' Value;
                case ParserState.Property:
                    {
                        if (Property(ref token)) break;
                        else goto Head;
                    }
                #endregion

                #region Value = Object | Array | String | Number | Boolean | Null;
                case ParserState.Value:
                    {
                        if (Value(ref token)) break;
                        else goto Head;
                    }
                #endregion

                #region Failure
                case ParserState.Failure:
                    {
                        if (!TryCorrectError(token))
                        {
                            endOfStream = true;
                            return false;
                        }
                        else return true;
                    }
                #endregion

                default:
                    throw new ArgumentException(BuilderState.Current.ToString());
            }
            return true;
        }

        #region Master
        protected virtual ProductionState Master(ref int state, ref Token token)
        {
            switch (token)
            {
                #region Object { ... }
                case Token.BeginObject:
                    {
                        BuilderState.Add(ParserState.Object);
                        CreateNode(JsonNodeType.Object);
                    }
                    return ProductionState.Next;
                #endregion

                #region Array [ ... ]
                case Token.BeginArray:
                    {
                        BuilderState.Add(ParserState.Array);
                        CreateNode(JsonNodeType.Array);
                    }
                    return ProductionState.Next;
                #endregion
            }
            errors.AddFormatted(ErrorMessages.InvalidStart, Carret);
            return ProductionState.Failure;
        }
        bool Master(ref Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Master(ref state, ref token), state);
        }
        #endregion

        #region Object = '{' (Property | (Property ',')+)? '}';
        protected virtual ProductionState Object(ref int state, ref Token token)
        {
            switch (state)
            {
                #region (Property | (Property ... '}';
                case 0: switch(token)
                    {
                        case Token.String:
                            {
                                BuilderState.Add(ParserState.Property);
                            }
                            return ProductionState.Shift | ProductionState.Preserve | ProductionState.Reduce;

                        case Token.EndObject:
                            return ProductionState.Success;
                    }
                    goto default;
                #endregion

                #region ... (Property ',')+) ... '}';
                case 1: switch(token)
                    {
                        case Token.Comma:
                            {
                                state = 0;
                            }
                            return ProductionState.Next;

                        case Token.EndObject:
                            return ProductionState.Success;
                    }
                    goto default;
                #endregion

                default: return ProductionState.Failure;
            }
        }
        bool Object(ref Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Object(ref state, ref token), state);
        }

        bool TryFixMalformedObject(Token token)
        {
            switch (token)
            {
                default:
                    {
                        if (!tokenizer.SkipToken((c) =>
                        {
                            switch (c)
                            {
                                case '}':
                                case ',':
                                    return true;
                                default:
                                    return false;
                            }

                        })) return false;
                    }
                    break;
                case Token.EndArray:
                case Token.Comma:
                    break;
            }
            productionStates.Push(0);
            return true;
        }
        #endregion

        #region Array = '[' (Value | (Value ',')+)? ']';
        protected virtual ProductionState Array(ref int state, ref Token token)
        {
            switch (state)
            {
                #region (Value | (Value ... ']';
                case 0: switch(token)
                    {
                        case Token.Null:
                        case Token.True:
                        case Token.False:
                        case Token.Numeric:
                        case Token.String:
                        case Token.BeginObject:
                        case Token.BeginArray:
                            {
                                BuilderState.Add(ParserState.Value);
                                CreateNode(JsonNodeType.Undefined);
                            }
                            return ProductionState.Shift | ProductionState.Preserve | ProductionState.Reduce;

                        case Token.EndArray:
                            return ProductionState.Success;
                    }
                    goto default;
                #endregion

                #region ... (Value ',')+) ... ']';
                case 1: switch(token)
                    {
                        case Token.Comma:
                            {
                                state = 0;
                            }
                            return ProductionState.Next;

                        case Token.EndArray:
                            return ProductionState.Success;
                    }
                    goto default;
                #endregion

                default: return ProductionState.Failure;
            }
        }
        bool Array(ref Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Array(ref state, ref token), state);
        }

        bool TryFixMalformedArray(Token token)
        {
            switch (token)
            {
                default:
                    {
                        if (!tokenizer.SkipToken((c) =>
                        {
                            switch (c)
                            {
                                case ']':
                                case ',':
                                    return true;
                                default:
                                    return false;
                            }

                        })) return false;
                    }
                    break;
                case Token.EndArray:
                case Token.Comma:
                    break;
            }
            productionStates.Push(0);
            return true;
        }
        #endregion

        #region Property = String ':' Value;
        protected virtual ProductionState Property(ref int state, ref Token token)
        {
            switch (state)
            {
                #region String
                case 0: switch (token)
                    {
                        case Token.String:
                            {
                                name = Current;
                            }
                            return ProductionState.Shift;
                    }
                    goto default;
                #endregion

                #region ':'
                case 1: switch (token)
                    {
                        case Token.Colon:
                            return ProductionState.Shift;
                    }
                    goto default;
                #endregion

                #region Value
                case 2: switch (token)
                    {
                        case Token.Null:
                        case Token.True:
                        case Token.False:
                        case Token.Numeric:
                        case Token.String:
                        case Token.BeginObject:
                        case Token.BeginArray:
                            {
                                BuilderState.Add(ParserState.Value);
                                CreateNode(JsonNodeType.Empty).Name = name;
                            }
                            return ProductionState.Shift | ProductionState.Preserve | ProductionState.Reduce;
                    }
                    goto default;
                #endregion

                case 3: return ProductionState.Preserve | ProductionState.Success;
                default:
                    return ProductionState.Failure;
            }
        }
        bool Property(ref Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Property(ref state, ref token), state);
        }

        bool TryFixMalformedProperty(Token token)
        {
            if (!tokenizer.SkipToken((c) =>
            {
                switch (c)
                {
                    case ']':
                    case '}':
                    case ',':
                        return true;
                    default:
                        return false;
                }

            })) return false;
            productionStates.Push(3);
            return true;
        }
        #endregion

        #region Value = Object | Array | String | Number | Boolean | Null;
        protected virtual ProductionState Value(ref int state, ref Token token)
        {
            switch (token)
            {
                case Token.BeginObject:
                    {
                        BuilderState.Change(ParserState.Object);
                        nodeStack[nodeStack.Count - 1].Type = JsonNodeType.Object;
                    }
                    return ProductionState.Next;
                case Token.BeginArray:
                    {
                        BuilderState.Change(ParserState.Array);
                        nodeStack[nodeStack.Count - 1].Type = JsonNodeType.Array;
                    }
                    return ProductionState.Next;
                case Token.String:
                    {
                        JsonNode node = nodeStack[nodeStack.Count - 1];
                        node.Type = JsonNodeType.String;
                        node.RawValue = Current;
                    }
                    return ProductionState.Success;
                case Token.Numeric:
                    {
                        JsonNode node = nodeStack[nodeStack.Count - 1];
                        decimal value; if (ParseNumber(out value))
                        {
                            if (value % 1 == 0)
                            {
                                node.Type = JsonNodeType.Integer;
                            }
                            else node.Type = JsonNodeType.Decimal;
                            node.RawValue = value;

                            return ProductionState.Success;
                        }
                        errors.AddFormatted(ErrorMessages.InvalidNumber, Carret, Current);
                        return ProductionState.Failure;
                    }
                case Token.True:
                case Token.False:
                    {
                        JsonNode node = nodeStack[nodeStack.Count - 1];
                        node.RawValue = (token == Token.True);
                        node.Type = JsonNodeType.Bool;
                    }
                    return ProductionState.Success;
                case Token.Null:
                    {
                        nodeStack[nodeStack.Count - 1].Type = JsonNodeType.Empty;
                    }
                    return ProductionState.Success;
            }
            errors.AddFormatted(ErrorMessages.InvalidToken, Carret, token, "Value");
            return ProductionState.Failure;
        }
        bool Value(ref Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Value(ref state, ref token), state);
        }
        #endregion

        /// <summary>
        /// Tries to restore tokens to a valid parser state
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        protected bool TryCorrectError(Token token)
        {
            BuilderState.Remove();
            switch (BuilderState.Current)
            {
                case ParserState.Object:
                    {
                        if (TryFixMalformedObject(token))
                            break;
                    }
                    goto default;
                case ParserState.Array:
                    {
                        if (TryFixMalformedArray(token))
                            break;
                    }
                    goto default;
                case ParserState.Property:
                    {
                        if (TryFixMalformedProperty(token))
                            break;
                    }
                    goto default;
                default: return false;
            }
            errors.AddFormatted(ErrorMessages.ParserError, Carret, Current);
            return true;
        }

        /// <summary>
        /// Converts current token into a managed number type
        /// </summary>
        protected bool ParseNumber(out decimal value)
        {
            StringBuilder sb = new StringBuilder(Current);
            bool isNegative; if (sb[0] == '-')
            {
                sb.Remove(0, 1);
                isNegative = true;
            }
            else isNegative = false;
            value = 0;

            if (sb.Length >= 2 && sb[0] == '0')
            {
                switch (sb[1])
                {
                    case 'x':
                    case 'X':
                        {
                            sb.Remove(0, 2);
                            goto HexValue;
                        }
                    case 'b':
                    case 'B':
                        {
                            sb.Remove(0, 2);
                            goto BinaryValue;
                        }
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9': goto OctalValue;
                    default: goto DecimalValue;
                }
            }
            else goto DecimalValue;

            #region Decimal
            DecimalValue:
            if (!decimal.TryParse(sb.ToString(), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
            {
                return false;
            }
            if (isNegative)
            {
                value = decimal.Negate(value);
            }
            return true;
            #endregion

            #region Hex
            HexValue:
            try
            {
                value = Convert.ToUInt64(sb.ToString(), 16);
                if (isNegative)
                {
                    value = decimal.Negate(value);
                }
                return true;
            }
            catch
            {
                return false;
            }
            #endregion

            #region Binary
            BinaryValue:
            try
            {
                value = Convert.ToUInt64(sb.ToString(), 2);
                if (isNegative)
                {
                    value = decimal.Negate(value);
                }
                return true;
            }
            catch
            {
                return false;
            }
            #endregion

            #region Octal
            OctalValue:
            try
            {
                value = Convert.ToUInt64(sb.ToString(), 8);
                if (isNegative)
                {
                    value = decimal.Negate(value);
                }
                return true;
            }
            catch
            {
                return false;
            }
            #endregion
        }

        JsonNode CreateNode(JsonNodeType type)
        {
            JsonNode root; if (nodeStack.Count > 0)
            {
                root = nodeStack[nodeStack.Count - 1];
            }
            else root = document.Root;
            if (root != null)
            {
                root = document.AddNode(root, type);
            }
            else root = document.AddNode(type);
            nodeStack.Add(root);
            return root;
        }
        
        protected override bool Finalize(bool result, object context)
        {
            if (result && nodeStack.Count > 0)
            {
                string source; if (tokenizer.BaseStream is FileStream)
                {
                    source = (tokenizer.BaseStream as FileStream).Name;
                }
                else source = tokenizer.BaseStream.ToString();

                errors.AddFormatted(ErrorMessages.InvalidEnd, Carret);
                return false;
            }
            else return result;
        }
    }
}
