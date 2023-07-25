using Sc.Credits.Helpers.ObjectsUtils;
using System;

namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// The token response entity
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// Token
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// Formatted Token
        /// </summary>
        public string FormattedToken { get; set; }

        /// <summary>
        /// Total time
        /// </summary>
        public int TotalTime { get; set; }

        /// <summary>
        /// Duration
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Expiration date
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Generation date
        /// </summary>
        public DateTime GenerationDate { get; set; }

        /// <summary>
        /// Setup visibility
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        public void SetupVisibility(CredinetAppSettings credinetAppSettings)
        {
            if (!credinetAppSettings.ReturnTokenValue)
            {
                Token.Value = string.Empty;
                FormattedToken = string.Empty;
            }
        }

        public static TokenResponse NotGenerated() =>
            new TokenResponse()
            {
                Token = new Token()
                {
                    Value = "",
                    RemainingSeconds = 300
                },
                FormattedToken = "",
                TotalTime = 300,
                Duration = TimeSpan.FromSeconds(300),
                ExpirationDate = DateTime.Now + TimeSpan.FromSeconds(300),
                GenerationDate = DateTime.Now
            };
        
    }
}