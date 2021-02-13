// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;
using SE.Reactive;

namespace SE.Actor
{
    /// <summary>
    /// Manages typed nested streams associated with a root stream
    /// </summary>
    public class TypedStreamContainer<TMessage, TDispatcher> : MessageStreamContainer<TMessage> where TDispatcher : IDispatcher<TMessage>, new()
    {
        private readonly static Type ObjectType = typeof(object);

        Dictionary<Type, IReactiveStream<TMessage, bool>> streams;
        ReadWriteLock streamsLock;

        bool allowDowncast;
        /// <summary>
        /// Enables type downcasting when resolving a message
        /// </summary>
        public bool AllowDowncast
        {
            get { return allowDowncast; }
            set { allowDowncast = value; }
        }

        /// <summary>
        /// Creates a new container
        /// </summary>
        public TypedStreamContainer()
        {
            this.streams = new Dictionary<Type, IReactiveStream<TMessage, bool>>();
            this.streamsLock = new ReadWriteLock();
        }

        public override bool TryGet(object[] parameter, out IReactiveStream<TMessage, bool> result)
        {
            if (parameter.Length == 0)
            {
                throw new IndexOutOfRangeException();
            }
            Type messageType = parameter[1] as Type;
            if (messageType == null)
            {
                throw new ArgumentOutOfRangeException("parameter");
            }
            streamsLock.ReadLock();
            try
            {
                if (streams.TryGetValue(messageType, out result))
                    return true;
            }
            finally
            {
                streamsLock.ReadRelease();
            }
            streamsLock.WriteLock();
            try
            {
                if (!streams.TryGetValue(messageType, out result))
                {
                    result = new MessageStream<TMessage>(new TDispatcher());
                    streams.Add(messageType, result);
                }
                return true;
            }
            finally
            {
                streamsLock.WriteRelease();
            }
        }
        public override bool TryGet(ref TMessage message, out IReactiveStream<TMessage, bool> result)
        {
            Type type = message.GetType();
            do
            {
                streamsLock.ReadLock();
                try
                {
                    if (streams.TryGetValue(type, out result))
                        return true;
                }
                finally
                {
                    streamsLock.ReadRelease();
                }
                if (allowDowncast)
                {
                    bool isArray = false;
                    if (type.IsArray)
                    {
                        type = type.GetElementType();
                        isArray = true;
                    }
                    type = type.BaseType;
                    if (isArray)
                    {
                        type = type.MakeArrayType();
                    }
                }
                else return false;
            }
            while(type.BaseType != ObjectType);
            return false;
        }
    }
}
