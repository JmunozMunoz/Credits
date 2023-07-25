using Sc.Credits.Domain.Model.Locations;

namespace Sc.Credits.Domain.Model.Stores
{
    /// <summary>
    /// The store request entity
    /// </summary>
    public class StoreRequest
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collect type's id
        /// </summary>
        public int CollectTypeId { get; set; }

        /// <summary>
        /// Gets or sets the business group's id
        /// </summary>
        public string BusinessGroupId { get; set; }

        /// <summary>
        /// Gets or sets the business group code on legacy system
        /// </summary>
        public string ScCodeBusinessGroup { get; set; }

        /// <summary>
        /// Gets or sets the business group name
        /// </summary>
        public string BusinessGroupName { get; set; }

        /// <summary>
        /// Gets or sets the mandatory down payment
        /// </summary>
        public bool? MandatoryDownPayment { get; set; }

        /// <summary>
        /// Gets or sets the minimum fee
        /// </summary>
        public decimal MinimumFee { get; set; }

        /// <summary>
        /// Gets or sets the down payment percentage
        /// </summary>
        public decimal DownPaymentPercentage { get; set; }

        /// <summary>
        /// Gets or sets the assurance type
        /// </summary>
        public int AssuranceType { get; set; }

        /// <summary>
        /// Gets or sets the vendor's id
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// Gets or sets the assurance company's id
        /// </summary>
        public long? AssuranceCompanyId { get; set; }

        /// <summary>
        /// Gets or sets the payment type's id
        /// </summary>
        public int PaymentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the code on legacy system
        /// </summary>
        public string ScCode { get; set; }

        /// <summary>
        /// Gets or sets the phones
        /// </summary>
        public string[] Phones { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the store profile code.
        /// </summary>
        public int StoreProfileCode { get; set; }

        /// <summary>
        /// Gets or sets the Nit.
        /// </summary>
        public string Nit { get; set; }

        /// <summary>
        /// Gets or sets the state
        /// </summary>
        public StateRequest State { get; set; }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        public CityRequest City { get; set; }
    }
}