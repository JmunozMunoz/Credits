using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Refinancings.Gateway
{
    public interface IRefinancingLogRepository
       : ICommandRepository<RefinancingLog>
    {
        Task<List<RefinancingLogDetail>> GetByStatusFromMasterAsync(Guid refinancingLogId,
                        IEnumerable<Field> fields, IEnumerable<Field> storeFields = null);
    }
}
