using System;

namespace Sc.Credits.Domain.UseCase.Credits.Strategy.NextFee
{
    /// <summary>
    /// Monthly next fee strategy
    /// </summary>
    public class MonthlyNextFeeStrategy
        : NextFeeStrategy
    {
        /// <summary>
        /// Creates a new instance of <see cref="MonthlyNextFeeStrategy"/>
        /// </summary>
        /// <param name="fee"></param>
        /// <param name="firstFeeDate"></param>
        public MonthlyNextFeeStrategy(int fee, DateTime firstFeeDate)
            : base(fee, firstFeeDate)
        {
        }

        /// <summary>
        /// <see cref="NextFeeStrategy.GetNextDate"/>
        /// </summary>
        /// <returns></returns>
        public override DateTime GetNextDate()
        {
            return FirstFeeDate.AddMonths(Fee);
        }
    }
}