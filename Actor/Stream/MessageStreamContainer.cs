// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Reactive;

namespace SE.Actor
{
    /// <summary>
    /// Manages conditionally nested streams associated with a root stream
    /// </summary>
    public class MessageStreamContainer<TMessage>
    {
        /// <summary>
        /// Creates a new container
        /// </summary>
        public MessageStreamContainer()
        { }

        /// <summary>
        /// Tries to obtain a stream responsible to handle passed parameters
        /// </summary>
        /// <returns>True if a stream was found or created, false otherwise</returns>
        public virtual bool TryGet(object[] parameter, out IReactiveStream<TMessage, bool> result)
        {
            result = null;
            return false;
        }
        /// <summary>
        /// Tries to obtain a stream responsible to handle the given message
        /// </summary>
        /// <returns>True if a stream was found or created, false otherwise</returns>
        public virtual bool TryGet(ref TMessage message, out IReactiveStream<TMessage, bool> result)
        {
            result = null;
            return false;
        }
    }
}
