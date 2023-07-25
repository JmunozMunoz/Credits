using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.ReportTemplates.Gateway
{
    /// <summary>
    /// The report templates gateway contract.
    /// </summary>
    public interface IReportTemplatesGateway
    {
        /// <summary>
        /// Generates a report template with specific requested data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="reportName"></param>
        /// <param name="renderName"></param>
        /// <returns></returns>
        Task<string> GenerateAsync<T>(T request, string reportName, string renderName = default)
            where T : class;
    }
}