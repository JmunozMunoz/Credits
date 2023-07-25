using Coravel.Invocable;
using Sc.Credits.Domain.Managment.Services.Credits;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.Jobs
{
    /// <summary>
    /// Reject cancellation request job is an implementation of <see cref="IInvocable"/>.
    /// </summary>
    public class RejectCancellationRequestJob : IInvocable
    {
        private readonly ICancelCreditService _cancelCreditService;
        private readonly ICancelPaymentService _cancelPaymentService;

        /// <summary>
        /// Creates a new instance of <see cref="RejectCancellationRequestJob"/>.
        /// </summary>
        /// <param name="cancelCreditService"></param>
        /// <param name="cancelPaymentService"></param>
        public RejectCancellationRequestJob(ICancelCreditService cancelCreditService,
            ICancelPaymentService cancelPaymentService)
        {
            _cancelCreditService = cancelCreditService;
            _cancelPaymentService = cancelPaymentService;
        }

        /// <summary>
        /// <see cref="IInvocable.Invoke"/>
        /// </summary>
        /// <returns></returns>
        public async Task Invoke()
        {
            await _cancelPaymentService.RejectUnprocessedRequestAsync();
            await _cancelCreditService.RejectUnprocessedRequestAsync();
        }
    }
}