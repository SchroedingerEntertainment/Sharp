// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public partial class IL : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly static bool IsMonoRuntime = (Type.GetType("Mono.Runtime") != null);

        readonly ILGenerator il;
        readonly Type ilReturnType;
        readonly Type[] ilParameterTypes;
        
        private IL(ILGenerator il, Type returnType, Type[] parameterTypes)
        {
            this.il = il;
            this.ilReturnType = returnType;
            this.ilParameterTypes = parameterTypes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameterTypes"></param>
        public IL(DynamicMethod method, Type[] parameterTypes)
            : this(method.GetILGenerator(), method.ReturnType, parameterTypes)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameterTypes"></param>
        public IL(MethodBuilder method, Type[] parameterTypes)
            : this(method.GetILGenerator(), method.ReturnType, parameterTypes)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctor"></param>
        /// <param name="parameterTypes"></param>
        public IL(ConstructorBuilder ctor, Type[] parameterTypes)
            : this(ctor.GetILGenerator(), typeof(void), parameterTypes)
        { }
        public void Dispose()
        { }

        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack
        /// </summary>
        /// <param name="method">A MethodInfo representing a method</param>
        /// <param name="constrained">Constrains the type on which a virtual method call is made</param>
        /// <param name="tailcall">
        /// Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual 
        /// call instruction is executed
        /// </param>
        /// <param name="optionalParameterTypes">An optional collection of additional parameter types of the method</param>
        public void Call(MethodInfo method, Type constrained, bool tailcall, params Type[] optionalParameterTypes)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            else
            {
                OpCode opCode = method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call;
                if (opCode == OpCodes.Callvirt)
                {
                    if (constrained != null && constrained.IsValueType)
                    {
                        il.Emit(OpCodes.Constrained, constrained);
                    }
                }
                if (tailcall)
                {
                    il.Emit(OpCodes.Tailcall);
                }
                il.EmitCall(opCode, method, optionalParameterTypes);
            }
        }
        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack
        /// </summary>
        /// <param name="method">A MethodInfo representing a method</param>
        /// <param name="constrained">Constrains the type on which a virtual method call is made</param>
        /// <param name="optionalParameterTypes">An optional collection of additional parameter types of the method</param>
        public void Call(MethodInfo method, Type constrained, params Type[] optionalParameterTypes)
        {
            Call(method, constrained, false, optionalParameterTypes);
        }
        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack
        /// </summary>
        /// <param name="method">A MethodInfo representing a method</param>
        /// <param name="tailcall">
        /// Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual 
        /// call instruction is executed
        /// </param>
        /// <param name="optionalParameterTypes">An optional collection of additional parameter types of the method</param>
        public void Call(MethodInfo method, bool tailcall, params Type[] optionalParameterTypes)
        {
            Call(method, null, tailcall, optionalParameterTypes);
        }
        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack
        /// </summary>
        /// <param name="method">A MethodInfo representing a method</param>
        /// <param name="optionalParameterTypes">An optional collection of additional parameter types of the method</param>
        public void Call(MethodInfo method, params Type[] optionalParameterTypes)
        {
            Call(method, null, false, optionalParameterTypes);
        }
        /// <summary>
        /// Calls the constructor indicated by the passed constructor descriptor
        /// </summary>
        /// <param name="ctor">A ConstructorInfo representing a constructor</param>
        public void Call(ConstructorInfo constructor)
        {
            if (constructor == null)
            {
                throw new ArgumentNullException("constructor");
            }
            else il.Emit(OpCodes.Call, constructor);
        }

        /// <summary>
        /// Calls the method indicated on the evaluation stack (as a pointer to an entry point) with arguments described by a calling convention
        /// </summary>
        /// <param name="callingConvention">The managed calling convention to be used</param>
        /// <param name="returnType">The return type of the function</param>
        /// <param name="parameterTypes">A collection of parameter types of the method</param>
        /// <param name="tailcall">
        /// Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual 
        /// call instruction is executed
        /// </param>
        /// <param name="optionalParameterTypes">An optional collection of additional parameter types of the method</param>
        public void Calli(CallingConventions callingConvention, Type returnType, Type[] parameterTypes, bool tailcall, params Type[] optionalParameterTypes)
        {
            if (tailcall)
            {
                il.Emit(OpCodes.Tailcall);
            }
            il.EmitCalli(OpCodes.Calli, callingConvention, returnType, parameterTypes, optionalParameterTypes);
        }
        /// <summary>
        /// Calls the method indicated on the evaluation stack (as a pointer to an entry point) with arguments described by a calling convention
        /// </summary>
        /// <param name="callingConvention">The managed calling convention to be used</param>
        /// <param name="returnType">The return type of the function</param>
        /// <param name="parameterTypes">A collection of parameter types of the method</param>
        /// <param name="optionalParameterTypes">An optional collection of additional parameter types of the method</param>
        public void Calli(System.Runtime.InteropServices.CallingConvention callingConvention, Type returnType, params Type[] parameterTypes)
        {
            il.EmitCalli(OpCodes.Calli, callingConvention, returnType, parameterTypes);
        }

        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        public void Emit(OpCode opCode)
        {
            il.Emit(opCode);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="local">A local variable</param>
        public void Emit(OpCode opCode, Local local)
        {
            il.Emit(opCode, local);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="type">A Type</param>
        public void Emit(OpCode opCode, Type type)
        {
            il.Emit(opCode, type);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="value">The numerical argument pushed onto the stream immediately after the instruction</param>
        public void Emit(OpCode opCode, byte value)
        {
            il.Emit(opCode, value);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="value">The numerical argument pushed onto the stream immediately after the instruction</param>
        public void Emit(OpCode opCode, sbyte value)
        {
            il.Emit(opCode, value);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="value">The numerical argument pushed onto the stream immediately after the instruction</param>
        public void Emit(OpCode opCode, Int32 value)
        {
            il.Emit(opCode, value);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="value">The numerical argument pushed onto the stream immediately after the instruction</param>
        public void Emit(OpCode opCode, Int64 value)
        {
            il.Emit(opCode, value);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="value">The numerical argument pushed onto the stream immediately after the instruction</param>
        public void Emit(OpCode opCode, float value)
        {
            il.Emit(opCode, value);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="value">The numerical argument pushed onto the stream immediately after the instruction</param>
        public void Emit(OpCode opCode, double value)
        {
            il.Emit(opCode, value);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="value">The string argument pushed onto the stream immediately after the instruction</param>
        public void Emit(OpCode opCode, string value)
        {
            il.Emit(opCode, value);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="label">The label to which to branch from this location</param>
        public void Emit(OpCode opCode, Label label)
        {
            il.Emit(opCode, label);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="labels">The labels to which to branch from this location</param>
        public void Emit(OpCode opCode, Label[] labels)
        {
            il.Emit(opCode, labels.Select(label => (System.Reflection.Emit.Label)label).ToArray());
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="field"><A FieldInfo representing a field/param>
        public void Emit(OpCode opCode, FieldInfo field)
        {
            il.Emit(opCode, field);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="method">A MethodInfo representing a method</param>
        public void Emit(OpCode opCode, MethodInfo method)
        {
            il.Emit(opCode, method);
        }
        /// <summary>
        /// Puts an instruction onto the Microsoft Intermediate Language (MSIL) stream for the just-in-time (JIT) compiler
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be put onto the stream</param>
        /// <param name="ctor">A ConstructorInfo representing a constructor</param>
        public void Emit(OpCode opCode, ConstructorInfo ctor)
        {
            il.Emit(opCode, ctor);
        }

        /// <summary>
        /// Emits the Common intermediate language (CIL) to call System.Console.WriteLine with the given local variable.
        /// </summary>
        /// <param name="local">A Local struct object that represents the local variable</param>
        public void WriteLine(Local local)
        {
            il.EmitWriteLine(local);
        }
        /// <summary>
        /// Emits the Common intermediate language (CIL) to call System.Console.WriteLine with a string
        /// </summary>
        /// <param name="str">A string constant to process</param>
        public void WriteLine(string str)
        {
            il.EmitWriteLine(str);
        }
    }
}