using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository.Base;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Messaging.Gateway;
using System.Data.Common;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    /// <summary>
    /// The default implementation of <see cref="IMessageLogRepository"/>
    /// </summary>
    public class MessageLogRepository
        : SqlRepository, IMessageLogRepository
    {
        private readonly ISqlDelegatedHandlers<MessageErrorLog> _messageErrorLogSqlDelegatedHandlers;

        /// <summary>
        /// Creates new instance of <see cref="MessageLogRepository"/>
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="messageErrorLogSqlDelegatedHandlers"></param>
        public MessageLogRepository(IMessagingConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<MessageErrorLog> messageErrorLogSqlDelegatedHandlers)
            : base(connectionFactory)
        {
            _messageErrorLogSqlDelegatedHandlers = messageErrorLogSqlDelegatedHandlers;
        }

        /// <summary>
        /// <see cref="IMessageLogRepository.AddErrorAsync(MessageErrorLog)"/>
        /// </summary>
        /// <param name="messageErrorLog"></param>
        public async Task AddErrorAsync(MessageErrorLog messageErrorLog)
        {
            using (DbConnection connection = ConnectionFactory.Create())
            {
                await connection.OpenAsync();

                await _messageErrorLogSqlDelegatedHandlers.InsertAsync(connection, messageErrorLog,
                    Tables.Catalog.MessageErrorLogs.Name);
            }
        }
    }
}