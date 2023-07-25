using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;

namespace Sc.Credits.Helper.Test.Model
{
    public static class AmortizationScheduleHelperTest
    {
        public static AmortizationScheduleRequest GetAmortizationScheduleRequest(decimal creditValue, DateTime initialDate, decimal feeValue, decimal interestRate, int frequency, int fees,
            decimal downPayment, decimal assuranceValue, decimal assuranceFeeValue, decimal assuranceTotalFeeValue) =>
            new AmortizationScheduleRequest
            {
                CreditValue = creditValue,
                InitialDate = initialDate,
                FeeValue = feeValue,
                InterestRate = interestRate,
                Frequency = frequency,
                Fees = fees,
                DownPayment = downPayment,
                AssuranceValue = assuranceValue,
                AssuranceFeeValue = assuranceFeeValue,
                AssuranceTotalFeeValue = assuranceTotalFeeValue
            };

        public static CurrentAmortizationScheduleResponse GetCurrentAmortizationScheduleResponse() =>
            new CurrentAmortizationScheduleResponse
            {
                AssuranceValue = 4000.0M,
                CreditValue = 4000.0M,
                DownPayment = 0M,
                CurrentAmortizationScheduleFees = new List<CurrentAmortizationScheduleFee>
                    {
                        new CurrentAmortizationScheduleFee
                        {
                            FeeStatus = "PAGADO",
                            ArrearPaymentValue = 0.0M,
                            Fee =  0,
                            FeeDate = new DateTime(2019,10,23),
                            Balance = 30000.0M,
                            FeeValue = 0.0M,
                            InterestValue = 0.0M,
                            CreditValuePayment = 0.0M,
                            FinalBalance = 30000.0M,
                            TotalFeeValue= 0.0M
                        },
                        new CurrentAmortizationScheduleFee
                        {
                            FeeStatus = "AL DIA",
                            ArrearPaymentValue = 0.0M,
                            Fee= 1,
                            FeeDate= new DateTime(2019,11,23),
                            Balance= 30000.0M,
                            FeeValue= 20632.0M,
                            InterestValue= 630.0M,
                            CreditValuePayment= 20002.0M,
                            FinalBalance= 9998.0M,
                            TotalFeeValue= 23012.0M
                        },
                        new CurrentAmortizationScheduleFee
                        {
                            FeeStatus = "AL DIA",
                            ArrearPaymentValue = 0.0M,
                            Fee= 2,
                            FeeDate= new DateTime(2019,12,23),
                            Balance= 9998,
                            FeeValue= 10208,
                            InterestValue= 210,
                            CreditValuePayment= 9998,
                            FinalBalance= 0,
                            TotalFeeValue= 12588
                        }
                    },
                CurrentAmortizationScheduleAssuranceFees = new List<AmortizationScheduleAssuranceFee>
                    {
                        new AmortizationScheduleAssuranceFee
                        {
                            Fee= 0,
                            FeeDate= new DateTime(2019,10,23),
                            Balance= 4000.0M,
                            AssuranceFeeValue= 0.0M,
                            AssuranceTaxValue= 0.0M,
                            AssurancePaymentValue= 0.0M,
                            FinalBalance= 4000.0M
                        },
                        new AmortizationScheduleAssuranceFee
                        {
                            Fee= 1,
                            FeeDate= new DateTime(2019,11,23),
                            Balance= 4000.0M,
                            AssuranceFeeValue= 2000,
                            AssuranceTaxValue= 380,
                            AssurancePaymentValue= 2380,
                            FinalBalance= 2000
                        },
                        new AmortizationScheduleAssuranceFee
                        {
                            Fee= 2,
                            FeeDate= new DateTime(2019,12,23),
                            Balance= 2000,
                            AssuranceFeeValue= 2000,
                            AssuranceTaxValue= 380,
                            AssurancePaymentValue= 2380,
                            FinalBalance= 0
                        }
                    }
            };

