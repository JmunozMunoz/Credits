using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits
{
    internal class UnapprovedCreditQueries
        : CommandQueries<UnapprovedCreditFields>
    {
        public UnapprovedCreditQueries() 
            : base(Tables.Catalog.UnapprovedCredits)
        {
        }
    }
}
