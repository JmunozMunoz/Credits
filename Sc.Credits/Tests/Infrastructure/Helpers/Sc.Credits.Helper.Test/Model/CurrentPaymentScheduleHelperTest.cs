using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using System;

namespace Sc.Credits.Helper.Test.Model
{
    /// <summary>
    /// The current payment schedule helper test
    /// </summary>
    public static class CurrentPaymentScheduleHelperTest
    {
        /// <summary>
        /// Get basic of data current payment schedule request
        /// </summary>
        /// <returns></returns>
        private static CurrentPaymentScheduleRequest GetCurrentPaymentScheduleRequest_BasicData() =>
            new CurrentPaymentScheduleRequest()
            {
                CreditValue = 3512720.0M,
                InitialDate = new DateTime(2020, month: 09, day: 30),
                FeeValue = 311600.0M,
                InterestRate = 0.009746M,
                Frequency = (int)Frequencies.Biweekly,
                Fees = 12,
                DownPayment = 0.0M,
                AssuranceValue = 351272.0M,
                AssuranceFeeValue = 29273.0M,
                AssuranceTotalFeeValue = 34835.0M,
                UpdatedPaymentPlanValue = 0.0M
            };

        /// <summary>
        /// Get current payment schedule request
        /// </summary>
        /// <returns></returns>
        public static CurrentPaymentScheduleRequest GetCurrentPaymentScheduleRequest()
        {
            var currentPayment = GetCurrentPaymentScheduleRequest_BasicData();

            currentPayment.LastPaymentDate = new DateTime(2020, month: 10, day: 18);
            currentPayment.Balance = 1660458.0M;
            currentPayment.AssuranceBalance = 174171.0M;
            currentPayment.HasArrearsCharge = false;
            currentPayment.ArrearsCharges = 32000.0M;
            currentPayment.CurrentEffectiveAnnualRate = 0.268242M;
            currentPayment.ChargeValue = 0.0M;
            currentPayment.PreviousInterest = 0.0M;
            currentPayment.PreviousArrears = 0.0M;

            return currentPayment;
        }

        /// <summary>
        /// Get current payment schedule with arrears
        /// </summary>
        /// <returns></returns>
        public static CurrentPaymentScheduleRequest GetCurrentPaymentSchedule_WithArrears()
        {
            var currentPayment = GetCurrentPaymentScheduleRequest_BasicData();

            currentPayment.LastPaymentDate = new DateTime(2020, month: 10, day: 18);
            currentPayment.Balance = 1660458.0M;
            currentPayment.AssuranceBalance = 174171.0M;
            currentPayment.HasArrearsCharge = true;
            currentPayment.ArrearsCharges = 32000.0M;
            currentPayment.CurrentEffectiveAnnualRate = 0.268242M;
            currentPayment.ChargeValue = 20000.0M;
            currentPayment.PreviousInterest = 0.0M;
            currentPayment.PreviousArrears = 0.0M;

            return currentPayment;
        }

        /// <summary>
        /// Get current payment schedule with arrears
        /// </summary>
        /// <returns></returns>
        public static CurrentPaymentScheduleRequest GetCurrentPaymentSchedule_WithCharges()
        {
            var currentPayment = GetCurrentPaymentScheduleRequest_BasicData();

            currentPayment.LastPaymentDate = new DateTime(2020, month: 10, day: 18);
            currentPayment.Balance = 1660458.0M;
            currentPayment.AssuranceBalance = 174171.0M;
            currentPayment.HasArrearsCharge = false;
            currentPayment.ArrearsCharges = 30000;
            currentPayment.CurrentEffectiveAnnualRate = 0.268242M;
            currentPayment.ChargeValue = 20000.0M;
            currentPayment.PreviousInterest = 0.0M;
            currentPayment.PreviousArrears = 0.0M;

            return currentPayment;
        }

        /// <summary>
        /// Get current payment schedule with previous interest and previous arrears
        /// </summary>
        /// <returns></returns>
        public static CurrentPaymentScheduleRequest GetCurrentPaymentSchedule_PreviousInterest_PreviousArrears()
        {
            var currentPayment = GetCurrentPaymentScheduleRequest_BasicData();

            currentPayment.LastPaymentDate = new DateTime(2020, month: 10, day: 18);
            currentPayment.Balance = 1660458.0M;
            currentPayment.AssuranceBalance = 174171.0M;
            currentPayment.HasArrearsCharge = false;
            currentPayment.ArrearsCharges = 0.0M;
            currentPayment.CurrentEffectiveAnnualRate = 0.268242M;
            currentPayment.ChargeValue = 0.0M;
            currentPayment.PreviousInterest = 90000.0M;
            currentPayment.PreviousArrears = 18000.0M;

            return currentPayment;
        }

        /// <summary>
        /// Get current payment shcedule with just last fee pending
        /// </summary>
        /// <returns></returns>
        public static CurrentPaymentScheduleRequest GetCurrentPaymentSchedule_JustLastFeePending_Request()
        {
            var currentPayment = GetCurrentPaymentScheduleRequest_BasicData();

            currentPayment.LastPaymentDate = new DateTime(2021, month: 03, day: 3);
            currentPayment.Balance = 308595.0M;
            currentPayment.AssuranceBalance = 34831.0M;
            currentPayment.HasArrearsCharge = false;
            currentPayment.ArrearsCharges = 0.0M;
            currentPayment.CurrentEffectiveAnnualRate = 0.283243M;
            currentPayment.ChargeValue = 0.0M;
            currentPayment.PreviousInterest = 0.0M;
            currentPayment.PreviousArrears = 0.0M;

            return currentPayment;
        }

