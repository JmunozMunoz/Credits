using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Refinancings.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Refinancings
{
    internal class RefinancingLogDetailQueries
        : ReadQueries<RefinancingLogDetailFields>
    {
        public RefinancingLogDetailQueries()
            : base(Tables.Catalog.RefinancingLogDetails)
        {
        }
    }
}
