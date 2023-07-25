using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Request cancel credit paged
    /// </summary>
    public class RequestCancelCreditPaged
    {
        /// <summary>
        /// Total records of Cancel Credits request
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// RequestCancelCredits list
        /// </summary>
        public List<RequestCancelCredit> RequestCancelCredit { get; set; }
    }
}