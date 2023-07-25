using Moq;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helper.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Credits
{
    public class CreditCustomerServiceTest
    {
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<ICreditMasterRepository> _creditMasterRepositoryMock = new Mock<ICreditMasterRepository>();
        private readonly Mock<ICreditNotificationRepository> _creditNotificationRepositoryMock = new Mock<ICreditNotificationRepository>();

        private ICreditCustomerService CreditCustomerService =>
            new CreditCustomerService(_customerServiceMock.Object,
                _creditMasterRepositoryMock.Object,
                _creditNotificationRepositoryMock.Object);

        public CreditCustomerServiceTest()
        {
        }

        [Fact]
        public async Task ShouldSendMigrationHistory()
        {
            CreditCustomerMigrationHistoryRequest creditCustomerMigrationHistoryRequest = new CreditCustomerMigrationHistoryRequest
            {
                DocumentType = "CC",
                IdDocument = "102355468"
            };

            Customer customer = CustomerHelperTest.GetCustomer();

            List<CreditMaster> creditsMasters = CreditMasterHelperTest.GetCreditMasterList(customer.Id);

            creditsMasters.ForEach(creditMaster => CreditMasterHelperTest.AddCountPayments(creditMaster, 6));

            _customerServiceMock.Setup(mock => mock.GetAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithTransactionsAsync(It.IsAny<Customer>(),
                    It.IsAny<IEnumerable<Statuses>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditsMasters);

            await CreditCustomerService.SendMigrationHistoryAsync(creditCustomerMigrationHistoryRequest);

            _creditNotificationRepositoryMock.Verify(mock => mock.SendEventAsync(It.IsAny<CreditMaster>(), It.IsAny<Customer>(), It.IsAny<Store>(),
                It.IsAny<Credit>(), It.IsAny<TransactionType>(), It.IsAny<string>()),
                Times.Exactly(42));
        }
    }
}