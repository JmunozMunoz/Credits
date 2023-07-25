using System.Globalization;

namespace Sc.Credits.Helpers.ObjectsUtils
{
    /// <summary>
    /// Number format
    /// </summary>
    public static class NumberFormat
    {
        /// <summary>
        /// Format currency
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        public static string Currency(decimal value, int decimalNumbersRound)
        {
            return string.Format($"{{0:C{decimalNumbersRound}}}", value);
        }

        /// <summary>
        /// Format currency sms
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="smsCurrencyCode"></param>
        /// <returns></returns>
        public static string CurrencySms(decimal value, int decimalNumbersRound, string smsCurrencyCode)
        {
            return Currency(value, decimalNumbersRound).Replace(NumberFormatInfo.CurrentInfo.CurrencySymbol, smsCurrencyCode);
        }
    }
}