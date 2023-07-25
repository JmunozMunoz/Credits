using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Queries.Reading;
using Sc.Credits.Domain.Model.Stores.Queries.Reading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Credit customer service is an implementation of <see cref="ICreditCustomerService"/>
    /// </summary>
    public class CreditCustomerService
        : ICreditCustomerService
    {
        private readonly ICustomerService _customerService;
        private readonly ICreditMasterRepository _creditMasterRepository;
        private readonly ICreditNotificationRepository _creditNotificationRepository;

        /// <summary>
        /// Creates a new instance of <see cref="CreditCustomerService"/>
        /// </summary>
        /// <param name="customerService"></param>
        /// <param name="creditMasterRepository"></param>
        /// <param name="creditNotificationRepository"></param>
        public CreditCustomerService(ICustomerService customerService,
            ICreditMasterRepository creditMasterRepository,
            ICreditNotificationRepository creditNotificationRepository)
        {
            _customerService = customerService;
            _creditMasterRepository = creditMasterRepository;
            _creditNotificationRepository = creditNotificationRepository;
        }

        #region ICreditCustomerService Members

        /// <summary>
        /// <see cref="ICreditCustomerService.SendMigrationHistoryAsync(CreditCustomerMigrationHistoryRequest)"/>
        /// </summary>
        /// <param name="creditCustomerMigrationHistoryRequest"></param>
        /// <returns></returns>
        public async Task SendMigrationHistoryAsync(CreditCustomerMigrationHistoryRequest creditCustomerMigrationHistoryRequest)
        {
            Customer customer = await _customerService.GetAsync(creditCustomerMigrationHistoryRequest.IdDocument,
                creditCustomerMigrationHistoryRequest.DocumentType, CustomerReadingFields.CreditCustomerInfo);

            List<CreditMaster> allCredits = await _creditMasterRepository.GetWithTransactionsAsync(customer,
                storeFields: StoreReadingFields.BasicInfo, transactionStoreFields: StoreReadingFields.BasicInfo);

            List<Credit> allTransactions = allCredits.SelectMany(credit => credit.History).ToList();

            foreach (Credit credit in allTransactions)
            {
                CreditMaster creditMaster = allCredits.First(master => master.Id == credit.GetCreditMasterId);
                await _creditNotificationRepository.SendEventAsync(creditMaster, customer, credit.Store, credit, complementEventName: "Migration");
            }
        }

        #endregion ICreditCustomerService Members
    }
}