using Sc.Credits.Domain.Model.Refinancings;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The create credit request entity
    /// </summary>
    public class CreateCreditRequest
    {
        /// <summary>
        /// Gets or sets the document's type
        /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Gets or sets the document's id
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Gets or sets the credit's value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the frequency
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the fees
        /// </summary>
        public int Fees { get; set; }

        /// <summary>
        /// Gets or sets the source
        /// </summary>
        public int Source { get; set; }

        /// <summary>
        /// Gets or sets the token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the user's id
        /// </summary>
        public string UserId { get; set; }

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
        /// Gets or sets the auth method
        /// </summary>
        public int AuthMethod { get; set; }

        /// <summary>
        /// Gets or sets the location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the user's name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the credit risk level
        /// </summary>
        public string CreditRiskLevel { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="CreateCreditRequest"/>
        /// </summary>
        /// <param name="refinancingCreditRequest"></param>
        /// <param name="refinancingStoreId"></param>
        /// <returns></returns>

        public static CreateCreditRequest FromRefinancing(RefinancingCreditRequest refinancingCreditRequest, string refinancingStoreId) =>
            new CreateCreditRequest
            {
                AuthMethod = refinancingCreditRequest.AuthMethod,
                Fees = refinancingCreditRequest.Fees,
                Frequency = (int)RefinancingParams.DefaultFrequency,
                IdDocument = refinancingCreditRequest.IdDocument,
                Location = refinancingCreditRequest.Location,
                StoreId = refinancingStoreId,
                Source = refinancingCreditRequest.Source,
                Token = refinancingCreditRequest.Token,
                TypeDocument = refinancingCreditRequest.DocumentType,
                UserId = refinancingCreditRequest.UserId,
                UserName = refinancingCreditRequest.UserName
            };
    }
}