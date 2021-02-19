// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;

namespace SE.CppLang
{
    public partial class Linter
    {
        CppToken GetToken()
        {
            ParserToken<CppToken> token = RawDataBuffer.Get();
            textPointer = token.Carret;
            textBuffer = token.Buffer;

            return token.Type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual bool Verify(CppToken token)
        {
            int overallState = 0;
            try
            {
                switch (token)
                {
                    #region Scope
                    case CppToken.CurlyBracketOpen:
                        {
                            scopeId++;
                        }
                        goto default;
                    case CppToken.CurlyBracketClose:
                        {
                            scopeId--;
                        }
                        goto default;
                    #endregion

                    #region Rules
                    default:
                        {
                            bool result = true;
                            foreach (ParserRule<CppToken> rule in rules)
                            {
                                result &= rule.OnNext(token);
                                overallState |= rule.State;

                                if (!result)
                                    break;
                            }
                            return result;
                        }
                     #endregion
                }
            }
            finally
            {
                if (overallState == 0 && RawDataBuffer.Position >= BackbufferThreshold)
                {
                    int count = ((int)RawDataBuffer.Position - (BackbufferThreshold / 2));

                    RawDataBuffer.Buffer.RemoveRange(0, count);
                    RawDataBuffer.Position -= count;
                }
            }
        }
    }
}
