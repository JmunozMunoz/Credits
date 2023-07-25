using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.UseCase.Credits;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Credit commons
    /// </summary>
    public class CreditCommons
    {
        /// <summary>
        /// <see cref="ICreditMasterRepository"/>
        /// </summary>
        internal ICreditMasterRepository CreditMasterRepository { get; }

        /// <summary>
        /// <see cref="ICreditUsesCase"/>
        /// </summary>
        internal ICreditUsesCase CreditUsesCase { get; }

        /// <summary>
        /// <see cref="ICreditCommonsService"/>
        /// </summary>
        internal ICreditCommonsService Service { get; }

        /// <summary>
        /// Payment commons
        /// </summary>
        internal PaymentCommons PaymentCommons { get; }

        /// <summary>
        /// Cancel commons
        /// </summary>
        internal CancelCommons CancelCommons { get; }

        /// <summary>
        /// Creates a new instance of <see cref="CreditCommons"/>
        /// </summary>
        /// <param name="creditMasterRepository"></param>
        /// <param name="creditUsesCase"></param>
        /// <param name="paymentCommons"></param>
        /// <param name="cancelCommons"></param>
        /// <param name="creditCommonsService"></param>
        public CreditCommons(ICreditMasterRepository creditMasterRepository,
            ICreditUsesCase creditUsesCase,
            PaymentCommons paymentCommons,
            CancelCommons cancelCommons,
            ICreditCommonsService creditCommonsService)
        {
            CreditMasterRepository = creditMasterRepository;
            CreditUsesCase = creditUsesCase;
            Service = creditCommonsService;
            PaymentCommons = paymentCommons;
            CancelCommons = cancelCommons;
        }
    }
}