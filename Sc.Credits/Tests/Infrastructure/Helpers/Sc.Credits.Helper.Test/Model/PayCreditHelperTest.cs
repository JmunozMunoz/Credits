using Sc.Credits.Domain.Model.Credits;
using System;

namespace Sc.Credits.Helper.Test.Model
{
    public static class PayCreditHelperTest
    {
        public static PaymentCreditResponse GetpaymentCreditResponse()
        {
            return new PaymentCreditResponse
            {
                ArrearsValuePaid = 100,
                AssuranceValuePaid = 100,
                Balance = 100,
                ChargeValuePaid = 100,
                CreditId = Guid.NewGuid(),
                CreditValuePaid = 100,
                HasCharges = false,
                IdDocument = "1025696698",
                NextDueDate = DateTime.Now,
                InterestValuePaid = 100,
                NextMinimumPayment = 100,
                PaidCredit = false,
                PaymentId = Guid.NewGuid(),
                PaymentNumber = 1,
                TypeDocument = "CC"
            };
        }
    }
}