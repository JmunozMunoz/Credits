namespace Sc.Credits.Domain.Model.Credits
{
    public class PaymentPlanResponse
    {
        public AmortizationScheduleResponse AmortizationSchedule { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerSecondName { get; set; }
        public string CustomerFullName { get; set; }
        public int DecimalNumbersRound { get; set; }
        public string StoreName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
        public string AmortizationScheduleUrl { get; set; }
    }
}