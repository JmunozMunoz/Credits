using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Queries.Extensions;
using Sc.Credits.Domain.Model.Sequences.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using SqlKata;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Sequences
{
    /// <summary>
    /// Sequence queries
    /// </summary>
    internal class SequenceQueries
        : CommandQueries<SequenceFieds>
    {
        /// <summary>
        /// New sequence queries
        /// </summary>
        public SequenceQueries()
            : base(Tables.Catalog.Sequences)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public SqlQuery Last(string storeId, string type)
        {
            Query query =
                Query
                    .Where(Fields.StoreId.Name, storeId)
                    .Where(Fields.Type.Name, type)
                    .Select(Fields.GetAllFields()
                        .Select(f => f.Name)
                        .ToArray());

            return ToReadQuery(query);
        }
    }
}