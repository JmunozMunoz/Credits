using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The credit token call request entity
    /// </summary>
    public class CreditTokenCallRequest
    {
        /// <summary>
        /// Gets or sets the document's id
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Gets or sets the document's type
        /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Gets or sets the additional data
        /// </summary>
        public string AdditionalData { get; set; }

        /// <summary>
        /// Gets or sets the mobile number
        /// </summary>
        public string Mobile { get; private set; }

        /// <summary>
        /// Gets or sets the customer id
        /// </summary>
        public Guid CustomerId { get; private set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Set mobile
        /// </summary>
        /// <param name="mobile"></param>
        public void SetMobile(string mobile)
        {
            Mobile = mobile;
        }

        /// <summary>
        /// Set customer id
        /// </summary>
        /// <param name="customerId"></param>
        public void SetCustomerId(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}