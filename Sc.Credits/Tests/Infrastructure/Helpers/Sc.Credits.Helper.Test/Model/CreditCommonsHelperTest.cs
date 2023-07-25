using Moq;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.UseCase.Credits;

namespace Sc.Credits.Helper.Test.Model
{
    public static class CreditCommonsHelperTest
    {
        public static CreditCommons Create(Mock<ICreditMasterRepository> creditMasterRepositoryMock,
            Mock<IRequestCancelCreditRepository> requestCancelCreditRepositoryMock,
            Mock<IRequestCancelPaymentRepository> requestCancelPaymentRepositoryMock,
            Mock<ICreditUsesCase> creditUseCaseMock,
            Mock<ICreditPaymentUsesCase> creditPaymentUseCaseMock,
            Mock<ICancelUseCase> cancelUseCaseMock,
            Mock<ICreditCommonsService> creditCommonsServiceMock) =>
            new CreditCommons(creditMasterRepositoryMock.Object,
                creditUseCaseMock.Object,
                new PaymentCommons(creditPaymentUseCaseMock.Object),
                new CancelCommons(requestCancelCreditRepositoryMock.Object,
                    requestCancelPaymentRepositoryMock.Object,
                    cancelUseCaseMock.Object),
                creditCommonsServiceMock.Object);
    }
}