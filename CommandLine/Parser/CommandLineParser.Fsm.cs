// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;

namespace SE.CommandLine
{
    public partial class CommandLineParser
    {
        Stack<int> productionStates = new Stack<int>();

        bool EndExecuteRule(ProductionState fsmCommand, int state)
        {
            switch (fsmCommand)
            {
                case ProductionState.Failure: BuilderState.Add(CommandLineParserState.Failure); return false;
                case ProductionState.Revert:
                case ProductionState.Success:
                    {
                        if (BuilderState.Remove() != default(CommandLineParserState))
                        {

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
    }
}
