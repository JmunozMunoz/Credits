using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Parameters;
using Sc.Credits.DrivenAdapters.SqlServer.Repository.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    /// <summary>
    /// <see cref="IAppParametersRepository"/>
    /// </summary>
    public class AppParametersRepository
        : SqlReadRepository, IAppParametersRepository
    {
        private static readonly ParameterQueries _parameterQueries = QueriesCatalog.Parameter;
        private static readonly TransactionTypeQueries _transactionTypeQueries = QueriesCatalog.TransactionType;
        private static readonly StatusQueries _statusQueries = QueriesCatalog.Status;
        private static readonly SourceQueries _sourceQueries = QueriesCatalog.Source;
        private static readonly PaymentTypeQueries _paymentTypeQueries = QueriesCatalog.PaymentType;
        private static readonly AuthMethodQueries _authMethodQueries = QueriesCatalog.AuthMethod;

        private readonly ISqlDelegatedHandlers<Parameter> _parameterSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<TransactionType> _transactionTypeSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<Status> _statusSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<Source> _sourceSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<PaymentType> _paymentTypeSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<AuthMethod> _authMethodSqlDelegatedHandlers;

        /// <summary>
        /// New app parameters repository
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="parameterSqlDelegatedHandlers"></param>
        /// <param name="transactionTypeSqlDelegatedHandlers"></param>
        /// <param name="statusSqlDelegatedHandlers"></param>
        /// <param name="sourceSqlDelegatedHandlers"></param>
        /// <param name="paymentTypeSqlDelegatedHandlers"></param>
        /// <param name="authMethodSqlDelegatedHandlers"></param>
        public AppParametersRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<Parameter> parameterSqlDelegatedHandlers,
            ISqlDelegatedHandlers<TransactionType> transactionTypeSqlDelegatedHandlers,
            ISqlDelegatedHandlers<Status> statusSqlDelegatedHandlers,
            ISqlDelegatedHandlers<Source> sourceSqlDelegatedHandlers,
            ISqlDelegatedHandlers<PaymentType> paymentTypeSqlDelegatedHandlers,
            ISqlDelegatedHandlers<AuthMethod> authMethodSqlDelegatedHandlers)
            : base(connectionFactory)
        {
            _parameterSqlDelegatedHandlers = parameterSqlDelegatedHandlers;
            _transactionTypeSqlDelegatedHandlers = transactionTypeSqlDelegatedHandlers;
            _statusSqlDelegatedHandlers = statusSqlDelegatedHandlers;
            _sourceSqlDelegatedHandlers = sourceSqlDelegatedHandlers;
            _paymentTypeSqlDelegatedHandlers = paymentTypeSqlDelegatedHandlers;
            _authMethodSqlDelegatedHandlers = authMethodSqlDelegatedHandlers;
        }

        /// <summary>
        /// <see cref="IAppParametersRepository.GetAllParametersAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task<List<Parameter>> GetAllParametersAsync() =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await _parameterSqlDelegatedHandlers.GetListAsync(connection,
                    _parameterQueries.All());
            });

        /// <summary>
        /// <see cref="IAppParametersRepository.GetTransactionTypeAsync(int)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TransactionType> GetTransactionTypeAsync(int id) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await _transactionTypeSqlDelegatedHandlers.GetSingleAsync(connection,
                    _transactionTypeQueries.ById(id));
            });

        /// <summary>
        /// <see cref="IAppParametersRepository.GetStatusAsync(int)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Status> GetStatusAsync(int id) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await _statusSqlDelegatedHandlers.GetSingleAsync(connection,
                    _statusQueries.ById(id));
            });

        /// <summary>
        /// <see cref="IAppParametersRepository.GetSourceAsync(int)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Source> GetSourceAsync(int id) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await _sourceSqlDelegatedHandlers.GetSingleAsync(connection,
                    _sourceQueries.ById(id));
            });

        /// <summary>
        /// <see cref="IAppParametersRepository.GetPaymentTypeAsync(int)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PaymentType> GetPaymentTypeAsync(int id) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await _paymentTypeSqlDelegatedHandlers.GetSingleAsync(connection,
                    _paymentTypeQueries.ById(id));
            });

        /// <summary>
        /// <see cref="IAppParametersRepository.GetAuthMethodAsync(int)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AuthMethod> GetAuthMethodAsync(int id) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await _authMethodSqlDelegatedHandlers.GetSingleAsync(connection,
                    _authMethodQueries.ById(id));
            });
    }
}