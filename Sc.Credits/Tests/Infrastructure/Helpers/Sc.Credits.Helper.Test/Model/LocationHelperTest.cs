namespace Sc.Credits.Helper.Test.Model
{
    using Sc.Credits.Domain.Model.Locations;
    using System.Collections.Generic;

    /// <summary>
    /// Location helper test
    /// </summary>
    public static class LocationHelperTest
    {
        /// <summary>
        /// Get state
        /// </summary>
        /// <returns></returns>
        public static State GetSate()
        {
            return new State("01", "Antioquia");
        }

        /// <summary>
        /// Get city
        /// </summary>
        /// <returns></returns>
        public static City GetCity()
        {
            return new City("0001", "Medellín", "01");
        }

        /// <summary>
        /// Get state request
        /// </summary>
        /// <returns></returns>
        public static StateRequest GetSateResquest()
        {
            return new StateRequest
            {
                Code = "01",
                Name = "Antioquia"
            };
        }

        /// <summary>
        /// Get state list
        /// </summary>
        /// <returns></returns>
        public static List<State> GetStateList()
        {
            return new List<State>
            {
                new State("01", "Antioquia"),
                new State("02", "Amazonas")
            };
        }

        /// <summary>
        /// Get city list
        /// </summary>
        /// <returns></returns>
        public static List<City> GetCityList()
        {
            return new List<City>
            {
                new City("001", "Medellín", "01"),
                new City("002", "Marinilla", "01")
            };
        }

        /// <summary>
        /// Get city request
        /// </summary>
        /// <returns></returns>
        public static CityRequest GetCityResquest()
        {
            return new CityRequest
            {
                Code = "001",
                Name = "Medellín"
            };
        }
    }
}