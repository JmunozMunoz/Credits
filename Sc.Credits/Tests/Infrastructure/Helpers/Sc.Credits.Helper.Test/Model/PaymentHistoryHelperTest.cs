using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Helper.Test.Model
{
    /// <summary>
    /// Payment history helper test
    /// </summary>
    public static class PaymentHistoryHelperTest
    {
        public static List<PaymentHistoryResponse> GetResponsesFromPayments(List<Credit> payments)
        {
            Random randomDays = new Random();
            return payments
                .Select(payment =>
                    new PaymentHistoryResponse
                    {
                        CancelDate = DateTime.Now.AddDays(-randomDays.Next(0, 100)),
                        CreationDate = payment.GetTransactionDate,
                        CreditId = payment.GetCreditMasterId,
                        PaymentId = payment.Id,
                        CreditNumber = payment.GetCreditNumber,
                        PaymentNumber = payment.CreditPayment.GetPaymentNumber,
                        Status = payment.GetStatusName,
                        StoreId = payment.GetStoreId,
                        StoreName = payment.Store.StoreName,
                        ValuePaid = payment.CreditPayment.GetTotalValuePaid
                    })
                .ToList();
        }
    }
}