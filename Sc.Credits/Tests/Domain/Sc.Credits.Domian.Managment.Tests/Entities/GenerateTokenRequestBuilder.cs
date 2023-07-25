using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Managment.Tests.Entities
{
    public class GenerateTokenRequestBuilder
    {
        /// <summary>
        /// Gets or sets the credit value.
        /// </summary>
        /// <value>
        /// The credit value.
        /// </value>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>
        /// The frequency.
        /// </value>
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the months.
        /// </summary>
        /// <value>
        /// The months.
        /// </value>
        public int Months { get; set; }

        /// <summary>
        /// Gets or sets the store identifier.
        /// </summary>
        /// <value>
        /// The store identifier.
        /// </value>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the type document.
        /// </summary>
        /// <value>
        /// The type document.
        /// </value>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Gets or sets the identifier document.
        /// </summary>
        /// <value>
        /// The identifier document.
        /// </value>
        public string IdDocument { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is refinancing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is refinancing; otherwise, <c>false</c>.
        /// </value>
        public bool IsRefinancing { get; set; }

        /// <summary>
        /// gets transaction identifier
        /// </summary>
        public string TransactionId { get; set; }

        private const decimal _creditValue = 40000;
        private const int _frequency = 30;
        private const int _months = 1;
        private const string _storeId = "5eebba27d5b0a1000105dce3";
        private const string _typeDocument = "CC";
        private const string _idDocument = "1049627642";
        private const string _source = "4";
        private const bool _isRefinancing = false;
        private const string _transactionId = "00BF5060-0247-42F3-BEBD-9B8AFF393A5E";

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateTokenRequestBuilder"/> class.
        /// </summary>
        public GenerateTokenRequestBuilder()
        {
            CreditValue = _creditValue;
            Frequency = _frequency;
            Months = _months;
            StoreId = _storeId;
            TypeDocument = _typeDocument;
            IdDocument = _idDocument;
            Source = _source;
            IsRefinancing = _isRefinancing;
            TransactionId = _transactionId;
        }

        /// <summary>
        /// Withes the credit value.
        /// </summary>
        /// <param name="creditValue">The credit value.</param>
        /// <returns></returns>
        public GenerateTokenRequestBuilder WithCreditValue(decimal creditValue)
        {
            this.CreditValue = creditValue;
            return this;
        }

        /// <summary>
        /// Withes the frequency.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        /// <returns></returns>
        public GenerateTokenRequestBuilder WithFrequency(int frequency)
        {
            this.Frequency = frequency;
            return this;
        }

        /// <summary>
        /// Withes the months.
        /// </summary>
        /// <param name="months">The months.</param>
        /// <returns></returns>
        public GenerateTokenRequestBuilder WithMonths(int months)
        {
            this.Months = months;
            return this;
        }

        /// <summary>
        /// Withes the store identifier.
        /// </summary>
        /// <param name="storeId">The store identifier.</param>
        /// <returns></returns>
        public GenerateTokenRequestBuilder WithStoreId(string storeId)
        {
            this.StoreId = storeId;
            return this;
        }

        /// <summary>
        /// Withes the type document.
        /// </summary>
        /// <param name="typeDocument">The type document.</param>
        /// <returns></returns>
        public GenerateTokenRequestBuilder WithTypeDocument(string typeDocument)
        {
            this.TypeDocument = typeDocument;
            return this;
        }

        /// <summary>
        /// Withes the identifier document.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <returns></returns>
        public GenerateTokenRequestBuilder WithIdDocument(string idDocument)
        {
            this.IdDocument = idDocument;
            return this;
        }

        /// <summary>
        /// Withes the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public GenerateTokenRequestBuilder WithSource(string source)
        {
            this.Source = source;
            return this;
        }

        /// <summary>
        /// Withes the is refinancing.
        /// </summary>
        /// <param name="isRefinancing">if set to <c>true</c> [is refinancing].</param>
        /// <returns></returns>
        public GenerateTokenRequestBuilder WithIsRefinancing(bool isRefinancing)
        {
            this.IsRefinancing = isRefinancing;
            return this;
        }

        /// <summary>
        /// Withes the is transactionId
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public GenerateTokenRequestBuilder WithIsTransactionId(string transactionId)
        {
            this.TransactionId = transactionId;
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public GenerateTokenRequest Build()
        {
            return new GenerateTokenRequest
            {
                CreditValue = CreditValue,
                Frequency = Frequency,
                Months = Months,
                StoreId = StoreId,
                TypeDocument = TypeDocument,
                IdDocument = IdDocument,
                Source = Source,
            };
        }
    }
}