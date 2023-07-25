namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// The user info entity
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Gets the user's name
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the user's id
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="UserInfo"/>
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        public UserInfo(string userName, string userId)
        {
            UserName = userName;
            UserId = userId;
        }
    }
}