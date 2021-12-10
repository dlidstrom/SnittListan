﻿
using System.Diagnostics;

#nullable enable

namespace EventStoreLite.Infrastructure;
internal static class PrivateReflectionDynamicObjectExtensions
{
    [DebuggerStepThrough]
    public static dynamic? AsDynamic(this object o)
    {
        return PrivateReflectionDynamicObject.WrapObjectIfNeeded(o);
    }
}
