using System;

namespace Sc.Credits.Domain.Model.Customers
{
    /// <summary>
    /// The customer Request entity
    /// </summary>
    public class CustomerRequest
    {
        /// <summary>
        /// Id Document
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Type Document
        /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// amount
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Amount Available
        /// </summary>
        public decimal AmountAvailable { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Second Name
        /// </summary>
        public string SecondName { get; set; }

        /// <summary>
        /// First Last Name
        /// </summary>
        public string FirstLastName { get; set; }

        /// <summary>
        /// Second Last Name
        /// </summary>
        public string SecondLastName { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Movile Number
        /// </summary>
        public string MovileNumber { get; set; }

        /// <summary>
        /// Profile Id
        /// </summary>
        public int? ProfileId { get; set; }

        /// <summary>
        /// Verified Email
        /// </summary>
        public bool? VerifiedEmail { get; set; }

        /// <summary>
        /// Send Token Mail
        /// </summary>
        public bool? SendTokenMail { get; set; }

        /// <summary>
        /// Send Token Sms
        /// </summary>
        public bool? SendTokenSms { get; set; }

        /// <summary>
        /// Customer Id
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Debtor
        /// </summary>
        public bool Debtor { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="CustomerRequest"/>
        /// </summary>

        public CustomerRequest()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="CustomerRequest"/>
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="typeDocument"></param>
        /// <param name="amount"></param>
        /// <param name="amountAvailable"></param>
        /// <param name="status"></param>
        /// <param name="email"></param>
        /// <param name="movileNumber"></param>
        /// <param name="profileId"></param>
        /// <param name="verifiedEmail"></param>
        /// <param name="customerId"></param>
        /// <param name="debtor"></param>
        /// <param name="firstName"></param>
        /// <param name="secondName"></param>
        /// <param name="firstLastName"></param>
        /// <param name="secondLastName"></param>
        public CustomerRequest(string idDocument, string typeDocument, decimal? amount, decimal amountAvailable, int status,
            string email, string movileNumber, int? profileId, bool? verifiedEmail, Guid customerId, bool debtor,
            string firstName, string secondName, string firstLastName, string secondLastName)
        {
            IdDocument = idDocument;
            TypeDocument = typeDocument;
            Amount = amount;
            AmountAvailable = amountAvailable;
            Status = status;
            Email = email;
            MovileNumber = movileNumber;
            ProfileId = profileId;
            VerifiedEmail = verifiedEmail;
            CustomerId = customerId;
            Debtor = debtor;
            FirstName = firstName;
            SecondName = secondName;
            FirstLastName = firstLastName;
            SecondLastName = secondLastName;
        }
    }
}