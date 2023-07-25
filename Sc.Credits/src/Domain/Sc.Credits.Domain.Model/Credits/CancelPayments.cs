﻿using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The cancel payments entity
    /// </summary>
    public class CancelPayments
    {
        /// <summary>
        /// Map into field Id in MasterCredits entity and creditMasterId in Credits entity.
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Gets or sets the user's id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string UserName { get; set; }
    }
}