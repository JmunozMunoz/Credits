using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Credits.Gateway
{
    /// <summary>
    /// Request cancel payment repository contract
    /// </summary>
    public interface IRequestCancelPaymentRepository
        : ICommandRepository<RequestCancelPayment>
    {
        /// <summary>
        /// Get from masters
        /// </summary>
        /// <param name="creditMasterIds"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<List<RequestCancelPayment>> GetFromMastersAsync(List<Guid> creditMasterIds, IEnumerable<Field> fields);

        /// <summary>
        /// Get by status from master
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<List<RequestCancelPayment>> GetByStatusFromMasterAsync(Guid creditMasterId, RequestStatuses status,
            IEnumerable<Field> fields);

        /// <summary>
        /// Get by status from master
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        Task<List<RequestCancelPayment>> GetByStatusFromMasterAsync(CreditMaster creditMaster, RequestStatuses status,
            IEnumerable<Field> fields, IEnumerable<Field> storeFields = null);

        /// <summary>
        /// Get by status until date
        /// </summary>
        /// <param name="cancellationRequestDate"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<List<RequestCancelPayment>> GetByStatusUntilDate(DateTime cancellationRequestDate, RequestStatuses status);

        /// <summary>
        /// Get undismissed for masters
        /// </summary>
        /// <param name="creditMasterIds"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<List<RequestCancelPayment>> GetUndismissedForMastersAsync(List<Guid> creditMasterIds, IEnumerable<Field> fields);

        /// <summary>
        /// Get undismissed for master
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<List<RequestCancelPayment>> GetUndismissedForMasterAsync(Guid creditMasterId, IEnumerable<Field> fields);

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
        /// <returns></returns>
        Task<RequestCancelPaymentPaged> GetByVendorAsync(string vendorId, int pageNumber, int valuePage, bool count,RequestStatuses status,
            IEnumerable<Field> fields, IEnumerable<Field> creditFields, IEnumerable<Field> paymentFields,
            IEnumerable<Field> customerFields, IEnumerable<Field> storeFields);

        /// <summary>
        /// Get by status
        /// </summary>
        /// <param name="paymentIds"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <param name="statusFields"></param>
        /// <returns></returns>
        Task<List<RequestCancelPayment>> GetByNotStatusAsync(List<Guid> paymentIds, RequestStatuses status, IEnumerable<Field> fields,
            IEnumerable<Field> statusFields);

        /// <summary>
        /// Get undismissed
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<RequestCancelPayment> GetUndismissedAsync(Guid creditId, IEnumerable<Field> fields);

        /// <summary>
        /// Get by cancellation id
        /// </summary>
        /// <param name="creditCancelId"></param>
        /// <param name="fields"></param>
        /// <param name="requestStatuses"></param>
        /// <returns></returns>
        Task<RequestCancelPayment> GetByCancellationIdAsync(Guid creditCancelId, IEnumerable<Field> fields, RequestStatuses requestStatuses);
    }
}