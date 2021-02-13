// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using SE.Parsing;

namespace SE.CommandLine
{
    public partial class CommandLineParser : TreeBuilder<Token, TokenizerState, CommandLineParserState>
    {
        CommandLineOptions options;

        /// <summary>
        /// Creates a new preprocessor instance
        /// <paramref name="stream">The output data stream to write the converted text to</paramref>
        /// </summary>
        public CommandLineParser(CommandLineOptions options)
        {
            this.options = options;
        }

        protected override StreamTokenizer<Token, TokenizerState> Begin(Stream stream, bool isUtf8)
        {
            return new Tokenizer(stream, isUtf8);
        }

        protected override bool DiscardToken(Token token, object context)
        {
            return false;
        }

        protected override bool ProcessToken(Token token, object context)
        {
        Head:
            switch (BuilderState.Current)
            {
                #region Initial
                case CommandLineParserState.Initial:
                    {
                        if (Initial(ref token)) break;
                        else goto Head;
                    }
                #endregion

                #region SingleDashProperty
                case CommandLineParserState.SingleDashProperty:
                    {
                        if (SingleDashProperty(ref token)) break;
                        else goto Head;
                    }
                #endregion

                #region DoubleDashProperty
                case CommandLineParserState.DoubleDashProperty:
                    {
                        if (DoubleDashProperty(ref token)) break;
                        else goto Head;
                    }
                #endregion

                #region PropertyValue
                case CommandLineParserState.PropertyValue:
                case CommandLineParserState.VerbValue:
                    {
                        if (PropertyValue(ref token)) break;
                        else goto Head;
                    }
                #endregion

                #region Failure
                case CommandLineParserState.Failure:
                    {
                        MoveToEnd();
                    }
                    return false;
                #endregion

                default: throw new ArgumentException(BuilderState.Current.ToString());
            }
            return true;
        }

        #region Initial
        protected virtual ProductionState Initial(ref int state, ref Token token)
        {
            switch (token)
            {
                #region ResponseFile (@) STRING_LITERAL
                case Token.ResponseFile:
                    {
                        token = PreserveToken();
                        switch (token)
                        {
                            case Token.StringLiteral:
                            case Token.QuotedStringLiteral:
                                {
                                    if (ReadResponseFile())
                                    {
                                        return ProductionState.Next;
                                    }
                                }
                                goto default;
                            default:
                                {
                                    MoveNext();
                                    PushToken(ValueTuple.Create(Token.StringLiteral, string.Concat("@", Current), Carret));
                                    BuilderState.Add(CommandLineParserState.DoubleDashProperty);
                                }
                                break;
                        }
                    }
                    break;
                #endregion

                #region Property Identifier (-) e.g. (/)
                case Token.Identifier:
                    {
                        if ((options.Flags & CommandLineFlags.AllowCompound) == CommandLineFlags.AllowCompound)
                        {
                            BuilderState.Add(CommandLineParserState.SingleDashProperty);
                        }
                        else BuilderState.Add(CommandLineParserState.DoubleDashProperty);
                    }
                    break;
                #endregion

                #region Property Identifier (--)
                case Token.LongIdentifier:
                    {
                        BuilderState.Add(CommandLineParserState.DoubleDashProperty);
                    }
                    break;
                #endregion

                #region Separator (=) e.g. (:)
                case Token.Separator:
                    {
                        BuilderState.Add(CommandLineParserState.PropertyValue);
                    }
                    break;
                #endregion

                #region PlainText delimiter (--)
                case Token.Delimiter:
                    {
                        BuilderState.Add(CommandLineParserState.PlainText);
                    }
                    break;
                #endregion

                default:
                    {
                        BuilderState.Add(CommandLineParserState.VerbValue);
                    }
                    return ProductionState.Preserve | ProductionState.Reduce;
            }
            return ProductionState.Reduce;
        }
        bool Initial(ref Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Initial(ref state, ref token), state);
        }
        #endregion

        #region SingleDashProperty (-) e.g. (:)
        protected virtual ProductionState SingleDashProperty(ref int state, ref Token token)
        {
            if (token == Token.StringLiteral)
            {
                string name; if ((options.Flags & CommandLineFlags.IgnoreCase) == CommandLineFlags.IgnoreCase)
                {
                    name = Current.ToLowerInvariant();
                }
                else name = Current;
                for (int i = 0; i < name.Length; i++)
                {
                    switch (name[i])
                    {
                        case '-':
                            if (i != 0)
                            {
                                options.AddToken(CommandLineTokenType.Bool).Value = false;
                            }
                            break;
                        case '+':
                            if (i != 0)
                            {
                                options.AddToken(CommandLineTokenType.Bool).Value = true;
                            }
                            break;
                        default:
                            {
                                options.AddToken(CommandLineTokenType.Property).Value = name[i].ToString();
                            }
                            break;
                    }
                }
                return ProductionState.Success;
            }
            else return ProductionState.Failure;
        }
        bool SingleDashProperty(ref Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(SingleDashProperty(ref state, ref token), state);
        }
        #endregion

