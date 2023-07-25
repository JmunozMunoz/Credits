namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The customer credit limit response entity
    /// </summary>
    public class CustomerCreditLimitResponse
    {
        /// <summary>
        /// Gets or sets the credit limit
        /// </summary>
        public decimal CreditLimit { get; set; }

        /// <summary>
        /// Gets or sets the available credit limit
        /// </summary>
        public decimal AvailableCreditLimit { get; set; }

        /// <summary>
        /// Gets or sets the validated mail indicator
        /// </summary>
        public bool ValidatedMail { get; set; }

        /// <summary>
        /// Gets or sets the new credit button enabled indicator
        /// </summary>
        public bool NewCreditButtonEnabled { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the mobile
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the defaulter indicator
        /// </summary>
        public bool Defaulter { get; set; }

        /// <summary>
        /// Gets or sets the credit limit increase indicator
        /// </summary>
        public bool CreditLimitIncrease { get; set; }

        /// <summary>
        /// Gets or sets the is available credit limit indicator
        /// </summary>
        public bool IsAvailableCreditLimit { get; set; }

        /// <summary>
        /// Gets or sets the is active indicator
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the status name
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Gets or sets the First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the Second Name
        /// </summary>
        public string SecondName { get; set; }
    }
}