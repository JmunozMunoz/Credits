using Sc.Credits.Domain.Model.Customers;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.Dapper.Map
{
    /// <summary>
    /// Customer mapper
    /// </summary>
    public class CustomerMapper
        : Mapper
    {
        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static CustomerMapper New()
            => new CustomerMapper();

        /// <summary>
        /// New customer mapper
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        protected CustomerMapper()
        {
        }

        /// <summary>
        /// From dynamic query
        /// </summary>
        /// <param name="dynamicQuery"></param>
        /// <returns></returns>
        public Customer FromDynamicQuery(IEnumerable<dynamic> dynamicQuery)
        {
            Customer customer = null;

            if (dynamicQuery is ICollection<object> rows && rows.Any())
            {
                customer = FromDynamicRow(rows.Single());
            }

            return customer;
        }

        /// <summary>
        /// From dynamic row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="loadTransaction"></param>
        /// <param name="loadCustomer"></param>
        /// <param name="loadStore"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="loadTransactionStore"></param>
        /// <returns></returns>
        public Customer FromDynamicRow(object row)
        {
            Customer customer = null;

            if (row is IDictionary<string, object> dictionary)
            {
                ICollection<IDictionary<string, object>> dictionaries = SplitDictionary(dictionary);

                customer = Customer.New();

                IEnumerator<IDictionary<string, object>> enumerator = dictionaries.GetEnumerator();

                enumerator.MoveNext();

                MapEntity(customer, enumerator);

                MapEntity(customer.Name, enumerator);
            }

            return customer;
        }
    }
}