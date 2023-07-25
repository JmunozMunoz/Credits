using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Customers.Gateway
{
    public interface IRiskLevelRepository
    {
        /// <summary>
        /// Generate credit token
        /// </summary>
        /// <param name="riskLevelRequest"></param>
        /// <returns></returns>
        Task<CustomerRiskLevel> CalculateRiskLevelAsync(CustomerRiskLevelRequest riskLevelRequest);
    }
}