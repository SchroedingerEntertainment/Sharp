// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Dynamic;
using System.Linq;

namespace SE.Flex
{
    /// <summary>
    /// Enables DLR dynamic object extension on properties, methods and interfaces
    /// </summary>
    public class FlexBinder : DynamicMetaObject
    {
        delegate bool TryGetPropertyByNameDelegate(TemplateId objectId, string name, out object result);
        delegate bool TryGetPropertyByIdDelegate(TemplateId propertyId, out object result);

        delegate bool TryGetMethodByNameDelegate(TemplateId objectId, string name, out Delegate result);
        delegate bool TryGetMethodByIdDelegate(TemplateId methodId, out Delegate result);

        private readonly static MethodInfo createIdFromString;

        private readonly static MethodInfo setProperty;
        private readonly static MethodInfo tryGetPropertyByName;
        private readonly static MethodInfo tryGetPropertyById;
        private readonly static MethodInfo removeProperty;

        private readonly static MethodInfo addMethod;
        private readonly static MethodInfo tryGetMethodByName;
        private readonly static MethodInfo tryGetMethodById;
        private readonly static MethodInfo removeMethod;

        private readonly static MethodInfo invoke;
        private readonly static MethodInfo createInterfaceProxy;

        private readonly static PropertyInfo template;
        private readonly static PropertyInfo componentId;
        private readonly static PropertyInfo method;

        bool enableExtensionMethods;
        /// <summary>
        /// Get if the binder adds implicit extension methods to the udnerlaying flex object
        /// </summary>
        public bool EnableExtensionMethods
        {
            get { return enableExtensionMethods; }
        }

