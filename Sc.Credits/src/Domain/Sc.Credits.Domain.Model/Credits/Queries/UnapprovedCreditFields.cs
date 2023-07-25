using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    public class UnapprovedCreditFields
        : EntityFields
    {
        public UnapprovedCreditFields(string alias) 
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the customer's id field
        /// </summary>
        public Field CustomerId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the store's id field
        /// </summary>
        public Field StoreId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the transaction date field
        /// </summary>
        public Field TransactionDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the transaction time field
        /// </summary>
        public Field TransactionTime => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit value field
        /// </summary>
        public Field CreditValue => GetField(MethodBase.GetCurrentMethod().Name);
    }
}
