using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Customers.Queries.Reading
{
    /// <summary>
    /// Customer reading fields
    /// </summary>
    public static class CustomerReadingFields
    {
        private static readonly CustomerFields _customerFields = Tables.Catalog.Customers.Fields;

        /// <summary>
        /// Basic info
        /// </summary>
        public static IEnumerable<Field> BasicInfo =>
            new List<Field>()
            {
                _customerFields.Id,
                _customerFields.ProfileId,
                _customerFields.Status
            };

        /// <summary>
        /// Document info
        /// </summary>
        public static IEnumerable<Field> DocumentInfo =>
            BasicInfo
                .Union(new List<Field>()
                {
                    _customerFields.DocumentType,
                    _customerFields.IdDocument
                });

        /// <summary>
        /// CreditCustomer Info
        /// </summary>
        public static IEnumerable<Field> CreditCustomerInfo =>
            new List<Field>()
            {
                _customerFields.Id,
                _customerFields.DocumentType,
                _customerFields.IdDocument,
                _customerFields.AvailableCreditLimit
            };

        /// <summary>
        /// Contact info
        /// </summary>
        public static IEnumerable<Field> ContactInfo =>
            DocumentInfo
                .Union(new List<Field>()
                {
                    _customerFields.Email,
                    _customerFields.Mobile
                });

        /// <summary>
        /// Credit limit
        /// </summary>
        public static IEnumerable<Field> CreditLimit =>
            ContactInfo
                .Union(new List<Field>()
                    {
                        _customerFields.AvailableCreditLimit,
                        _customerFields.CreditLimit,
                        _customerFields.CreditLimitUpdateDate,
                        _customerFields.ValidatedMail,
                        _customerFields.FullName
                    });

        /// <summary>
        /// Credit limit
        /// </summary>
        public static IEnumerable<Field> InformationCustomer =>
            CreditLimit
                .Union(new List<Field>()
                    {
                        _customerFields.FirstName,
                        _customerFields.SecondName
                    });

        /// <summary>
        /// Limit months
        /// </summary>
        public static IEnumerable<Field> LimitMonths =>
            CreditLimit;

        /// <summary>
        /// Credit details
        /// </summary>
        public static IEnumerable<Field> CreditDetails =>
            LimitMonths;

        /// <summary>
        /// Create credit
        /// </summary>
        public static IEnumerable<Field> CreateCredit =>
            CreditDetails;

        /// <summary>
        /// Credit creation
        /// </summary>
        public static IEnumerable<Field> CreditCreation =>
            CreateCredit
                .Union(new List<Field>()
                {
                     _customerFields.FirstName,
                     _customerFields.SecondName,
                     _customerFields.FirstLastName,
                     _customerFields.SecondLastName
                });

        /// <summary>
        /// Create credit
        /// </summary>
        public static IEnumerable<Field> Token =>
            CreditLimit
                .Union(new List<Field>()
                {
                    _customerFields.SendTokenMail,
                    _customerFields.SendTokenSms,
                    _customerFields.FirstLastName,
                     _customerFields.FirstName
                });

        /// <summary>
        /// Create credit
        /// </summary>
        public static IEnumerable<Field> TokenWithFirstName =>
            Token;

        /// <summary>
        /// Document info and name
        /// </summary>
        private static IEnumerable<Field> DocumentInfoAndName =>
            DocumentInfo
                .Union(new List<Field>()
                {
                    _customerFields.FullName
                });

        /// <summary>
        /// Paid credit certificate
        /// </summary>
        public static IEnumerable<Field> PaidCreditCertificate =>
            DocumentInfoAndName;

        /// <summary>
        /// Active credits
        /// </summary>
        public static IEnumerable<Field> ActiveCredits =>
            DocumentInfoAndName.Union(new List<Field>()
                {
                    _customerFields.Mobile,
                    _customerFields.Email
                });

        /// <summary>
        /// Prommisory note
        /// </summary>
        public static IEnumerable<Field> PrommisoryNote =>
            DocumentInfoAndName;

        /// <summary>
        /// Pay credit
        /// </summary>
        public static IEnumerable<Field> PayCredit =>
            CreateCredit;

        /// <summary>
        /// Payment fees
        /// </summary>
        public static IEnumerable<Field> PaymentFees =>
            CreditDetails;

        /// <summary>
        /// Credit history
        /// </summary>
        public static IEnumerable<Field> CreditHistory =>
            DocumentInfo;

        /// <summary>
        /// Payment history
        /// </summary>
        public static IEnumerable<Field> PaymentHistory =>
            CreditHistory;

        /// <summary>
        /// Update mail
        /// </summary>
        public static IEnumerable<Field> UpdateMail =>
            BasicInfo
                .Union(new List<Field>()
                {
                    _customerFields.Email,
                    _customerFields.ValidatedMail
                });

        /// <summary>
        /// Update mobile
        /// </summary>
        public static IEnumerable<Field> UpdateMobile =>
            BasicInfo
                .Union(new List<Field>()
                {
                    _customerFields.Mobile
                });

        /// <summary>
        /// Active and pending cancellation credits
        /// </summary>
        public static IEnumerable<Field> ActiveAndPendingCancellationCredits =>
            ActiveCredits;

        /// <summary>
        /// Pending cancellation credits
        /// </summary>
        public static IEnumerable<Field> PendingCancellationCredits =>
            DocumentInfoAndName;

        /// <summary>
        /// Pending cancellation payments
        /// </summary>
        public static IEnumerable<Field> PendingCancellationPayments =>
            DocumentInfoAndName;

        /// <summary>
        /// Payment templates
        /// </summary>
        public static IEnumerable<Field> PaymentTemplates =>
            CreditLimit;

        /// <summary>
        /// Refinancing
        /// </summary>
        public static IEnumerable<Field> Refinancing =>
            ContactInfo
                .Union(new List<Field>()
                {
                    _customerFields.FullName,
                    _customerFields.FirstName,
                    _customerFields.SecondName
                });

        /// <summary>
        /// Customer names 
        /// </summary>
        public static IEnumerable<Field> CustomerNames=>
            ActiveCredits.Union(new List<Field>()
            {
                _customerFields.FirstName,
                _customerFields.SecondName
            });
    }
}