        static FlexBinder()
        {
            createIdFromString = ((Func<string, UInt32, UInt32>)Fnv.Fnv32).Method;

            setProperty = ((Func<TemplateId, object, bool>)PropertyManager.SetProperty).Method;
            tryGetPropertyByName = ((TryGetPropertyByNameDelegate)PropertyManager.TryGetProperty).Method;
            tryGetPropertyById = ((TryGetPropertyByIdDelegate)PropertyManager.TryGetProperty).Method;
            removeProperty = ((Func<TemplateId, bool>)PropertyManager.RemoveProperty).Method;

            addMethod = ((Func<TemplateId, string, Delegate, bool>)MethodManager.AddMethod).Method;
            tryGetMethodByName = ((TryGetMethodByNameDelegate)MethodManager.TryGetMethod).Method;
            tryGetMethodById = ((TryGetMethodByIdDelegate)MethodManager.TryGetMethod).Method;
            removeMethod = ((Func<TemplateId, string, bool>)MethodManager.RemoveMethod).Method;

            invoke = typeof(MethodInfo).GetMethod("Invoke", new Type[]{ typeof(object), typeof(object[]) });
            createInterfaceProxy = ((Func<Type, object, object>)InterfaceProxyManager.GetBindingProxy).Method;

            template = typeof(IFlexObject).GetProperty("Template", BindingFlags.Instance | BindingFlags.Public);
            componentId = typeof(TemplateId).GetProperty("ComponentId", BindingFlags.Instance | BindingFlags.Public);
            method = typeof(Delegate).GetProperty("Method", BindingFlags.Public | BindingFlags.Instance);
        }
        /// <summary>
        /// Creates a new instance of this binder object
        /// </summary>
        public FlexBinder(IFlexObject value, bool enableExtensionMethods, Expression expression)
            : base(expression, BindingRestrictions.Empty, value)
        {
            this.enableExtensionMethods = enableExtensionMethods;
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            DynamicMetaObject getDefault = binder.FallbackGetMember(this);
            ParameterExpression id = Expression.Variable(typeof(TemplateId), "template");
            ParameterExpression tmp = Expression.Variable(typeof(object), "property");
            Expression expression = Expression.Block
            (
                new ParameterExpression[] { id, tmp },
                Expression.Assign
                (
                    id,
                    CreatePropertyIdExpression(binder)
                ),
                Expression.Condition
                (
                    Expression.Call
                    (
                        tryGetPropertyById,
                        id,
                        tmp
                    ),
                    tmp,
                    Expression.Default(binder.ReturnType),
                    binder.ReturnType
                )
            );
            BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
            DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
            return binder.FallbackGetMember(this, dynamicSuggestion);
        }
        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            DynamicMetaObject getDefault = binder.FallbackSetMember(this, value);
            ParameterExpression id = Expression.Variable(typeof(TemplateId), "template");
            Expression expression = Expression.Block
            (
                new ParameterExpression[] { id },
                Expression.Assign
                (
                    id,
                    CreatePropertyIdExpression(binder)
                ),
                Expression.Convert
                (
                    Expression.Call
                    (
                        setProperty,
                        id,
                        Expression.Convert
                        (
                            value.Expression,
                            typeof(object)
                        )
                    ),
                    typeof(object)
                )
            );
            BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
            DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
            return binder.FallbackSetMember(this, value, dynamicSuggestion);
        }
        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            IEnumerable<Type> genericArgs = null; 
            foreach (Type it in binder.GetType().GetInterfaces())
            {
                PropertyInfo pi = it.GetProperty("TypeArguments", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (pi != null)
                {
                    genericArgs = (pi.GetValue(binder, null) as IEnumerable<Type>);
                    break;
                }
            }

            Expression e = this.Expression;
            DynamicMetaObject getDefault = binder.FallbackInvokeMember(this, args);
            ParameterExpression tmp = Expression.Variable(typeof(TemplateId), "template");
            Expression expression = Expression.Block
            (
                new ParameterExpression[]{ tmp },
                Expression.Assign
                (
                    tmp,
                    CreatePropertyIdExpression(binder)
                ),
                Expression.Property
                (
                    tmp,
                    componentId
                ),
                CreateInvokeMemberFunctionExpression(binder, genericArgs, args, getDefault.Expression, tmp)
            );
            BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
            DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
            return binder.FallbackInvokeMember(this, args, dynamicSuggestion);
        }
        public override DynamicMetaObject BindConvert(ConvertBinder binder)
        {
            DynamicMetaObject getDefault = binder.FallbackConvert(this);
            ParameterExpression tmp = Expression.Variable(typeof(object), "result");
            Expression expression = Expression.Block
            (
                binder.ReturnType,
                new ParameterExpression[] { tmp },
                CreateInterfaceProxyExpression(binder, tmp),
                Expression.Condition
                (
                    Expression.Equal
                    (
                        tmp,
                        Expression.Constant(null)
                    ),
                    getDefault.Expression,
                    Expression.Convert
                    (
                        tmp,
                        binder.ReturnType
                    )
                )
            );

            BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
            return new DynamicMetaObject(expression, restrictions);
        }

