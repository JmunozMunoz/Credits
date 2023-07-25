using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sc.Credits.Helpers.ObjectsUtils.Extensions
{
    /// <summary>
    /// The object activador signature.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate object ObjectActivator(params object[] args);

    /// <summary>
    /// The type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Creates new instance of specific type.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="args"></param>
        /// <returns></returns>
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

            Expression[] argumentExpressions = new Expression[parametersInfo.Length];
            for (int index = 0; index < parametersInfo.Length; index++)
            {
                argumentExpressions[index] = NewArgumentExpression(parametersInfo[index], parameterExpression, index);
            }

            NewExpression newExpression = Expression.New(constructorInfo, argumentExpressions);

            LambdaExpression lambdaActivator = Expression.Lambda(typeof(ObjectActivator), newExpression, parameterExpression);

            ObjectActivator activator = (ObjectActivator)lambdaActivator.Compile();

            return activator(args);
        }

        /// <summary>
        /// Creates a new argument extension.
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <param name="parameterExpression"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static Expression NewArgumentExpression(ParameterInfo parameterInfo, ParameterExpression parameterExpression, int index)
        {
            ConstantExpression indexExpression = Expression.Constant(index);
            Type parameterType = parameterInfo.ParameterType;
            BinaryExpression accessorExpression = Expression.ArrayIndex(parameterExpression, indexExpression);
            UnaryExpression castExpression = Expression.Convert(accessorExpression, parameterType);

            return castExpression;
        }
    }
}
