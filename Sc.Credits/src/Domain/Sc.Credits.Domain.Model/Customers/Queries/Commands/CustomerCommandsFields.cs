using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Customers.Queries.Commands
{
    /// <summary>
    /// Customer commands fields
    /// </summary>
    public static class CustomerCommandsFields
    {
        private static readonly CustomerFields _customerFields = Tables.Catalog.Customers.Fields;

        /// <summary>
        /// Update
        /// </summary>
        public static IEnumerable<Field> Update =>
            new List<Field>()
            {
                _customerFields.Email,
                _customerFields.FullName,
                _customerFields.Mobile,
                _customerFields.ProfileId,
                _customerFields.SendTokenMail,
                _customerFields.SendTokenSms,
                _customerFields.Status,
                _customerFields.UpdateDate,
                _customerFields.UpdateTime,
                _customerFields.ValidatedMail,
                _customerFields.FirstName,
                _customerFields.SecondName,
                _customerFields.FirstLastName,
                _customerFields.SecondLastName
            };

        /// <summary>
        /// Update credit limit
        /// </summary>
        public static IEnumerable<Field> UpdateCreditLimit =>
            new List<Field>()
            {
                _customerFields.AvailableCreditLimit,
                _customerFields.CreditLimit,
                _customerFields.CreditLimitUpdateDate,
                _customerFields.UpdateDate,
                _customerFields.UpdateTime
            };

        /// <summary>
        /// Update status
        /// </summary>
        public static IEnumerable<Field> UpdateStatus =>
            new List<Field>()
            {
                _customerFields.Status,
                _customerFields.UpdateDate,
                _customerFields.UpdateTime
            };

        /// <summary>
        /// Update mail
        /// </summary>
        public static IEnumerable<Field> UpdateMail =>
            new List<Field>()
            {
                _customerFields.Email,
                _customerFields.ValidatedMail,
                _customerFields.UpdateDate,
                _customerFields.UpdateTime
            };

        /// <summary>
        /// Update mail
        /// </summary>
        public static IEnumerable<Field> UpdateMobile =>
            new List<Field>()
            {
                _customerFields.Mobile,
                _customerFields.UpdateDate,
                _customerFields.UpdateTime
            };
    }
}