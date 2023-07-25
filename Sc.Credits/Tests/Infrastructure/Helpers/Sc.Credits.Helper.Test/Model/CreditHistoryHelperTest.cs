using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Helper.Test.Model
{
    /// <summary>
    /// Credit history helper test
    /// </summary>
    public static class CreditHistoryHelperTest
    {
        public static List<CreditHistoryResponse> GetResponsesFromMasters(List<CreditMaster> creditMasters)
        {
            Random randomDays = new Random();
            return creditMasters
                .Select(creditMaster =>
                    new CreditHistoryResponse
                    {
                        ArrearsDays = randomDays.Next(0, 100),
                        CancelDate = DateTime.Now.AddDays(-randomDays.Next(0, 100)),
                        CreditDate = creditMaster.GetCreditDate,
                        CreditId = creditMaster.Id,
                        CreditNumber = creditMaster.GetCreditNumber,
                        CreditValue = creditMaster.Current.GetCreditValue,
                        Status = creditMaster.Status.Name,
                        StoreId = creditMaster.GetStoreId,
                        StoreName = creditMaster.Store.StoreName
                    })
                .ToList();
        }
    }
}