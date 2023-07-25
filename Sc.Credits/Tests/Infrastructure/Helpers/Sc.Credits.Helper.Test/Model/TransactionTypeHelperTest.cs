using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;

namespace Sc.Credits.Helper.Test.Model
{
    /// <summary>
    /// Transaction Type Helper Test
    /// </summary>
    public static class TransactionTypeHelperTest
    {
        /// <summary>
        /// Get Create Credit Type
        /// </summary>
        /// <returns></returns>
        public static TransactionType GetCreateCreditType()
        {
            return new TransactionType("Create Credit").SetId((int)TransactionTypes.CreateCredit);
        }

        /// <summary>
        /// Get Payment Type
        /// </summary>
        /// <returns></returns>
        public static TransactionType GetPaymentType()
        {
            return new TransactionType("Payment").SetId((int)TransactionTypes.Payment);
        }

        /// <summary>
        /// Cancel payment
        /// </summary>
        /// <returns></returns>
        public static TransactionType GetCancelPaymentType()
        {
            return new TransactionType("Cancel Payment").SetId((int)TransactionTypes.CancelPayment);
        }

        /// <summary>
        /// Get cancel credit
        /// </summary>
        /// <returns></returns>
        public static TransactionType GetCancelCreditType()
        {
            return new TransactionType("Cancel Credit").SetId((int)TransactionTypes.CancelCredit);
        }

        /// <summary>
        /// Get cancel credit
        /// </summary>
        /// <returns></returns>
        public static TransactionType GetUpdateCreditType()
        {
            return new TransactionType("Update Credit").SetId((int)TransactionTypes.UpdateCredit);
        }

        /// <summary>
        /// Get transaction type
        /// </summary>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public static TransactionType GetTransactionType(TransactionTypes transactionType)
        {
            switch (transactionType)
            {
                case TransactionTypes.CreateCredit:
                    return new TransactionType("Create Credit").SetId((int)TransactionTypes.CreateCredit);

                case TransactionTypes.Payment:
                    return new TransactionType("Payment").SetId((int)TransactionTypes.Payment);

                case TransactionTypes.UpdateChargesPaymentPlan:
                    return new TransactionType("Update Charges Payment Plan").SetId((int)TransactionTypes.UpdateChargesPaymentPlan);

                case TransactionTypes.CancelCredit:
                    return new TransactionType("Cancel Credit").SetId((int)TransactionTypes.CancelCredit);

                case TransactionTypes.CancelPayment:
                    return new TransactionType("Cancel Payment").SetId((int)TransactionTypes.CancelPayment);

                case TransactionTypes.UpdateCredit:
                    return new TransactionType("Update Credit").SetId((int)TransactionTypes.UpdateCredit);

                default:
                    return new TransactionType("Error").SetId(0);
            }
        }
    }
}