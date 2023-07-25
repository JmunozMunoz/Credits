using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Credits
{
    public class PartialCancelationCreditServiceTest
    {
        private readonly Mock<IRequestCancelCreditRepository> _requestCancelCreditRepositoryMock = new Mock<IRequestCancelCreditRepository>();
        private readonly Mock<ICreditUsesCase> _creditUsesCaseMock = new Mock<ICreditUsesCase>();
        private readonly Mock<ICreditMasterRepository> _creditMasterRepositoryMock = new Mock<ICreditMasterRepository>();
        private readonly Mock<ICreditCommonsService> _creditCommonServiceMock = new Mock<ICreditCommonsService>();
        private readonly Mock<ICreditPaymentService> _creditPaymentServiceMock = new Mock<ICreditPaymentService>();
        private readonly Mock<CredinetAppSettings> _appSettingsMock = new Mock<CredinetAppSettings>();
        private readonly Mock<ILoggerService<PartialCancelationCreditService>> _loggerServiceMock = new Mock<ILoggerService<PartialCancelationCreditService>>();
        private readonly Mock<IRequestCancelPaymentRepository> _requestCancelPaymentRepositoryMock = new Mock<IRequestCancelPaymentRepository>();
        private readonly Mock<ICancelUseCase> _cancelUseCaseMock = new Mock<ICancelUseCase>();

        private CreditCommons CreditCommons =>
           CreditCommonsHelperTest.Create(_creditMasterRepositoryMock,
               _requestCancelCreditRepositoryMock,
               _requestCancelPaymentRepositoryMock,
               _creditUsesCaseMock,
               new Mock<ICreditPaymentUsesCase>(),
               _cancelUseCaseMock,
               _creditCommonServiceMock);

        private IPartialCancelationCreditService _partialCancelationCreditService =>
            new PartialCancelationCreditService(CreditCommons, _creditPaymentServiceMock.Object, _loggerServiceMock.Object);

        public PartialCancelationCreditServiceTest()
        {
            _creditCommonServiceMock.SetupGet(mock => mock.Commons.CredinetAppSettings)
             .Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());
        }

        [Fact]
        public async Task ApproveCancellationAsync()
        {
            //Arrange

            RequestCancelCredit requestCancelCredit = RequestCancelCreditHelperTest.GetRequestCancelCredit();
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            creditMaster.SetStatusId((int)Statuses.CancelRequest);

            _creditPaymentServiceMock.Setup(mock => mock.PayCreditAsync(It.IsAny<PaymentCreditRequest>(), It.IsAny<bool>(),
                                                                       It.IsAny<AppParameters>(), It.IsAny<Transaction>(), It.IsAny<bool>()));

            _requestCancelCreditRepositoryMock.Setup(mock => mock.UpdateAsync(It.IsAny<RequestCancelCredit>(), It.IsAny<IEnumerable<Field>>(),
                                                                             It.IsAny<Transaction>()));

            //Act

            await _partialCancelationCreditService.ApproveCancellationAsync(new UserInfo("CancelacionParcial", "sfsv23e"), requestCancelCredit, creditMaster);

            //Assert

            _creditUsesCaseMock.Verify(mock => mock.UpdateStatus(It.IsAny<int>(), It.IsAny<CreditMaster>()), Times.Once);

            _creditMasterRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                                                                             It.IsAny<Transaction>()), Times.Once);

            _creditPaymentServiceMock.Verify(mock => mock.PayCreditAsync(It.IsAny<PaymentCreditRequest>(), It.IsAny<bool>(),
                                                                      It.IsAny<AppParameters>(), It.IsAny<Transaction>(), It.IsAny<bool>()), Times.Once);

            _requestCancelCreditRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<RequestCancelCredit>(), It.IsAny<IEnumerable<Field>>(),
                                                                             It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task ApproveCancellationAsyncThyowException()
        {
            //Arrange

            RequestCancelCredit requestCancelCredit = RequestCancelCreditHelperTest.GetRequestCancelCredit();
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            creditMaster.SetStatusId((int)Statuses.CancelRequest);

            _creditPaymentServiceMock.Setup(mock => mock.PayCreditAsync(It.IsAny<PaymentCreditRequest>(), It.IsAny<bool>(),
                                                                       It.IsAny<AppParameters>(), It.IsAny<Transaction>(), It.IsAny<bool>()));

            _requestCancelCreditRepositoryMock.Setup(mock => mock.UpdateAsync(It.IsAny<RequestCancelCredit>(), It.IsAny<IEnumerable<Field>>(),
                                                                             It.IsAny<Transaction>())).Throws(new Exception());

            //Act

            await _partialCancelationCreditService.ApproveCancellationAsync(new UserInfo("CancelacionParcial", "sfsv23e"), requestCancelCredit, creditMaster);

            //Assert

            _loggerServiceMock.Verify(mock => mock.LogError(It.IsAny<string>(), It.IsAny<string>(),
                                                            It.IsAny<object>(), It.IsAny<Exception>()), Times.Once);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(),
                                                              It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(400000)]
        public async Task ValidationToPartiallyCancelACreditWithBusinessException(decimal valueCancel)
        {
            //Arrange

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            PartialCancellationRequest partialCancellationRequest = new PartialCancellationRequest()
            {
                CreditId = Guid.NewGuid(),
                ValueCancel = valueCancel
            };

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            //Act
            BusinessException exception = await Assert.ThrowsAnyAsync<BusinessException>(() =>
                                              _partialCancelationCreditService.GetValidationToPartiallyCancelACreditAsync(partialCancellationRequest));

            //Assert

            Assert.Equal((int)BusinessResponse.PartialCancellationValueIsInvalid, exception.code);
        }

        [Theory]
        [InlineData(10000)]
        [InlineData(40000)]
        public async Task ValidationToPartiallyCancelACreditWithStatus200(decimal valueCancel)
        {
            //Arrange

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            PartialCancellationRequest partialCancellationRequest = new PartialCancellationRequest()
            {
                CreditId = Guid.NewGuid(),
                ValueCancel = valueCancel
            };

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            //Act
            var result = await _partialCancelationCreditService.GetValidationToPartiallyCancelACreditAsync(partialCancellationRequest);

            //Assert

            Assert.True(result);
        }
    }
}