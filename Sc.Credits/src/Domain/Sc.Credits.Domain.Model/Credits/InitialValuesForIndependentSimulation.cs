using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Initial values for making an independent simulation
    /// </summary>
    /// <seealso cref="Sc.Credits.Domain.Model.Credits.RequiredInitialValues" />
    public class InitialValuesForIndependentSimulation : RequiredInitialValues
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string userName { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        /// <value>
        /// The user email.
        /// </value>
        public string userEmail { get; set; }

        /// <summary>
        /// Gets or sets the validation token.
        /// </summary>
        /// <value>
        /// The validation token.
        /// </value>
        public string ValidationToken { get; set; }

        /// <summary>
        /// Validate Email
        /// </summary>
        public bool emailIsValid
        {
            get
            {
                try
                {
                    MailAddress m = new MailAddress(userEmail);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
        }
    }
}
