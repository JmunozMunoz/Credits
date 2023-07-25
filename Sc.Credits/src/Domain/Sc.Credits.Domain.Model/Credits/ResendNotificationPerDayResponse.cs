using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Credits
{
    public class ResendNotificationPerDayResponse
    {
        public List<Guid> NotifySuccessfullIds { get; private set; }
        public List<Guid> NotifyUnsuccessfullIds { get; private set; }
        public ResendNotificationPerDayResponse()
        {
            NotifySuccessfullIds = Array.Empty<Guid>().ToList();
            NotifyUnsuccessfullIds = Array.Empty<Guid>().ToList(); 
        }

        public void AddNotifySuccessfull(Guid id)
        {
            NotifySuccessfullIds.Add(id);
        }
        public void AddNotifyUnsuccessfull(Guid id)
        {
            NotifyUnsuccessfullIds.Add(id);
        }

    }
}
