using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sc.Credits.Helpers.ObjectsUtils
{
    /// <summary>
    /// Template Parameter Replace class
    /// </summary>
    public static class TemplateParameterReplace
    {
        /// <summary>
        /// Replace template parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="parameters"></param>
        /// <param name="formatParameter"></param>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static string ParametersReplace<T>(string template, T parameters, string formatParameter, Dictionary<string, string> tables = null)
            where T : class
        {
            Type tModelType = parameters.GetType();

            PropertyInfo[] arrayPropertyInfos = tModelType.GetProperties();

            foreach (PropertyInfo property in arrayPropertyInfos.Where(prop => prop.GetValue(parameters) != null))
            {
                template = template.Replace(string.Format(formatParameter, property.Name), property.GetValue(parameters).ToString());
            }

            foreach (KeyValuePair<string, string> item in tables)
            {
                template = template.Replace(item.Key, item.Value);
            }

            return template;
        }
    }
}