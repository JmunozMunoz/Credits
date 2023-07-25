using System;

namespace Sc.Credits.Helpers.ObjectsUtils
{
    /// <summary>
    /// Date time helper
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Number of months in one year
        /// </summary>
        public const int MonthsInAYear = 12;

        /// <summary>
        /// Latest date
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="date3"></param>
        /// <returns></returns>
        public static DateTime LatestDate(DateTime date1, DateTime date2, DateTime date3)
        {
            return LatestDate(LatestDate(date1, date2), date3);
        }

        /// <summary>
        /// Latest date
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dateToCompare"></param>
        /// <returns></returns>
        public static DateTime LatestDate(DateTime date, DateTime dateToCompare)
        {
            return date.CompareTo(dateToCompare) > 0 ? date : dateToCompare;
        }

        /// <summary>
        /// Earliest date
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dateToCompare"></param>
        /// <returns></returns>
        public static DateTime EarliestDate(DateTime date, DateTime dateToCompare)
        {
            return date.CompareTo(dateToCompare) < 0 ? date : dateToCompare;
        }

        /// <summary>
        /// Difference 360 between dates
        /// </summary>
        /// <param name="initialDate"></param>
        /// <param name="finalDate"></param>
        /// <returns></returns>
        public static int Difference360BetweenDates(DateTime initialDate, DateTime finalDate)
        {
            int monthDaysDifference = ((finalDate.Year * 12 + finalDate.Month) - (initialDate.Year * 12 + initialDate.Month)) * 30;
            int finalDateDay = finalDate.Day == 31 ? 30 : finalDate.Day;
            int initialDateDay = initialDate.Day == 31 ? 30 : initialDate.Day;
            int interestDays = monthDaysDifference + (finalDateDay - initialDateDay);

            return interestDays <= 0 ? 0 : interestDays;
        }
    }
}