using System;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Refinancings
{
    /// <summary>
    /// Customer credits response
    /// </summary>
    public class CustomerCreditsResponse
    {
        /// <summary>
        /// Customer full name
        /// </summary>
        public string CustomerFullName { get; set; }

        /// <summary>
        /// Customer First name
        /// </summary>
        public string CustomerFirstName { get; set; }

        /// <summary>
        /// Customer second name
        /// </summary>
        public string CustomerSecondName { get; set; }

        /// <summary>
        /// Customer email
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Customer mobile
        /// </summary>
        public string CutomerMobile { get; set; }

        /// <summary>
        /// Details
        /// </summary>
        public List<CustomerCreditDetailResponse> Details { get; set; }
    }

    /// <summary>
    /// Customer credits details response
    /// </summary>
    public class CustomerCreditDetailResponse
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Store name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Arrears days
        /// </summary>
        public int ArrearsDays { get; set; }

        /// <summary>
        /// Total value paid
        /// </summary>
        public decimal TotalValuePaid { get; set; }
    }
}