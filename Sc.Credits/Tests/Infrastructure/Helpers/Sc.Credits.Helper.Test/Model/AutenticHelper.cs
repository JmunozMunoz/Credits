using Sc.Credits.Domain.Model.Common;

namespace Sc.Credits.Helper.Test.Model
{
    public class AutenticHelper
    {
        public static OAuthResponseAutentic GetOAuthResponseAutentic()
        {
            return new OAuthResponseAutentic()
            {
                AccessToken = "CfDJ8Old0_grEkNKrQh7Y0CX",
                ExpiresIn = 600,
                Scope = "signingcore:fullaccess",
                TokenType = "Bearer"
            };
        }
    }
}