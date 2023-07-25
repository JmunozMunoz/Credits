using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.UseCase.Credits;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Cancel commons
    /// </summary>
    public class CancelCommons
    {
        /// <summary>
        /// <see cref="IRequestCancelCreditRepository"/>
        /// </summary>
        internal IRequestCancelCreditRepository RequestCancelCreditRepository { get; }

        /// <summary>
        /// <see cref="IRequestCancelPaymentRepository"/>
        /// </summary>
        internal IRequestCancelPaymentRepository RequestCancelPaymentRepository { get; }

        /// <summary>
        /// <see cref="ICancelUseCase"/>
        /// </summary>
        internal ICancelUseCase CancelUseCase { get; }

        /// <summary>
        /// Creates a new instance of <see cref="CancelCommons"/>
        /// </summary>
        /// <param name="requestCancelCreditRepository"></param>
        /// <param name="requestCancelPaymentRepository"></param>
        /// <param name="cancelUseCase"></param>
        public CancelCommons(IRequestCancelCreditRepository requestCancelCreditRepository,
            IRequestCancelPaymentRepository requestCancelPaymentRepository,
            ICancelUseCase cancelUseCase)
        {
            RequestCancelCreditRepository = requestCancelCreditRepository;
            RequestCancelPaymentRepository = requestCancelPaymentRepository;
            CancelUseCase = cancelUseCase;
        }
    }
}