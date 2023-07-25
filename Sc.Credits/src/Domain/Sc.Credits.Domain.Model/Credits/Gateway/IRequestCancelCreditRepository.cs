using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Credits.Gateway
{
    /// <summary>
    /// Request cancel credit repository contract
    /// </summary>
    public interface IRequestCancelCreditRepository
        : ICommandRepository<RequestCancelCredit>
    {
        /// <summary>
        /// Get by status
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="status"></param>
        /// <param name="storeFields"></param>
        /// <param name="businessGroupFields"></param>
        /// <returns></returns>
        Task<RequestCancelCredit> GetByStatusAsync(Guid creditMasterId, RequestStatuses status,
            IEnumerable<Field> storeFields = null, IEnumerable<Field> businessGroupFields = null);

        /// <summary>
        /// Get by status
        /// </summary>
        /// <param name="creditMasterIds"></param>
        /// <param name="status"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        Task<List<RequestCancelCredit>> GetByStatusAsync(List<Guid> creditMasterIds, RequestStatuses status,
            IEnumerable<Field> storeFields = null);

        /// <summary>
        /// Gets by status until date
        /// </summary>
        /// <param name="cancellationRequestDate"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<List<RequestCancelCredit>> GetByStatusUntilDateAsync(DateTime cancellationRequestDate, RequestStatuses status);

        /// <summary>
        /// Get by vendor
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="valuePage"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <param name="creditFields"></param>
        /// <param name="paymentFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<RequestCancelCreditPaged> GetByVendorAsync(string vendorId, int pageNumber,int valuePage, bool count, RequestStatuses status,
            IEnumerable<Field> fields, IEnumerable<Field> creditFields, IEnumerable<Field> paymentFields,
            IEnumerable<Field> customerFields, IEnumerable<Field> storeFields);
    }
}