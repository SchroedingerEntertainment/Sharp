// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;
using SE.Reactive;

namespace SE.Actor
{
    /// <summary>
    /// A list based endpoint dispatching strategy
    /// </summary>
    public class BatchDispatcher<TMessage> : IDispatcher<TMessage>
    {
        HashSet<IReceiver<TMessage, bool>> subscriptions;
        ReadWriteLock subscriptionLock;

        public int Count
        {
            get { return subscriptions.Count; }
        }

        /// <summary>
        /// Creates a new dispatcher instance
        /// </summary>
        public BatchDispatcher()
        {
            this.subscriptions = new HashSet<IReceiver<TMessage, bool>>();
            this.subscriptionLock = new ReadWriteLock();
        }
        public void Dispose()
        {
            Clear();
        }

        public void Register(IReceiver<TMessage, bool> observer)
        {
            subscriptionLock.WriteLock();
            try
            {
                subscriptions.Add(observer);
            }
            finally
            {
                subscriptionLock.WriteRelease();
            }
        }
        public void Remove(IReceiver<TMessage, bool> observer)
        {
            subscriptionLock.WriteLock();
            try
            {
                if (subscriptions.Remove(observer))
                    observer.OnCompleted();
            }
            catch (Exception er)
            {
                try
                {
                    observer.OnError(er);
                }
                catch { }
            }
            finally
            {
                subscriptionLock.WriteRelease();
            }
        }
        
        public bool Dispatch(ref TMessage message)
        {
            bool dispatched = false;
            subscriptionLock.ReadLock();
            try
            {
                foreach (IReceiver<TMessage, bool> observer in subscriptions)
                {
                    try
                    {
                        if (observer.OnNext(message) && !dispatched)
                            dispatched = true;
                    }
                    catch (Exception er)
                    {
                        try
                        {
                            observer.OnError(er);
                        }
                        catch { }
                    }
                }
            }
            finally
            {
                subscriptionLock.ReadRelease();
            }
            return dispatched;
        }

        public void Clear()
        {
            subscriptionLock.WriteLock();
            try
            {
                foreach (IReceiver<TMessage, bool> observer in subscriptions)
                {
                    try
                    {
                        observer.OnCompleted();
                    }
                    catch (Exception er)
                    {
                        try
                        {
                            observer.OnError(er);
                        }
                        catch { }
                    }
                }
                subscriptions.Clear();
            }
            finally
            {
                subscriptionLock.WriteRelease();
            }
        }
    }
}