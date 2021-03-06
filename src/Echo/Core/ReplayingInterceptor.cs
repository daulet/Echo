﻿using Castle.DynamicProxy;
using Echo.Utilities;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Echo.Core
{
    internal class ReplayingInterceptor<TTarget> : IInterceptor
        where TTarget : class
    {
        private readonly IInvocationReader _invocationReader;

        internal ReplayingInterceptor(IInvocationReader invocationReader)
        {
            _invocationReader = invocationReader;
        }

        public void Intercept(IInvocation invocation)
        {
            var isTask = false;
            var returnType = invocation.Method.ReturnType;
            if (typeof(Task).IsAssignableFrom(returnType))
            {
                isTask = true;
                if (returnType.IsConstructedGenericType)
                {
                    returnType = returnType.GenericTypeArguments[0];
                }
            }

            try
            {
                var recordedResult = FindInvocationResult(invocation.Method, invocation.Arguments);

                if (recordedResult is ExceptionInvocationResult)
                {
                    throw (recordedResult as ExceptionInvocationResult).ThrownException;
                }
                else if (recordedResult is ValueInvocationResult)
                {
                    object returnValue;
                    if (returnType.GetTypeInfo().IsValueType)
                    {
                        // due to a loss of type for value types in Newtonsoft.JSON serialization,
                        // e.g. int will be deserialized as long, hence this "casting down"
                        returnValue = Convert.ChangeType(
                            (recordedResult as ValueInvocationResult).ReturnedValue, returnType);
                    }
                    else
                    {
                        returnValue = (recordedResult as ValueInvocationResult).ReturnedValue;
                    }

                    if (isTask)
                    {
                        invocation.ReturnValue = Task.FromResult((dynamic)returnValue);
                    }
                    else
                    {
                        invocation.ReturnValue = returnValue;
                    }
                }
                else if (recordedResult is VoidInvocationResult)
                {
                    // no ReturnValue in this case
                    if (isTask)
                    {
                        invocation.ReturnValue = Task.CompletedTask;
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            catch (NoEchoFoundException)
            {
                // TODO This behaviour needs to be configurable: return default or throw - should be parameter of the constructor
                // TODO does null work for value types?
                invocation.ReturnValue = Activator.CreateInstance(returnType);
            }
        }

        private InvocationResult FindInvocationResult(MethodInfo methodInfo, object[] arguments)
        {
            var recordedInvocations = _invocationReader.GetAllInvocations();
            foreach (var invocation in recordedInvocations)
            {
                // TODO test case when method signatures match on different interfaces
                if (invocation.Target == typeof(TTarget))
                {
                    if (string.Equals(invocation.Method, methodInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        if (InvocationUtility.IsArgumentListMatch(invocation.Arguments, arguments))
                        {
                            return invocation.Result;
                        }
                    }
                }
            }
            throw new NoEchoFoundException();
        }
    }
}
