// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Parsing
{
    /// <summary>
    /// Provides a single state based parser rule implementation
    /// </summary>
    public abstract class ParserRule<TokenId> : IDisposable
    {
        int productionState;
        /// <summary>
        /// A state machine indicator used to track the state of this rule
        /// </summary>
        public int State
        {
            get { return productionState; }
            protected set { productionState = value; }
        }

        /// <summary>
        /// Creates a new rule instance
        /// </summary>
        public ParserRule()
        { }
        public virtual void Dispose()
        { }

        /// <summary>
        /// Provides the rule with new data
        /// </summary>
        /// <param name="value">The new element in the sequence</param>
        /// <returns>True if the rule is in a valid state, false otherwise</returns>
        public bool OnNext(TokenId value)
        {
            ProductionState fsmCommand = Process(value);
            switch (fsmCommand)
            {
                case ProductionState.Failure:
                case ProductionState.Revert:
                case ProductionState.Success:
                    {
                        if (fsmCommand == ProductionState.Success)
                        {
                            OnCompleted();
                        }
                        OnReset();
                        productionState = 0;
                    }
                    return (fsmCommand != ProductionState.Failure);
                case ProductionState.Preserve: return true;
                default:
                    {
                        productionState++;
                    }
                    return true;
            }
        }

        /// <summary>
        /// When overriden in an inheriting class, indicates the processing of a new
        /// item from the data stream
        /// </summary>
        /// <param name="value">The item to be processed</param>
        /// <returns>A state that indicates Success, Failure or ongoing processing of the rule</returns>
        protected abstract ProductionState Process(TokenId value);
        
        /// <summary>
        /// Notifies the rule that it is about to be resetted
        /// </summary>
        public virtual void OnReset()
        { }

        /// <summary>
        /// Notifies the rule that it has completed successfully
        /// </summary>
        public virtual void OnCompleted()
        { }
    }
}