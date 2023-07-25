using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Credits.Queries.Reading
{
    /// <summary>
    /// Credit reading fields
    /// </summary>
    public static class CreditReadingFields
    {
        private static readonly CreditFields _creditFields = Tables.Catalog.Credits.Fields;

        /// <summary>
        /// Paid credit certificate
        /// </summary>
        public static IEnumerable<Field> PaidCreditCertificate =>
            new List<Field>()
            {
                _creditFields.Id,
                _creditFields.CreditMasterId,
                _creditFields.TransactionDate
            };

        /// <summary>
        /// Promissory note
        /// </summary>
        public static IEnumerable<Field> PromissoryNote =>
            new List<Field>()
            {
                _creditFields.Id,
                _creditFields.CreditMasterId,
                _creditFields.CreditValue,
                _creditFields.Frequency,
                _creditFields.DownPayment,
                _creditFields.InterestRate,
                _creditFields.FeeValue,
                _creditFields.AssuranceFee,
                _creditFields.AssuranceValue,
                _creditFields.AssuranceTotalFeeValue,
                _creditFields.Fees
            };

        /// <summary>
        /// Customer credit history
        /// </summary>
        public static IEnumerable<Field> CustomerCreditHistory =>
            new List<Field>()
            {
                _creditFields.Id,
                _creditFields.CreditMasterId,
                _creditFields.DueDate,
                _creditFields.LastPaymentDate,
                _creditFields.CreditValue
            };

        /// <summary>
        /// Customer payment history
        /// </summary>
        public static IEnumerable<Field> CustomerPaymentHistory =>
            CustomerCreditHistory
                .Union(new List<Field>()
                {
                    _creditFields.TransactionTypeId,
                    _creditFields.TransactionDate,
                    _creditFields.TransactionTime,
                    _creditFields.CreditNumber,
                    _creditFields.PaymentNumber,
                    _creditFields.TotalValuePaid
                });

        /// <summary>
        /// Active credits
        /// </summary>
        public static IEnumerable<Field> ActiveCredits =>
            CustomerCreditHistory
                .Union(new List<Field>()
                {
                    _creditFields.UpdatedPaymentPlanValue,
                    _creditFields.Frequency,
                    _creditFields.FeeValue,
                    _creditFields.InterestRate,
                    _creditFields.Fees,
                    _creditFields.DownPayment,
                    _creditFields.AssuranceValue,
                    _creditFields.AssuranceFee,
                    _creditFields.AssuranceTotalFeeValue,
                    _creditFields.Balance,
                    _creditFields.AssuranceBalance,
                    _creditFields.PreviousInterest,
                    _creditFields.ArrearsCharge,
                    _creditFields.HasArrearsCharge,
                    _creditFields.PreviousArrears,
                    _creditFields.ChargeValue,
                    _creditFields.ActiveFeeValuePaid,
                    _creditFields.CreditNumber,
                    _creditFields.AssurancePercentage
                });

        /// <summary>
        /// Active and pending cancellation credits
        /// </summary>
        public static IEnumerable<Field> ActiveAndPendingCancellationCredits =>
            new List<Field>()
            {
                _creditFields.Id,
                _creditFields.CreditValue,
                _creditFields.CreditMasterId,
                _creditFields.TransactionTypeId,
                _creditFields.TransactionDate,
                _creditFields.Balance
            };

        /// <summary>
        /// Request cancel credit
        /// </summary>
        public static IEnumerable<Field> RequestCancelCredit =>
            new List<Field>()
            {
                _creditFields.Id,
                _creditFields.TransactionTypeId,
                _creditFields.CreditMasterId,
                _creditFields.TransactionDate,
                _creditFields.TransactionTime,
                _creditFields.Balance
            };

        /// <summary>
        /// Pending cancellation credits
        /// </summary>
        public static IEnumerable<Field> PendingCancellationCredits =>
            new List<Field>()
            {
                _creditFields.Id,
                _creditFields.CreditMasterId,
                _creditFields.Balance
            };

        /// <summary>
        /// Active and pending cancellation payments
        /// </summary>
        public static IEnumerable<Field> ActiveAndPendingCancellationPayments =>
            ActiveAndPendingCancellationCredits
                .Union(new List<Field>()
                {
                    _creditFields.CreditNumber,
                    _creditFields.PaymentNumber,
                    _creditFields.TotalValuePaid,
                    _creditFields.LastPaymentDate,
                    _creditFields.TransactionTime
                });

        /// <summary>
        /// Pending cancellation payments
        /// </summary>
        public static IEnumerable<Field> PendingCancellationPayments =>
            PendingCancellationCredits
                .Union(new List<Field>()
                {
                    _creditFields.TransactionTypeId,
                    _creditFields.PaymentNumber,
                    _creditFields.TotalValuePaid,
                    _creditFields.TransactionDate
                });

        /// <summary>
        /// Request cancel payment
        /// </summary>
        public static IEnumerable<Field> RequestCancelPayment =>
            RequestCancelCredit
                .Union(new List<Field>()
                {
                    _creditFields.StoreId
                });

        /// <summary>
        /// Payment fees
        /// </summary>
        public static IEnumerable<Field> PaymentFees =>
              ActiveCredits;

        /// <summary>
        /// Payment templates
        /// </summary>
        public static IEnumerable<Field> PaymentTemplates =>
              ActiveCredits
                .Union(new List<Field>()
                {
                    _creditFields.PaymentNumber,
                    _creditFields.TransactionDate,
                    _creditFields.TransactionTime,
                    _creditFields.CreditValuePaid,
                    _creditFields.TotalValuePaid,
                    _creditFields.InterestValuePaid,
                    _creditFields.ArrearsValuePaid,
                    _creditFields.AssuranceValuePaid,
                    _creditFields.ChargeValuePaid,
                    _creditFields.StatusId
                });
    }
}