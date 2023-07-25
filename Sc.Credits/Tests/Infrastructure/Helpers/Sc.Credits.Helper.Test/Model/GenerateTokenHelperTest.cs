using Sc.Credits.Domain.Model.Credits;

namespace Sc.Credits.Helper.Test.Model
{
    public static class GenerateTokenHelperTest
    {
        public static GenerateTokenRequest GetDefaultResquest() =>
            new GenerateTokenRequest
            {
                CreditValue = 200000,
                Months = 5,
                StoreId = "099ca38qa4544sf45s",
                TypeDocument = "CC",
                IdDocument = "1056587455",
                Frequency = 14
            };
    }
}