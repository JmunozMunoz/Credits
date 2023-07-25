using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Refinancings.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Refinancings
{
    internal class RefinancingLogQueries
        : CommandQueries<RefinancingLogFields>
    {

        private static readonly RefinancingLogDetailFields _refinancingLogDetailFields = Tables.Catalog.RefinancingLogDetails.Fields;
        public RefinancingLogQueries() 
            : base(Tables.Catalog.RefinancingLogs)
        {
        }

        public SqlQuery ByStatusFromMaster(Guid creditMasterId, IEnumerable<Field> fields,
           IEnumerable<Field> storeFields = null)
        {
            Query query =
                Query
                    .Where(Fields.CreditMasterId.NameWithAlias, creditMasterId)
                    .When(fields != null,
                        q =>
                            q.Join(Tables.Catalog.RefinancingLogDetails.Name,
                                Fields.Id.NameWithAlias,
                                _refinancingLogDetailFields.RefinancingLogId.NameWithAlias))
                    .Select(fields
                        .Union(storeFields ?? Enumerable.Empty<Field>())
                            .Select(f => f.NameWithAlias)
                            .ToArray());


            return ToReadQuery(query);
        }
    }
}
