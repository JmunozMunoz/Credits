namespace Sc.Credits.DrivenAdapters.SqlServer.Connection
{
    /// <summary>
    /// <see cref="ICreditsConnectionFactory"/>
    /// </summary>
    public class CreditsSqlConnectionFactory : SqlConnectionFactory, ICreditsConnectionFactory
    {
        /// <summary>
        /// New credits sql connection factory
        /// </summary>
        /// <param name="connectionString"></param>
        public CreditsSqlConnectionFactory(string connectionString)
            : base(connectionString)
        {
        }
    }
}