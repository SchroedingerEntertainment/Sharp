// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Dynamic;

namespace SE.Flex
{
    /// <summary>
    /// Implementing this interface turns an object into a flex entity
    /// </summary>
    public interface IFlexObject : IDynamicMetaObjectProvider
    {
        /// <summary>
        /// A template this object is bound to
        /// </summary>
        TemplateId Template { get; }
    }
}
