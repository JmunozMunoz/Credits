using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Stores;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.Dapper.Map
{
    /// <summary>
    /// Request cancel payment mapper
    /// </summary>
    public class RequestCancelPaymentMapper
        : Mapper
    {
        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static RequestCancelPaymentMapper New()
            => new RequestCancelPaymentMapper();

        /// <summary>
        /// New request cancel payment mapper
        /// </summary>
        protected RequestCancelPaymentMapper()
        {
        }

        /// <summary>
        /// From dynamic query
        /// </summary>
        /// <param name="dynamicQuery"></param>
        /// <param name="loadStatus"></param>
        /// <param name="requestStatus"></param>
        /// <returns></returns>
        public IEnumerable<RequestCancelPayment> FromDynamicQuery(IEnumerable<dynamic> dynamicQuery, bool loadStatus,
            RequestStatus requestStatus = null)
        {
            if (dynamicQuery is ICollection<object> rows && rows.Any())
            {
                foreach (object row in rows)
                {
                    yield return FromDynamicRow(row, loadStatus, requestStatus);
                }
            }
        }

        /// <summary>
        /// From dynamic row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="loadStatus"></param>
        /// <param name="requestStatus"></param>
        /// <returns></returns>
        public RequestCancelPayment FromDynamicRow(object row, bool loadStatus, RequestStatus requestStatus)
        {
            RequestCancelPayment requestCancelPayment = null;

            if (row is IDictionary<string, object> dictionary)
            {
                ICollection<IDictionary<string, object>> dictionaries = SplitDictionary(dictionary);

                requestCancelPayment = RequestCancelPayment.New();

                IEnumerator<IDictionary<string, object>> enumerator = dictionaries.GetEnumerator();

                enumerator.MoveNext();

                MapEntity(requestCancelPayment, enumerator);

                bool next = enumerator.MoveNext();

                TryMapStatus(requestCancelPayment, loadStatus, requestStatus, enumerator,
                    ref next);

                if (next)
                {
                    TryMapStore(requestCancelPayment, enumerator);
                }
            }
            return requestCancelPayment;
        }

        /// <summary>
        /// Try map store
        /// </summary>
        /// <param name="requestCancelPayment"></param>
        /// <param name="enumerator"></param>
        private void TryMapStore(RequestCancelPayment requestCancelPayment, IEnumerator<IDictionary<string, object>> enumerator)
        {
            Store store = Store.New();
            MapEntity(store, enumerator);

            enumerator.MoveNext();

            BusinessGroup businessGroup = BusinessGroup.New();
            MapEntity(businessGroup, enumerator);

            if (store != null)
            {
                requestCancelPayment.SetStore(store);

                if (businessGroup.Id !=null)
                {
                    requestCancelPayment.Store.SetBusinessGroup(businessGroup);

                }
            }
        }

        /// <summary>
        /// Try map status
        /// </summary>
        /// <param name="requestCancelPayment"></param>
        /// <param name="load"></param>
        /// <param name="requestStatus"></param>
        /// <param name="enumerator"></param>
        /// <param name="next"></param>
        private void TryMapStatus(RequestCancelPayment requestCancelPayment, bool load, RequestStatus requestStatus,
            IEnumerator<IDictionary<string, object>> enumerator, ref bool next)
        {
            if (load && next)
            {
                requestStatus = RequestStatus.New();

                MapEntity(requestStatus, enumerator);

                next = enumerator.MoveNext();
            }

            if (requestStatus != null)
            {
                requestCancelPayment.SetRequestStatus(requestStatus);
            }
        }
    }
}