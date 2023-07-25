using System;

namespace Sc.Credits.Domain.UseCase.Credits.Strategy.NextFee
{
    /// <summary>
    /// Ordinary next fee strategy
    /// </summary>
    public class OrdinaryNextFeeStrategy
        : NextFeeStrategy
    {
        private readonly int _frequency;

        /// <summary>
        /// Creates a new instance of
        /// </summary>
        /// <param name="fee"></param>
        /// <param name="frequency"></param>
        /// <param name="firstFeeDate"></param>
        public OrdinaryNextFeeStrategy(int fee, int frequency, DateTime firstFeeDate)
            : base(fee, firstFeeDate)
        {
            _frequency = frequency;
        }

        /// <summary>
        /// <see cref="NextFeeStrategy.GetNextDate"/>
        /// </summary>
        /// <returns></returns>
        public override DateTime GetNextDate()
        {
            return FirstFeeDate.AddDays(_frequency * Fee);
        }
    }
}