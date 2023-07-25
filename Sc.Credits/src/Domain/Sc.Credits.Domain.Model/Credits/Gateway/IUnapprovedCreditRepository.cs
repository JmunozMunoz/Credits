using Sc.Credits.Domain.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits.Gateway
{
    public interface IUnapprovedCreditRepository
        : ICommandRepository<UnapprovedCredit>
    {
    }
}
