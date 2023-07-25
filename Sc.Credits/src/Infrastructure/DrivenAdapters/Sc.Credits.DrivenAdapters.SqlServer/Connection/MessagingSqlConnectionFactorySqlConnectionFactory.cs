namespace Sc.Credits.DrivenAdapters.SqlServer.Connection
{
    /// <summary>
    /// <see cref="IMessagingConnectionFactory"/>
    /// </summary>
    public class MessagingSqlConnectionFactory : SqlConnectionFactory, IMessagingConnectionFactory
    {
        /// <summary>
        /// New messaging sql connection factory
        /// </summary>
        /// <param name="connectionString"></param>
        public MessagingSqlConnectionFactory(string connectionString)
            : base(connectionString)
        {
        }
    }
}