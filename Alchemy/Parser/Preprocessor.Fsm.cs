// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;

namespace SE.Alchemy
{
    public partial class Preprocessor
    {
        Stack<int> productionStates = new Stack<int>();
        Stack<ValueTuple<Token, TextPointer, bool>> scopeStack = new Stack<ValueTuple<Token, TextPointer, bool>>();

        bool discardNonControlTokens;
        bool discardWhitespaceToken;
        bool discardNewLineToken;
        bool enabled;

        UInt32 id;

        bool EndExecuteRule(ProductionState fsmCommand, int state)
        {
            switch (fsmCommand)
            {
                case ProductionState.Failure: BuilderState.Add(TextPreprocessorState.Failure); return false;
                case ProductionState.Revert:
                case ProductionState.Success:
                    {
                        if (BuilderState.Remove() != default(TextPreprocessorState))
                        {
                            /*if ((fsmCommand & ProductionState.Revert) != ProductionState.Revert)
                            {
                                CppNode node = stack.Peek();
                                PushToken(ValueTuple.Create(node.CurrentToken, string.Empty, node.Carret));
                            }*/
                        }
                        else BuilderState.Reset();
                    }
                    break;
                default:
                    {
                        if ((fsmCommand & ProductionState.Shift) == ProductionState.Shift)
                            productionStates.Push(state + 1);
                        else
                            productionStates.Push(state);

                        if ((fsmCommand & ProductionState.Reduce) == ProductionState.Reduce)
                            productionStates.Push(0);

                        if ((fsmCommand & ProductionState.Revert) == ProductionState.Revert)
                            goto case ProductionState.Revert;
                    }
                    break;
            }
            if ((fsmCommand & ProductionState.Preserve) == ProductionState.Preserve) return false;
            else return true;
        }
        int BeginExecuteRule()
        {
            if (productionStates.Count == 0)
                productionStates.Push(0);

            return productionStates.Pop();
        }

        void BeginConditional(Token token, bool state)
        {
            scopeStack.Push(ValueTuple.Create(token, Carret, state));
            EvaluateConditionalScope();
        }

        bool GetConditionalScope(bool skipCurrent)
        {
            bool result = true;
            foreach (ValueTuple<Token, TextPointer, bool> scope in scopeStack)
            {
                if (!skipCurrent)
                {
                    result &= scope.Item3;
                    switch (scope.Item1)
                    {
                        case Token.ElifDirective:
                        case Token.ElseDirective:
                            {
                                skipCurrent = true;
                            }
                            break;
                    }
                }
                else switch (scope.Item1)
                    {
                        case Token.IfdefDirective:
                        case Token.IfndefDirective:
                        case Token.IfDirective:
                            {
                                skipCurrent = false;
                            }
                            break;
                    }
            }

            return result;
        }
        bool GetConditionalState()
        {
            bool result = false;
            foreach (ValueTuple<Token, TextPointer, bool> scope in scopeStack)
                switch (scope.Item1)
                {
                    case Token.IfdefDirective:
                    case Token.IfndefDirective:
                    case Token.IfDirective:
                        {
                            result |= scope.Item3;
                        }
                        return result;
                    default:
                        {
                            result |= scope.Item3;
                        }
                        break;
                }

            return result;
        }

        void EvaluateConditionalScope()
        {
            discardNonControlTokens = !GetConditionalScope(false);
        }
        void EndConditional()
        {
            for (; ; )
            {
                if (scopeStack.Count > 0)
                {
                    switch (scopeStack.Pop().Item1)
                    {
                        case Token.IfdefDirective:
                        case Token.IfndefDirective:
                        case Token.IfDirective:
                            {
                                EvaluateConditionalScope();
                            }
                            return;
                    }
                }
                else
                {
                    errors.AddFormatted(ErrorMessages.UnexpectedEndConditional, file.FullName, Carret);
                    break;
                }
            }
        }
    }
}
