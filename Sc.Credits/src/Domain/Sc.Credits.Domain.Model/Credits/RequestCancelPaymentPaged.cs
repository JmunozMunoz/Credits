using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.Commons.Domain;
using System;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Request cancel payment paged
    /// </summary>
    public class RequestCancelPaymentPaged
    {
        /// <summary>
        /// Total records of Cancel Payment request
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// RequestCancelPayment list
        /// </summary>
        public List<RequestCancelPayment> RequestCancelPayment { get; set; }
    }
}