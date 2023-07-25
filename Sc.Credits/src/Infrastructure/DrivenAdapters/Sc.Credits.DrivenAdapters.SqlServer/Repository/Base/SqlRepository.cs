using Sc.Credits.DrivenAdapters.SqlServer.Connection;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository.Base
{
    /// <summary>
    /// The sql repository base class.
    /// </summary>
    public abstract class SqlRepository
    {
        /// <summary>
        /// The current connection factory.
        /// </summary>
        protected readonly IConnectionFactory ConnectionFactory;

        /// <summary>
        /// Creates new instance of <see cref="SqlRepository"/>
        /// </summary>
        /// <param name="connectionFactory"></param>
        protected SqlRepository(IConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }
    }
}