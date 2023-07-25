using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The cancel credit detail response entity
    /// </summary>
    public class CancelCreditDetailResponsePaged
    {
        /// <summary>
        /// Total records of Cancel Credits request
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// CreditDetailResponses list
        /// </summary>
        public List<CancelCreditDetailResponse> CreditDetailResponses { get; set; }
    }
}