using System;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace EventStoreLite.Infrastructure
{
    internal class PrivateReflectionDynamicObject : DynamicObject
    {
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private object RealObject { get; set; }

        // Called when a method is called
        [DebuggerStepThrough]
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = InvokeMemberOnType(RealObject.GetType(), RealObject, binder.Name, args);

            // Wrap the sub object if necessary. This allows nested anonymous objects to work.
            result = WrapObjectIfNeeded(result);

            return true;
        }

        [DebuggerStepThrough]
        internal static object WrapObjectIfNeeded(object o)
        {
            // Don't wrap primitive types, which don't have many interesting internal APIs
            if (o == null || o.GetType().IsPrimitive || o is string)
                return o;

            return new PrivateReflectionDynamicObject { RealObject = o };
        }

        [DebuggerStepThrough]
        private static object InvokeMemberOnType(Type type, object target, string name, object[] args)
        {
            try
            {
                // Try to incoke the method
                return type.InvokeMember(
                    name,
                    BindingFlags.InvokeMethod | bindingFlags,
                    null,
                    target,
                    args);
            }
            catch (MissingMethodException)
            {
                // If we couldn't find the method, try on the base class
                if (type.BaseType != null)
                {
                    return InvokeMemberOnType(type.BaseType, target, name, args);
                }
                //Don't care if the method don't exist.
                return null;
            }
        }
    }
}