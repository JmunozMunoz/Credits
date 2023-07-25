using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using System;

namespace Sc.Credits.Helper.Test.Model
{
    /// <summary>
    /// The current amortization schedule helper test
    /// </summary>
    public static class CurrentAmortizationScheduleHelperTest
    {
        /// <summary>
        /// Get basic of data current amortization schedule request
        /// </summary>
        /// <returns></returns>
        private static CurrentAmortizationScheduleRequest GetCurrentAmortizationScheduleRequest_BasicData() =>
            new CurrentAmortizationScheduleRequest()
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
                AssuranceTotalFeeValue = 34835.0M
            };

        /// <summary>
        /// Get current amortization schedule request
        /// </summary>
        /// <returns></returns>
        public static CurrentAmortizationScheduleRequest GetCurrentAmortizationScheduleRequest()
        {
            var currentAmortization = GetCurrentAmortizationScheduleRequest_BasicData();

            currentAmortization.LastPaymentDate = new DateTime(2020, month: 10, day: 18);
            currentAmortization.Balance = 1660458.0M;
            currentAmortization.AssuranceBalance = 174171.0M;
            currentAmortization.HasArrearsCharge = false;
            currentAmortization.ArrearsCharges = 32000.0M;
            currentAmortization.CurrentEffectiveAnnualRate = 0.268242M;
            currentAmortization.ChargeValue = 0.0M;
            currentAmortization.PreviousInterest = 0.0M;
            currentAmortization.PreviousArrears = 0.0M;

            return currentAmortization;
        }

        /// <summary>
        /// Get current amortization schedule with arrears
        /// </summary>
        /// <returns></returns>
        public static CurrentAmortizationScheduleRequest GetCurrentAmortizationSchedule_WithArrears()
        {
            var currentAmortization = GetCurrentAmortizationScheduleRequest_BasicData();

            currentAmortization.LastPaymentDate = new DateTime(2020, month: 10, day: 18);
            currentAmortization.Balance = 1660458.0M;
            currentAmortization.AssuranceBalance = 174171.0M;
            currentAmortization.HasArrearsCharge = true;
            currentAmortization.ArrearsCharges = 32000.0M;
            currentAmortization.CurrentEffectiveAnnualRate = 0.268242M;
            currentAmortization.ChargeValue = 20000.0M;
            currentAmortization.PreviousInterest = 0.0M;
            currentAmortization.PreviousArrears = 0.0M;

            return currentAmortization;
        }

        /// <summary>
        /// Get current amortization schedule with previous interest and previous arrears
        /// </summary>
        /// <returns></returns>
        public static CurrentAmortizationScheduleRequest GetCurrentAmortizationSchedule_PreviousInterest_PreviousArrears()
        {
            var currentAmortization = GetCurrentAmortizationScheduleRequest_BasicData();

            currentAmortization.LastPaymentDate = new DateTime(2020, month: 10, day: 18);
            currentAmortization.Balance = 1660458.0M;
            currentAmortization.AssuranceBalance = 174171.0M;
            currentAmortization.HasArrearsCharge = false;
            currentAmortization.ArrearsCharges = 0.0M;
            currentAmortization.CurrentEffectiveAnnualRate = 0.268242M;
            currentAmortization.ChargeValue = 0.0M;
            currentAmortization.PreviousInterest = 90000.0M;
            currentAmortization.PreviousArrears = 18000.0M;

            return currentAmortization;
        }

        /// <summary>
        /// Get current amortization shcedule with just last fee pending
        /// </summary>
        /// <returns></returns>
        public static CurrentAmortizationScheduleRequest GetCurrentAmortizationSchedule_JustLastFeePending_Request()
        {
            var currentAmortization = GetCurrentAmortizationScheduleRequest_BasicData();

            currentAmortization.LastPaymentDate = new DateTime(2021, month: 03, day: 3);
            currentAmortization.Balance = 308595.0M;
            currentAmortization.AssuranceBalance = 34831.0M;
            currentAmortization.HasArrearsCharge = false;
            currentAmortization.ArrearsCharges = 0.0M;
            currentAmortization.CurrentEffectiveAnnualRate = 0.283243M;
            currentAmortization.ChargeValue = 0.0M;
            currentAmortization.PreviousInterest = 0.0M;
            currentAmortization.PreviousArrears = 0.0M;

            return currentAmortization;
        }

        /// <summary>
        /// Get current amortization schedule with only first fee paid
        /// </summary>
        /// <returns></returns>
        public static CurrentAmortizationScheduleRequest GetCurrentAmortizationSchedule_FirstFeePay_Request()
        {
            var currentAmortization = GetCurrentAmortizationScheduleRequest_BasicData();

            currentAmortization.LastPaymentDate = new DateTime(2020, month: 10, day: 13);
            currentAmortization.Balance = 3235355.0M;
            currentAmortization.AssuranceBalance = 383181.0M;
            currentAmortization.HasArrearsCharge = false;
            currentAmortization.ArrearsCharges = 0.0M;
            currentAmortization.CurrentEffectiveAnnualRate = 0.283243M;
            currentAmortization.ChargeValue = 0.0M;
            currentAmortization.PreviousInterest = 0.0M;
            currentAmortization.PreviousArrears = 0.0M;

            return currentAmortization;
        }
    }
}