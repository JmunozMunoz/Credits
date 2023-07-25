using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Helper.Test.Model
{
    public static class RequestCancelCreditHelperTest
    {
        /// <summary>
        /// Get request cancel credit
        /// </summary>
        /// <returns></returns>
        public static RequestCancelCredit GetRequestCancelCredit()
        {
            return new RequestCancelCredit(Guid.NewGuid(), "Pepito", "123456", "pruebas", 0, "54as545as8a", 1, 20000);
        }

        /// <summary>
        /// Get request cancel credit
        /// </summary>
        /// <param name="creditId"></param>
        /// <returns></returns>
        public static RequestCancelCredit GetRequestCancelCredit(Guid creditId)
        {
            return new RequestCancelCredit(creditId, "Pepito", "123456", "pruebas", 0, "a875d4a54ra5", 1, 20000);
        }

        /// <summary>
        /// Get request cancel credit
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="storeId"></param>
        /// <param name="requestStatus"></param>
        /// <returns></returns>
        public static RequestCancelCredit GetRequestCancelCredit(Guid creditId, string storeId, RequestStatuses requestStatus)
        {
            return new RequestCancelCredit(creditId, "Pepito", storeId, "pruebas", (int)requestStatus, "a875d4a54ra5", 1, 20000);
        }

        /// <summary>
        /// Get request cancel credit
        /// </summary>
        /// <returns></returns>
        public static List<RequestCancelCredit> GetRequestCancelCreditList()
        {
            return new List<RequestCancelCredit>
            {
                new RequestCancelCredit(Guid.NewGuid(), "Pepito", "123456", "pruebas", 0, "54as545as8a", 1 , 20000).SetId(default),
                new RequestCancelCredit(Guid.NewGuid(), "Pepito", "123456", "pruebas", 0, "54as545as8a", 1 , 20000),
                new RequestCancelCredit(Guid.NewGuid(), "Pepito 2", "1234567", "pruebas 2", 0, "54as545as8a2", 1 , 20000)
            };
        }

        /// <summary>
        /// Get request cancel credit list with credit master
        /// </summary>
        /// <returns></returns>
        public static RequestCancelCreditPaged GetRequestCancelCreditListWihtCreditMaster()
        {
            RequestCancelCreditPaged requestCancelCredit = new RequestCancelCreditPaged();

            requestCancelCredit.RequestCancelCredit= new List<RequestCancelCredit>
            {
                new RequestCancelCredit(Guid.NewGuid(), "Pepito", "123456", "pruebas", 0, "54as545as8a", 1 , 20000).SetCreditMaster(CreditMasterHelperTest.GetCreditMaster()).SetId(default),
                new RequestCancelCredit(Guid.NewGuid(), "Pepito", "123456", "pruebas", 0, "54as545as8a", 2, null).SetCreditMaster(CreditMasterHelperTest.GetCreditMaster())
            };

            requestCancelCredit.TotalRecords = 2;


            return requestCancelCredit;
        }

        /// <summary>
        /// Get request cancel credits with same store canceled
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <returns></returns>
        public static List<RequestCancelCredit> GetRequestCancelCreditsWithSameStoreCanceleds(List<CreditMaster> creditMasters)
        {
            string storeId = "8a78s7a887sa8";
            return creditMasters.Select(item =>
                GetRequestCancelCredit(item.Id, storeId, RequestStatuses.Cancel)
                .SetCreditMaster(item)
                .SetProcessDate(DateTime.Now)).ToList();
        }

        /// <summary>
        /// Get request cancel credits from creditmasters
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <returns></returns>
        public static List<RequestCancelCredit> GetRequestCancelCreditsFromCreditMasters(List<CreditMaster> creditMasters)
        {
            return creditMasters
                .Select(item =>
                    GetRequestCancelCredit(item.Id, item.GetStoreId, RequestStatuses.Cancel)
                .SetCreditMaster(item)
                .SetProcessDate(DateTime.Now)).ToList();
        }
    }
}