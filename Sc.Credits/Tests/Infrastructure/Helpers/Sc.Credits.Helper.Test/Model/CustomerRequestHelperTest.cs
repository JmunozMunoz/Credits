using Sc.Credits.Domain.Model.Customers;
using System;

namespace Sc.Credits.Helper.Test.Model
{
    /// <summary>
    /// Customer Request Helper Test
    /// </summary>
    public static class CustomerRequestHelperTest
    {
        /// <summary>
        /// Get Customer Request
        /// </summary>
        /// <returns></returns>
        public static CustomerRequest GetCustomerRequest()
        {
            return new CustomerRequest("1037610201", "CE", amount: 3600000.00M, amountAvailable: 500000.0M, 1, "JuanPerez@sistecredito.com",
                "304559888", 2, true, new Guid("55094b4d-5d5e-cd3b-435f-08d72284f7ae"), false, "Juan", "", "Perez", "Prueba");
        }

        /// <summary>
        /// Get Customer Request For Empty CustomerId
        /// </summary>
        /// <returns></returns>
        public static CustomerRequest GetCustomerRequestForEmptyCustomerId()
        {
            return new CustomerRequest("1033340573", "CE", amount: 500000.0M, amountAvailable: 500000.0M, 1, "juanPerez@sistecredito.com",
                "3205875958", 1, true, Guid.Empty, false, "Juan", "", "Perez", "");
        }

        /// <summary>
        /// Get Customer Request For Empty CustomerId
        /// </summary>
        /// <returns></returns>
        public static CustomerRequest GetCustomerRequestInvalidAmount()
        {
            return new CustomerRequest("1033340573", "CE", amount: 5000.0M, amountAvailable: 500.0M, 1, "juanPerez@sistecredito.com",
                "3205875958", 1, true, Guid.Empty, false, "Juan", "", "Perez", "");
        }

        /// <summary>
        /// Get Customer Request For Empty CustomerId
        /// </summary>
        /// <returns></returns>
        public static CustomerRequest GetCustomerRequestValidAmount()
        {
            return new CustomerRequest("1033340573", "CE", amount: 2000000.00M, amountAvailable: 1500000.0M, 1, "juanPerez@sistecredito.com",
                "3205875958", 1, true, Guid.Empty, false, "Juan", "", "Perez", "");
        }
    }
}