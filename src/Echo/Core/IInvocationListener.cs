﻿using System.Reflection;

namespace Echo.Core
{
    public interface IInvocationListener
    {
        void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class;
    }
}
