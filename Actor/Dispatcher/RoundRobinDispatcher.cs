// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;
using SE.Reactive;

namespace SE.Actor
{
    /// <summary>
    /// A queue based round-robin dispatching strategy
    /// </summary>
    public class RoundRobinDispatcher<TMessage> : IDispatcher<TMessage>
    {
        Queue<IReceiver<TMessage, bool>> subscriptions;
        Spinlockʾ subscriptionLock;

        public int Count
        {
            get { return subscriptions.Count; }
        }

        /// <summary>
        /// Creates a new dispatcher instance with the given fixed capacity
        /// </summary>
        public RoundRobinDispatcher(int capacity)
        {
            this.subscriptions = new Queue<IReceiver<TMessage, bool>>(capacity);
            this.subscriptionLock = new Spinlockʾ();
        }
        /// <summary>
        /// Creates a new dispatcher instance with the given fixed capacity
        /// </summary>
        public RoundRobinDispatcher()
            : this(4)
        { }
        public void Dispose()
        {
            Clear();
        }

        public void Register(IReceiver<TMessage, bool> observer)
        {
            subscriptionLock.Lock();
            try
            {
                subscriptions.Enqueue(observer);
            }
            finally
            {
                subscriptionLock.Release();
            }
        }
        public void Remove(IReceiver<TMessage, bool> observer)
        {
            subscriptionLock.Lock();
            try
            {
                if (subscriptions.Count > 0)
                {
                    IReceiver<TMessage, bool> head = subscriptions.Peek();
                    IReceiver<TMessage, bool> item = null;
                    for (; ; )
                    {
                        if (observer == item)
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
                            break;
                        }
                        else if (item != null)
                        {
                            subscriptions.Enqueue(item);
                            if (head == item)
                                break;
                        }
                        item = subscriptions.Dequeue();
                    }
                }
            }
            finally
            {
                subscriptionLock.Release();
            }
        }

        public bool Dispatch(ref TMessage message)
        {
            IReceiver<TMessage, bool> end = null;
            IReceiver<TMessage, bool> head = null;
            for (; ; )
            {
                subscriptionLock.Lock();
                try
                {
                    if (subscriptions.Count == 0)
                    {
                        return false;
                    }
                    else if (subscriptions.Peek() == end)
                    {
                        break;
                    }
                    else
                    {
                        head = subscriptions.Dequeue();
                        subscriptions.Enqueue(head);
                    }
                }
                finally
                {
                    subscriptionLock.Release();
                }

                if (end == null)
                {
                    end = head;
                }
                if (head != null)
                {
                    try
                    {
                        if (head.OnNext(message))
                            return true;
                    }
                    catch (Exception er)
                    {
                        try
                        {
                            head.OnError(er);
                        }
                        catch { }
                    }
                }
            }
            return false;
        }

        public void Clear()
        {
            subscriptionLock.Lock();
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
                subscriptionLock.Release();
            }
        }
    }
}