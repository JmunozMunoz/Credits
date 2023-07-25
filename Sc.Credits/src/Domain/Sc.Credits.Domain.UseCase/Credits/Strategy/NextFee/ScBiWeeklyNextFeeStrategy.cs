using System;

namespace Sc.Credits.Domain.UseCase.Credits.Strategy.NextFee
{
    /// <summary>
    /// legacy biweekly next fee strategy
    /// </summary>
    public class ScBiWeeklyNextFeeStrategy
        : NextFeeStrategy
    {
        private readonly DateTime? _previousFeeDate;

        /// <summary>
        /// Creates a new instance of <see cref="ScBiWeeklyNextFeeStrategy"/>
        /// </summary>
        /// <param name="fee"></param>
        /// <param name="firstFeeDate"></param>
        /// <param name="previousFeeDate"></param>
        public ScBiWeeklyNextFeeStrategy(int fee, DateTime firstFeeDate, DateTime? previousFeeDate)
            : base(fee, firstFeeDate)
        {
            _previousFeeDate = previousFeeDate;
        }

        /// <summary>
        /// <see cref="NextFeeStrategy.GetNextDate"/>
        /// </summary>
        /// <returns></returns>
        public override DateTime GetNextDate()
        {
            if (_previousFeeDate == null)
                return FirstFeeDate;

            DateTime previousFeeDate = (DateTime)_previousFeeDate;

            bool isDateOnFebruary = previousFeeDate.Month == 2;

            if (!isDateOnFebruary)
                return previousFeeDate.Day > 15
                  ? previousFeeDate.AddDays(-15).AddMonths(1)
                  : previousFeeDate.AddDays(15);

            int lastDayOfFebruary = DateTime.IsLeapYear(previousFeeDate.Year) ? 29 : 28;
            bool isFebruaryDayMidMonth = previousFeeDate.Day == 14 || previousFeeDate.Day == 15;

            if (isFebruaryDayMidMonth)
            {
                int daysRemainingEndOfMonth = lastDayOfFebruary - previousFeeDate.Day;
                return previousFeeDate.AddDays(daysRemainingEndOfMonth);
            }

            bool isFebruaryDayEndOfMonth = previousFeeDate.Day == lastDayOfFebruary;
            bool isInitialDateAlmostEndOfMonth = FirstFeeDate.Day == 29 || FirstFeeDate.Day == 30;

            if (isFebruaryDayEndOfMonth && isInitialDateAlmostEndOfMonth)
            {
                int offsetDays = FirstFeeDate.Day - lastDayOfFebruary;
                return previousFeeDate.AddMonths(1).AddDays(-15 + offsetDays);
            }

            return previousFeeDate.Day > 15
              ? previousFeeDate.AddMonths(1).AddDays(-15)
              : previousFeeDate.AddDays(15);
        }
    }
}