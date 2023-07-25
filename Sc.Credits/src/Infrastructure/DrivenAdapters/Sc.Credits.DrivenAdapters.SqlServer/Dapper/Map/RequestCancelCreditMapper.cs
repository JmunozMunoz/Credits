using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Stores;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.Dapper.Map
{
    /// <summary>
    /// Request cancel credit mapper
    /// </summary>
    public class RequestCancelCreditMapper
        : Mapper
    {
        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static RequestCancelCreditMapper New()
            => new RequestCancelCreditMapper();

        /// <summary>
        /// New request cancel credit mapper
        /// </summary>
        protected RequestCancelCreditMapper()
        {
        }

        /// <summary>
        /// From dynamic query
        /// </summary>
        /// <param name="dynamicQuery"></param>
        /// <returns></returns>
        public RequestCancelCredit FromDynamicQuery(IEnumerable<dynamic> dynamicQuery)
        {
            RequestCancelCredit requestCancelCredit = null;

            if (dynamicQuery is ICollection<object> rows && rows.Any())
            {
                requestCancelCredit = FromDynamicRow(rows.Single());
            }

            return requestCancelCredit;
        }

        /// <summary>
        /// From dynamic row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public RequestCancelCredit FromDynamicRow(object row)
        {
            RequestCancelCredit requestCancelCredit = null;

            if (row is IDictionary<string, object> dictionary)
            {
                ICollection<IDictionary<string, object>> dictionaries = SplitDictionary(dictionary);

                requestCancelCredit = RequestCancelCredit.New();

                IEnumerator<IDictionary<string, object>> enumerator = dictionaries.GetEnumerator();

                enumerator.MoveNext();

                MapEntity(requestCancelCredit, enumerator);

                bool next = enumerator.MoveNext();

                if (next)
                {
                    TryMapStore(requestCancelCredit, enumerator);
                }
            }
            return requestCancelCredit;
        }

        /// <summary>
        /// Try map store
        /// </summary>
        /// <param name="requestCancelCredit"></param>
        /// <param name="enumerator"></param>
        private void TryMapStore(RequestCancelCredit requestCancelCredit, IEnumerator<IDictionary<string, object>> enumerator)
        {
            Store store = Store.New();
            MapEntity(store, enumerator);

            enumerator.MoveNext();

            BusinessGroup businessGroup = BusinessGroup.New();
            MapEntity(businessGroup, enumerator);

            if (store != null)
            {
                requestCancelCredit.SetStore(store);

                if (businessGroup.Id != null)
                {
                    requestCancelCredit.Store.SetBusinessGroup(businessGroup);

                }
            }
        }
    }
}