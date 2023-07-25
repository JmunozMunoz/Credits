using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Queries.Extensions;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Stores.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Extensions;
using SqlKata;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Stores
{
    /// <summary>
    /// The store identification queries type.
    /// </summary>
    internal class StoreIdentificationQueries
        : Queries<StoreIdentificationFields>
    {
        /// <summary>
        /// Creates new instance of <see cref="StoreIdentificationQueries"/>.
        /// </summary>
        public StoreIdentificationQueries()
            : base(Tables.Catalog.StoreIdentifications)
        {
        }

        /// <summary>
        /// By store id query.
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public SqlQuery ByStoreId(string storeId)
        {
            Query query =
                Query
                    .Where(Fields.StoreId.Name, storeId)
                    .Select(Fields.GetAllFields().Select(f => f.Name).ToArray());

            return query.AsQueryResult(Compiler).AsReadQuery();
        }

        /// <summary>
        /// Update query.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fields"></param>
        /// <param name="entitiesForMerge"></param>
        /// <returns></returns>
        public SqlQuery Update(StoreIdentification storeIdentification, IEnumerable<Field> fields)
        {
            string[] updateFieldsNames = fields.Select(f => f.Name).ToArray();

            Query query =
                Query
                    .Where(Fields.StoreId.Name, storeIdentification.StoreId)
                    .AsUpdate(storeIdentification.GetValues(updateFieldsNames)
                        .ToDictionary(updatePairKey => updatePairKey.Key, updatePairValue => updatePairValue.Value));

            return query.AsQueryResult(Compiler).AsCommandQuery();
        }
    }
}