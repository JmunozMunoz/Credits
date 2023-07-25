using System;

namespace Sc.Credits.Domain.UseCase.Credits.Strategy.NextFee
{
    /// <summary>
    /// Next fee strategy
    /// </summary>
    public abstract class NextFeeStrategy
    {
        /// <summary>
        /// Gets the fee
        /// </summary>
        protected int Fee { get; private set; }

        /// <summary>
        /// Gets the first fee date
        /// </summary>
        protected DateTime FirstFeeDate { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="NextFeeStrategy"/>
        /// </summary>
        /// <param name="fee"></param>
        /// <param name="firstFeeDate"></param>
        protected NextFeeStrategy(int fee, DateTime firstFeeDate)
        {
            Fee = fee;
            FirstFeeDate = firstFeeDate;
        }

        /// <summary>
        /// Get next date
        /// </summary>
        /// <returns></returns>
        public abstract DateTime GetNextDate();
    }
}