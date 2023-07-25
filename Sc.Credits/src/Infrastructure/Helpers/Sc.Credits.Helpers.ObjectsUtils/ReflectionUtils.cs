using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sc.Credits.Helpers.ObjectsUtils
{
    /// <summary>
    /// Reflection utils
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Map private fields to public
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void MapPrivateFeldsToPublic<T>(object source, T target)
        {
            FieldInfo[] privateFields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            PropertyInfo[] publicProperties = target.GetType().GetProperties();

            foreach (FieldInfo privateField in privateFields)
            {
                PropertyInfo publicProperty = publicProperties.SingleOrDefault(property =>
                    property.Name.ToLower() == FormatFieldName(privateField.Name).ToLower());

                if (publicProperty != null)
                {
                    SetPropertyValue(target, publicProperty, privateField.GetValue(source));
                }
            }
        }

        /// <summary>
        /// Map dictionary to entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="target"></param>
        public static void MapDictionaryToEntity<T>(IDictionary<string, object> dictionary, T target)
        {
            if (dictionary == null)
                return;

            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (dictionary.TryGetValue(FormatFieldName(field.Name), out object value))
                {
                    field.SetValue(target, value);
                }
            }

            PropertyInfo[] properties = target.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (dictionary.TryGetValue(FormatFieldName(property.Name), out object value))
                {
                    SetPropertyValue(target, property, value);
                }
            }
        }

        /// <summary>
        /// Get values as dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="fieldsNames"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, object> GetValuesAsDictionary<T>(T target, params string[] fieldsNames)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            bool getAllFields = fieldsNames == null || !fieldsNames.Any();

            var allFields = target.GetType()
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            var fields = getAllFields ?
                allFields
                :
                allFields
                    .Where(field =>
                        fieldsNames
                            .Any(fieldName =>
                                fieldName.ToLower() == FormatFieldName(field.Name).ToLower()));

            foreach (var field in fields)
            {
                dictionary.Add(FormatFieldName(field.Name), field.GetValue(target));
            }

            var allProperties = target.GetType().GetProperties();
            var properties = getAllFields ?
                allProperties
                :
                allProperties
                    .Where(property =>
                        fieldsNames
                            .Any(fieldName =>
                                fieldName.ToLower() == FormatFieldName(property.Name).ToLower()));

            foreach (var property in properties)
            {
                dictionary.Add(FormatFieldName(property.Name), property.GetValue(target));
            }

            return dictionary;
        }

        /// <summary>
        /// Get value
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetValue(object entity, string fieldName)
        {
            var allFields = entity.GetType()
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            var field = allFields
                .FirstOrDefault(f => FormatFieldName(f.Name).ToLower() == fieldName.ToLower());

            if (field != null)
            {
                return field.GetValue(entity);
            }

            var allProperties = entity.GetType().GetProperties();
            var property = allProperties
                .FirstOrDefault(f => FormatFieldName(f.Name).ToLower() == fieldName.ToLower());

            if (property != null)
            {
                return property.GetValue(entity);
            }

            return null;
        }

        /// <summary>
        /// Set property value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="value"></param>
        private static void SetPropertyValue<T>(T instance, PropertyInfo propertyInfo, object value)
        {
            Type type = instance.GetType();

            ParameterExpression instanceParam = Expression.Parameter(type);
            ParameterExpression argumentParam = Expression.Parameter(typeof(object));

            MethodInfo getSetMethod = propertyInfo.GetSetMethod() ?? propertyInfo.GetSetMethod(true);

            Action<T, object> expression =
                Expression.Lambda<Action<T, object>>(Expression.Call(instanceParam, getSetMethod, Expression.Convert(argumentParam, propertyInfo.PropertyType)),
                    instanceParam, argumentParam).Compile();

            expression(instance, value);
        }

        /// <summary>
        /// Format field name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string FormatFieldName(string name) =>
            ToUpperFirstLetter(name.Replace("_", string.Empty));

        /// <summary>
        /// To upper first letter
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string ToUpperFirstLetter(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            char[] letters = source.ToCharArray();

            letters[0] = char.ToUpper(letters[0]);

            return new string(letters);
        }
    }
}