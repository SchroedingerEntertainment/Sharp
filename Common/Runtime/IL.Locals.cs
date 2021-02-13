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
        /// Declares a local variable of the specified type, optionally pinning the object referred to by the variable
        /// </summary>
        /// <param name="localType">A Type object that represents the type of the local variable</param>
        /// <param name="pinned">true to pin the object in memory; otherwise, false</param>
        /// <returns>A Local struct object that represents the local variable</returns>
        public Local DeclareLocal(Type localType, bool pinned = false)
        {
            return new Local(il.DeclareLocal(localType, pinned), Guid.NewGuid().ToString());
        }
        /// <summary>
        /// Declares a local variable of the specified type, optionally pinning the object referred to by the variable
        /// </summary>
        /// <param name="localType">A Type object that represents the type of the local variable</param>
        /// <param name="name">A name to be associated with the local variable</param>
        /// <param name="pinned">true to pin the object in memory; otherwise, false</param>
        /// <returns>A Local struct object that represents the local variable</returns>
        public Local DeclareLocal(Type localType, string name, bool pinned = false)
        {
            return new Local(il.DeclareLocal(localType, pinned), name);
        }

        /// <summary>
        /// Allocates a certain number of bytes from the local dynamic memory pool and pushes the address (a transient pointer, type *)
        /// of the first allocated byte onto the evaluation stack
        /// </summary>
        public void Localloc()
        {
            Emit(OpCodes.Localloc);
        }

        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack
        /// </summary>
        /// <param name="local">A Local struct object that represents the local variable</param>
        public void Ldloc(Local local)
        {
            Emit(OpCodes.Ldloc, local);
        }
        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack
        /// </summary>
        /// <param name="index">The argument indexed at index, where arguments are indexed from 0 onwards</param>
        public void Ldloc(int index)
        {
            if (index < 0 || index > 255)
            {
                throw new IndexOutOfRangeException();
            }
            switch (index)
            {
                case 0: Emit(OpCodes.Ldloc_0); break;
                case 1: Emit(OpCodes.Ldloc_1); break;
                case 2: Emit(OpCodes.Ldloc_2); break;
                case 3: Emit(OpCodes.Ldloc_3); break;
                default: Emit(OpCodes.Ldloc_S, (byte)index); break;
            }
        }

        /// <summary>
        /// Loads the address of the local variable at a specific index onto the evaluation stack
        /// </summary>
        /// <param name="local">A Local struct object that represents the local variable</param>
        public void Ldloca(Local local)
        {
            Emit(OpCodes.Ldloca, local);
        }
        /// <summary>
        /// Loads the address of the local variable at a specific index onto the evaluation stack
        /// </summary>
        /// <param name="index">The argument indexed at index, where arguments are indexed from 0 onwards</param>
        public void Ldloca(int index)
        {
            if (index < 0 || index > 255)
            {
                throw new IndexOutOfRangeException();
            }
            else Emit(OpCodes.Ldloca_S, (byte)index);
        }

        /// <summary>
        /// Loads the element at a specified array index onto the top of the evaluation stack as the type specified in the instruction
        /// </summary>
        /// <param name="elementType">The type of object to be loaded</param>
        public void Ldelem(Type elementType)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }
            else if (elementType.IsStruct())
            {
                Ldelema(elementType);
                Ldobj(elementType);
            }
            else if (!elementType.IsValueType)
            {
                Emit(OpCodes.Ldelem_Ref);
            }
            else if (elementType == typeof(IntPtr) || elementType == typeof(UIntPtr))
            {
                Emit(OpCodes.Ldelem_I);
            }
            else
            {
                switch (Type.GetTypeCode(elementType))
                {
                    case TypeCode.Boolean:
                    case TypeCode.SByte: Emit(OpCodes.Ldelem_I1); break;
                    case TypeCode.Byte: Emit(OpCodes.Ldelem_U1); break;
                    case TypeCode.Char:
                    case TypeCode.UInt16: Emit(OpCodes.Ldelem_U2); break;
                    case TypeCode.Int16: Emit(OpCodes.Ldelem_I2); break;
                    case TypeCode.Int32: Emit(OpCodes.Ldelem_I4); break;
                    case TypeCode.UInt32: Emit(OpCodes.Ldelem_U4); break;
                    case TypeCode.Int64:
                    case TypeCode.UInt64: Emit(OpCodes.Ldelem_I8); break;
                    case TypeCode.Single: Emit(OpCodes.Ldelem_R4); break;
                    case TypeCode.Double: Emit(OpCodes.Ldelem_R8); break;
                    default: throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// Loads the address of the array element at a specified array index onto the top of the evaluation stack as type & (managed pointer)
        /// </summary>
        /// <param name="elementType">The type of object to be loaded</param>
        /// <param name="readonly">
        /// Specifies that the subsequent array address operation performs no type check at run time, and that it 
        /// returns a managed pointer whose mutability is restricted
        /// </param>
        public void Ldelema(Type elementType, bool @readonly = false)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }
            else
            {
                if (@readonly)
                {
                    il.Emit(OpCodes.Readonly);
                }
                Emit(OpCodes.Ldelema, elementType);
            }
        }

        /// <summary>
        /// Copies the value type object pointed to by an address to the top of the evaluation stack
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Ldobj(Type type, bool isVolatile, int alignment)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (!type.IsValueType)
            {
                throw new ArgumentException();
            }
            else
            {
                ModifyOperation(isVolatile, alignment);
                Emit(OpCodes.Ldobj, type);
            }
        }
        /// <summary>
        /// Copies the value type object pointed to by an address to the top of the evaluation stack
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        public void Ldobj(Type type, bool isVolatile)
        {
            Ldobj(type, isVolatile, 0);
        }
        /// <summary>
        /// Copies the value type object pointed to by an address to the top of the evaluation stack
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Ldobj(Type type, int alignment = 0)
        {
            Ldobj(type, false, alignment);
        }

        /// <summary>
        /// Converts a metadata token to its runtime representation, pushing it onto the evaluation stack
        /// </summary>
        /// <param name="type">A RuntimeHandle to a typeref/typedef</param>
        public void Ldtoken(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else Emit(OpCodes.Ldtoken, type);
        }
        /// <summary>
        /// Converts a metadata token to its runtime representation, pushing it onto the evaluation stack
        /// </summary>
        /// <param name="field">A RuntimeHandle to a fieldref/fielddef</param>
        public void Ldtoken(FieldInfo field)
        {
            if (field == null)
            {    
                throw new ArgumentNullException("field");
            }
            else Emit(OpCodes.Ldtoken, field);
        }
        /// <summary>
        /// Converts a metadata token to its runtime representation, pushing it onto the evaluation stack
        /// </summary>
        /// <param name="method">A RuntimeHandle to a methodref/methoddef</param>
        public void Ldtoken(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            else Emit(OpCodes.Ldtoken, method);
        }

        /// <summary>
        /// Loads a value of of the provided type onto the evaluation stack indirectly
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Ldind(Type type, bool isVolatile, int alignment)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (type.IsStruct())
            {
                Ldobj(type, isVolatile, alignment);
            }
            else
            {
                ModifyOperation(isVolatile, alignment);
                if (!type.IsValueType)
                {
                    Emit(OpCodes.Ldind_Ref);
                }
                else
                {
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Boolean:
                        case TypeCode.SByte: Emit(OpCodes.Ldind_I1); break;
                        case TypeCode.Byte: Emit(OpCodes.Ldind_U1); break;
                        case TypeCode.Char:
                        case TypeCode.UInt16: Emit(OpCodes.Ldind_U2); break;
                        case TypeCode.Int16: Emit(OpCodes.Ldind_I2); break;
                        case TypeCode.Int32: Emit(OpCodes.Ldind_I4); break;
                        case TypeCode.UInt32: Emit(OpCodes.Ldind_U4); break;
                        case TypeCode.Int64:
                        case TypeCode.UInt64: Emit(OpCodes.Ldind_I8); break;
                        case TypeCode.Single: Emit(OpCodes.Ldind_R4); break;
                        case TypeCode.Double: Emit(OpCodes.Ldind_R8); break;
                        default: throw new NotSupportedException();
                    }
                }
            }
        }
        /// <summary>
        /// Loads a value of of the provided type onto the evaluation stack indirectly
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        public void Ldind(Type type, bool isVolatile)
        {
            Ldind(type, isVolatile, 0);
        }
        /// <summary>
        /// Loads a value of of the provided type onto the evaluation stack indirectly
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Ldind(Type type, int alignment = 0)
        {
            Ldind(type, false, alignment);
        }

        /// <summary>
        /// Finds the address of a field in the object whose reference is currently on the evaluation stack
        /// </summary>
        /// <param name="field">The field to load</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Ldfld(FieldInfo field, bool isVolatile, int alignment)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            else if (field.IsStatic && alignment > 0)
            {
                throw new ArgumentException();
            }
            else
            {
                ModifyOperation(isVolatile, alignment);
                Emit(field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field);
            }
        }
        /// <summary>
        /// Finds the address of a field in the object whose reference is currently on the evaluation stack
        /// </summary>
        /// <param name="field">The field to load</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        public void Ldfld(FieldInfo field, bool isVolatile)
        {
            Ldfld(field, isVolatile, 0);
        }
        /// <summary>
        /// Finds the address of a field in the object whose reference is currently on the evaluation stack
        /// </summary>
        /// <param name="field">The field to load</param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Ldfld(FieldInfo field, int alignment = 0)
        {
            Ldfld(field, false, alignment);
        }

        /// <summary>
        /// Finds the address of a field in the object whose reference is currently on the evaluation stack
        /// </summary>
        /// <param name="field">The field to load</param>
        public void Ldflda(FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            else Emit(field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, field);
        }

        /// <summary>
        /// Pushes an unmanaged pointer (type native int) to the native code implementing a specific method onto the evaluation stack
        /// </summary>
        /// <param name="method">The method to be loaded</param>
        public void Ldftn(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            else Emit(OpCodes.Ldftn, method);
        }
        /// <summary>
        /// Pushes an unmanaged pointer (type native int) to the native code implementing a particular virtual method associated with a 
        /// specified object onto the evaluation stack
        /// </summary>
        /// <param name="method">The method to be loaded</param>
        public void Ldvirtftn(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            else Emit(OpCodes.Ldvirtftn, method);
        }

        /// <summary>
        /// Initializes each field of the value type at a specified address to a null reference or a 0 of the appropriate primitive type
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        public void Initobj(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (!type.IsValueType)
            {
                throw new ArgumentException();
            }
            else Emit(OpCodes.Initobj, type);
        }

        /// <summary>
        /// Copies the value type located at the address of an object (type &, or native int) to the address of the destination object 
        /// (type &, or native int)
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        public void Cpobj(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (!type.IsValueType)
            {
                throw new ArgumentException();
            }
            else Emit(OpCodes.Cpobj, type);
        }

        /// <summary>
        /// Attempts to cast an object passed by reference to the specified class
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        public void Castclass(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (type.IsValueType)
            {
                throw new ArgumentException();
            }
            else Emit(OpCodes.Castclass, type);
        }

        /// <summary>
        /// Releases the pinned object referred to by the local variable
        /// </summary>
        /// <param name="local">A Local struct object that represents the local variable</param>
        public void FreePinnedLocal(Local local)
        {
            if (!IsMonoRuntime || !(local.Type.IsPointer || local.Type.IsByRef))
            {
                Ldnull();
                Stloc(local);
            }
            else
            {
                Ldc_I4(0);
                Conv<UIntPtr>();
                Stloc(local);
            }
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at a specified index
        /// </summary>
        /// <returns>A Local struct object that represents the local variable</returns>
        public void Stloc(Local local)
        {
            Emit(OpCodes.Stloc, local);
        }
        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at a specified index
        /// </summary>
        /// <param name="index">The argument indexed at index, where arguments are indexed from 0 onwards</param>
        public void Stloc(int index)
        {
            if (index < 0 || index > 255)
            {
                throw new IndexOutOfRangeException();
            }
            else switch (index)
            {
                case 0: Emit(OpCodes.Stloc_0); break;
                case 1: Emit(OpCodes.Stloc_1); break;
                case 2: Emit(OpCodes.Stloc_2); break;
                case 3: Emit(OpCodes.Stloc_3); break;
                default: Emit(OpCodes.Stloc_S, (byte)index); break;
            }
        }

        /// <summary>
        /// Replaces the array element at a given index with the value on the evaluation stack, whose type is specified in the instruction
        /// </summary>
        /// <param name="elementType">The type of object to be loaded</param>
        public void Stelem(Type elementType)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }
            else if (elementType.IsStruct())
            {
                throw new InvalidOperationException();
            }
            if (!elementType.IsValueType)
            {
                Emit(OpCodes.Stelem_Ref);
            }
            else if (elementType == typeof(IntPtr) || elementType == typeof(UIntPtr))
            {
                Emit(OpCodes.Stelem_I);
            }
            else
            {
                switch (Type.GetTypeCode(elementType))
                {
                    case TypeCode.Boolean:
                    case TypeCode.SByte:
                    case TypeCode.Byte: Emit(OpCodes.Stelem_I1); break;
                    case TypeCode.Char:
                    case TypeCode.UInt16:
                    case TypeCode.Int16: Emit(OpCodes.Stelem_I2); break;
                    case TypeCode.Int32:
                    case TypeCode.UInt32: Emit(OpCodes.Stelem_I4); break;
                    case TypeCode.Int64:
                    case TypeCode.UInt64: Emit(OpCodes.Stelem_I8); break;
                    case TypeCode.Single: Emit(OpCodes.Stelem_R4); break;
                    case TypeCode.Double: Emit(OpCodes.Stelem_R8); break;
                    default: throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// Copies a value of a specified type from the evaluation stack into a supplied memory address
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Stobj(Type type, bool isVolatile, int alignment)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (!type.IsValueType)
            {
                throw new ArgumentException();
            }
            else
            {
                ModifyOperation(isVolatile, alignment);
                Emit(OpCodes.Stobj, type);
            }
        }
        /// <summary>
        /// Copies a value of a specified type from the evaluation stack into a supplied memory address
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        public void Stobj(Type type, bool isVolatile)
        {
            Stobj(type, isVolatile, 0);
        }
        /// <summary>
        /// Copies a value of a specified type from the evaluation stack into a supplied memory address
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Stobj(Type type, int alignment = 0)
        {
            Stobj(type, false, alignment);
        }

        /// <summary>
        /// Stores the value at a supplied address
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Stind(Type type, bool isVolatile, int alignment)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (type.IsStruct())
            {
                Stobj(type, isVolatile, alignment);
            }
            else
            {
                ModifyOperation(isVolatile, alignment);

                if (!type.IsValueType)
                {
                    Emit(OpCodes.Stind_Ref);
                }
                else
                {
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Boolean:
                        case TypeCode.SByte:
                        case TypeCode.Byte: Emit(OpCodes.Stind_I1); break;
                        case TypeCode.Int16:
                        case TypeCode.Char:
                        case TypeCode.UInt16: Emit(OpCodes.Stind_I2); break;
                        case TypeCode.Int32:
                        case TypeCode.UInt32: Emit(OpCodes.Stind_I4); break;
                        case TypeCode.Int64:
                        case TypeCode.UInt64: Emit(OpCodes.Stind_I8); break;
                        case TypeCode.Single: Emit(OpCodes.Stind_R4); break;
                        case TypeCode.Double: Emit(OpCodes.Stind_R8); break;
                        default: throw new NotSupportedException();
                    }
                }
            }
        }
        /// <summary>
        /// Stores the value at a supplied address
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        public void Stind(Type type, bool isVolatile)
        {
            Stind(type, isVolatile, 0);
        }
        /// <summary>
        /// Stores the value at a supplied address
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Stind(Type type, int alignment = 0)
        {
            Stind(type, false, alignment);
        }

        /// <summary>
        /// Replaces the value stored in the field of an object reference or pointer with a new value
        /// </summary>
        /// <param name="field">The field to load</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Stfld(FieldInfo field, bool isVolatile, int alignment)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            else if (field.IsStatic && alignment > 0)
            {
                throw new ArgumentException();
            }
            else
            {
                ModifyOperation(isVolatile, alignment);
                Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field);
            }
        }
        /// <summary>
        /// Replaces the value stored in the field of an object reference or pointer with a new value
        /// </summary>
        /// <param name="field">The field to load</param>
        /// <param name="isVolatile">
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that 
        /// location cannot be cached or that multiple stores to that location cannot be suppressed
        /// </param>
        public void Stfld(FieldInfo field, bool isVolatile)
        {
            Stfld(field, isVolatile, 0);
        }
        /// <summary>
        /// Replaces the value stored in the field of an object reference or pointer with a new value
        /// </summary>
        /// <param name="field">The field to load</param>
        /// <param name="alignment">
        /// Indicates wether an address currently atop the evaluation stack is properly aligned (0) or suspects the
        /// provided alignment to the evaluation stack
        /// </param>
        public void Stfld(FieldInfo field, int alignment = 0)
        {
            Stfld(field, false, alignment);
        }

        void ModifyOperation(bool isVolatile, int alignment)
        {
            if (isVolatile)
            {
                il.Emit(OpCodes.Volatile);
            }
            switch(alignment)
            {
                case 0: break;
                case 1:
                case 2:
                case 4:
                    {
                        il.Emit(OpCodes.Unaligned, (byte)alignment);
                    }
                    break;
                default: throw new ArgumentException();
            }
        }
    }
}
