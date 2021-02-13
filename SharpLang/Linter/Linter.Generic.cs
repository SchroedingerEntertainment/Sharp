// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;

namespace SE.SharpLang
{
    public partial class Linter
    {
        SharpToken GetToken()
        {
            ParserToken<SharpToken> token = RawDataBuffer.Get();
            textPointer = token.Carret;
            textBuffer = token.Buffer;

            return token.Type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual bool Verify(SharpToken token)
        {
            int overallState = 0;
            try
            {
                switch (token)
                {
                    #region Scope
                    case SharpToken.CurlyBracketOpen:
                        {
                            scopeId++;
                        }
                        goto default;
                    case SharpToken.CurlyBracketClose:
                        {
                            scopeId--;
                        }
                        goto default;
                    #endregion

                    #region Rules
                    default:
                        {
                            bool result = true;
                            foreach (ParserRule<SharpToken> rule in rules)
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
                if (overallState == 0 && RawDataBuffer.Length >= BackbufferThreshold)
                {
                    int count = ((int)RawDataBuffer.Length - (BackbufferThreshold / 2));

                    RawDataBuffer.Buffer.RemoveRange(0, count);
                    RawDataBuffer.Position -= count;
                }
            }
        }
    }
}
