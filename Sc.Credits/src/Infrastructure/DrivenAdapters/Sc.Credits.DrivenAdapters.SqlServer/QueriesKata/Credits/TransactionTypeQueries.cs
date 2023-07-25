using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits
{
    /// <summary>
    /// Transaction type queries
    /// </summary>
    internal class TransactionTypeQueries
        : ReadQueries<TransactionTypeFields>
    {
        /// <summary>
        /// New transaction type queries
        /// </summary>
        public TransactionTypeQueries()
            : base(Tables.Catalog.TransactionTypes)
        {
        }
    }
}