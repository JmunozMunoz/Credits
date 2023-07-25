using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;

namespace Sc.Credits.Helper.Test.Model
{
    public static class CreateCreditDomainRequestHelperTest
    {
        public static CreateCreditDomainRequest GetDefault(CreateCreditRequest createCreditRequest,
            Source source, AuthMethod authMethod, AppParameters appParameters) =>
            new CreateCreditDomainRequest(createCreditRequest, CustomerHelperTest.GetCustomer(),
                StoreHelperTest.GetStoreWithCustomStoreCategory("Default", 4, 8, 30000M, 800000M, 40000M), appParameters)
            .SetCreateCreditTransactionType(TransactionTypeHelperTest.GetCreateCreditType())
            .SetOrdinaryPaymentType(PaymentTypeHelperTest.GetOrdinaryPaymentType())
            .SetSeedMasters(StatusHelperTest.GetActiveStatus(), source, authMethod);
    }
}