
// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Reactive;

namespace SE.Actor
{
    public interface IActor<TMessage> : IReceiver<TMessage, bool>
    {

    }
}