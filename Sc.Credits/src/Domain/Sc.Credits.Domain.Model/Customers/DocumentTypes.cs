using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Customers
{
    /// <summary>
    ///  Document types.
    /// </summary>
    public static class DocumentTypes
    {
        /// <summary>
        /// Names of document types
        /// </summary>
        public static readonly IReadOnlyDictionary<string, string> Names = new Dictionary<string, string>
        {
            { "CC", "Cédula de Ciudadanía" },
            { "CE", "Cédula de Extranjería" }
        };
    }
}
