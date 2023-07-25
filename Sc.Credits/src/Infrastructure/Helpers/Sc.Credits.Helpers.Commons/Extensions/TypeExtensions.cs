using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sc.Credits.Helpers.Commons.Extensions
{
    /// <summary>
    /// Object activator
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate object ObjectActivator(params object[] args);

    /// <summary>
    /// Type extensions
    /// </summary>
    public static class TypeExtensions
    {
        public static object New(this Type input, params object[] args)
        {
            IEnumerable<Type> constructorTypes = args.Select(p => p.GetType());
            ConstructorInfo constructorInfo =
                input.GetConstructor(constructorTypes.ToArray())
                ??
                input.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single();

            ParameterInfo[] parametersInfo = constructorInfo?.GetParameters() ?? Array.Empty<ParameterInfo>();

            ParameterExpression parameterExpression = Expression.Parameter(typeof(object[]), "args");

            Expression[] argex = new Expression[parametersInfo.Length];
            for (int index = 0; index < parametersInfo.Length; index++)
            {
                ConstantExpression indexExpression = Expression.Constant(index);
                Type parameterType = parametersInfo[index].ParameterType;
                BinaryExpression accessorExpression = Expression.ArrayIndex(parameterExpression, indexExpression);
                UnaryExpression castExpression = Expression.Convert(accessorExpression, parameterType);
                argex[index] = castExpression;
            }

            NewExpression newExpression = Expression.New(constructorInfo, argex);
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator), newExpression, parameterExpression);
            var result = (ObjectActivator)lambda.Compile();
            return result(args);
        }
    }

    /// <summary>
    /// Type instance
    /// </summary>
    public static class TypeInstance
    {
        /// <summary>
        /// Generates new instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T New<T>(params object[] args)
        {
            return (T)typeof(T).New(args);
        }

        /// <summary>
        /// Generates new instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object New(Type type, params object[] args)
        {
            return type.New(args);
        }
    }
}