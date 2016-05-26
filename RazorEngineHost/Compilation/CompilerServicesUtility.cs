namespace RazorEngineHost.Compilation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class CompilerServicesUtility
    {
        private static readonly Type DynamicType = typeof(DynamicObject);
        private static readonly Type ExpandoType = typeof(ExpandoObject);
        private static readonly Type EnumerableType = typeof(IEnumerable);
        private static readonly Type EnumeratorType = typeof(IEnumerator);
        private static readonly Type GenericEnumerableType = typeof(IEnumerable<>);

        /// <summary>
        /// Determines if the specified type is an anonymous type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is an anonymous type, otherwise false.</returns>
        public static bool IsAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsClass && type.IsSealed && type.BaseType == typeof(object) && (type.Name.StartsWith("<>", StringComparison.Ordinal) || type.Name.StartsWith("VB$Anonymous", StringComparison.Ordinal)))
                return type.IsDefined(typeof(CompilerGeneratedAttribute), true);
            return false;
        }

        /// <summary>
        /// Checks if the given type is a anonymous type or a generic type containing a
        /// reference type as generic type argument
        /// </summary>
        /// <param name="t">the type to check</param>
        /// <returns>true when there exists a reference to an anonymous type.</returns>
        public static bool IsAnonymousTypeRecursive(Type t)
        {
            if (!(t != null))
                return false;
            if (!IsAnonymousType(t))
            {
                Type[] genericArguments = t.GetGenericArguments();
                if (!genericArguments.Any(IsAnonymousTypeRecursive))
                {
                    if (t.IsArray)
                        return IsAnonymousTypeRecursive(t.GetElementType());
                    return false;
                }
            }
            return true;
        }

        /// <summary>Determines if the specified type is a dynamic type.</summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is an anonymous type, otherwise false.</returns>
        public static bool IsDynamicType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!DynamicType.IsAssignableFrom(type) && !ExpandoType.IsAssignableFrom(type))
                return IsAnonymousType(type);
            return true;
        }

        /// <summary>
        /// Determines if the specified type is a compiler generated iterator type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is an iterator type, otherwise false.</returns>
        public static bool IsIteratorType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsNestedPrivate || !type.Name.StartsWith("<", StringComparison.Ordinal))
                return false;
            if (!EnumerableType.IsAssignableFrom(type))
                return EnumeratorType.IsAssignableFrom(type);
            return true;
        }

        /// <summary>Generates a random class name.</summary>
        /// <returns>A new random class name.</returns>
        public static string GenerateClassName()
        {
            return $"{"RazorEngine_"}{Guid.NewGuid().ToString("N")}";
        }

        /// <summary>
        /// Gets the public or protected constructors of the specified type.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>An enumerable of constructors.</returns>
        public static IEnumerable<ConstructorInfo> GetConstructors(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>Resolves the C# name of the given type.</summary>
        /// <param name="type">the type to emit.</param>
        /// <returns>The full type name or dynamic if the type is an instance of an dynamic type.</returns>
        public static string ResolveCSharpTypeName(Type type)
        {
            if (IsIteratorType(type))
                type = GetIteratorInterface(type);
            if (IsDynamicType(type))
                return "dynamic";
            string rawTypeName = CSharpGetRawTypeName(type);
            if (!type.IsGenericType)
                return rawTypeName;
            return rawTypeName + "<" + string.Join(", ", type.GetGenericArguments().Select(ResolveCSharpTypeName)) + ">";
        }

        /// <summary>Resolves the VB.net name of the given type.</summary>
        /// <param name="type">the type to emit.</param>
        /// <returns>The full type name or Object if the type is an instance of an dynamic type.</returns>
        public static string ResolveVBTypeName(Type type)
        {
            if (IsIteratorType(type))
                type = GetIteratorInterface(type);
            if (IsDynamicType(type))
                return "Object";
            string rawTypeName = VBGetRawTypeName(type);
            if (!type.IsGenericType)
                return rawTypeName;
            return rawTypeName + "(Of " + string.Join(", ", type.GetGenericArguments().Select(ResolveVBTypeName)) + ")";
        }

        /// <summary>
        /// Gets the Iterator type for the given compiler generated iterator.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>Tries to return IEnumerable of T if possible.</returns>
        public static Type GetIteratorInterface(Type type)
        {
            Type type1 = null;
            foreach (Type @interface in type.GetInterfaces())
            {
                if (type1 == null)
                    type1 = @interface;
                if (@interface.IsGenericType && !@interface.IsGenericTypeDefinition && @interface.GetGenericTypeDefinition() == GenericEnumerableType)
                    return @interface;
            }
            foreach (Type @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType)
                    return @interface;
            }
            return type1 ?? type;
        }

        /// <summary>
        /// Gets an enumerable of all assemblies loaded in the current domain.
        /// </summary>
        /// <returns>An enumerable of loaded assemblies.</returns>
        public static IEnumerable<Assembly> GetLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// Return the raw type name (including namespace) without any generic arguments.
        /// Returns the typename in a way it can be used in C# code.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CSharpGetRawTypeName(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            string str = type.FullName;
            if (type.IsGenericTypeDefinition || type.IsGenericType)
                str = type.FullName.Substring(0, type.FullName.IndexOf('`'));
            return str.Replace("+", ".");
        }

        /// <summary>
        /// Return the raw type name (including namespace) without any generic arguments.
        /// Returns the typename in a way it can be used in VB.net code.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string VBGetRawTypeName(Type type)
        {
            return CSharpGetRawTypeName(type);
        }

        /// <summary>
        /// Return the raw type name (including namespace) with the given modelTypeName as generic argument (if applicable).
        /// Returns the typename in a way it can be used in C# code.
        /// </summary>
        /// <param name="templateType"></param>
        /// <param name="modelTypeName"></param>
        /// <param name="throwWhenNotGeneric"></param>
        /// <returns></returns>
        public static string CSharpCreateGenericType(Type templateType, string modelTypeName, bool throwWhenNotGeneric)
        {
            string rawTypeName = CSharpGetRawTypeName(templateType);
            if (templateType.IsGenericTypeDefinition && templateType.IsGenericType)
                return rawTypeName + "<" + modelTypeName + ">";
            if (throwWhenNotGeneric)
                throw new NotSupportedException("The given base type is not generic!");
            return rawTypeName;
        }

        /// <summary>
        /// Return the raw type name (including namespace) with the given modelTypeName as generic argument (if applicable).
        /// Returns the typename in a way it can be used in VB.net code.
        /// </summary>
        /// <param name="templateType"></param>
        /// <param name="modelTypeName"></param>
        /// <param name="throwWhenNotGeneric"></param>
        /// <returns></returns>
        public static string VBCreateGenericType(Type templateType, string modelTypeName, bool throwWhenNotGeneric)
        {
            string rawTypeName = VBGetRawTypeName(templateType);
            if (templateType.IsGenericTypeDefinition && templateType.IsGenericType)
                return rawTypeName + "(Of " + modelTypeName + ")";
            if (throwWhenNotGeneric)
                throw new NotSupportedException("The given base type is not generic!");
            return rawTypeName;
        }
    }
}