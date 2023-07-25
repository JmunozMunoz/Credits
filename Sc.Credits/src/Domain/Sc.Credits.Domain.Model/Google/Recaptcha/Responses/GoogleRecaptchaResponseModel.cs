using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Google.Recaptcha.Responses
{
    public class GoogleRecaptchaResponseModel
    {
        /// <summary>
        /// Gets or sets the success.
        /// </summary>
        /// <value>
        /// The success.
        /// </value>
        public bool success { get; set; }

        /// <summary>
        /// Gets or sets the challenge ts.
        /// </summary>
        /// <value>
        /// The challenge ts.
        /// </value>
        public DateTime challenge_ts { get; set; }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        public string hostname { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public decimal score { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string action { get; set; }
    }
}
