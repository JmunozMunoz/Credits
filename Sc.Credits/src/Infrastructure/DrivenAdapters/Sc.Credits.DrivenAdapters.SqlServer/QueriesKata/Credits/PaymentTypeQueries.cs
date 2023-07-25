using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits
{
    /// <summary>
    /// Payment type queries
    /// </summary>
    internal class PaymentTypeQueries
        : ReadQueries<PaymentTypeFields>
    {
        /// <summary>
        /// New payment type queries
        /// </summary>
        public PaymentTypeQueries()
            : base(Tables.Catalog.PaymentTypes)
        {
        }
    }
}