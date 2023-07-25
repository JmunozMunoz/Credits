namespace Sc.Credits.Domain.Model.Enums
{
    /// <summary>
    /// Payment types enumerator
    /// </summary>
    public enum PaymentTypes
    {
        Ordinary = 1,
        CreditNote = 2,
        Fraud = 3,
        Deceased = 4,
        Bank = 5,
        PaymentGateways = 6,
        CollectNetworks = 7,
        Cash = 8,
        DownPayment = 9
    }
}