        Expression CreatePropertyIdExpression(DynamicMetaObjectBinder binder)
        {
            return Expression.MakeBinary
            (
                ExpressionType.Or,
                Expression.Property
                (
                    Expression.Convert
                    (
                        Expression.Convert(Expression, LimitType),
                        typeof(IFlexObject)
                    ),
                    template
                ),
                Expression.Call
                (
                    createIdFromString,
                    Expression.Property
                    (
                        Expression.Constant(binder),
                        binder.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.Public)
                    ),
                    Expression.Constant(Fnv.FnvOffsetBias)
                )
            );
        }
        Expression CreateInterfaceProxyExpression(ConvertBinder binder, ParameterExpression variable)
        {
            return Expression.Assign
            (
                variable,
                Expression.Call
                (
                    createInterfaceProxy,
                    Expression.Constant(binder.Type),
                    Expression.Convert
                    (
                        Expression.Constant(Value),
                        typeof(object)
                    )
                )
            );
        }

        /// <summary>
        /// Creates an expression called on the DLR binding a method invocation to the underlaying flex object.
        /// </summary>
        /// <param name="binder">The DLR binder object</param>
        /// <param name="genericArgs">A list of generic arguments this method was called with</param>
        /// <param name="args">The invocation arguments passed from the DLR</param>
        /// <param name="defaultAction">An expression to perform the runtime specific fallback action</param>
        /// <param name="variable">A parameter expression containing the desired methodId</param>
        /// <returns>An expression to be bound in the DLR</returns>
        protected Expression CreateInvokeMemberFunctionExpression(InvokeMemberBinder binder, IEnumerable<Type> genericArgs, DynamicMetaObject[] args, Expression defaultAction, ParameterExpression variable)
        {
            if (enableExtensionMethods && (genericArgs == null || !genericArgs.Any()))
            {
                switch (binder.Name)
                {
                    /**
                     Properties
                    */
                    case "SetProperty": return Expression.Convert
                        (
                            Expression.Call
                            (
                                setProperty,
                                args.ToExpressionList(variable)
                            ),
                            typeof(object)
                        );
                    case "TryGetProperty": return Expression.Convert
                        (
                            Expression.Call
                            (
                                tryGetPropertyByName,
                                args.ToExpressionList(variable)
                            ),
                            typeof(object)
                        );
                    case "RemoveProperty": return Expression.Convert
                        (
                            Expression.Call
                            (
                                removeProperty,
                                args.ToExpressionList(variable)
                            ),
                            typeof(object)
                        );
                    /**
                     Extension Methods
                    */
                    case "AddMethod": return Expression.Convert
                        (
                            Expression.Call
                            (
                                addMethod,
                                args.ToExpressionList(variable)
                            ),
                            typeof(object)
                        );
                    case "TryGetMethod": return Expression.Convert
                        (
                            Expression.Call
                            (
                                tryGetMethodByName,
                                args.ToExpressionList(variable)
                            ),
                            typeof(object)
                        );
                    case "RemoveMethod": return Expression.Convert
                        (
                            Expression.Call
                            (
                                removeMethod,
                                args.ToExpressionList(variable)
                            ),
                            typeof(object)
                        );
                    default: return CreateInvokeMemberDefaultExpression(binder, genericArgs, args, defaultAction, variable);
                }
            }
            else return CreateInvokeMemberDefaultExpression(binder, genericArgs, args, defaultAction, variable);
        }
        /// <summary>
        /// Creates an expression called on the DLR binding a method invocation to the underlaying flex object
        /// </summary>
        /// <param name="binder">The DLR binder object</param>
        /// <param name="genericArgs">A list of generic arguments this method was called with</param>
        /// <param name="args">The invocation arguments passed from the DLR</param>
        /// <param name="defaultAction">An expression to perform the runtime specific fallback action</param>
        /// <param name="variable">A parameter expression containing the desired methodId</param>
        /// <returns>An expression to be bound in the DLR</returns>
        protected Expression CreateInvokeMemberDefaultExpression(InvokeMemberBinder binder, IEnumerable<Type> genericArgs, DynamicMetaObject[] args, Expression defaultAction, ParameterExpression variable)
        {
            ParameterExpression tmp = Expression.Variable(typeof(Delegate), "tmp0");
            if (genericArgs == null || !genericArgs.Any())
            {
                return Expression.Block
                (
                    new ParameterExpression[] { tmp },
                    Expression.Condition
                    (
                        Expression.Call
                        (
                            tryGetMethodById,
                            variable,
                            tmp
                        ),
                        Expression.Call
                        (
                            Expression.Property
                            (
                                tmp,
                                method
                            ),
                            invoke,
                            Expression.Constant(null),
                            Expression.NewArrayInit
                            (
                                typeof(object),
                                args.ToExpressionList<object>(Expression.Constant(Value))
                            )
                        ),
                        defaultAction,
                        binder.ReturnType
                    )
                );
            }
            else return defaultAction;
        }

        BindingRestrictions GetRestrictions()
        {
            if (Value == null && HasValue) return BindingRestrictions.GetInstanceRestriction(Expression, null);
            else return BindingRestrictions.GetTypeRestriction(Expression, LimitType);
        }
    }
}
