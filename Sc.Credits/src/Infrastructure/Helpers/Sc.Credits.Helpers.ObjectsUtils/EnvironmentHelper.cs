using System;

namespace Sc.Credits.Helpers.ObjectsUtils
{
    /// <summary>
    /// Environment helper
    /// </summary>
    public static class EnvironmentHelper
    {
        /// <summary>
        /// Get country or default
        /// </summary>
        /// <param name="defaultCountry"></param>
        /// <returns></returns>
        public static string GetCountryOrDefault(string defaultCountry)
        {
            const string CREDINET_COUNTRY = "CREDINET_COUNTRY";
            string country = GetVariable(CREDINET_COUNTRY);

            return string.IsNullOrEmpty(country) ? defaultCountry : country;
        }

        /// <summary>
        /// Get subDomain or default
        /// </summary>
        /// <param name="defaultSubDomain"></param>
        /// <returns></returns>
        public static string GetSubDomainOrDefault(string defaultSubDomain)
        {
            const string CREDINET_SUBDOMAIN = "CREDINET_SUBDOMAIN";
            string subDomain = GetVariable(CREDINET_SUBDOMAIN);

            return string.IsNullOrEmpty(subDomain) ? defaultSubDomain : subDomain;
        }

        /// <summary>
        /// Current environment
        /// </summary>
        /// <returns></returns>
        public static string CurrentEnvironment() =>
            GetVariable("ASPNETCORE_ENVIRONMENT");

        /// <summary>
        /// Get variable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}