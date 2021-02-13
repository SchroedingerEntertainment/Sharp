// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Parsing
{
    /// <summary>
    /// Default rule policy
    /// </summary>
    public class ParserRulePolicy<T, TokenId> : IPoolingPolicy<T> where T : ParserRule<TokenId>, new()
    {
        /// <summary>
        /// A default rule pooling policy instance
        /// </summary>
        public readonly static ParserRulePolicy<T, TokenId> Default = new ParserRulePolicy<T, TokenId>();

        /// <summary>
        /// Creates a new pooling policy instance
        /// </summary>
        public ParserRulePolicy()
        { }

        public T Create()
        {
            return new T();
        }
        public bool Return(T instance)
        {
            instance.Dispose();
            return true;
        }
        public bool Delete(T instance)
        {
            instance.Dispose();
            return true;
        }
    }
}