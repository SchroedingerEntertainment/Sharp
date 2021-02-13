// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Runtime
{
    public partial class IL
    {
        /// <summary>
        /// Returns an unmanaged pointer to the argument list of the current method
        /// </summary>
        public void Arglist()
        {
            Emit(OpCodes.Arglist);
        }

        /// <summary>
        /// Loads the argument (referenced by a specified short form index) onto the evaluation stack
        /// </summary>
        /// <param name="index">The argument indexed at index, where arguments are indexed from 0 onwards</param>
        public void Ldarg(int index)
        {
            if (index < 0 || index >= ilParameterTypes.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            switch (index)
            {
                case 0: Emit(OpCodes.Ldarg_0); break;
                case 1: Emit(OpCodes.Ldarg_1); break;
                case 2: Emit(OpCodes.Ldarg_2); break;
                case 3: Emit(OpCodes.Ldarg_3); break;
                default: Emit(OpCodes.Ldarg_S, (byte)index); break;
            }
        }

        /// <summary>
        /// Load an argument address, in short form, onto the evaluation stack
        /// </summary>
        /// <param name="index">The address (of type*) of the argument indexed by index, where arguments are indexed from 0 onwards.</param>
        public void Ldarga(int index)
        {
            if (index < 0 || index >= ilParameterTypes.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            else Emit(OpCodes.Ldarga_S, (byte)index);
        }

        /// <summary>
        /// Pushes a supplied value of type int onto the evaluation stack as an int32
        /// </summary>
        /// <param name="value">The value pushed onto the stack</param>
        public void Ldc_I4(Int32 value)
        {
            switch (value)
            {
                case -1: Emit(OpCodes.Ldc_I4_M1); break;
                case 0: Emit(OpCodes.Ldc_I4_0); break;
                case 1: Emit(OpCodes.Ldc_I4_1); break;
                case 2: Emit(OpCodes.Ldc_I4_2); break;
                case 3: Emit(OpCodes.Ldc_I4_3); break;
                case 4: Emit(OpCodes.Ldc_I4_4); break;
                case 5: Emit(OpCodes.Ldc_I4_5); break;
                case 6: Emit(OpCodes.Ldc_I4_6); break;
                case 7: Emit(OpCodes.Ldc_I4_7); break;
                case 8: Emit(OpCodes.Ldc_I4_8); break;
                default:
                    {
                        if (value < 128 && value >= -128)
                        {
                            Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                        }
                        else Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }
        /// <summary>
        /// Pushes a supplied value of type int onto the evaluation stack as an int64
        /// </summary>
        /// <param name="value">The value pushed onto the stack</param>
        public void Ldc_I8(Int64 value)
        {
            Emit(OpCodes.Ldc_I8, value);
        }
        /// <summary>
        /// Pushes a supplied value of type float32 onto the evaluation stack as type F (float)
        /// </summary>
        /// <param name="value">The value pushed onto the stack</param>
        public void Ldc_R4(float value)
        {
            Emit(OpCodes.Ldc_R4, value);
        }
        /// <summary>
        /// Pushes a supplied value of type float64 onto the evaluation stack as type F (float)
        /// </summary>
        /// <param name="value">The value pushed onto the stack</param>
        public void Ldc_R8(double value)
        {
            Emit(OpCodes.Ldc_R8, value);
        }
        /// <summary>
        /// Pushes a supplied value of type decimal onto the evaluation stack as an int32 array
        /// </summary>
        /// <param name="value">The value pushed onto the stack</param>
        public void Ldc_Dec(decimal value)
        {
            Ldc_I4(4);
            Newarr(typeof(int));

            int[] bits = decimal.GetBits(value);
            for (int i = 0; i < 4; i++)
            {
                Dup();
                Ldc_I4(i);
                Ldc_I4(bits[i]);
                Stelem(typeof(int));
            }

            Newobj(typeof(decimal).GetConstructor(new[] { typeof(int[]) }));
        }
        /// <summary>
        /// Pushes a supplied value of type native int onto the evaluation stack as type native
        /// </summary>
        /// <param name="value">The value pushed onto the stack</param>
        public void Ldc_IntPtr(IntPtr value)
        {
            if (IntPtr.Size == 4)
            {
                Ldc_I4(value.ToInt32());
            }
            else Ldc_I8(value.ToInt64());
            Conv<IntPtr>();
        }

        /// <summary>
        /// Pushes a new object reference to a string literal stored in the metadata
        /// </summary>
        /// <param name="value">The reference pushed onto the stack</param>
        public void Ldstr(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            Emit(OpCodes.Ldstr, value);
        }

        /// <summary>
        /// Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack
        /// </summary>
        public void Ldlen()
        {
            Emit(OpCodes.Ldlen);
        }

        /// <summary>
        /// Pushes a null reference (type O) onto the evaluation stack
        /// </summary>
        public void Ldnull()
        {
            Emit(OpCodes.Ldnull);
        }

        /// <summary>
        /// Creates a new object or a new instance of a value type, pushing an object reference (type O) onto the evaluation stack
        /// </summary>
        public void Newobj(ConstructorInfo ctor)
        {
            if (ctor == null)
            {
                throw new ArgumentNullException("constructor");
            }
            else Emit(OpCodes.Newobj, ctor);
        }

        /// <summary>
        /// Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack
        /// </summary>
        public void Newarr(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else Emit(OpCodes.Newarr, type);
        }

        /// <summary>
        /// Initializes a specified block of memory at a specific address to a given size and initial value
        /// </summary>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Initblk(bool isVolatile, int alignment)
        {
            ModifyOperation(isVolatile, alignment);
            Emit(OpCodes.Initblk);
        }
        /// <summary>
        /// Initializes a specified block of memory at a specific address to a given size and initial value
        /// </summary>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        public void Initblk(bool isVolatile)
        {
            Initblk(isVolatile, 0);
        }
        /// <summary>
        /// Initializes a specified block of memory at a specific address to a given size and initial value
        /// </summary>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Initblk(int alignment = 0)
        {
            Initblk(false, alignment);
        }

        /// <summary>
        /// Copies a specified number bytes from a source address to a destination address
        /// </summary>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Cpblk(bool isVolatile, int alignment)
        {
            ModifyOperation(isVolatile, alignment);
            Emit(OpCodes.Cpblk);
        }
        /// <summary>
        /// Copies a specified number bytes from a source address to a destination address
        /// </summary>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        public void Cpblk(bool isVolatile)
        {
            Cpblk(isVolatile, 0);
        }
        /// <summary>
        /// Copies a specified number bytes from a source address to a destination address
        /// </summary>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Cpblk(int alignment = 0)
        {
            Cpblk(false, alignment);
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to the specified number type
        /// </summary>
        public void Conv<T>()
        {
            Type type = typeof(T);
            OpCode opCode; if (type == typeof(IntPtr))
            {
                opCode = OpCodes.Conv_I;
            }
            else if (type == typeof(UIntPtr))
            {
                opCode = OpCodes.Conv_U;
            }
            else
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Byte: opCode = OpCodes.Conv_U1; break;
                    case TypeCode.SByte: opCode = OpCodes.Conv_I1; break;
                    case TypeCode.Int16: opCode = OpCodes.Conv_I2; break;
                    case TypeCode.UInt16: opCode = OpCodes.Conv_U2; break;
                    case TypeCode.Int32: opCode = OpCodes.Conv_I4; break;
                    case TypeCode.UInt32: opCode = OpCodes.Conv_U4; break;
                    case TypeCode.Int64: opCode = OpCodes.Conv_I8; break;
                    case TypeCode.UInt64: opCode = OpCodes.Conv_U8; break;
                    case TypeCode.Single: opCode = OpCodes.Conv_R4; break;
                    case TypeCode.Double: opCode = OpCodes.Conv_R8; break;
                    default: throw new ArgumentException();
                }
            }
            Emit(opCode);
        }
        /// <summary>
        /// Converts the value on top of the evaluation stack to the specified number type, throwing OverflowException on overflow
        /// </summary>
        /// <param name="unsigned">Determines if value is to be handled as unsigned value</param>
        public void Conv_Ovf<T>(bool unsigned)
        {
            var type = typeof(T);
            OpCode opCode;
            if (!unsigned)
            {
                if (type == typeof(IntPtr))
                {
                    opCode = OpCodes.Conv_Ovf_I;
                }
                else if (type == typeof(UIntPtr))
                {
                    opCode = OpCodes.Conv_Ovf_U;
                }
                else
                {
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.SByte: opCode = OpCodes.Conv_Ovf_I1; break;
                        case TypeCode.Byte: opCode = OpCodes.Conv_Ovf_U1; break;
                        case TypeCode.Int16: opCode = OpCodes.Conv_Ovf_I2; break;
                        case TypeCode.UInt16: opCode = OpCodes.Conv_Ovf_U2; break;
                        case TypeCode.Int32: opCode = OpCodes.Conv_Ovf_I4; break;
                        case TypeCode.UInt32: opCode = OpCodes.Conv_Ovf_U4; break;
                        case TypeCode.Int64: opCode = OpCodes.Conv_Ovf_I8; break;
                        case TypeCode.UInt64: opCode = OpCodes.Conv_Ovf_U8; break;
                        default: throw new ArgumentException();
                    }
                }
            }
            else
            {
                if (type == typeof(IntPtr))
                {
                    opCode = OpCodes.Conv_Ovf_I_Un;
                }
                else if (type == typeof(UIntPtr))
                {
                    opCode = OpCodes.Conv_Ovf_U_Un;
                }
                else
                {
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Byte: opCode = OpCodes.Conv_Ovf_U1_Un; break;
                        case TypeCode.SByte: opCode = OpCodes.Conv_Ovf_I1_Un; break;
                        case TypeCode.Int16: opCode = OpCodes.Conv_Ovf_I2_Un; break;
                        case TypeCode.UInt16: opCode = OpCodes.Conv_Ovf_U2_Un; break;
                        case TypeCode.Int32: opCode = OpCodes.Conv_Ovf_I4_Un; break;
                        case TypeCode.UInt32: opCode = OpCodes.Conv_Ovf_U4_Un; break;
                        case TypeCode.Int64: opCode = OpCodes.Conv_Ovf_I8_Un; break;
                        case TypeCode.UInt64: opCode = OpCodes.Conv_Ovf_U8_Un; break;
                        default: throw new ArgumentException();
                    }
                }
            }
            Emit(opCode);
        }

        /// <summary>
        /// Converts the unsigned integer value on top of the evaluation stack to float32
        /// </summary>
        public void Conv_R_Un()
        {
            Emit(OpCodes.Conv_R_Un);
        }

        /// <summary>
        /// Converts a value type to an object reference (type O)
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        public void Box(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (!type.IsValueType && !type.IsGenericParameter)
            {
                throw new ArgumentException();
            }
            else Emit(OpCodes.Box, type);
        }

        /// <summary>
        /// Converts the boxed representation of a type specified in the instruction to its unboxed form
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        public void Unbox(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsValueType && !type.IsGenericParameter)
            {
                throw new ArgumentException();
            }
            else Emit(OpCodes.Unbox_Any, type);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at a specified index
        /// </summary>
        /// <param name="index">The argument indexed at index, where arguments are indexed from 0 onwards</param>
        public void Starg(int index)
        {
            if (index < 0 || index >= ilParameterTypes.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (index < 256)
            {
                Emit(OpCodes.Starg_S, (byte)index);
            }
            else Emit(OpCodes.Starg, index);
        }
    }
}
