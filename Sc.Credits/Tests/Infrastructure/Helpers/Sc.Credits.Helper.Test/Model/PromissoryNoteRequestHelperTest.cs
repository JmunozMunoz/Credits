
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Helper.Test.Model
{
    public static class PromissoryNoteRequestHelperTest
    {
        public static PromissoryNoteRequest GetPromissoryNote()
        {
            return new PromissoryNoteRequest(new CreditDetailResponse(), CreditMasterHelperTest.GetCreditMaster(), 
                "sitecredito", 14, 0.26M , 0.26M , 2, 2, "4303033", "4343543", 0.26M, 0.10M);
        }
    }
}
