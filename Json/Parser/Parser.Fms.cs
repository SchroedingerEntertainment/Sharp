// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;

namespace SE.Json
{
    public partial class Parser
    {
        Stack<int> productionStates = new Stack<int>();

        string name;

        bool EndExecuteRule(ProductionState fsmCommand, int state)
        {
            switch (fsmCommand)
            {
                case ProductionState.Failure: BuilderState.Add(ParserState.Failure);
                    return false;
                case ProductionState.Revert:
                case ProductionState.Success:
                    {
                        ParserState last = BuilderState.Current;
                        if (BuilderState.Remove() != default(ParserState))
                        {
                            switch (last)
                            {
                                case ParserState.Value:
                                    {
                                        JsonNode node = nodeStack[nodeStack.Count - 1];
                                        if (node.Type != JsonNodeType.Array && node.Type != JsonNodeType.Object)
                                        {
                                            nodeStack.RemoveAt(nodeStack.Count - 1);
                                        }
                                    }
                                    break;
                                case ParserState.Object:
                                    {
                                        while (nodeStack[nodeStack.Count - 1].Type != JsonNodeType.Object)
                                            nodeStack.RemoveAt(nodeStack.Count - 1);

                                        nodeStack.RemoveAt(nodeStack.Count - 1);
                                    }
                                    break;
                                case ParserState.Array:
                                    {
                                        while (nodeStack[nodeStack.Count - 1].Type != JsonNodeType.Array)
                                            nodeStack.RemoveAt(nodeStack.Count - 1);

                                        nodeStack.RemoveAt(nodeStack.Count - 1);
                                    }
                                    break;
                            }
                            if (nodeStack.Count == 0)
                            {
                                endOfStream = true;
                            }
                        }
                        else
                        {
                            BuilderState.Reset();
                            endOfStream = true;
                        }
                    }
                    break;
                default:
                    {
                        if ((fsmCommand & ProductionState.Success) == ProductionState.Success)
                            goto case ProductionState.Success;

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
            if ((fsmCommand & ProductionState.Preserve) == ProductionState.Preserve)
                return false;
            else
                return true;
        }
        int BeginExecuteRule()
        {
            if (productionStates.Count == 0)
                productionStates.Push(0);

            return productionStates.Pop();
        }
    }
}