        public static CurrentAmortizationScheduleRequest GetCurrentAmortizationScheduleRequest(decimal creditValue, decimal balance, DateTime initialDate, decimal feeValue, decimal interestRate,
            int frequency, int fees, decimal downPayment, decimal assuranceValue, decimal assuranceFeeValue, decimal assuranceTotalFeeValue, decimal assuranceBalance, DateTime calculationDate,
            DateTime lastPaymentDate, bool hasArrearsCharge, decimal arrearsCharges) =>
            new CurrentAmortizationScheduleRequest
            {
                CreditValue = creditValue,
                InitialDate = initialDate,
                FeeValue = feeValue,
                InterestRate = interestRate,
                Frequency = frequency,
                Fees = fees,
                DownPayment = downPayment,
                AssuranceValue = assuranceValue,
                AssuranceFeeValue = assuranceFeeValue,
                AssuranceTotalFeeValue = assuranceTotalFeeValue,
                LastPaymentDate = lastPaymentDate,
                Balance = balance,
                HasArrearsCharge = hasArrearsCharge,
                ArrearsCharges = arrearsCharges,
                AssuranceBalance = assuranceBalance
            };

        public static AmortizationScheduleResponse GetAmortizationScheduleResponse() =>
            new AmortizationScheduleResponse()
            {
                CreditValue = 3512720.0M,
                DownPayment = 0.0M,
                AssuranceValue = 351272.0M,
                AmortizationScheduleFees = new List<AmortizationScheduleFee>()
                {
                    new AmortizationScheduleFee()
                    {
                        Fee = 0,
                        FeeDate = new DateTime(2020, month: 09, day: 30),
                        Balance = 3512720.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 34235.0M,
                        CreditValuePayment = 277365.0M,
                        FinalBalance = 3235355.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 1,
                        FeeDate = new DateTime(2020, month: 10, day: 14),
                        Balance = 3512720.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 34235.0M,
                        CreditValuePayment = 277365.0M,
                        FinalBalance = 3235355.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 2,
                        FeeDate = new DateTime(2020, month: 10, day: 28),
                        Balance = 3235355.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 31532.0M,
                        CreditValuePayment = 280068.0M,
                        FinalBalance = 2955287.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 3,
                        FeeDate = new DateTime(2020, month: 11, day: 11),
                        Balance = 2955287.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 28802.0M,
                        CreditValuePayment = 282798.0M,
                        FinalBalance = 2672489.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 4,
                        FeeDate = new DateTime(2020, month: 11, day: 25),
                        Balance = 2672489.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 26046.0M,
                        CreditValuePayment = 285554.0M,
                        FinalBalance = 2386935.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 5,
                        FeeDate = new DateTime(2020, month: 12, day: 9),
                        Balance = 2386935.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 23263.0M,
                        CreditValuePayment = 288337.0M,
                        FinalBalance = 2098598.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 6,
                        FeeDate = new DateTime(2020, month: 12, day: 23),
                        Balance = 2098598.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 20453.0M,
                        CreditValuePayment = 291147.0M,
                        FinalBalance = 1807451.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 7,
                        FeeDate = new DateTime(2021, month: 1, day: 6),
                        Balance = 1807451.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 17615.0M,
                        CreditValuePayment = 293985.0M,
                        FinalBalance = 1513466.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 8,
                        FeeDate = new DateTime(2021, month: 1, day: 20),
                        Balance = 1513466.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 14750.0M,
                        CreditValuePayment = 296850.0M,
                        FinalBalance = 1216616.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 9,
                        FeeDate = new DateTime(2021, month: 2, day: 3),
                        Balance = 1216616.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 11857.0M,
                        CreditValuePayment = 299743.0M,
                        FinalBalance = 916873.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 10,
                        FeeDate = new DateTime(2021, month: 2, day: 17),
                        Balance = 916873.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 8936.0M,
                        CreditValuePayment = 302664.0M,
                        FinalBalance = 614209.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 11,
                        FeeDate = new DateTime(2021, month: 3, day: 3),
                        Balance = 614209.0M,
                        FeeValue = 311600.0M,
                        InterestValue = 5986.0M,
                        CreditValuePayment = 305614.0M,
                        FinalBalance = 308595.0M,
                        TotalFeeValue = 346435.0M
                    },
                    new AmortizationScheduleFee()
                    {
                        Fee = 12,
                        FeeDate = new DateTime(2021, month: 3, day: 17),
                        Balance = 308595.0M,
                        FeeValue = 311603.0M,
                        InterestValue = 3008.0M,
                        CreditValuePayment = 308595.0M,
                        FinalBalance = 0.0M,
                        TotalFeeValue = 346434.0M
                    }
                },
                AmortizationScheduleAssuranceFees = new List<AmortizationScheduleAssuranceFee>()
                {
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 0,
                        FeeDate = new DateTime(2020, month: 9, day: 30),
                        Balance = 351272.0M,
                        AssuranceFeeValue = 0.0M,
                        AssuranceTaxValue = 0.0M,
                        AssurancePaymentValue = 0.0M,
                        FinalBalance = 351272.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 1,
                        FeeDate = new DateTime(2020, month: 10, day: 14),
                        Balance = 351272.0M,
                        AssuranceFeeValue = 29273.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34835.0M,
                        FinalBalance = 321999.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 2,
                        FeeDate = new DateTime(2020, month: 10, day: 28),
                        Balance = 321999.0M,
                        AssuranceFeeValue = 29273.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34835.0M,
                        FinalBalance = 292726.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 3,
                        FeeDate = new DateTime(2020, month: 11, day: 11),
                        Balance = 292726.0M,
                        AssuranceFeeValue = 29273.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34835.0M,
                        FinalBalance = 263453.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 4,
                        FeeDate = new DateTime(2020, month: 11, day: 25),
                        Balance = 263453.0M,
                        AssuranceFeeValue = 29273.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34835.0M,
                        FinalBalance = 234180.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 5,
                        FeeDate = new DateTime(2020, month: 12, day: 9),
                        Balance = 234180.0M,
                        AssuranceFeeValue = 29273.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34835.0M,
                        FinalBalance = 204907.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 6,
                        FeeDate = new DateTime(2020, month: 12, day: 23),
                        Balance = 204907.0M,
                        AssuranceFeeValue = 29273.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34835.0M,
                        FinalBalance = 175634.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 7,
                        FeeDate = new DateTime(2021, month: 1, day: 6),
                        Balance = 175634.0M,
                        AssuranceFeeValue = 29273.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34835.0M,
                        FinalBalance = 146361.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 8,
                        FeeDate = new DateTime(2021, month: 1, day: 20),
                        Balance = 146361.0M,
                        AssuranceFeeValue = 29273.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34835.0M,
                        FinalBalance = 117088.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 9,
                        FeeDate = new DateTime(2021, month: 2, day: 3),
                        Balance = 117088.0M,
                        AssuranceFeeValue = 29273.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34835.0M,
                        FinalBalance = 87815.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 10,
                        FeeDate = new DateTime(2021, month: 2, day: 17),
                        Balance = 87815.0M,
                        AssuranceFeeValue = 29273.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34835.0M,
                        FinalBalance = 58542.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 11,
                        FeeDate = new DateTime(2021, month: 3, day: 3),
                        Balance = 58542.0M,
                        AssuranceFeeValue = 29273.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34835.0M,
                        FinalBalance = 29269.0M
                    },
                    new AmortizationScheduleAssuranceFee()
                    {
                        Fee = 12,
                        FeeDate = new DateTime(2021, month: 3, day: 17),
                        Balance = 29269.0M,
                        AssuranceFeeValue = 29269.0M,
                        AssuranceTaxValue = 5562.0M,
                        AssurancePaymentValue = 34831.0M,
                        FinalBalance = 0.0M
                    }
                }
            };

        public static CurrentPaymentScheduleResponse GetCurrentPaymentScheduleAsyncResponse() =>
            new CurrentPaymentScheduleResponse()
            {
                PaymentCreditScheduleFees = GetCurrentAmortizationScheduleResponse().CurrentAmortizationScheduleFees,
                PaymentAssuranceScheduleFees = GetCurrentAmortizationScheduleResponse().CurrentAmortizationScheduleAssuranceFees,
            };
    }
}