using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helpers.Commons.Extensions;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.UseCase.Refinancings
{
    /// <summary>
    /// Refinancing uses case is an implementation of <see cref="IRefinancingUsesCase"/>
    /// </summary>
    public class RefinancingUsesCase
        : IRefinancingUsesCase
    {
        private readonly ICreditUsesCase _creditUsesCase;
        private readonly ICreditPaymentUsesCase _creditPaymentUsesCase;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Creates a new instance of <see cref="RefinancingUsesCase"/>
        /// </summary>
        /// <param name="creditUsesCase"></param>
        /// <param name="creditPaymentUsesCase"></param>
        /// <param name="appSettings"></param>
        public RefinancingUsesCase(ICreditUsesCase creditUsesCase,
            ICreditPaymentUsesCase creditPaymentUsesCase,
            ISettings<CredinetAppSettings> appSettings)
        {
            _creditUsesCase = creditUsesCase;
            _creditPaymentUsesCase = creditPaymentUsesCase;
            _credinetAppSettings = appSettings.Get();
        }

        #region IRefinancingUsesCase Members

        /// <summary>
        /// <see cref="IRefinancingUsesCase.CustomerCredits(Customer, List{CreditMaster}, RefinancingApplication, AppParameters)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="activeCredits"></param>
        /// <param name="application"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CustomerCreditsResponse CustomerCredits(Customer customer, List<CreditMaster> activeCredits,
            RefinancingApplication application, AppParameters parameters)
        {
            List<CreditMaster> arrearsCredits = GetArrearsCredits(activeCredits, parameters);

            List<CreditMaster> filteredArrearsCredits = FilterCreditsByApplicationParams(arrearsCredits, application,
                parameters);

            if (!filteredArrearsCredits.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);
            }

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            return new CustomerCreditsResponse
            {
                CustomerEmail = customer.GetEmail,
                CustomerFullName = customer.GetFullName,
                CustomerFirstName = customer.Name.GetFirstName,
                CustomerSecondName = customer.Name.GetSecondName,
                CutomerMobile = customer.GetMobile,
                Details =
                    filteredArrearsCredits
                        .Select(creditMaster =>
                        {
                            PaymentAlternatives paymentAlternatives = _creditPaymentUsesCase.GetPaymentAlternatives(creditMaster, creditMaster.Current, parameters,
                                calculationDate: RefinancingParams.CalculationDate);

                            return new CustomerCreditDetailResponse
                            {
                                ArrearsDays = (int)creditMaster.Current.GetArrearsDays,
                                CreationDate = creditMaster.GetCreditDate,
                                Id = creditMaster.Id,
                                StoreName = creditMaster.Store.StoreName,
                                TotalValuePaid = paymentAlternatives.TotalPayment.Round(decimalNumbersRound)
                            };
                        })
                        .ToList()
            };
        }

        /// <summary>
        /// <see cref="IRefinancingUsesCase.FeesResponse(Customer, Store, List{CreditMaster}, AppParameters, RefinancingApplication)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="refinancingCreditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        public CalculateFeesResponse FeesResponse(Customer customer, Store store, List<CreditMaster> refinancingCreditMasters,
            AppParameters parameters, RefinancingApplication application)
        {
            ValidateRefinancingCredits(refinancingCreditMasters, parameters, application);

            ValidateCreditValue(out decimal creditValue, refinancingCreditMasters, parameters, application);

            customer.SetCreditLimitValidation(false);

            int decimalNumbersRound = parameters.DecimalNumbersRound;
            decimal partialCreditLimit = parameters.PartialCreditLimit;

            int limitMonths = _creditUsesCase.GetTimeLimitInMonths(customer, creditValue, store, decimalNumbersRound, partialCreditLimit);

            int[] fees = Array.ConvertAll(_credinetAppSettings.RefinancingFeesAllowed.Split(','),
                fee => int.Parse(fee));

            return new CalculateFeesResponse
            {
                CreditValue = creditValue.Round(decimalNumbersRound),
                NextPaymentDate = _creditUsesCase.GetNextFeeDateByFrequency(fee: 1, RefinancingParams.DefaultFrequency,
                    firstFeeDate: RefinancingParams.CalculationDate),
                Fees =
                    fees
                    .Where(fee => fee <= limitMonths)
                    .Select(fee =>
                    {
                        CreditDetailDomainRequest detailRequest = new CreditDetailDomainRequest(
                                customer, store, creditValue, (int)RefinancingParams.DefaultFrequency, parameters);
                        detailRequest.SetFees(fee);

                        CreditDetailResponse creditDetailResponse = _creditUsesCase.GetCreditDetails(detailRequest);

                        return new FeeResponse
                        {
                            Fees = fee,
                            FeeValue = creditDetailResponse.TotalFeeValue
                        };
                    })
                    .ToList()
            };
        }

        /// <summary>
        /// Refinance
        /// </summary>
        /// <param name="refinancingDomainRequest"></param>
        /// <param name="transaction"></param>
        /// <param name="setCreditLimit"></param>
        /// <returns></returns>
        public async Task<RefinancingCreditResponse> RefinanceAsync(RefinancingDomainRequest refinancingDomainRequest,
            Transaction transaction, bool setCreditLimit = true)
        {
            (List<PaymentCreditResponse> Payments, decimal CreditValue) = await PayOffCreditsAsync(refinancingDomainRequest, transaction, setCreditLimit);

            refinancingDomainRequest.CreateCreditDomainRequest.SetCreditValue(CreditValue);

            CreateCreditResponse createCreditResponse = await _creditUsesCase.CreateAsync(refinancingDomainRequest.CreateCreditDomainRequest, transaction, setCreditLimit);

            RefinancingCreditResponse refinancingCreditResponse = RefinancingCreditResponse.FromCreateCredit(createCreditResponse, Payments);

            return refinancingCreditResponse;
        }

        /// <summary>
        /// <see cref="IRefinancingUsesCase.CreateLog(RefinancingDomainRequest, RefinancingCreditResponse)"/>
        /// </summary>
        /// <returns></returns>
        public RefinancingLog CreateLog(RefinancingDomainRequest refinancingDomainRequest, RefinancingCreditResponse refinancingCreditResponse)
        {
            RefinancingLog log = new RefinancingLog(refinancingDomainRequest.Application.Id, refinancingCreditResponse.CreditId,
                    refinancingCreditResponse.CreditValue, refinancingDomainRequest.Store.Id, refinancingDomainRequest.Customer.Id)
                .SetAdditionalInfo(refinancingDomainRequest.RefinancingCreditRequest.ReferenceText,
                    refinancingDomainRequest.RefinancingCreditRequest.ReferenceCode,
                    new UserInfo(refinancingDomainRequest.RefinancingCreditRequest.UserName,
                        refinancingDomainRequest.RefinancingCreditRequest.UserId));

            log.SetDetails(refinancingCreditResponse.PaymentCreditResponses
                .Select(paymentCreditResponse =>
                    log.CreateDetail(paymentCreditResponse.CreditMaster.Id, paymentCreditResponse.Credit.Id,
                        paymentCreditResponse.TotalValuePaid, paymentCreditResponse.BalanceRefinanced))
                .ToList());

            return log;
        }

        #endregion IRefinancingUsesCase Members

        #region Private

        /// <summary>
        /// Get arrears credits
        /// </summary>
        /// <param name="activeCredits"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private List<CreditMaster> GetArrearsCredits(List<CreditMaster> activeCredits, AppParameters parameters)
        {
            int arrearsGracePeriod = parameters.ArrearsGracePeriod;

            List<CreditMaster> arrearsCredits = activeCredits
                .Where(creditMaster =>
                    _creditUsesCase.HasArrears(calculationDate: RefinancingParams.CalculationDate, creditMaster.Current.CreditPayment.GetDueDate,
                        arrearsGracePeriod))
                .ToList();

            return SetCreditsArrearsDays(arrearsCredits);
        }

        /// <summary>
        /// Set credits arrears days
        /// </summary>
        /// <param name="arrearsCredits"></param>
        /// <returns></returns>
        private List<CreditMaster> SetCreditsArrearsDays(List<CreditMaster> arrearsCredits)
            =>
            arrearsCredits.Select(creditMaster =>
            {
                creditMaster.Current.SetArrearsDays(_creditUsesCase.GetArrearsDays(calculationDate: RefinancingParams.CalculationDate,
                                                    creditMaster.Current.CreditPayment.GetDueDate));

                return creditMaster;
            })
            .ToList();

        /// <summary>
        /// Filter credits by application params
        /// </summary>
        /// <param name="arrearsCredits"></param>
        /// <param name="application"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private List<CreditMaster> FilterCreditsByApplicationParams(List<CreditMaster> arrearsCredits, RefinancingApplication application,
            AppParameters parameters)
            =>
            arrearsCredits
                .Where(creditMaster =>
                {
                    decimal totalPayment = _creditPaymentUsesCase.GetPaymentAlternatives(creditMaster, creditMaster.Current,
                        parameters, calculationDate: RefinancingParams.CalculationDate).TotalPayment;

                    return application.CreditIsValid(creditMaster, totalPayment.Round(parameters.DecimalNumbersRound),
                        _credinetAppSettings);
                })
            .ToList();

        /// <summary>
        /// Validate credit value
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="refinancingCreditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="application"></param>
        private void ValidateCreditValue(out decimal creditValue, List<CreditMaster> refinancingCreditMasters, AppParameters parameters,
            RefinancingApplication application)
        {
            creditValue = CalculateCreditValue(refinancingCreditMasters, parameters, application);

            if (creditValue <= 0)
            {
                throw new BusinessException(nameof(BusinessResponse.RefinancingValueShouldBeGreaterThanZero),
                    (int)BusinessResponse.RefinancingValueShouldBeGreaterThanZero);
            }
        }

        /// <summary>
        /// Calculate credit value
        /// </summary>
        /// <param name="refinancingCreditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        public decimal CalculateCreditValue(List<CreditMaster> refinancingCreditMasters, AppParameters parameters, RefinancingApplication application)
            =>
            SetCreditsArrearsDays(refinancingCreditMasters)
                .Sum(creditMaster =>
                {
                    decimal totalPayment = _creditPaymentUsesCase.GetPaymentAlternatives(creditMaster, creditMaster.Current,
                        parameters, calculationDate: RefinancingParams.CalculationDate)
                    .TotalPayment.Round(parameters.DecimalNumbersRound);

                    application.ValidateCredit(creditMaster, totalPayment, _credinetAppSettings);

                    return totalPayment;
                });

        /// <summary>
        /// Validate refinancing credits
        /// </summary>
        /// <param name="refinancingCreditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="application"></param>
        private void ValidateRefinancingCredits(List<CreditMaster> refinancingCreditMasters, AppParameters parameters,
            RefinancingApplication application)
        {
            List<CreditMaster> validRefinancingCredits =
                FilterCreditsByApplicationParams(GetArrearsCredits(refinancingCreditMasters, parameters), application,
                    parameters);

            if (refinancingCreditMasters.Count != validRefinancingCredits.Count)
            {
                throw new BusinessException(nameof(BusinessResponse.CreditsAreNotValidForRefinancing),
                    (int)BusinessResponse.CreditsAreNotValidForRefinancing);
            }
        }

        /// <summary>
        /// Pay off credits
        /// </summary>
        /// <param name="refinancingDomainRequest"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task<(List<PaymentCreditResponse>, decimal)> PayOffCreditsAsync(RefinancingDomainRequest refinancingDomainRequest, Transaction transaction, bool setCreditLimit = true)
        {
            List<CreditMaster> refinancingCreditMasters = refinancingDomainRequest.RefinancingCreditMasters;
            AppParameters parameters = refinancingDomainRequest.Parameters;
            RefinancingApplication application = refinancingDomainRequest.Application;

            ValidateCreditValue(out decimal creditValue, refinancingCreditMasters, parameters, application);

            RefinancingCreditRequest refinancingCreditRequest = refinancingDomainRequest.RefinancingCreditRequest;

            Store store = refinancingDomainRequest.Store;

            List<PaymentCreditResponse> paymentCreditResponses = new List<PaymentCreditResponse>();
            foreach (CreditMaster creditMaster in refinancingCreditMasters)
            {
                decimal totalPayment = _creditPaymentUsesCase.GetPaymentAlternatives(creditMaster, creditMaster.Current, parameters,
                            RefinancingParams.CalculationDate).TotalPayment;

                PaymentCreditRequestComplete paymentCreditRequest =
                    new PaymentCreditRequestComplete(PaymentCreditRequest.FromRefinancingCreditRequest(refinancingCreditRequest,
                        creditMaster.Id, store.Id, totalValuePaid: totalPayment.Round(parameters.DecimalNumbersRound)));

                PaymentDomainRequest paymentDomainRequest = new PaymentDomainRequest(paymentCreditRequest, creditMaster, store,
                        parameters, _credinetAppSettings, refinancingDomainRequest.Source, setCreditLimit)
                    .SetMasters(refinancingDomainRequest.PaymentTransactionType, refinancingDomainRequest.ActiveStatus,
                        refinancingDomainRequest.PaidStatus);

                PaymentCreditResponse paymentCreditResponse = await _creditPaymentUsesCase.PayAsync(paymentDomainRequest, transaction: transaction, setCreditLimit : setCreditLimit );

                paymentCreditResponses.Add(paymentCreditResponse);
            }

            return (paymentCreditResponses, creditValue);
        }

        #endregion Private
    }
}