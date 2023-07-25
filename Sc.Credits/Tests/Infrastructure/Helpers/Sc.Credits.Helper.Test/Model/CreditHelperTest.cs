namespace Sc.Credits.Helper.Test.Model
{
    using Sc.Credits.Domain.Model.Common;
    using Sc.Credits.Domain.Model.Credits;
    using Sc.Credits.Domain.Model.Credits.Events;
    using Sc.Credits.Domain.Model.Enums;
    using Sc.Credits.Domain.Model.Stores;
    using Sc.Credits.DrivenAdapters.ServiceBus.Model;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Credits Helper Test
    /// </summary>
    public static class CreditHelperTest
    {
        /// <summary>
        /// Get Credit Details
        /// </summary>
        /// <returns></returns>
        public static CreditDetailResponse GetCreditDetails()
        {
            return new CreditDetailResponse
            {
                CreditValue = 184000,
                Fees = 4,
                TotalFeeValue = 53913.76M,
                AssuranceValue = 18400,
                DownPayment = 16000,
                InterestRate = 0.02M,
                TotalInterestValue = 0,
                TotalPaymentValue = 215655.04M
            };
        }

        /// <summary>
        /// Get Credit Details
        /// </summary>
        /// <returns></returns>
        public static CreditDetailResponse GetCreditDetailsWithoutDownPayment()
        {
            return new CreditDetailResponse
            {
                CreditValue = 184000,
                Fees = 4,
                TotalFeeValue = 53913.76M,
                AssuranceValue = 18400,
                DownPayment = 0,
                InterestRate = 0.02M,
                TotalInterestValue = 0,
            };
        }

        /// <summary>
        /// Get Credit Limit Client
        /// </summary>
        /// <returns></returns>
        public static CustomerCreditLimitResponse GetCustomerCreditLimit()
        {
            return new CustomerCreditLimitResponse
            {
                CreditLimit = 500000,
                AvailableCreditLimit = 303914,
                ValidatedMail = true,
                NewCreditButtonEnabled = false,
                IsAvailableCreditLimit = true
            };
        }

        /// <summary>
        /// Get Create Credit Request
        /// </summary>
        /// <returns></returns>
        public static CreateCreditRequest GetCreateCreditRequest()
        {
            return new CreateCreditRequest
            {
                TypeDocument = "CE",
                IdDocument = "1037610201",
                CreditValue = 250000,
                Frequency = 30,
                Source = SourceHelperTest.GetCredinetSource().Id,
                Token = "11111",
                StoreId = "5ce5a1ae195f4428203a1d9e",
                UserId = "12a3sd",
                UserName = "NombreDelUsuario",
                Seller = "as2d132",
                Products = "Camisetas",
                Invoice = "89665",
                AuthMethod = 1,
                Location = "asd123",
                Fees = 6,
                CreditRiskLevel = "1"
            };
        }

        /// <summary>
        /// Get Create Credit Request
        /// </summary>
        /// <returns></returns>
        public static CreateCreditRequest GetCreateCreditRequestSourcePaymentGateways()
        {
            return new CreateCreditRequest
            {
                TypeDocument = "CE",
                IdDocument = "1037610201",
                CreditValue = 250000.55M,
                Frequency = 30,
                Source = SourceHelperTest.GetCredinetSourcePaymentGateways().Id,
                Token = "11111",
                StoreId = "5ce5a1ae195f4428203a1d9e",
                UserId = "12a3sd",
                UserName = "NombreDelUsuario",
                Seller = "as2d132",
                Products = "Camisetas",
                Invoice = "89665",
                AuthMethod = 1,
                Location = "asd123",
                Fees = 6,
                CreditRiskLevel = "1"
            };
        }

        /// <summary>
        /// Get Create Credit Response
        /// </summary>
        /// <returns></returns>
        public static CreateCreditResponse GetCreateCreditResponse()
        {
            return new CreateCreditResponse
            {
                TypeDocument = "CE",
                IdDocument = "1004",
                CreditValue = 250000,
                DownPayment = 25000,
                TotalFeeValue = 1,
                Fees = 1,
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
                CreditId = Guid.NewGuid(),
                CreditNumber = 1,
                EffectiveAnnualRate = 1,
                AssuranceValue = 200,
                AlternatePayment = false,
                AssuranceTotalFeeValue = 1,
                DownPaymentId = Guid.NewGuid()
            };
        }

        /// <summary>
        /// Get Create Credit Response
        /// </summary>
        /// <returns></returns>
        public static CreateCreditResponse GetCreateCreditResponseWithAlternatePaymentTrue()
        {
            return new CreateCreditResponse
            {
                TypeDocument = "CE",
                IdDocument = "1004",
                CreditValue = 250000,
                DownPayment = 25000,
                TotalFeeValue = 1,
                Fees = 1,
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
                CreditId = Guid.NewGuid(),
                CreditNumber = 1,
                EffectiveAnnualRate = 1,
                AssuranceValue = 200,
                AlternatePayment = true,
                AssuranceTotalFeeValue = 1,
                DownPaymentId = Guid.NewGuid()
            };
        }

        /// <summary>
        /// Get Create Credit Event
        /// </summary>
        /// <returns></returns>
        public static CreditEvent GetCreateCreditEvent()
        {
            return new CreditEvent
            {
                AlternatePayment = true,
                ArrearsCharge = 1,
                ArrearsDays = 1,
                AssuranceBalance = 1,
                AssuranceFee = 1,
                AssurancePercentage = 1,
                AssuranceTotalValue = 1,
                AssuranceValue = 1,
                AuthMethodId = 1,
                AuthMethodName = "",
                Balance = 1,
                ChargeValue = 1,
                ComputedArrearsDays = 1,
                CreateDate = DateTime.Now,
                CreateTime = TimeSpan.FromSeconds(3600),
                CreditMasterId = Guid.NewGuid(),
                CreditNumber = 1,
                CreditValue = 1,
                CustomerId = Guid.NewGuid(),
                Fees = 1,
                FeeValue = 1,
                Frequency = 1,
                InterestRate = 1,
                LastFee = 1,
                Location = "",
                Products = "",
                SourceId = 1,
                SourceName = "",
                StatusId = 1,
                StatusName = "",
                StoreId = "",
                TransactionDate = DateTime.Now,
                TransactionTime = TimeSpan.FromSeconds(3600),
                UpdatedPaymentPlanBalance = 1,
                UpdatedPaymentPlanValue = 1,
                UserId = "",
                ArrearsValuePaid = 1,
                AssuranceValuePaid = 1,
                BankAccount = "",
                CalculationDate = DateTime.Now,
                CertifiedId = "",
                CertifyingAuthority = "",
                ChargeValuePaid = 1,
                CreditValuePaid = 1,
                CustomerDocumentId = "",
                CustomerDocumentType = "",
                DueDate = DateTime.Now,
                EffectiveAnnualRate = 1,
                Fee = 1,
                InterestValuePaid = 1,
                Invoice = "",
                LastPaymentDate = DateTime.Now,
                LastPaymentTime = TimeSpan.FromSeconds(3600),
                PaymentTypeId = 1,
                PaymentTypeName = "",
                Reason = "",
                ScCode = "",
                Seller = "",
                TotalValuePaid = 1,
                TransactionTypeId = 1,
                TransactionTypeName = ""
            };
        }

        public static AddPaymentMasterEvent GetAddPaymentMasterEvent(CreditMaster creditMaster, TransactionType transactionType, Store store,
            DateTime dueDate, DateTime calculationDate, int lastFee, int creditStatusId, DateTime lastPaymentDate)
        {
            return new AddPaymentMasterEvent(creditMaster, creditMaster.Current.CreditPayment.GetPaymentNumber + 1, transactionType,
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), StatusHelperTest.GetStatusByType((Statuses)creditStatusId),
                    store, interestRate: 1,  1000M)
                .AddPaymentValues(totalValuePaid: creditMaster.Current.GetFeeValue + creditMaster.Current.GetAssuranceTotalFeeValue,
                    creditValuePaid: creditMaster.Current.GetFeeValue, interestValuePaid: 0, chargeValuePaid: 0, arrearsValuePaid: 0,
                    assuranceValuePaid: creditMaster.Current.GetAssuranceTotalFeeValue, lastFee: lastFee)
                .SetDates(dueDate, calculationDate, lastPaymentDate)
                .SetAdditionalInfo(bankAccount: string.Empty, location: "1,1", new UserInfo(userName: "TestUser", userId: "TestUserId"))
                .SetArrears(1);
        }

        public static AmortizationScheduleResponse GetOriginalAmortizationSchedule()
        {
            decimal feeValue = 228842.86M;
            decimal assuranceFeeValue = 24663.64M;
            decimal assuranceTaxValue = 4686.09M;

            decimal assurancePaymentValue = assuranceFeeValue + assuranceTaxValue;

            decimal totalFeeValue = feeValue + assurancePaymentValue;

            return new AmortizationScheduleResponse
            {
                CreditValue = 2713000,
                DownPayment = 542600,
                AssuranceValue = 271300,
                AmortizationScheduleFees = new List<AmortizationScheduleFee>()
                {
                    new AmortizationScheduleFee { Fee = 0, FeeDate = new DateTime(2019, 11, 23), Balance = 2713000M, FeeValue = 0M,
                        InterestValue = 0M, CreditValuePayment = 542600M, FinalBalance =  2170400M, TotalFeeValue = totalFeeValue },
                    new AmortizationScheduleFee { Fee = 1, FeeDate = new DateTime(2019, 12, 7), Balance = 2170400M, FeeValue = feeValue,
                        InterestValue = 211512.07M, CreditValuePayment = 207690.79M, FinalBalance =  2170400M, TotalFeeValue = totalFeeValue },
                    new AmortizationScheduleFee { Fee = 2, FeeDate = new DateTime(2019, 12, 21), Balance = 1962709.21M, FeeValue = feeValue,
                        InterestValue = 19127.98M, CreditValuePayment = 209714.88M, FinalBalance =  1962709.21M, TotalFeeValue = totalFeeValue },
                    new AmortizationScheduleFee { Fee = 3, FeeDate = new DateTime(2020, 01, 4), Balance = 1752994.33M, FeeValue = feeValue,
                        InterestValue = 17084.16M, CreditValuePayment = 211758.70M, FinalBalance =  1752994.33M, TotalFeeValue = totalFeeValue },
                    new AmortizationScheduleFee { Fee = 4, FeeDate = new DateTime(2020, 01, 18), Balance = 1541235.63M, FeeValue = feeValue,
                        InterestValue = 15020.42M, CreditValuePayment = 213822.44M, FinalBalance =  1327413.19M, TotalFeeValue = totalFeeValue },
                    new AmortizationScheduleFee { Fee = 5, FeeDate = new DateTime(2020, 02, 1), Balance = 1327413.19M, FeeValue = feeValue,
                        InterestValue = 12936.57M, CreditValuePayment = 215906.29M, FinalBalance =  1111506.90M, TotalFeeValue = totalFeeValue },
                    new AmortizationScheduleFee { Fee = 6, FeeDate = new DateTime(2020, 02, 15), Balance = 1111506.90M, FeeValue = feeValue,
                        InterestValue = 10832.41M, CreditValuePayment = 218010.45M, FinalBalance =  893496.45M, TotalFeeValue = totalFeeValue },
                    new AmortizationScheduleFee { Fee = 7, FeeDate = new DateTime(2020, 02, 29), Balance = 893496.45M, FeeValue = feeValue,
                        InterestValue = 8707.75M, CreditValuePayment = 220135.11M, FinalBalance =  673361.34M, TotalFeeValue = totalFeeValue },
                    new AmortizationScheduleFee { Fee = 8, FeeDate = new DateTime(2020, 03, 14), Balance = 673361.34M, FeeValue = feeValue,
                        InterestValue = 6562.38M, CreditValuePayment = 222280.48M, FinalBalance =  451080.86M, TotalFeeValue = totalFeeValue },
                    new AmortizationScheduleFee { Fee = 9, FeeDate = new DateTime(2020, 03, 28), Balance = 451080.86M, FeeValue = feeValue,
                        InterestValue = 4396.10M, CreditValuePayment = 224446.76M, FinalBalance =  226634.10M, TotalFeeValue = totalFeeValue },
                    new AmortizationScheduleFee { Fee = 10, FeeDate = new DateTime(2020, 04, 11), Balance = 2713000M, FeeValue = 228842.81M,
                        InterestValue = 2208.71M, CreditValuePayment = 226634.10M, FinalBalance =  0M, TotalFeeValue = 228842.81M + 29349.73M }
                },
                AmortizationScheduleAssuranceFees = new List<AmortizationScheduleAssuranceFee>()
                {
                    new AmortizationScheduleAssuranceFee {
                        Fee = 0, FeeDate = new DateTime(2019, 11, 23), Balance = 2713000M, AssuranceFeeValue = assuranceFeeValue, AssuranceTaxValue = 4686.09M, AssurancePaymentValue = 29349.73M, FinalBalance =  246636.36M },
                    new AmortizationScheduleAssuranceFee {
                        Fee = 1, FeeDate = new DateTime(2019, 12, 7), Balance = 246636.36M, AssuranceFeeValue = assuranceFeeValue, AssuranceTaxValue = assuranceTaxValue, AssurancePaymentValue = assurancePaymentValue, FinalBalance =  221972.73M },
                    new AmortizationScheduleAssuranceFee {
                        Fee = 2, FeeDate = new DateTime(2019, 12, 21), Balance = 221972.73M, AssuranceFeeValue = assuranceFeeValue, AssuranceTaxValue = assuranceTaxValue, AssurancePaymentValue = assurancePaymentValue, FinalBalance =  197309.09M },
                    new AmortizationScheduleAssuranceFee {
                        Fee = 3, FeeDate = new DateTime(2020, 01, 4), Balance = 197309.09M, AssuranceFeeValue = assuranceFeeValue, AssuranceTaxValue = assuranceTaxValue, AssurancePaymentValue = assurancePaymentValue, FinalBalance =  172645.45M },
                    new AmortizationScheduleAssuranceFee {
                        Fee = 4, FeeDate = new DateTime(2020, 01, 18), Balance = 172645.45M, AssuranceFeeValue = assuranceFeeValue, AssuranceTaxValue = assuranceTaxValue, AssurancePaymentValue = assurancePaymentValue, FinalBalance =  147981.82M },
                    new AmortizationScheduleAssuranceFee {
                        Fee = 5, FeeDate = new DateTime(2020, 02, 1), Balance = 147981.82M, AssuranceFeeValue = assuranceFeeValue, AssuranceTaxValue = assuranceTaxValue, AssurancePaymentValue = assurancePaymentValue, FinalBalance =  123318.18M },
                    new AmortizationScheduleAssuranceFee {
                        Fee = 6, FeeDate = new DateTime(2020, 02, 15), Balance = 123318.18M, AssuranceFeeValue = assuranceFeeValue, AssuranceTaxValue = assuranceTaxValue, AssurancePaymentValue = assurancePaymentValue, FinalBalance =  98654.55M },
                    new AmortizationScheduleAssuranceFee {
                        Fee = 7, FeeDate = new DateTime(2020, 02, 29), Balance = 98654.55M, AssuranceFeeValue = assuranceFeeValue, AssuranceTaxValue = assuranceTaxValue, AssurancePaymentValue = assurancePaymentValue, FinalBalance =  73990.91M },
                    new AmortizationScheduleAssuranceFee {
                        Fee = 8, FeeDate = new DateTime(2020, 03, 14), Balance = 73990.91M, AssuranceFeeValue = assuranceFeeValue, AssuranceTaxValue = assuranceTaxValue, AssurancePaymentValue = assurancePaymentValue, FinalBalance =  49327.27M },
                    new AmortizationScheduleAssuranceFee {
                        Fee = 9, FeeDate = new DateTime(2020, 03, 28), Balance = 49327.27M, AssuranceFeeValue = assuranceFeeValue, AssuranceTaxValue = assuranceTaxValue, AssurancePaymentValue = assurancePaymentValue, FinalBalance =  24663.64M },
                    new AmortizationScheduleAssuranceFee {
                        Fee = 10, FeeDate = new DateTime(2020, 04, 11), Balance = 24663.64M, AssuranceFeeValue = 24663.64M, AssuranceTaxValue = 4686.09M, AssurancePaymentValue = 24663.64M + 4686.09M, FinalBalance =  0M }
                }
            };
        }
    }
}