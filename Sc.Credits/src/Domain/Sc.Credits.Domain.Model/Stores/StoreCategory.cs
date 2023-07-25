using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Helpers.Commons.Extensions;
using System;

namespace Sc.Credits.Domain.Model.Stores
{
    /// <summary>
    /// The store category type.
    /// </summary>
    public class StoreCategory : Entity<int>
    {
        private string _name;
        private int _regularFeesNumber;
        private int _maximumFeesNumber;
        private decimal _minimumFeeValue;
        private decimal _maximumCreditValue;
        private decimal _feeCutoffValue;

        /// <summary>
        /// Gets the minimum fee value field.
        /// </summary>
        public decimal GetMinimumFeeValue => _minimumFeeValue;

        /// <summary>
        /// Gets the maximum credit value field.
        /// </summary>
        public decimal GetMaximumCreditValue => _maximumCreditValue;

        /// <summary>
        /// Creates a new instance of <see cref="StoreCategory"/>
        /// </summary>
        public StoreCategory()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="StoreCategory"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="regularFeesNumber"></param>
        /// <param name="maximumFeesNumber"></param>
        /// <param name="minimumFeeValue"></param>
        /// <param name="maximumCreditValue"></param>
        /// <param name="feeCutoffValue"></param>
        public StoreCategory(string name, int regularFeesNumber, int maximumFeesNumber, decimal minimumFeeValue, decimal maximumCreditValue, decimal feeCutoffValue)
        {
            _name = name;
            _regularFeesNumber = regularFeesNumber;
            _maximumFeesNumber = maximumFeesNumber;
            _minimumFeeValue = minimumFeeValue;
            _maximumCreditValue = maximumCreditValue;
            _feeCutoffValue = feeCutoffValue;
        }

        /// <summary>
        /// Gets the time limit in months.
        /// </summary>
        /// <param name="creditValue"></param>
        /// <returns></returns>
        internal int GetTimeLimitInMonths(decimal creditValue)
        {
            int monthLimitByFeeCutoffValue = Convert.ToInt32(Math.Floor(creditValue / _feeCutoffValue));

            if (monthLimitByFeeCutoffValue > _regularFeesNumber)
            {
                return Math.Min(monthLimitByFeeCutoffValue, _maximumFeesNumber);
            }

            int monthLimitByMinimumFeeValue = Convert.ToInt32(Math.Floor(creditValue / _minimumFeeValue));

            return Math.Min(monthLimitByMinimumFeeValue, _regularFeesNumber);
        }

        /// <summary>
        /// Validates if a creditValue is in the specified range
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        internal void ValidateCreditValue(decimal creditValue, int decimalNumbersRound)
        {
            if (creditValue < GetMinimumFeeValue || creditValue > GetMaximumCreditValue)
            {
                throw new BusinessException(GetMinimumFeeValue.Round(decimalNumbersRound).ToString() + "-" + GetMaximumCreditValue.Round(decimalNumbersRound).ToString(),
                    (int)BusinessResponse.InvalidAmountCredit);
            }
        }
    }
}