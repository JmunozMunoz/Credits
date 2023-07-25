using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Refinancings.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Refinancings
{
    /// <summary>
    /// Refinancing application queries
    /// </summary>
    internal class RefinancingApplicationQueries
        : CommandQueries<RefinancingApplicationFields>
    {



        /// <summary>
        /// New refinancing application queries
        /// </summary>
        public RefinancingApplicationQueries()
            : base(Tables.Catalog.RefinancingApplications)
        {
        }

    }
}