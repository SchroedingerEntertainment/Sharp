// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace System.Runtime
{
    public partial class IL
    {
        /**
         Arithmetic Operator
        */

        /// <summary>
        /// Adds two values (+) and pushes the result onto the evaluation stack
        /// </summary>
        public void Add()
        {
            Emit(OpCodes.Add);
        }
        /// <summary>
        /// Adds two integers (+), performs an overflow check, and pushes the result onto the evaluation stack
        /// </summary>
        /// <param name="unsigned">Determines if value is to be handled as unsigned value</param>
        public void Add_Ovf(bool unsigned)
        {
            Emit(unsigned ? OpCodes.Add_Ovf_Un : OpCodes.Add_Ovf);
        }

        /// <summary>
        /// Subtracts one value from another (-) and pushes the result onto the evaluation stack
        /// </summary>
        public void Sub()
        {
            Emit(OpCodes.Sub);
        }
        /// <summary>
        /// Subtracts one integer value from another (-), performs an overflow check, and pushes the result onto the evaluation stack
        /// </summary>
        /// <param name="unsigned">Determines if value is to be handled as unsigned value</param>
        public void Sub_Ovf(bool unsigned)
        {
            Emit(unsigned ? OpCodes.Sub_Ovf_Un : OpCodes.Sub_Ovf);
        }

        /// <summary>
        /// Multiplies two values (*) and pushes the result on the evaluation stack
        /// </summary>
        public void Mul()
        {
            Emit(OpCodes.Mul);
        }
        /// <summary>
        /// Multiplies two integer values (*), performs an overflow check, and pushes the result onto the evaluation stack
        /// </summary>
        /// <param name="unsigned">Determines if value is to be handled as unsigned value</param>
        public void Mul_Ovf(bool unsigned)
        {
            Emit(unsigned ? OpCodes.Mul_Ovf_Un : OpCodes.Mul_Ovf);
        }

        /// <summary>
        /// Divides two values (/) and pushes the result as a floating-point (type F) or quotient (type int32) onto the evaluation stack
        /// </summary>
        /// <param name="unsigned">Determines if value is to be handled as unsigned value</param>
        public void Div(bool unsigned)
        {
            Emit(unsigned ? OpCodes.Div_Un : OpCodes.Div);
        }

        /// <summary>
        /// Divides two values and pushes the remainder onto the evaluation stack
        /// </summary>
        /// <param name="unsigned">Determines if value is to be handled as unsigned value</param>
        public void Rem(bool unsigned)
        {
            Emit(unsigned ? OpCodes.Rem_Un : OpCodes.Rem);
        }

        /// <summary>
        /// Throws ArithmeticException if value is not a finite number
        /// </summary>
        public void Ckfinite()
        {
            Emit(OpCodes.Ckfinite);
        }

        /**
         Shift Operator
        */

        /// <summary>
        /// Shifts an integer value to the left (in zeroes) by a specified number of bits, pushing the result onto the evaluation stack
        /// </summary>
        public void Shl()
        {
            Emit(OpCodes.Shl);
        }

        /// <summary>
        /// Shifts an integer value (in sign) to the right by a specified number of bits, pushing the result onto the evaluation stack
        /// </summary>
        /// <param name="unsigned">Determines if value is to be handled as unsigned value</param>
        public void Shr(bool unsigned)
        {
            Emit(unsigned ? OpCodes.Shr_Un : OpCodes.Shr);
        }

        /**
         Logical Operator
        */

        /// <summary>
        /// Negates a value and pushes the result onto the evaluation stack
        /// </summary>
        public void Neg()
        {
            Emit(OpCodes.Neg);
        }

        /// <summary>
        /// Computes the bitwise complement of the integer value on top of the stack and pushes the result onto the evaluation stack as the same type
        /// </summary>
        public void Not()
        {
            Emit(OpCodes.Not);
        }

        /// <summary>
        /// Computes the bitwise AND of two values and pushes the result onto the evaluation stack
        /// </summary>
        public void And()
        {
            Emit(OpCodes.And);
        }

        /// <summary>
        /// Compute the bitwise complement of the two integer values on top of the stack and pushes the result onto the evaluation stack
        /// </summary>
        public void Or()
        {
            Emit(OpCodes.Or);
        }

        /// <summary>
        /// Computes the bitwise XOR of the top two values on the evaluation stack, pushing the result onto the evaluation stack
        /// </summary>
        public void Xor()
        {
            Emit(OpCodes.Xor);
        }

        /// <summary>
        /// Compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack
        /// </summary>
        public void Ceq()
        {
            Emit(OpCodes.Ceq);
        }

        /// <summary>
        /// Compares two values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack
        /// </summary>
        /// <param name="unsigned">Determines if value is to be handled as unsigned value</param>
        public void Cgt(bool unsigned)
        {
            Emit(unsigned ? OpCodes.Cgt_Un : OpCodes.Cgt);
        }

        /// <summary>
        /// Compares two values. If the first value is less than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack
        /// </summary>
        /// <param name="unsigned">Determines if value is to be handled as unsigned value</param>
        public void Clt(bool unsigned)
        {
            Emit(unsigned ? OpCodes.Clt_Un : OpCodes.Clt);
        }

        /// <summary>
        /// ests whether an object reference (type O) is an instance of a particular class
        /// </summary>
        /// <param name="type">The type of object to be loaded</param>
        public void Isinst(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else Emit(OpCodes.Isinst, type);
        }
    }
}
