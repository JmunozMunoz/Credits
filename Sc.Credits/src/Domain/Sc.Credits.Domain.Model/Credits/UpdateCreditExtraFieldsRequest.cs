using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The update credit extra fields request
    /// </summary>
    public class UpdateCreditExtraFieldsRequest
    {
        /// <summary>
        /// Gets or sets the credit's id
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Gets or sets the seller
        /// </summary>
        public string Seller { get; set; }

        /// <summary>
        /// Gets or sets the products
        /// </summary>
        public string Products { get; set; }

        /// <summary>
        /// Gets or sets the invoice
        /// </summary>
        public string Invoice { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="UpdateCreditExtraFieldsRequest"/>
        /// </summary>
        public UpdateCreditExtraFieldsRequest()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="UpdateCreditExtraFieldsRequest"/>
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="seller"></param>
        /// <param name="products"></param>
        /// <param name="invoice"></param>
        public UpdateCreditExtraFieldsRequest(Guid creditId, string seller, string products, string invoice)
        {
            CreditId = creditId;
            Seller = seller;
            Products = products;
            Invoice = invoice;
        }
    }
}