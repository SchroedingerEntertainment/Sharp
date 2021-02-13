// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Runtime
{
    public partial class IL
    {
        /// <summary>
        /// Create a position marker for the Common intermediate language (CIL) stream
        /// </summary>
        /// <param name="name">A name to be associated with the local variable</param>
        /// <returns>A Local struct object that represents a position marker</returns>
        public Label DefineLabel(string name)
        {
            return new Label(il.DefineLabel(), name);
        }

        /// <summary>
        /// Fills space if opcodes are patched. No meaningful operation is performed although a 
        /// processing cycle can be consumed
        /// </summary>
        public void Nop()
        {
            Emit(OpCodes.Nop);
        }

        /// <summary>
        /// Copies the current topmost value on the evaluation stack, and then pushes the copy onto the evaluation stack
        /// </summary>
        public void Dup()
        {
            Emit(OpCodes.Dup);
        }
        
        /// <summary>
        /// Removes the value currently on top of the evaluation stack
        /// </summary>
        public void Pop()
        {
            Emit(OpCodes.Pop);
        }

        /// <summary>
        /// Marks the Common intermediate language (CIL) stream's current position with the given label
        /// </summary>
        /// <param name="name">A name to be associated with the local variable</param>
        public void MarkLabel(Label label)
        {
            il.MarkLabel(label);
        }

        /// <summary>
        /// Unconditionally transfers control to a target instruction
        /// </summary>
        /// <param name="label">A position marker to jump to</param>
        public void Br(Label label)
        {
            if (!label.IsSet)
            {
                throw new ArgumentNullException("label");
            }
            else Emit(OpCodes.Br, label);
        }

        /// <summary>
        /// Transfers control to a target instruction if value is false, a null reference, or zero
        /// </summary>
        /// <param name="label">A position marker to jump to</param>
        public void Brfalse(Label label)
        {
            if (!label.IsSet)
            {
                throw new ArgumentNullException("label");
            }
            else Emit(OpCodes.Brfalse, label);
        }

        /// <summary>
        /// Transfers control to a target instruction if value is true, not null, or non-zero
        /// </summary>
        /// <param name="label">A position marker to jump to</param>
        public void Brtrue(Label label)
        {
            if (!label.IsSet)
            {
                throw new ArgumentNullException("label");
            }
            else Emit(OpCodes.Brtrue, label);
        }

        /// <summary>
        /// Transfers control to a target instruction if the first value is less than or equal to the second value
        /// </summary>
        /// <param name="label">A position marker to jump to</param>
        public void Ble(Label label, bool unsigned)
        {
            if (!label.IsSet)
            {
                throw new ArgumentNullException("label");
            }
            else Emit(unsigned ? OpCodes.Ble_Un : OpCodes.Ble, label);
        }

        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than or equal to the second value
        /// </summary>
        /// <param name="label">A position marker to jump to</param>
        public void Bge(Label label, bool unsigned)
        {
            if (!label.IsSet)
            {
                throw new ArgumentNullException("label");
            }
            else Emit(unsigned ? OpCodes.Bge_Un : OpCodes.Bge, label);
        }

        /// <summary>
        /// Transfers control to a target instruction if the first value is less than the second value
        /// </summary>
        /// <param name="label">A position marker to jump to</param>
        public void Blt(Label label, bool unsigned)
        {
            if (!label.IsSet)
            {
                throw new ArgumentNullException("label");
            }
            else Emit(unsigned ? OpCodes.Blt_Un : OpCodes.Blt, label);
        }

        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than the second value
        /// </summary>
        /// <param name="label">A position marker to jump to</param>
        public void Bgt(Label label, bool unsigned)
        {
            if (!label.IsSet)
            {
                throw new ArgumentNullException("label");
            }
            else Emit(unsigned ? OpCodes.Bgt_Un : OpCodes.Bgt, label);
        }

        /// <summary>
        /// Transfers control to a target instruction when two unsigned integer values or unordered float values are not equal
        /// </summary>
        /// <param name="label">A position marker to jump to</param>
        public void Bne_Un(Label label)
        {
            if (!label.IsSet)
            {
                throw new ArgumentNullException("label");
            }
            else Emit(OpCodes.Bne_Un, label);
        }

        /// <summary>
        /// Transfers control to a target instruction if two values are equal
        /// </summary>
        /// <param name="label">A position marker to jump to</param>
        public void Beq(Label label)
        {
            if (!label.IsSet)
            {
                throw new ArgumentNullException("label");
            }
            else Emit(OpCodes.Beq, label);
        }

        /// <summary>
        /// Signals the Common Language Infrastructore (CLI) to inform the debugger that a break point has been tripped
        /// </summary>
        public void Break()
        {
            Emit(OpCodes.Break);
        }

        /// <summary>
        /// Throws the exception object currently on the evaluation stack
        /// </summary>
        public void Throw()
        {
            Emit(OpCodes.Throw);
        }

        /// <summary>
        /// Rethrows the current exception.
        /// </summary>
        public void Rethrow()
        {
            Emit(OpCodes.Rethrow);
        }

        /// <summary>
        /// Exits current method and jumps to specified method
        /// </summary>
        /// <param name="method">A MethodInfo representing a method</param>
        public void Jmp(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            else if (method.ReturnType != ilReturnType)
            {
                throw new ArgumentException();
            }
            else
            {
                IEnumerator<Type> parameter = method.GetParameters().Select(info => info.ParameterType).GetEnumerator();
                for (var i = 0; i < ilParameterTypes.Length && parameter.MoveNext(); ++i)
                {
                    if (parameter.Current != ilParameterTypes[i])
                        throw new ArgumentException(string.Concat("Argument mismatch at ", i.ToString()));
                }
                if (parameter.MoveNext())
                {
                    throw new ArgumentOutOfRangeException();
                }
                Emit(OpCodes.Jmp, method);
            }
        }

        /// <summary>
        /// Exits a protected region of code, unconditionally transferring control to a specific target instruction
        /// </summary>
        /// <param name="label">A position marker to jump to</param>
        public void Leave(Label label)
        {
            if (!label.IsSet)
            {
                throw new ArgumentNullException("label");
            }
            else Emit(OpCodes.Leave, label);
        }

        /// <summary>
        /// Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto 
        /// the caller's evaluation stack.
        /// </summary>
        public void Ret()
        {
            Emit(OpCodes.Ret);
        }
    }
}
