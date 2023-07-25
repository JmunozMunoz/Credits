using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Google.Recaptcha.Requests
{
    public class GoogleRecaptchaRequestModel
    {
        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        /// <value>
        /// The secret.
        /// </value>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public string Response { get; set; }
    }
}
