using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits;
using Sc.Credits.DrivenAdapters.SqlServer.Repository.Base;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    public class UnapprovedCreditRepository 
        : CommandRepository<UnapprovedCredit, UnapprovedCreditFields>, IUnapprovedCreditRepository
    {
        private static readonly UnapprovedCreditQueries _unapprovedCreditQueries = QueriesCatalog.UnapprovedCredit;

        public UnapprovedCreditRepository(ICreditsConnectionFactory connectionFactory, 
            ISqlDelegatedHandlers<UnapprovedCredit> unapprovedCreditSqlDelegatedHandlers) 
            : base(_unapprovedCreditQueries, unapprovedCreditSqlDelegatedHandlers, connectionFactory)
        {
        }
    }
}
    