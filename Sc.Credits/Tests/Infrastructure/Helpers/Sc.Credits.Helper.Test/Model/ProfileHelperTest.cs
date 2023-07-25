namespace Sc.Credits.Helper.Test.Model
{
    using Domain.Model.Customers;
    using System.Collections.Generic;

    /// <summary>
    ///Profile helper test
    /// </summary>
    public static class ProfileHelperTest
    {
        /// <summary>
        /// Get profile
        /// </summary>
        /// <returns></returns>
        public static Profile GetProfile()
        {
            return new Profile("Ordinary", 2).SetId(1);
        }

        /// <summary>
        /// Get Custom Profile
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mandatoryDownPayment"></param>
        /// <returns></returns>
        public static Profile GetCustomProfile(string name, int mandatoryDownPayment)
        {
            return new Profile(name, mandatoryDownPayment).SetId(1);
        }

        /// <summary>
        /// Get profile list
        /// </summary>
        /// <returns></returns>
        public static List<Profile> GetProfileList()
        {
            return new List<Profile>
            {
                new Profile("Ordinary", 2).SetId(1),
                new Profile("Other type", 0).SetId(2),
                new Profile("Default", 0).SetId(default)
            };
        }
    }
}