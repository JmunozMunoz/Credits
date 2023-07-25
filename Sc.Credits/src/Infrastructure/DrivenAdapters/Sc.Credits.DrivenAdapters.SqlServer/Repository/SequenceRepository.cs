using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Sequences;
using Sc.Credits.Domain.Model.Sequences.Gateway;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Sequences;
using Sc.Credits.DrivenAdapters.SqlServer.Repository.Base;
using Sc.Credits.Helpers.Commons.Domain;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    /// <summary>
    /// <see cref="ISequenceRepository"/>
    /// </summary>
    public class SequenceRepository
        : SqlRepository, ISequenceRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        private readonly SequenceQueries _sequenceQueries = QueriesCatalog.Sequence;

        private readonly ISqlDelegatedHandlers<Sequence> _sequenceSqlDelegatedHandlers;

        /// <summary>
        /// New sequence repository
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="sequenceSqlDelegatedHandlers"></param>
        public SequenceRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<Sequence> sequenceSqlDelegatedHandlers)
            : base(connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException("connectionFactory");
            _sequenceSqlDelegatedHandlers = sequenceSqlDelegatedHandlers;
        }

        /// <summary>
        /// <see cref="ISequenceRepository.GetNextAsync(string, string)"/>
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<long> GetNextAsync(string storeId, string type)
        {
            Guid newId = IdentityGenerator.NewSequentialGuid();

            Sequence currentSequence = null;

            using (DbConnection connection = _connectionFactory.Create())
            {
                connection.Open();

                currentSequence =
                    await _sequenceSqlDelegatedHandlers.GetSingleAsync(connection,
                        _sequenceQueries.Last(storeId, type))
                    ??
                    new Sequence(newId, 0, storeId, type);

                currentSequence.SetLastNumber(currentSequence.LastNumber + 1);

                if (currentSequence.LastNumber == 1 && currentSequence.Id == newId)
                {
                    await _sequenceSqlDelegatedHandlers.InsertAsync(connection, currentSequence, Tables.Catalog.Sequences.Name);
                }
                else
                {
                    await _sequenceSqlDelegatedHandlers.ExcecuteAsync(connection, _sequenceQueries.Update(currentSequence, new List<Field>()
                    {
                        _sequenceQueries.Fields.LastNumber
                    }));
                }
            }

            return currentSequence.LastNumber;
        }
    }
}