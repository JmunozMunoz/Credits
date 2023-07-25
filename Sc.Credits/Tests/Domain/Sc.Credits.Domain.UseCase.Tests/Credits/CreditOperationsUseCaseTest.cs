using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.UseCase.Credits;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Customers;
using System.Linq;

namespace Sc.Credits.Domain.UseCase.Tests.Credits
{
    public class CreditOperationsUseCaseTest
    {
        public CreditOperationsUseCase creditOperations;

        public CreditOperationsUseCaseTest()
        {
            creditOperations = new CreditOperationsUseCase();
        }

        [Fact]
        public void ShouldCalculateFeeTypeA()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("99656586", 20000, 0.10M, "Store Test", true, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 4, 8, 60000M, 300000M, 100000);
            Customer customer = CustomerHelperTest.GetCustomerCustom(500000M, 500000, DownPayments.Never);
            decimal creditValue = 250000;
            decimal expectedTotalFeeValue = 73252.36M;

            CreditDetailDomainRequest detailRequest = new CreditDetailDomainRequest(customer, store, creditValue, 30, parameters);
            detailRequest.SetFees(4);

            detailRequest.SetdownPayment(0);

            //Act
            CreditDetailResponse result = creditOperations.CreateCreditDetails(detailRequest);

            //Assert
            Assert.IsType<CreditDetailResponse>(result);
            Assert.Equal(expectedTotalFeeValue, result.TotalFeeValue);
        }

        [Fact]
        public void ShouldCalculateFeeTypeAWithoutInterest()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("99656586", 20000, 0.10M, "Store Test", true, AssuranceTypes.TypeA, "vendorId", effectiveAnualRate: 0, "Default", 4, 8, 60000M, 300000M, 100000);
            Customer customer = CustomerHelperTest.GetCustomerCustom(500000M, 500000, DownPayments.Never);

            decimal expectedTotalFeeValue = 69937.50M;
            decimal expectedTotalInterestValue = 0;

            CreditDetailDomainRequest detailRequest = new CreditDetailDomainRequest(customer, store, 250000, 30, parameters);
            detailRequest.SetFees(4);

            detailRequest.SetdownPayment(0);

            //Act
            CreditDetailResponse result = creditOperations.CreateCreditDetails(detailRequest);

            //Assert
            Assert.IsType<CreditDetailResponse>(result);
            Assert.Equal(expectedTotalFeeValue, result.TotalFeeValue);
            Assert.Equal(expectedTotalInterestValue, result.TotalInterestValue);
        }

        [Fact]
        public void ShouldCalculateFeeTypeAWithoutInterest_DownPayment()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("99656586", 20000, 0.10M, "Store Test", true, AssuranceTypes.TypeA, "vendorId", effectiveAnualRate: 0, "Default", 4, 8, 60000M, 300000M, 100000);
            Customer customer = CustomerHelperTest.GetCustomerCustom(500000M, 500000, DownPayments.Always);
            decimal creditValue = 250000;

            decimal expectedTotalFeeValue = 62200.0M;
            decimal expectedTotalInterestValue = 0;

            CreditDetailDomainRequest detailRequest = new CreditDetailDomainRequest(customer, store, creditValue, 30, parameters);
            detailRequest.SetFees(4);

            detailRequest.SetdownPayment(store.DownPayment(creditValue));

            //Act
            CreditDetailResponse result = creditOperations.CreateCreditDetails(detailRequest);

            //Assert
            Assert.IsType<CreditDetailResponse>(result);
            Assert.Equal(expectedTotalFeeValue, result.TotalFeeValue);
            Assert.Equal(expectedTotalInterestValue, result.TotalInterestValue);
        }

        [Fact]
        public void ShouldCalculateTotalPaymentValue_DownPayment()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("99656586", 20000, 0.10M, "Store Test", true, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 4, 8, 60000M, 300000M, 100000);
            decimal creditValue = 250000;

            decimal expectedTotalPaymentValue = 291683.52M;

            Customer customer = CustomerHelperTest.GetCustomerCustom(500000M, 500000, DownPayments.Always);

            CreditDetailDomainRequest detailRequest = new CreditDetailDomainRequest(customer, store, creditValue, 30, parameters);
            detailRequest.SetFees(4);

            detailRequest.SetdownPayment(store.DownPayment(creditValue));

            //Act
            CreditDetailResponse result = creditOperations.CreateCreditDetails(detailRequest);

            //Assert
            Assert.IsType<CreditDetailResponse>(result);
            Assert.Equal(expectedTotalPaymentValue, result.TotalPaymentValue);
        }

        [Fact]
        public void ShouldCalculateTotalPaymentValue_NoDownPayment()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("99656586", 20000, 0.10M, "Store Test", true, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 4, 8, 60000M, 300000M, 100000);
            Customer customer = CustomerHelperTest.GetCustomerCustom(500000M, 500000, DownPayments.Never);
            decimal creditValue = 250000;

            decimal expectedTotalPaymentValue = 293009.44M;

            CreditDetailDomainRequest detailRequest = new CreditDetailDomainRequest(customer, store, creditValue, 30, parameters);
            detailRequest.SetFees(4);

            detailRequest.SetdownPayment(0);

            //Act
            CreditDetailResponse result = creditOperations.CreateCreditDetails(detailRequest);

            //Assert
            Assert.IsType<CreditDetailResponse>(result);
            Assert.Equal(expectedTotalPaymentValue, result.TotalPaymentValue);
        }
    }
}