        #region DoubleDashProperty (--)
        protected virtual ProductionState DoubleDashProperty(ref int state, ref Token token)
        {
            if (token == Token.StringLiteral)
            {
                string name; if ((options.Flags & CommandLineFlags.IgnoreCase) == CommandLineFlags.IgnoreCase)
                {
                    name = Current.ToLowerInvariant();
                }
                else name = Current;
                switch (name[name.Length - 1])
                {
                    case '-':
                        {
                            options.AddToken(CommandLineTokenType.Property).Value = name.Substring(0, name.Length - 1);
                            options.AddToken(CommandLineTokenType.Bool).Value = false;
                        }
                        break;
                    case '+':
                        {
                            options.AddToken(CommandLineTokenType.Property).Value = name.Substring(0, name.Length - 1);
                            options.AddToken(CommandLineTokenType.Bool).Value = true;
                        }
                        break;
                    default:
                        {
                            options.AddToken(CommandLineTokenType.Property).Value = name;
                        }
                        break;
                }
                return ProductionState.Success;
            }
            else return ProductionState.Failure;
        }
        bool DoubleDashProperty(ref Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(DoubleDashProperty(ref state, ref token), state);
        }
        #endregion

        #region PropertyValue
        protected virtual ProductionState PropertyValue(ref int state, ref Token token)
        {
            switch (token)
            {
                #region Empty
                case Token.Separator:
                    {
                        AddProperty(CommandLineTokenType.String, string.Empty);
                    }
                    return ProductionState.Success;
                #endregion

                #region Numeric
                case Token.Numeric:
                    {
                        decimal value; if (ParseNumber(out value))
                        {
                            if (value % 1 == 0)
                            {
                                AddProperty(CommandLineTokenType.Integer, value);
                            }
                            else AddProperty(CommandLineTokenType.Decimal, value);
                            return ProductionState.Success;
                        }
                    }
                    break;
                #endregion

                #region String
                case Token.QuotedStringLiteral:
                    {
                        AddProperty(CommandLineTokenType.String, Current);
                        return ProductionState.Success;
                    }
                case Token.StringLiteral:
                    {
                        switch (Current.ToLowerInvariant())
                        {
                            #region Boolean (true)
                            case "true":
                            case "on":
                            case "yes":
                                {
                                    AddProperty(CommandLineTokenType.Bool, true);
                                }
                                break;
                            #endregion

                            #region Boolean (false)
                            case "false":
                            case "off":
                            case "no":
                                {
                                    AddProperty(CommandLineTokenType.Bool, true);
                                }
                                break;
                            #endregion

                            default:
                                {
                                    AddProperty(CommandLineTokenType.String, Current);
                                }
                                break;
                        }
                        return ProductionState.Success;
                    }
                #endregion
            }
            return ProductionState.Failure;
        }
        bool PropertyValue(ref Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(PropertyValue(ref state, ref token), state);
        }
        #endregion

        /// <summary>
        /// Converts current token into a managed number type
        /// </summary>
        protected bool ParseNumber(out decimal value)
        {
            StringBuilder sb = new StringBuilder(Current);
            sb.Replace("_", string.Empty);
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
            return true;
            #endregion

            #region Hex
            HexValue:
            try
            {
                value = Convert.ToUInt64(sb.ToString(), 16);
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
                return true;
            }
            catch
            {
                return false;
            }
            #endregion
        }

        /// <summary>
        /// Adds a property value to the token list. Free float properties are attached
        /// to an auto-generated property if AllowFreeValues is unset
        /// </summary>
        protected void AddProperty(CommandLineTokenType type, object value)
        {
            if (BuilderState.Current == CommandLineParserState.VerbValue && (options.Flags & CommandLineFlags.AllowVerbValues) != CommandLineFlags.AllowVerbValues)
            {
                options.GenerateProperty();
                CommandLineToken root = options.First;
                while(root.Next != null && root.Next.Type > CommandLineTokenType.Property && root.Next.Type < CommandLineTokenType.PlainText)
                {
                    root = root.Next;
                }
                options.AddToken(root, type).Value = value;
            }
            else options.AddToken(type).Value = value;
        }

        protected bool ReadResponseFile()
        {
            if (File.Exists(Current))
                try
                {
                    using (FileStream fs = new FileStream(Current, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (StreamReader reader = new StreamReader(fs, true))
                        options.Load(GetLines(reader));

                    return true;
                }
                catch (UnauthorizedAccessException)
                { }
                catch (FileNotFoundException)
                { }
            return false;
        }
        IEnumerable<string> GetLines(StreamReader reader)
        {
            while (reader.Peek() != -1)
            {
                yield return reader.ReadLine();
            }
        }

        protected override bool Finalize(bool result, object context)
        {
            return result;
        }
    }
}
