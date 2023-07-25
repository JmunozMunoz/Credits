using Sc.Credits.Domain.UseCase.Credits;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    public class SimulatorCommons
    {

        /// <summary>
        /// <see cref="ICreditUsesCase"/>
        /// </summary>
        internal ICreditUsesCase CreditUsesCase { get; }

        /// <summary>
        /// <see cref="ICreditCommonsService"/>
        /// </summary>
        internal ISimulatorCommonService Service { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatorCommons"/> class.
        /// </summary>
        /// <param name="creditUsesCase">The simulator uses case.</param>
        /// <param name="creditCommonsService">The credit commons service.</param>
        public SimulatorCommons(
            ICreditUsesCase creditUsesCase,
            ISimulatorCommonService creditCommonsService)
        {
            CreditUsesCase = creditUsesCase;
            Service = creditCommonsService;
        }

    }
}
