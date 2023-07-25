using Moq;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sc.Credits.Domain.UseCase.Tests.Credits
{
    public class CancelUseCaseTest
    {
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();

        public ICancelUseCase CancelUseCase =>
            new CancelUseCase(_appSettingsMock.Object);

        public CancelUseCaseTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
                .Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());
        }

        [Fact]
        public void ShouldListCreditsOrderByRecentCreditDateFirst()
        {
            List<CreditMaster> creditMasterList = CreditMasterHelperTest.GetCreditMasterList(customerId: new Guid("3346A813-BE00-4C6A-9662-00563C246670"));
            creditMasterList.ForEach(creditMaster => CreditMasterHelperTest.AddRandomPayments(creditMaster));

            List<Credit> payments = creditMasterList.SelectMany(creditMaster => creditMaster.History)
                .Where(credit => credit.CreditPayment.GetTransactionTypeId == (int)TransactionTypes.Payment)
                .ToList();

            List<RequestCancelPayment> requestCancelPaymentList = RequestCancelPaymentHelperTest.GetRandomCancelPaymentRequestListWithStatus(payments)
                .Where(requestCancelPayment => requestCancelPayment.GetRequestStatusId == (int)RequestStatuses.Cancel)
                .ToList();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            List<DateTime> expectedDates = new List<DateTime>();

            foreach (var creditMaster in creditMasterList.OrderByDescending(item => item.GetCreditDateComplete))
            {
                expectedDates.Add(creditMaster.GetCreditDate);
            }

            List<ActivePendingCancellationCreditResponse> actualResponse = CancelUseCase.GetActiveAndPendingCancellationCredits(
                new ActiveAndPendingCancellationCreditsRequest(creditMasterList, requestCancelPaymentList), parameters);

            Assert.Equal(expectedDates, actualResponse.Select(actual => actual.CreateDate));
        }

        [Fact]
        public void ShouldIndicateIfCreditsHasPayments()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStore();
            List<CreditMaster> creditMasterList = CreditMasterHelperTest.GetCreditMasterList(customerId: new Guid("3346A813-BE00-4C6A-9662-00563C246670"));

            Enumerable.Range(1, 2).ToList().ForEach(x =>
            {
                CreditMasterHelperTest.AddTransaction(creditMasterList[1], TransactionTypes.Payment, store, Statuses.Active, DateTime.Now);
                CreditMasterHelperTest.AddTransaction(creditMasterList[3], TransactionTypes.Payment, store, Statuses.Active, DateTime.Now);
                CreditMasterHelperTest.AddTransaction(creditMasterList[5], TransactionTypes.Payment, store, Statuses.Active, DateTime.Now);
            });

            CreditMasterHelperTest.AddTransaction(creditMasterList[2], TransactionTypes.Payment, store, Statuses.Active, DateTime.Now);
            CreditMasterHelperTest.AddTransaction(creditMasterList[5], TransactionTypes.Payment, store, Statuses.Active, DateTime.Now);

            List<RequestCancelPayment> requestCancelPaymentsCalceled = new List<RequestCancelPayment>
            {
                RequestCancelPaymentHelperTest.GetCancelPaymentRequestWithStatus(creditMasterList[1].History[2], (int)RequestStatuses.Cancel)
                    .SetPayment(creditMasterList[1].History[2]),
                RequestCancelPaymentHelperTest.GetCancelPaymentRequestWithStatus(creditMasterList[1].History[1], (int)RequestStatuses.Cancel)
                    .SetPayment(creditMasterList[1].History[1]),

                RequestCancelPaymentHelperTest.GetCancelPaymentRequestWithStatus(creditMasterList[2].History[1], (int)RequestStatuses.Pending)
                    .SetPayment(creditMasterList[2].History[1]),

                RequestCancelPaymentHelperTest.GetCancelPaymentRequestWithStatus(creditMasterList[3].History[2], (int)RequestStatuses.Cancel)
                    .SetPayment(creditMasterList[3].History[2]),
                RequestCancelPaymentHelperTest.GetCancelPaymentRequestWithStatus(creditMasterList[3].History[1], (int)RequestStatuses.Cancel)
                    .SetPayment(creditMasterList[3].History[1]),

                RequestCancelPaymentHelperTest.GetCancelPaymentRequestWithStatus(creditMasterList[5].History[3], (int)RequestStatuses.Cancel)
                    .SetPayment(creditMasterList[5].History[3]),
                RequestCancelPaymentHelperTest.GetCancelPaymentRequestWithStatus(creditMasterList[5].History[2], (int)RequestStatuses.Dismissed)
                    .SetPayment(creditMasterList[5].History[2]),
                RequestCancelPaymentHelperTest.GetCancelPaymentRequestWithStatus(creditMasterList[5].History[1], (int)RequestStatuses.Cancel)
                    .SetPayment(creditMasterList[5].History[1])
            }
            .Where(requestCancelPayment => requestCancelPayment.GetRequestStatusId == (int)RequestStatuses.Cancel)
            .ToList();

            bool[] expectedHasPaymentsResponses = { false, true, false, true, false, false };

            List<ActivePendingCancellationCreditResponse> actualResponse = CancelUseCase.GetActiveAndPendingCancellationCredits(
                new ActiveAndPendingCancellationCreditsRequest(creditMasterList, requestCancelPaymentsCalceled), parameters);

            CreditMaster currentCreditBalance = creditMasterList.FirstOrDefault();
            decimal activeAndPendingCancellationCredit = actualResponse.FirstOrDefault(x => x.CreditId == currentCreditBalance.Id).Balance;

            Assert.Equal(expectedHasPaymentsResponses, actualResponse.Select(actual => actual.HasPayments));
            Assert.Equal(activeAndPendingCancellationCredit, currentCreditBalance.Current.GetBalance);
        }

        [Fact]
        public void ShouldCancelCredit()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            Credit credit = creditMaster.Current;
            TransactionType transactionType = TransactionTypeHelperTest.GetCancelCreditType();
            Status status = StatusHelperTest.GetCalceledStatus();

            CancelUseCase.CancelCredit(
                new CancelCreditDomainRequest(creditMaster, transactionType, status, new UserInfo("Test user", "askjkds456a56s5")));

            Assert.Equal(creditMaster.GetStatusId, status.Id);
            Assert.Equal(creditMaster.Current.CreditPayment.GetTransactionTypeId, transactionType.Id);
            Assert.NotEqual(credit.Id, creditMaster.Current.Id);
        }

        [Fact]
        public void ShouldCancelPayment()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            Credit credit = creditMaster.Current;
            Credit cancelCredit = CreditMasterHelperTest.GetCreditMaster().Current;
            TransactionType transactionType = TransactionTypeHelperTest.GetCancelCreditType();
            Status statusActive = StatusHelperTest.GetActiveStatus();

            CancelUseCase.CancelPayment(new CancelPaymentDomainRequest(creditMaster, cancelCredit, transactionType, statusActive, new UserInfo("Test user", "askjkds456a56s5")), 0, false);

            Assert.Equal(creditMaster.GetStatusId, statusActive.Id);
            Assert.Equal(creditMaster.Current.GetBalance, credit.GetBalance + cancelCredit.CreditPayment.GetCreditValuePaid);
            Assert.Equal(creditMaster.Current.GetAssuranceBalance, credit.GetAssuranceBalance + cancelCredit.CreditPayment.GetArrearsValuePaid);
            Assert.NotEqual(credit.Id, creditMaster.Current.Id);
        }

        [Fact]
        public void ShouldShowIfPaymentsHasPendingCancelRequest()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStore();

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterList(new Guid("3346A813-BE00-4C6A-9662-00563C246670")).FirstOrDefault();
            CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, store, Statuses.Active, DateTime.Now);
            CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, store, Statuses.Active, DateTime.Now);
            CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, store, Statuses.Active, DateTime.Now);

            List<RequestCancelPayment> requestCancelPaymentList = new List<RequestCancelPayment>
            {
                RequestCancelPaymentHelperTest.GetCancelPaymentRequestWithStatus(creditMaster.History[2], (int)RequestStatuses.Pending)
                    .SetPayment(creditMaster.History[2]),
            };

            bool[] expectedHasCancelRequestResponses = { false, true, false };

            List<ActivePendingCancellationPaymentResponse> actualResponse =
                CancelUseCase.GetActiveAndPendingCancellationPayments(
                    new ActiveAndPendingCancellationPaymentsRequest(
                        new List<CreditMaster>
                        {
                            creditMaster
                        },
                    requestCancelPaymentList,
                    store.Id),
                parameters);

            Assert.Equal(expectedHasCancelRequestResponses, actualResponse.Select(actual => actual.HasCancelRequest));
        }

        [Fact]
        public void ShouldShowOnlyPaymentsMadeInTheStore()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterList(new Guid("3346A813-BE00-4C6A-9662-00563C246670")).FirstOrDefault();
            string expectedStoreId = "RandomId123";
            Store store = StoreHelperTest.GetStore(expectedStoreId);
            List<string> otherStoresIds = new List<string>() { "OtherRandomId1", "OtherRandomId2", "OtherRandomId3" };

            Enumerable.Range(1, 6).ToList().ForEach(x =>
                        CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, store, Statuses.Active, DateTime.Now.AddDays(-x)));

            otherStoresIds.ForEach(otherStoreId =>
                CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, StoreHelperTest.GetStore(otherStoreId), Statuses.Active, DateTime.Now));

            List<ActivePendingCancellationPaymentResponse> actualResponses = CancelUseCase.GetActiveAndPendingCancellationPayments(
                new ActiveAndPendingCancellationPaymentsRequest(new List<CreditMaster> { creditMaster }, new List<RequestCancelPayment>() { }, expectedStoreId),
                ParameterHelperTest.GetAppParameters());

            List<string> actualStoreIds = actualResponses.Select(actualResponse => actualResponse.StoreId).ToList();

            Assert.Equal(6, actualResponses.Count);
            Assert.All(actualStoreIds, actualStoreId => Assert.Equal(expectedStoreId, actualStoreId));
        }

        [Fact]
        public void ShouldGetActiveAndPendingCancellationPaymentsWhithCancelableCredit()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterList(new Guid("3346A813-BE00-4C6A-9662-00563C246670")).FirstOrDefault();
            Store store = StoreHelperTest.GetStore();
            Enumerable.Range(1, 6).ToList().ForEach(x =>
                        CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, store, Statuses.Active, DateTime.Now.AddDays(-x)));

            Credit expectedCancelableCredit = creditMaster.History.Where(h => h.CreditPayment.GetTransactionTypeId == (int)TransactionTypes.Payment).OrderByDescending(h => h.GetTransactionDateComplete).First();

            List<ActivePendingCancellationPaymentResponse> actualResponses = CancelUseCase.GetActiveAndPendingCancellationPayments(
                new ActiveAndPendingCancellationPaymentsRequest(
                    new List<CreditMaster>
                    {
                        creditMaster
                    },
                new List<RequestCancelPayment>()
                {
                },
                store.Id),
                ParameterHelperTest.GetAppParameters());

            Assert.Equal(1, actualResponses.Count(r => r.Cancelable));
            Assert.Equal(expectedCancelableCredit.Id, actualResponses.First(r => r.Cancelable).PaymentId);
        }

        [Fact]
        public void ShouldUpdateStatusRequestPaymentCancel()
        {
            RequestStatuses expectedRequestStatuses = RequestStatuses.Pending;

            RequestCancelPayment requestCancelPayment = RequestCancelPaymentHelperTest.GetRequestCancelPayment(new Guid("33a286f9-50ab-ce2c-253b-08d79f81e975"));

            CancelUseCase.UpdateStatusRequestPaymentCancel(expectedRequestStatuses, requestCancelPayment, new UserInfo("OtherUserName", "OtherUserId"));

            Assert.Equal((int)expectedRequestStatuses, requestCancelPayment.GetRequestStatusId);
        }

        [Fact]
        public void ShouldUpdateStatusRequestCreditCancel()
        {
            RequestStatuses expectedRequestStatuses = RequestStatuses.Pending;

            RequestCancelCredit requestCancelCredit = RequestCancelCreditHelperTest.GetRequestCancelCredit(new Guid("33a286f9-50ab-ce2c-253b-08d79f81e975"));

            CancelUseCase.UpdateStatusRequestCreditCancel(expectedRequestStatuses, requestCancelCredit, new UserInfo("OtherUserName", "OtherUserId"));

            Assert.Equal((int)expectedRequestStatuses, requestCancelCredit.GetRequestStatusId);
        }
    }
}