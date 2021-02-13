// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace System.Runtime
{
    public partial class IL
    {
        /// <summary>
        /// Begins an exception block for a non-filtered exception
        /// </summary>
        public void BeginExceptionBlock()
        {
            il.BeginExceptionBlock();
        }
        
        /// <summary>
        /// Begins an exception block for a filtered exception
        /// </summary>
        public void BeginExceptFilterBlock()
        {
            il.BeginExceptFilterBlock();
        }
        
        /// <summary>
        /// Begins an exception fault block in the Common intermediate language (CIL) stream
        /// </summary>
        public void BeginFaultBlock()
        {
            il.BeginFaultBlock();
        }

        /// <summary>
        /// Begins a catch block
        /// </summary>
        /// <param name="exceptionType">The exception type to handle</param>
        public void BeginCatchBlock(Type exceptionType)
        {
            il.BeginCatchBlock(exceptionType);
        }

        /// <summary>
        /// Begins a finally block in the Common intermediate language (CIL) instruction stream
        /// </summary>
        public void BeginFinallyBlock()
        {
            il.BeginFinallyBlock();
        }

        /// <summary>
        /// Ends an exception block
        /// </summary>
        public void EndExceptionBlock()
        {
            il.EndExceptionBlock();
        }

        /// <summary>
        /// Implements a jump table
        /// </summary>
        /// <param name="label">A collection of position marker to jump to</param>
        public void Switch(params Label[] labels)
        {
            if (labels == null)
            {
                throw new ArgumentNullException("labels");
            }
            else if (labels.Length == 0)
            {
                throw new ArgumentException();
            }
            else if (labels.Any(label => !label.IsSet))
            {
                throw new ArgumentException();
            }
            else Emit(OpCodes.Switch, labels);
        }
    }
}
