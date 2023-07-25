using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    public class SimulationDetailsResponse
    {
        /// <summary>
        /// Gets or sets the total fee value
        /// </summary>
        public decimal TotalFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the fees
        /// </summary>
        public int Fees { get; set; }

        public SimulationDetailsResponse()
        {
        }

        public SimulationDetailsResponse(decimal totalFeeValue, int fees)
        {
            TotalFeeValue = totalFeeValue;
            Fees = fees;
        }
    }
}
