using System;
using System.Collections.Generic;
using System.Text;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Parameters;

namespace Sc.Credits.Domain.Model.Credits
{
    public class SimulatedCreditRequest : GeneralCreditDetailDomainRequest
    {

        public SimulatedCreditRequest(Store store, decimal creditValue, int frequency,
    AppParameters appParameters) : base(store, creditValue, frequency, appParameters)
        {
        }
    }
}
