using System;

namespace Sc.Credits.Helpers.Commons.Extensions
{
    /// <summary>
    /// Decimal extensions
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// Round
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal Round(this decimal value, int decimals = 0) => Math.Round(value, decimals);
    }
}