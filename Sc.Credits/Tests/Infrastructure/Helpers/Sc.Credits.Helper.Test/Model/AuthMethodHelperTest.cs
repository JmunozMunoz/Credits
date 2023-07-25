using Sc.Credits.Domain.Model.Credits;
using System.Collections.Generic;

namespace Sc.Credits.Helper.Test.Model
{
    /// <summary>
    /// Auth Method Helper Test
    /// </summary>
    public static class AuthMethodHelperTest
    {
        /// <summary>
        /// Get Token Auth Method
        /// </summary>
        /// <returns></returns>
        public static AuthMethod GetTokenAuthMethod()
        {
            return new AuthMethod("Token").SetId(1);
        }

        /// <summary>
        /// Get Auth Methods
        /// </summary>
        /// <returns></returns>
        public static List<AuthMethod> GetAuthMethods()
        {
            return new List<AuthMethod>
            {
                new AuthMethod("Token").SetId(1),
                new AuthMethod("Face Api").SetId(2),
                new AuthMethod("Other Type").SetId(3),
                new AuthMethod("Default").SetId(default)
            };
        }
    }
}