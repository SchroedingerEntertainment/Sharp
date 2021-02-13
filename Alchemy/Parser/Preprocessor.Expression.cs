// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SE.Alchemy
{
    public partial class Preprocessor
    {
        bool Primary(out decimal result)
        {
            result = 0;

        Head:
            Token current = PreserveToken();
            switch (current)
            {
                #region ( Expression )
                case Token.RoundBracketOpen:
                    {
                        MoveNext();

                        if (!LogicalOr(out result))
                        {
                            return false;
                        }
                        else if (PreserveToken() != Token.RoundBracketClose)
                        {
                            break;
                        }
                        else MoveNext();
                    }
                    return true;
                #endregion

                #region Numeric
                case Token.Numeric:
                    {
                        MoveNext();

                        StringBuilder sb = new StringBuilder(Current);

                        #region Prefix
                        for (int i = sb.Length - 1; i >= 0; i--)
                            if(sb[i] >= '0' && sb[i] <= '9')
                            {
                                sb.Replace("ll", string.Empty, i, sb.Length - i);
                                sb.Replace("lL", string.Empty, i, sb.Length - i);
                                sb.Replace("LL", string.Empty, i, sb.Length - i);
                                sb.Replace("Ll", string.Empty, i, sb.Length - i);
                                sb.Replace("l", string.Empty, i, sb.Length - i);
                                sb.Replace("L", string.Empty, i, sb.Length - i);
                                sb.Replace("u", string.Empty, i, sb.Length - i);
                                sb.Replace("U", string.Empty, i, sb.Length - i);
                            }
                        #endregion

                        if (sb.Length >= 2 && sb[0] == '0')
                        {
                            if (sb.ToString(0, 2) == "0x")
                            {
                                sb.Remove(0, 2);
                                goto HexValue;
                            }
                            else if (sb[0] >= '0' || sb[0] <= '9')
                            {
                                goto OctalValue;
                            }
                            else goto DecimalValue;
                        }
                        else goto DecimalValue;

                        #region Decimal
                        DecimalValue:
                        if (!decimal.TryParse(sb.ToString(), NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out result))
                        {
                            errors.AddFormatted(ErrorMessages.InvalidNumberExpression, file.FullName, Carret);
                            return false;
                        }
                        else return true;
                        #endregion

                        #region Hex
                        HexValue:
                        try
                        {
                            result = Convert.ToUInt64(sb.ToString(), 16);
                            return true;
                        }
                        catch
                        {
                            errors.AddFormatted(ErrorMessages.InvalidNumberExpression, file.FullName, Carret);
                            return false;
                        }
                        #endregion

                        #region Octal
                        OctalValue:
                        try
                        {
                            result = Convert.ToUInt64(sb.ToString(), 8);
                            return true;
                        }
                        catch
                        {
                            errors.AddFormatted(ErrorMessages.InvalidNumberExpression, file.FullName, Carret);
                            return false;
                        }
                        #endregion
                    }
                #endregion

                #region Identifier
                case Token.Identifier:
                    {
                        MoveNext();

                        Macro macro; if (defines.TryGetValue(Current.Fnv32(), out macro))
                        {
                            if (macro.ReplacementList.Count == 0)
                            {
                                result = 1;
                            }
                            else if (ExpandMacro(Current, null, false, true, false))
                            {
                                goto Head;
                            }
                        }
                        else switch (Current)
                            {
                                #region 'false'
                                case "false":
                                    break;
                                #endregion

                                #region 'true'
                                case "true":
                                    {
                                        result = 1;
                                    }
                                    break;
                                #endregion

                                default:
                                    break;
                            }
                    }
                    return true;
                #endregion

                #region Empty
                case Token.NewLine:
                    {
                        errors.AddFormatted(ErrorMessages.MissingExpressionValue, file.FullName, Carret);
                    }
                    return false;
                #endregion
            }
            errors.AddFormatted(ErrorMessages.InvalidExpressionValue, file.FullName, Carret);
            return false;
        }

        /// <summary>
        /// ~ Expression
        /// ! Expression
        /// </summary>
        bool Unary(out decimal result)
        {
            Token current = PreserveToken();
            switch (current)
            {
                #region ~ Expression
                case Token.BitwiseNot:
                    {
                        MoveNext();

                        if (Unary(out result))
                        {
                            result = ~Decimal.ToInt64(result);
                        }
                        else return false;
                    }
                    return true;
                #endregion

                #region ! Expression
                case Token.LogicalNot:
                    {
                        MoveNext();

                        if (Unary(out result))
                        {
                            result = (result == 0) ? 1 : 0;
                        }
                        else return false;
                    }
                    return true;
                #endregion

                default: return Primary(out result);
            }
        }

        /// <summary>
        /// Expression <=> Expression
        /// </summary>
        bool ThreeWayComparsion(out decimal result)
        {
            if (Unary(out result))
            {
                Token current = PreserveToken();
                switch (current)
                {
                    #region Expression <=> Expression
                    case Token.ThreeWayComparsion:
                        {
                            MoveNext();

                            decimal tmp; if (ThreeWayComparsion(out tmp))
                            {
                                result = result.CompareTo(tmp);
                            }
                            else return false;
                        }
                        return true;
                        #endregion
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Expression < Expression
        /// Expression <= Expression
        /// Expression > Expression
        /// Expression >= Expression
        /// </summary>
        bool ArithmeticEquality(out decimal result)
        {
            if (ThreeWayComparsion(out result))
            {
                Token current = PreserveToken();
                switch (current)
                {
                    #region Expression < Expression
                    case Token.LessThan:
                        {
                            MoveNext();

                            decimal tmp; if (ArithmeticEquality(out tmp))
                            {
                                result = (result < tmp) ? 1 : 0;
                            }
                            else return false;
                        }
                        return true;
                    #endregion

                    #region Expression <= Expression
                    case Token.LessEqual:
                        {
                            MoveNext();

                            decimal tmp; if (ArithmeticEquality(out tmp))
                            {
                                result = (result <= tmp) ? 1 : 0;
                            }
                            else return false;
                        }
                        return true;
                    #endregion

                    #region Expression > Expression
                    case Token.GreaterThan:
                        {
                            MoveNext();

                            decimal tmp; if (ArithmeticEquality(out tmp))
                            {
                                result = (result > tmp) ? 1 : 0;
                            }
                            else return false;
                        }
                        return true;
                    #endregion

                    #region Expression >= Expression
                    case Token.GreaterEqual:
                        {
                            MoveNext();

                            decimal tmp; if (ArithmeticEquality(out tmp))
                            {
                                result = (result >= tmp) ? 1 : 0;
                            }
                            else return false;
                        }
                        return true;
                        #endregion
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Expression == Expression
        /// Expression != Expression
        /// </summary>
        bool LogicalEquality(out decimal result)
        {
            if (ArithmeticEquality(out result))
            {
                Token current = PreserveToken();
                switch (current)
                {
                    #region Expression == Expression
                    case Token.Equal:
                        {
                            MoveNext();

                            decimal tmp; if (LogicalEquality(out tmp))
                            {
                                result = (result == tmp) ? 1 : 0;
                            }
                            else return false;
                        }
                        return true;
                    #endregion

                    #region Expression != Expression
                    case Token.NotEqual:
                        {
                            MoveNext();

                            decimal tmp; if (LogicalEquality(out tmp))
                            {
                                result = (result == tmp) ? 0 : 1;
                            }
                            else return false;
                        }
                        return true;
                        #endregion
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Expression & Expression
        /// </summary>
        bool BitwiseAnd(out decimal result)
        {
            if (LogicalEquality(out result))
            {
                Token current = PreserveToken();
                switch (current)
                {
                    #region Expression & Expression
                    case Token.BitwiseAnd:
                        {
                            MoveNext();

                            decimal tmp; if (BitwiseAnd(out tmp))
                            {
                                result = (Decimal.ToInt64(result) & Decimal.ToInt64(tmp));
                            }
                            else return false;
                        }
                        return true;
                        #endregion
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Expression ^ Expression
        /// </summary>
        bool BitwiseXor(out decimal result)
        {
            if (BitwiseAnd(out result))
            {
                Token current = PreserveToken();
                switch (current)
                {
                    #region Expression ^ Expression
                    case Token.BitwiseXor:
                        {
                            MoveNext();

                            decimal tmp; if (BitwiseXor(out tmp))
                            {
                                result = (Decimal.ToInt64(result) ^ Decimal.ToInt64(tmp));
                            }
                            else return false;
                        }
                        return true;
                        #endregion
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Expression | Expression
        /// </summary>
        bool BitwiseOr(out decimal result)
        {
            if (BitwiseXor(out result))
            {
                Token current = PreserveToken();
                switch (current)
                {
                    #region Expression | Expression
                    case Token.BitwiseOr:
                        {
                            MoveNext();

                            decimal tmp; if (BitwiseOr(out tmp))
                            {
                                result = (Decimal.ToInt64(result) | Decimal.ToInt64(tmp));
                            }
                            else return false;
                        }
                        return true;
                        #endregion
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Expression && Expression
        /// </summary>
        bool LogicalAnd(out decimal result)
        {
            if (BitwiseOr(out result))
            {
                Token current = PreserveToken();
                switch (current)
                {
                    #region Expression && Expression
                    case Token.LogicalAnd:
                        {
                            MoveNext();

                            decimal tmp; if (LogicalAnd(out tmp))
                            {
                                result = ((result != 0) && (tmp != 0)) ? 1 : 0;
                            }
                            else return false;
                        }
                        return true;
                        #endregion
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Expression || Expression
        /// </summary>
        bool LogicalOr(out decimal result)
        {
            if (LogicalAnd(out result))
            {
                Token current = PreserveToken();
                switch (current)
                {
                    #region Expression || Expression
                    case Token.LogicalOr:
                        {
                            MoveNext();

                            decimal tmp; if (LogicalOr(out tmp))
                            {
                                result = ((result != 0) || (tmp != 0)) ? 1 : 0;
                            }
                            else return false;
                        }
                        return true;
                    #endregion
                }
                return true;
            }
            return false;
        }
    }
}
