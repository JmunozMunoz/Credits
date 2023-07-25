using Sc.Credits.Domain.Model.Customers.Queries;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Queries.Extensions;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using SqlKata;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Customers
{
    /// <summary>
    /// Customer queries
    /// </summary>
    internal class CustomerQueries
        : CommandQueries<CustomerFields>
    {
        /// <summary>
        /// New customer queries
        /// </summary>
        public CustomerQueries()
            : base(Tables.Catalog.Customers)
        {
        }

        /// <summary>
        /// Select by document info
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="fields"></param>
        /// <param name="entitiesForMerge"></param>
        /// <returns></returns>
        public SqlQuery ByDocumentInfo(string idDocument, string documentType, IEnumerable<Field> fields, params object[] entitiesForMerge)
        {
            Query query =
                Query
                    .Where(Fields.IdDocument.Name, idDocument)
                    .Where(Fields.DocumentType.Name, documentType)
                    .Select((fields ?? Fields.GetAllFields()).Select(f => f.Name).ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Gets the customer by document.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public SqlQuery GetCustomerByDocument(string idDocument, IEnumerable<Field> fields)
        {
            Query query =
                Query.Where(Fields.IdDocument.Name, idDocument)
                .Select((fields ?? Fields.GetAllFields()).Select(f => f.Name).ToArray());

            return ToReadQuery(query);
        }
    }
}