using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sc.Credits.Domain.Model.ReportTemplates.Extensions
{
    public static class ReportTemplatesExtensions
    {
        /// <summary>
        /// Cast a specific object to compatible report template request data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IEnumerable<RequestData> ToReportRequestData<T>(this T element)
            where T : class
        {
            return element.GetType().GetProperties()
                .Select(property => new RequestData
                {
                    Key = property.Name,
                    Value = property.GetValue(element)?.ToString()
                });
        }
    }
}