using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Refinancings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Helper.Test.Model
{
    public static class RefinancingCreditRequestHelperTest
    {
        public static RefinancingCreditRequest GetDefault(List<CreditMaster> refinancingCreditMasters) =>
            new RefinancingCreditRequest
            {
                ApplicationId = Guid.NewGuid(),
                AuthMethod = (int)AuthMethods.Token,
                CreditIds = refinancingCreditMasters.Select(creditMaster => creditMaster.Id).ToArray(),
                DocumentType = "CC",
                Fees = 3,
                IdDocument = "2883737728",
                Location = "1,1",
                ReferenceCode = "09090",
                ReferenceText = "Almacén de prueba",
                Source = (int)Sources.Refinancing,
                Token = "123456",
                UserId = "testUserId",
                UserName = "testUser"
            };
    }
}