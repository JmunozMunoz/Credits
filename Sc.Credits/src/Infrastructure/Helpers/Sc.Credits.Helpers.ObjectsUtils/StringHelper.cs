using System;
using System.Reflection;
using System.Text;

namespace Sc.Credits.Helpers.ObjectsUtils
{
    /// <summary>
    /// String helper
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// To debug string
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public static string ToDebugString(object @object)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PropertyInfo propertyInfo in @object.GetType().GetProperties())
            {
                object value = propertyInfo.GetValue(@object);
                if (value != null)
                {
                    if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType.BaseType == typeof(object))
                    {
                        stringBuilder.AppendLine($"{propertyInfo.Name} = {value}");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"{propertyInfo.Name}");
                        stringBuilder.AppendLine("===============");
                        stringBuilder.AppendLine($"{value}");
                    }
                }
            }

            return stringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }
    }
}