        /// <summary>
        /// Get current payment schedule with only first fee paid
        /// </summary>
        /// <returns></returns>
        public static CurrentPaymentScheduleRequest GetCurrentPaymentSchedule_FirstFeePay_Request()
        {
            var currentPayment = GetCurrentPaymentScheduleRequest_BasicData();

            currentPayment.LastPaymentDate = new DateTime(2020, month: 10, day: 13);
            currentPayment.Balance = 3235355.0M;
            currentPayment.AssuranceBalance = 383181.0M;
            currentPayment.HasArrearsCharge = false;
            currentPayment.ArrearsCharges = 0.0M;
            currentPayment.CurrentEffectiveAnnualRate = 0.283243M;
            currentPayment.ChargeValue = 0.0M;
            currentPayment.PreviousInterest = 0.0M;
            currentPayment.PreviousArrears = 0.0M;

            return currentPayment;
        }

        /// <summary>
        /// Get current payment schedule with pending fees
        /// </summary>
        /// <returns></returns>
        public static CurrentPaymentScheduleRequest GetCurrentPaymentSchedule_PendingFeesQuantity()
        {
            var currentPayment = GetCurrentPaymentScheduleRequest_BasicData();

            currentPayment.LastPaymentDate = new DateTime(2020, month: 10, day: 13);
            currentPayment.Balance = 1513466.0M;
            currentPayment.AssuranceBalance = 174171.0M;
            currentPayment.HasArrearsCharge = false;
            currentPayment.ArrearsCharges = 0.0M;
            currentPayment.CurrentEffectiveAnnualRate = 0.283243M;
            currentPayment.ChargeValue = 0.0M;
            currentPayment.PreviousInterest = 0.0M;
            currentPayment.PreviousArrears = 0.0M;

            return currentPayment;
        }

        /// <summary>
        /// Get current payment schedule with calculaiton date before last payment date
        /// </summary>
        /// <returns></returns>
        public static CurrentPaymentScheduleRequest GetCurrentPaymentSchedule_calculationDate_Before_LastPayment()
        {
            var currentPayment = GetCurrentPaymentScheduleRequest_BasicData();

            currentPayment.LastPaymentDate = new DateTime(2021, month: 02, day: 15);
            currentPayment.Balance = 916873.0M;
            currentPayment.AssuranceBalance = 104501.0M;
            currentPayment.HasArrearsCharge = false;
            currentPayment.ArrearsCharges = 0.0M;
            currentPayment.CurrentEffectiveAnnualRate = 0.283243M;
            currentPayment.ChargeValue = 0.0M;
            currentPayment.PreviousInterest = 0.0M;
            currentPayment.PreviousArrears = 0.0M;

            return currentPayment;
        }

        /// <summary>
        /// Get current payment schedule for calculationdate after lastFee date test
        /// </summary>
        /// <returns></returns>
        public static CurrentPaymentScheduleRequest GetCurrentPaymentSchedule_calculationDate_After_lastFeeDate()
        {
            var currentPayment = GetCurrentPaymentScheduleRequest_BasicData();

            currentPayment.LastPaymentDate = new DateTime(2020, month: 10, day: 13);
            currentPayment.Balance = 1513466.0M;
            currentPayment.AssuranceBalance = 174171.0M;
            currentPayment.HasArrearsCharge = false;
            currentPayment.ArrearsCharges = 0.0M;
            currentPayment.CurrentEffectiveAnnualRate = 0.283243M;
            currentPayment.ChargeValue = 0.0M;
            currentPayment.PreviousInterest = 0.0M;
            currentPayment.PreviousArrears = 0.0M;

            return currentPayment;
        }

        /// <summary>
        /// Get current payment schedule for first and last fee test
        /// </summary>
        /// <returns></returns>
        public static CurrentPaymentScheduleRequest GetCurrentPaymentSchedule_Payments_ShouldBe_FirstAndLastFee()
        {
            var currentPayment = GetCurrentPaymentScheduleRequest_BasicData();

            currentPayment.LastPaymentDate = new DateTime(2021, month: 02, day: 15);
            currentPayment.Balance = 916873.0M;
            currentPayment.AssuranceBalance = 104501.0M;
            currentPayment.HasArrearsCharge = false;
            currentPayment.ArrearsCharges = 0.0M;
            currentPayment.CurrentEffectiveAnnualRate = 0.283243M;
            currentPayment.ChargeValue = 0.0M;
            currentPayment.PreviousInterest = 0.0M;
            currentPayment.PreviousArrears = 0.0M;

            return currentPayment;
        }

        /// <summary>
        /// Get current payment schedule for minimun payment equal first fee
        /// </summary>
        /// <returns></returns>
        public static CurrentPaymentScheduleRequest GetCurrentPaymentSchedule_Minimun_Payment_With_Arrear_ShouldBe_FirstFee()
        {
            var currentPayment = GetCurrentPaymentScheduleRequest_BasicData();

            currentPayment.LastPaymentDate = new DateTime(2020, month: 09, day: 30);
            currentPayment.Balance = 2955287.0M;
            currentPayment.AssuranceBalance = 348346.0M;
            currentPayment.HasArrearsCharge = false;
            currentPayment.ArrearsCharges = 0.0M;
            currentPayment.CurrentEffectiveAnnualRate = 0.283243M;
            currentPayment.ChargeValue = 0.0M;
            currentPayment.PreviousInterest = 0.0M;
            currentPayment.PreviousArrears = 0.0M;

            return currentPayment;
        }
    }
}