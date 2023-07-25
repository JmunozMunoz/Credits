using Newtonsoft.Json;
using Sc.Credits.Domain.Model.Refinancings;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The payment credit request
    /// </summary>
    public class PaymentCreditRequest
    {
        /// <summary>
        /// Map into field Id in MasterCredits entity and creditMasterId in Credits entity.
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Gets or sets the total value paid
        /// </summary>
        public decimal TotalValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the user's id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the bank account
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// Gets or sets the user's name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the transaction id
        /// </summary>
        [JsonIgnore]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the location
        /// </summary>
        [JsonIgnore]
        public string Location { get; set; }

        public PaymentCreditRequest()
        {
        }

        public PaymentCreditRequest(Guid creditId, decimal totalValuePaid, string storeId, string userId,
            string bankAccount, string userName, string location)
        {
            CreditId = creditId;
            TotalValuePaid = totalValuePaid;
            StoreId = storeId;
            UserId = userId;
            BankAccount = bankAccount;
            UserName = userName;
            Location = location;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PaymentCreditRequest"/>
        /// </summary>
        /// <param name="refinancingCreditRequest"></param>
        /// <param name="creditId"></param>
        /// <param name="storeId"></param>
        /// <param name="totalValuePaid"></param>
        /// <returns></returns>
        public static PaymentCreditRequest FromRefinancingCreditRequest(RefinancingCreditRequest refinancingCreditRequest,
            Guid creditId, string storeId, decimal totalValuePaid) =>
            new PaymentCreditRequest
            {
                BankAccount = string.Empty,
                CreditId = creditId,
                Location = refinancingCreditRequest.Location,
                StoreId = storeId,
                TotalValuePaid = totalValuePaid,
                UserId = refinancingCreditRequest.UserId,
                UserName = refinancingCreditRequest.UserName
            };
    }
}