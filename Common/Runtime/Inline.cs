// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Runtime
{
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    public abstract class inline
    {
        readonly Delegate func;

        /// <summary>
        /// 
        /// </summary>
        public MethodInfo Method
        {
            get { return func.Method; }
        }

        protected inline(Delegate func)
        {
            this.func = func;
        }

        /// <summary>
        /// Invokes the encapsulated method
        /// </summary>
        /// <param name="args">The parameter of the method that this wrapper encapsulates</param>
        /// <returns>The return value of the method that this wrapper encapsulates, if any</returns>
        public virtual object Invoke(params object[] args)
        {
            return func.DynamicInvoke(args);
        }
    }
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    /// <remarks>Use System.Runtime._void in order to define a method without any return value</remarks>
    public class inline<TRet> : inline
    {
        private inline(Delegate func)
            : base(func)
        { }

        public override object Invoke(params object[] args)
        {
            if (args.Length != 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            else return base.Invoke(args);
        }

        /// <summary>
        /// Builds an encapsulated method from user defined IL code
        /// </summary>
        /// <param name="ilLoader">An user defined callback to the IL code</param>
        public static inline Declare(Action<IL> ilLoader)
        {
            Type returnType = typeof(TRet);
            if (returnType == _void.ProxyType)
            {
                returnType = _void.Type;
            }
            DynamicMethod body = new DynamicMethod(Guid.NewGuid().ToString(), returnType, ArrayExtension.Empty<Type>(), _void.PolicyModule);
            using (IL il = new IL(body, ArrayExtension.Empty<Type>()))
            {
                ilLoader(il);
            }
            if (returnType != _void.Type)
            {
                return new inline<TRet>(body.CreateDelegate(typeof(Func<TRet>)));
            }
            else return new inline<TRet>(body.CreateDelegate(typeof(Action)));
        }
    }
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    /// <remarks>Use System.Runtime._void in order to define a method without any return value</remarks>
    public class inline<T, TRet> : inline
    {
        private inline(Delegate func)
            : base(func)
        { }

        public override object Invoke(params object[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            else return base.Invoke(args);
        }

        /// <summary>
        /// Builds an encapsulated method from user defined IL code
        /// </summary>
        /// <param name="ilLoader">An user defined callback to the IL code</param>
        public static inline Declare(Action<IL> ilLoader)
        {
            Type returnType = typeof(TRet);
            if (returnType == _void.ProxyType)
            {
                returnType = _void.Type;
            }
            Type[] parameterTypes = new Type[] 
            {
                typeof(T)
            };
            DynamicMethod body = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes, _void.PolicyModule);
            using (IL il = new IL(body, parameterTypes))
            {
                ilLoader(il);
            }
            if (returnType != _void.Type)
            {
                return new inline<T, TRet>(body.CreateDelegate(typeof(Func<T, TRet>)));
            }
            else return new inline<T, TRet>(body.CreateDelegate(typeof(Action<T>)));
        }
    }
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    /// <remarks>Use System.Runtime._void in order to define a method without any return value</remarks>
    public class inline<T0, T1, TRet> : inline
    {
        private inline(Delegate func)
            : base(func)
        { }

        public override object Invoke(params object[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentOutOfRangeException();
            }
            else return base.Invoke(args);
        }

        /// <summary>
        /// Builds an encapsulated method from user defined IL code
        /// </summary>
        /// <param name="ilLoader">An user defined callback to the IL code</param>
        public static inline Declare(Action<IL> ilLoader)
        {
            Type returnType = typeof(TRet);
            if (returnType == _void.ProxyType)
            {
                returnType = _void.Type;
            }
            Type[] parameterTypes = new Type[] 
            {
                typeof(T0),
                typeof(T1)
            };
            DynamicMethod body = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes, _void.PolicyModule);
            using (IL il = new IL(body, parameterTypes))
            {
                ilLoader(il);
            }
            if (returnType != _void.Type)
            {
                return new inline<T0, T1, TRet>(body.CreateDelegate(typeof(Func<T0, T1, TRet>)));
            }
            else return new inline<T0, T1, TRet>(body.CreateDelegate(typeof(Action<T0, T1>)));
        }
    }
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    /// <remarks>Use System.Runtime._void in order to define a method without any return value</remarks>
    public class inline<T0, T1, T2, TRet> : inline
    {
        private inline(Delegate func)
            : base(func)
        { }

        public override object Invoke(params object[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentOutOfRangeException();
            }
            else return base.Invoke(args);
        }

        /// <summary>
        /// Builds an encapsulated method from user defined IL code
        /// </summary>
        /// <param name="ilLoader">An user defined callback to the IL code</param>
        public static inline Declare(Action<IL> ilLoader)
        {
            Type returnType = typeof(TRet);
            if (returnType == _void.ProxyType)
            {
                returnType = _void.Type;
            }
            Type[] parameterTypes = new Type[] 
            {
                typeof(T0),
                typeof(T1),
                typeof(T2)
            };
            DynamicMethod body = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes, _void.PolicyModule);
            using (IL il = new IL(body, parameterTypes))
            {
                ilLoader(il);
            }
            if (returnType != _void.Type)
            {
                return new inline<T0, T1, T2, TRet>(body.CreateDelegate(typeof(Func<T0, T1, T2, TRet>)));
            }
            else return new inline<T0, T1, T2, TRet>(body.CreateDelegate(typeof(Action<T0, T1, T2>)));
        }
    }
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    /// <remarks>Use System.Runtime._void in order to define a method without any return value</remarks>
    public class inline<T0, T1, T2, T3, TRet> : inline
    {
        private inline(Delegate func)
            : base(func)
        { }

        public override object Invoke(params object[] args)
        {
            if (args.Length != 4)
            {
                throw new ArgumentOutOfRangeException();
            }
            else return base.Invoke(args);
        }

        /// <summary>
        /// Builds an encapsulated method from user defined IL code
        /// </summary>
        /// <param name="ilLoader">An user defined callback to the IL code</param>
        public static inline Declare(Action<IL> ilLoader)
        {
            Type returnType = typeof(TRet);
            if (returnType == _void.ProxyType)
            {
                returnType = _void.Type;
            }
            Type[] parameterTypes = new Type[] 
            {
                typeof(T0),
                typeof(T1),
                typeof(T2),
                typeof(T3)
            };
            DynamicMethod body = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes, _void.PolicyModule);
            using (IL il = new IL(body, parameterTypes))
            {
                ilLoader(il);
            }
            if (returnType != _void.Type)
            {
                return new inline<T0, T1, T2, T3, TRet>(body.CreateDelegate(typeof(Func<T0, T1, T2, T3, TRet>)));
            }
            else return new inline<T0, T1, T2, T3, TRet>(body.CreateDelegate(typeof(Action<T0, T1, T2, T3>)));
        }
    }
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    /// <remarks>Use System.Runtime._void in order to define a method without any return value</remarks>
    public class inline<T0, T1, T2, T3, T4, TRet> : inline
    {
        private inline(Delegate func)
            : base(func)
        { }

        public override object Invoke(params object[] args)
        {
            if (args.Length != 5)
            {
                throw new ArgumentOutOfRangeException();
            }
            else return base.Invoke(args);
        }

        /// <summary>
        /// Builds an encapsulated method from user defined IL code
        /// </summary>
        /// <param name="ilLoader">An user defined callback to the IL code</param>
        public static inline Declare(Action<IL> ilLoader)
        {
            Type returnType = typeof(TRet);
            if (returnType == _void.ProxyType)
            {
                returnType = _void.Type;
            }
            Type[] parameterTypes = new Type[] 
            {
                typeof(T0),
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4)
            };
            DynamicMethod body = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes, _void.PolicyModule);
            using (IL il = new IL(body, parameterTypes))
            {
                ilLoader(il);
            }
            if (returnType != _void.Type)
            {
                return new inline<T0, T1, T2, T3, T4, TRet>(body.CreateDelegate(typeof(Func<T0, T1, T2, T3, T4, TRet>)));
            }
            else return new inline<T0, T1, T2, T3, T4, TRet>(body.CreateDelegate(typeof(Action<T0, T1, T2, T3, T4>)));
        }
    }
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    /// <remarks>Use System.Runtime._void in order to define a method without any return value</remarks>
    public class inline<T0, T1, T2, T3, T4, T5, TRet> : inline
    {
        private inline(Delegate func)
            : base(func)
        { }

        public override object Invoke(params object[] args)
        {
            if (args.Length != 6)
            {
                throw new ArgumentOutOfRangeException();
            }
            else return base.Invoke(args);
        }

        /// <summary>
        /// Builds an encapsulated method from user defined IL code
        /// </summary>
        /// <param name="ilLoader">An user defined callback to the IL code</param>
        public static inline Declare(Action<IL> ilLoader)
        {
            Type returnType = typeof(TRet);
            if (returnType == _void.ProxyType)
            {
                returnType = _void.Type;
            }
            Type[] parameterTypes = new Type[] 
            {
                typeof(T0),
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5)
            };
            DynamicMethod body = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes, _void.PolicyModule);
            using (IL il = new IL(body, parameterTypes))
            {
                ilLoader(il);
            }
            if (returnType != _void.Type)
            {
                return new inline<T0, T1, T2, T3, T4, T5, TRet>(body.CreateDelegate(typeof(Func<T0, T1, T2, T3, T4, T5, TRet>)));
            }
            else return new inline<T0, T1, T2, T3, T4, T5, TRet>(body.CreateDelegate(typeof(Action<T0, T1, T2, T3, T4, T5>)));
        }
    }
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    /// <remarks>Use System.Runtime._void in order to define a method without any return value</remarks>
    public class inline<T0, T1, T2, T3, T4, T5, T6, TRet> : inline
    {
        private inline(Delegate func)
            : base(func)
        { }

        public override object Invoke(params object[] args)
        {
            if (args.Length != 7)
            {
                throw new ArgumentOutOfRangeException();
            }
            else return base.Invoke(args);
        }

        /// <summary>
        /// Builds an encapsulated method from user defined IL code
        /// </summary>
        /// <param name="ilLoader">An user defined callback to the IL code</param>
        public static inline Declare(Action<IL> ilLoader)
        {
            Type returnType = typeof(TRet);
            if (returnType == _void.ProxyType)
            {
                returnType = _void.Type;
            }
            Type[] parameterTypes = new Type[] 
            {
                typeof(T0),
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6)
            };
            DynamicMethod body = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes, _void.PolicyModule);
            using (IL il = new IL(body, parameterTypes))
            {
                ilLoader(il);
            }
            if (returnType != _void.Type)
            {
                return new inline<T0, T1, T2, T3, T4, T5, T6, TRet>(body.CreateDelegate(typeof(Func<T0, T1, T2, T3, T4, T5, T6, TRet>)));
            }
            else return new inline<T0, T1, T2, T3, T4, T5, T6, TRet>(body.CreateDelegate(typeof(Action<T0, T1, T2, T3, T4, T5, T6>)));
        }
    }
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    /// <remarks>Use System.Runtime._void in order to define a method without any return value</remarks>
    public class inline<T0, T1, T2, T3, T4, T5, T6, T7, TRet> : inline
    {
        private inline(Delegate func)
            : base(func)
        { }

        public override object Invoke(params object[] args)
        {
            if (args.Length != 8)
            {
                throw new ArgumentOutOfRangeException();
            }
            else return base.Invoke(args);
        }

        /// <summary>
        /// Builds an encapsulated method from user defined IL code
        /// </summary>
        /// <param name="ilLoader">An user defined callback to the IL code</param>
        public static inline Declare(Action<IL> ilLoader)
        {
            Type returnType = typeof(TRet);
            if (returnType == _void.ProxyType)
            {
                returnType = _void.Type;
            }
            Type[] parameterTypes = new Type[] 
            {
                typeof(T0),
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7)
            };
            DynamicMethod body = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes, _void.PolicyModule);
            using (IL il = new IL(body, parameterTypes))
            {
                ilLoader(il);
            }
            if (returnType != _void.Type)
            {
                return new inline<T0, T1, T2, T3, T4, T5, T6, T7, TRet>(body.CreateDelegate(typeof(Func<T0, T1, T2, T3, T4, T5, T6, T7, TRet>)));
            }
            else return new inline<T0, T1, T2, T3, T4, T5, T6, T7, TRet>(body.CreateDelegate(typeof(Action<T0, T1, T2, T3, T4, T5, T6, T7>)));
        }
    }
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    /// <remarks>Use System.Runtime._void in order to define a method without any return value</remarks>
    public class inline<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet> : inline
    {
        private inline(Delegate func)
            : base(func)
        { }

        public override object Invoke(params object[] args)
        {
            if (args.Length != 9)
            {
                throw new ArgumentOutOfRangeException();
            }
            else return base.Invoke(args);
        }

        /// <summary>
        /// Builds an encapsulated method from user defined IL code
        /// </summary>
        /// <param name="ilLoader">An user defined callback to the IL code</param>
        public static inline Declare(Action<IL> ilLoader)
        {
            Type returnType = typeof(TRet);
            if (returnType == _void.ProxyType)
            {
                returnType = _void.Type;
            }
            Type[] parameterTypes = new Type[] 
            {
                typeof(T0),
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8)
            };
            DynamicMethod body = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes, _void.PolicyModule);
            using (IL il = new IL(body, parameterTypes))
            {
                ilLoader(il);
            }
            if (returnType != _void.Type)
            {
                return new inline<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>(body.CreateDelegate(typeof(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>)));
            }
            else return new inline<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>(body.CreateDelegate(typeof(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>)));
        }
    }
    /// <summary>
    /// Encapsulates a method that has been built from user defined IL code
    /// </summary>
    /// <remarks>Use System.Runtime._void in order to define a method without any return value</remarks>
    public class inline<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet> : inline
    {
        private inline(Delegate func)
            : base(func)
        { }

        public override object Invoke(params object[] args)
        {
            if (args.Length != 10)
            {
                throw new ArgumentOutOfRangeException();
            }
            else return base.Invoke(args);
        }

        /// <summary>
        /// Builds an encapsulated method from user defined IL code
        /// </summary>
        /// <param name="ilLoader">An user defined callback to the IL code</param>
        public static inline Declare(Action<IL> ilLoader)
        {
            Type returnType = typeof(TRet);
            if (returnType == _void.ProxyType)
            {
                returnType = _void.Type;
            }
            Type[] parameterTypes = new Type[] 
            {
                typeof(T0),
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9)
            };
            DynamicMethod body = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes, _void.PolicyModule);
            using (IL il = new IL(body, parameterTypes))
            {
                ilLoader(il);
            }
            if (returnType != _void.Type)
            {
                return new inline<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(body.CreateDelegate(typeof(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>)));
            }
            else return new inline<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(body.CreateDelegate(typeof(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>)));
        }
    }
}
