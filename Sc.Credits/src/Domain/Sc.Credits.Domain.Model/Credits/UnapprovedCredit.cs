using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.Commons.Domain;
using System;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Unapproved credit
    /// </summary>
    public class UnapprovedCredit:
        Entity<Guid>, IAggregateRoot
    {

        private Guid _customerId;
        private string _storeId;
        private DateTime _transactionDate;
        private TimeSpan _transactionTime;
        private decimal _creditValue;

        /// <summary>
        /// Gets the customer's id field
        /// </summary>
        [Write(false)]
        public Guid CustomerId => _customerId;

        /// <summary>
        /// Gets the store's id field
        /// </summary>
        [Write(false)]
        public string StoreId => _storeId;

        /// <summary>
        /// Gets the transaction date field
        /// </summary>
        [Write(false)]
        public DateTime TransactionDate => _transactionDate;

        /// <summary>
        /// Gets the transaction time field
        /// </summary>
        [Write(false)]
        public TimeSpan TransactionTime => _transactionTime;

        /// <summary>
        /// Gets the credit value field
        /// </summary>
        [Write(false)]
        public decimal CreditValue => _creditValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnapprovedCredit"/> class.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <param name="transactionDate">The transaction date.</param>
        /// <param name="transactionTime">The transaction time.</param>
        /// <param name="creditValue">The credit value.</param>
        public UnapprovedCredit(Guid customerId, string storeId, DateTime transactionDate, TimeSpan transactionTime,
            decimal creditValue)
        {
             Id = IdentityGenerator.NewSequentialGuid();
            _customerId = customerId;
            _storeId = storeId;
            _transactionDate = transactionDate;
            _transactionTime = transactionTime;
            _creditValue = creditValue;
        }
    }
}
