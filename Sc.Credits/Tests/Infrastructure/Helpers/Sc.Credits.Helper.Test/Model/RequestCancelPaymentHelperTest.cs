using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Helper.Test.Model
{
    public static class RequestCancelPaymentHelperTest
    {
        /// <summary>
        /// Get request cancel payment
        /// </summary>
        /// <returns></returns>
        public static RequestCancelPayment GetRequestCancelPayment()
        {
            return new RequestCancelPayment(Guid.NewGuid(), Guid.NewGuid(), "Pepito", Guid.NewGuid().ToString(), "Error", 3, "dksd45a8z42za");
        }

        /// <summary>
        /// Get request cancel payment
        /// </summary>
        /// <param name="CreditId"></param>
        /// <returns></returns>
        public static RequestCancelPayment GetRequestCancelPayment(Guid CreditId)
        {
            return new RequestCancelPayment(CreditId, Guid.NewGuid(), "Pepito", Guid.NewGuid().ToString(), "Error", (int)RequestStatuses.Pending, "as545xz2a6gf5s4a");
        }

        /// <summary>
        /// Get request cancel payment
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="creditMasterId"></param>
        /// <param name="storeId"></param>
        /// <param name="requestStatusId"></param>
        /// <returns></returns>
        public static RequestCancelPayment GetRequestCancelPayment(Guid paymentId, Guid creditMasterId, string storeId, int requestStatusId)
        {
            return new RequestCancelPayment(
                paymentId, creditMasterId, "Pepito", storeId,
                "Error", requestStatusId, Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Get request cancel payment
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="creditMasterId"></param>
        /// <param name="storeId"></param>
        /// <param name="requestStatus"></param>
        /// <returns></returns>
        public static RequestCancelPayment GetRequestCancelPayment(Guid paymentId, Guid creditMasterId, string storeId, RequestStatus requestStatus)
        {
            return new RequestCancelPayment(
                paymentId, creditMasterId, "Pepito", storeId,
                "Error", requestStatus.Id, Guid.NewGuid().ToString())
                .SetRequestStatus(requestStatus);
        }

        /// <summary>
        /// Get request cancel payment
        /// </summary>
        /// <returns></returns>
        public static List<RequestCancelPayment> GetRequestCancelPaymentList()
        {
            return new List<RequestCancelPayment>
            {
                new RequestCancelPayment(Guid.NewGuid(), Guid.NewGuid(), "Pepito", "123456",
                    "Error", 3, "dksd45a8z42za").SetId(default),
                new RequestCancelPayment(Guid.NewGuid(), Guid.NewGuid(), "Pepito", "123456",
                    "Error", 3, "dksd45a8z42za"),
                new RequestCancelPayment(Guid.NewGuid(), Guid.NewGuid(), "Pepito 2", "1234567",
                    "Error 2", 3, "dksd45a8z42za")
            };
        }

        /// <summary>
        /// Get cancel payment request with status
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="requestStatusId"></param>
        /// <returns></returns>
        public static RequestCancelPayment GetCancelPaymentRequestWithStatus(Credit payment, int requestStatusId)
        {
            return GetRequestCancelPayment(payment.Id, payment.GetCreditMasterId, payment.GetStoreId, requestStatusId).SetPayment(payment);
        }

        /// <summary>
        /// Get random cancel payment request list With Status
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        public static List<RequestCancelPayment> GetRandomCancelPaymentRequestListWithStatus(List<Credit> payments)
        {
            Random rnd = new Random();
            int numberOfRequestStatuses = Enum.GetNames(typeof(RequestStatuses)).Length;
            bool randomBoolean = false;
            int randomRequestStatusId = 1;

            List<RequestCancelPayment> randomCancelPaymentRequestWithStatus = new List<RequestCancelPayment>();

            payments.ForEach(payment =>
            {
                randomBoolean = rnd.Next(0, 2) == 0;
                if (randomBoolean)
                {
                    randomRequestStatusId = rnd.Next(1, numberOfRequestStatuses + 1);
                    randomCancelPaymentRequestWithStatus.Add(GetCancelPaymentRequestWithStatus(payment, randomRequestStatusId).SetPayment(payment));
                }
            });

            return randomCancelPaymentRequestWithStatus;
        }

        /// <summary>
        /// Get request cancel payments with same store canceleds
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <returns></returns>
        public static List<RequestCancelPayment> GetRequestCancelPaymentsWithSameStoreCanceleds(List<CreditMaster> creditMasters)
        {
            string storeId = "8a78s7a887sa8";
            return creditMasters.Select(item =>
                GetRequestCancelPayment(item.Current.Id, item.Id, storeId, (int)RequestStatuses.Cancel)
                .SetPayment(item.Current)
                .SetCreditMaster(item)
                .SetProcessDate(DateTime.Now)).ToList();
        }

        /// <summary>
        /// Get request cancel payments from payments
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        public static List<RequestCancelPayment> GetRequestCancelPaymentsFromPayments(List<Credit> payments)
        {
            return payments
                .Select(item =>
                    GetRequestCancelPayment(item.Id, item.GetCreditMasterId, item.GetStoreId,
                        new RequestStatus(nameof(RequestStatuses.Cancel)).SetId((int)RequestStatuses.Cancel))
                .SetPayment(item)
                .SetProcessDate(DateTime.Now)).ToList();
        }
    }
}