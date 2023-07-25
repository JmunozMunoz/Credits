namespace Sc.Credits.Domain.Model.Enums
{
    /// <summary>
    /// Transaction types enumerator
    /// </summary>
    public enum TransactionTypes
    {
        CreateCredit = 1,
        Payment = 2,
        UpdateChargesPaymentPlan = 3,
        CancelPaymentUpdate = 4,
        CancelCredit = 5,
        CancelPayment = 6,
        UpdateCredit = 7
    }
}