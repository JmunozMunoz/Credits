namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// Token entity
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets the token's value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the remaining seconds
        /// </summary>
        public int RemainingSeconds { get; set; }
    }
}