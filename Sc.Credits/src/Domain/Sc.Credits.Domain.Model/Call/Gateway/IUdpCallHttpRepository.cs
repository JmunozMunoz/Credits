using Sc.Credits.Domain.Model.Credits;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Call.Gateway
{
    public interface IUdpCallHttpRepository
    {
        /// <summary>
        /// Calls the asynchronous.
        /// </summary>
        /// <param name="CallRequest">The call request.</param>
        /// <returns></returns>
        Task CallAsync(CreditTokenCallRequest CallRequest);
    }
}