using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Helper.Test.Model
{
    public static class CreditDetailResponseHelperTest
    {
        public static CreditDetailResponse GetCreditDetailResponse_BasicData() =>
                    new CreditDetailResponse()
                    {
                        DownPayment = 1,
                        TotalFeeValue = 1,
                        CreditValue = 1,
                        Fees = 1,
                        AssuranceValue = 1,
                        InterestRate = 1,
                        TotalInterestValue = 1,
                        TotalDownPayment = 1,
                        FeeCreditValue = 1,
                        AssuranceFeeValue = 1,
                        AssuranceTotalValue = 1,
                        AssuranceTaxFeeValue = 1,
                        AssuranceTaxValue = 1,
                        DownPaymentPercentage = 1,
                        AssurancePercentage = 1,
                        AssuranceTotalFeeValue = 1,
                        TotalPaymentValue = 1,
                        CustomerAllowPhotoSignature = true,
                    };
    }
}
