namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Payment matrix basic info contract
    /// </summary>
    public interface IPaymentMatrixBasicInfo
    {
        /// <summary>
        /// Basic Info
        /// </summary>
        /// <param name="amortizationSchedule"></param>
        /// <param name="balance"></param>
        /// <param name="assuranceBalance"></param>
        /// <returns></returns>
        IPaymentMatrixDateInfo BasicInfo(AmortizationScheduleResponse amortizationSchedule,
            decimal balance, decimal assuranceBalance);
    }
}