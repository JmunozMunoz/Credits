using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Builders;
using Sc.Credits.Domain.Model.Credits.Events;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries.Commands;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Domain.Model.Customers.Queries.Commands;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Sequences.Gateway;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.UseCase.Credits.Strategy.Payment;
using Sc.Credits.Helpers.Commons.Extensions;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.UseCase.Credits
{
    /// <summary>
    /// Credit payment uses case is an implementation of <see cref="ICreditPaymentUsesCase"/>
    /// </summary>
    public class CreditPaymentUsesCase : ICreditPaymentUsesCase
    {
        private readonly ICreditUsesCase _creditsUseCase;
        private readonly ICreditMasterRepository _creditMasterRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISequenceRepository _sequenceRepository;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Creates a new instance of <see cref="CreditPaymentUsesCase"/>
        /// </summary>
        /// <param name="creditsUseCase"></param>
        /// <param name="creditMasterRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="sequenceRepository"></param>
        /// <param name="appSettings"></param>
        public CreditPaymentUsesCase(ICreditUsesCase creditsUseCase,
            ICreditMasterRepository creditMasterRepository,
            ICustomerRepository customerRepository,
            ISequenceRepository sequenceRepository,
            ISettings<CredinetAppSettings> appSettings)
        {
            _creditsUseCase = creditsUseCase;
            _creditMasterRepository = creditMasterRepository;
            _customerRepository = customerRepository;
            _sequenceRepository = sequenceRepository;
            _credinetAppSettings = appSettings.Get();
        }

        /// <summary>
        /// Pay
        /// </summary>
        /// <param name="paymentDomainRequest"></param>
        /// <param name="paymentType"></param>
        /// <param name="transaction"></param>
        /// <param name="simulation"></param>
        /// <param name="setCreditLimit"></param>
        /// <returns></returns>
        /// <exception cref="credinet.exception.middleware.models.BusinessException">
        /// CreditNotActive
        /// or
        /// PaymentExceedsTotalPayment
        /// or
        /// PaymentUnauthorized
        /// </exception>
        public async Task<PaymentCreditResponse> PayAsync(PaymentDomainRequest paymentDomainRequest,
                                                          PaymentType paymentType = null,
                                                          Transaction transaction = null,
                                                          bool simulation = false,
                                                          bool setCreditLimit = true)
        {
            CreditMaster creditMaster = paymentDomainRequest.CreditMaster;

            if (!creditMaster.IsActive())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditNotActive), (int)BusinessResponse.CreditNotActive);
            }

            PaymentCreditRequestComplete paymentCreditRequest = paymentDomainRequest.PaymentCreditRequest;
            Credit credit = creditMaster.Current;

            ValidatePaymentDate(paymentCreditRequest, credit);

            AppParameters parameters = paymentDomainRequest.Parameters;

            decimal totalPayment =
                GetPaymentAlternatives(creditMaster, credit, parameters, paymentCreditRequest.CalculationDate).TotalPayment;

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            totalPayment = totalPayment.Round(decimalNumbersRound);

            if (paymentCreditRequest.TotalValuePaid > totalPayment)
            {
                throw new BusinessException(nameof(BusinessResponse.PaymentExceedsTotalPayment), (int)BusinessResponse.PaymentExceedsTotalPayment);
            }

            Store store = paymentDomainRequest.Store;

            if (store.PaymentUnautorized)
            {
                throw new BusinessException(nameof(BusinessResponse.PaymentUnauthorized), (int)BusinessResponse.PaymentUnauthorized);
            }

            long paymentNumber = 0;

            if (simulation == false)
            {
                string[] storesWithoutPaymentNumber = _credinetAppSettings.StoresWithoutPaymentNumber.ToLower().Split(',');

                paymentNumber = storesWithoutPaymentNumber.Contains(paymentDomainRequest.PaymentCreditRequest.StoreId.ToLower())
                                     ? 0
                                     : await _sequenceRepository.GetNextAsync(store.Id, nameof(SequenceTypes.Payments));
            }

            AddPayment(paymentDomainRequest, paymentNumber, totalPayment, paymentType);

            if (simulation == false)
            {
                try
                {
                    await _creditMasterRepository.AddTransactionAsync(creditMaster, transaction: transaction,
                    additionalMasterUpdateFields: CreditMasterCommandsFields.PayCredit);
                }
                catch (BusinessException be)
                {
                    if (be.code != (int)BusinessResponse.DuplicatedPaymentNumber)
                    {
                        throw;
                    }
                    paymentNumber = await _sequenceRepository.GetNextAsync(store.Id, nameof(SequenceTypes.Payments));

                    AddPayment(paymentDomainRequest, paymentNumber, totalPayment, paymentType, setCreditLimit);

                    await _creditMasterRepository.AddTransactionAsync(creditMaster, transaction: transaction,
                        additionalMasterUpdateFields: CreditMasterCommandsFields.PayCredit);
                }

                if (creditMaster.Customer.CreditLimitIsUpdated())
                {
                    await _customerRepository.UpdateAsync(creditMaster.Customer, CustomerCommandsFields.UpdateCreditLimit, transaction);
                }
            }

            Credit currentCredit = creditMaster.Current;

            bool paidCredit = creditMaster.Paid();

            DateTime nextMinimumPaymentCalculationDate = currentCredit.CreditPayment.GetDueDate > paymentCreditRequest.CalculationDate ?
                currentCredit.CreditPayment.GetDueDate : paymentCreditRequest.CalculationDate;

            decimal nextMinimumPayment = paidCredit ? 0 :
                GetPaymentAlternatives(creditMaster, currentCredit, parameters,
                    nextMinimumPaymentCalculationDate).MinimumPayment.Round(decimalNumbersRound);

            return new PaymentCreditResponse
            {
                ArrearsValuePaid = currentCredit.CreditPayment.GetArrearsValuePaid.Round(decimalNumbersRound),
                AssuranceValuePaid = currentCredit.CreditPayment.GetAssuranceValuePaid.Round(decimalNumbersRound),
                Balance = currentCredit.GetBalance.Round(decimalNumbersRound),
                ChargeValuePaid = currentCredit.CreditPayment.GetChargeValuePaid.Round(decimalNumbersRound),
                CreditId = creditMaster.Id,
                CreditValuePaid = currentCredit.CreditPayment.GetCreditValuePaid.Round(decimalNumbersRound),
                IdDocument = creditMaster.Customer.IdDocument,
                InterestValuePaid = currentCredit.CreditPayment.GetInterestValuePaid.Round(decimalNumbersRound),
                NextDueDate = paidCredit ? null : (DateTime?)currentCredit.CreditPayment.GetDueDate,
                NextMinimumPayment = nextMinimumPayment,
                PaymentId = currentCredit.Id,
                PaymentNumber = currentCredit.CreditPayment.GetPaymentNumber,
                TypeDocument = creditMaster.Customer.DocumentType,
                PaidCredit = paidCredit,
                HasCharges = currentCredit.HasCharges(),
                CreditMaster = creditMaster,
                Credit = currentCredit,
                Store = store,
                TotalValuePaid = paymentCreditRequest.TotalValuePaid,
                BalanceRefinanced = credit.GetBalance.Round(decimalNumbersRound)
            };
        }

        /// <summary>
        /// Add payment
        /// </summary>
        /// <param name="paymentDomainRequest"></param>
        /// <param name="paymentNumber"></param>
        /// <param name="totalPayment"></param>
        /// <param name="paymentType"></param>
        private void AddPayment(PaymentDomainRequest paymentDomainRequest,
                                long paymentNumber,
                                decimal totalPayment,
                                PaymentType paymentType = null,
                                bool setCreditLimit = true)
        {
            CreditMaster creditMaster = paymentDomainRequest.CreditMaster;
            Store store = paymentDomainRequest.Store;
            paymentType = paymentType ?? store.PaymentType;
            bool increaseCreditLimit = false;

            PaymentDetail paymentDetail = GetPaymentDetailByPaymentType(paymentType, creditMaster, paymentDomainRequest.PaymentCreditRequest.TotalValuePaid,
                totalPayment, paymentDomainRequest.PaymentCreditRequest.CalculationDate, paymentDomainRequest.Parameters);

            Status status = paymentDetail.CreditPaid ? paymentDomainRequest.PaidStatus : paymentDomainRequest.ActiveStatus;

            if (paymentDetail.CreditPaid && RefinancingParams.IsAllowedSource(_credinetAppSettings, creditMaster.Current.GetSourceId))
            {
                increaseCreditLimit = creditMaster.ValidateDateOfRefinancedCredits(_credinetAppSettings, setCreditLimit);
                creditMaster.Current.SetCustomer(creditMaster.Customer, _credinetAppSettings, increaseCreditLimit);
            }

            creditMaster.HandleEvent(new AddPaymentMasterEvent(creditMaster, paymentNumber, paymentDomainRequest.PaymentTransactionType,
                    paymentType, status, store, paymentDetail.InterestRate, increaseCreditLimit ? paymentDomainRequest.BalanceReleaseForRefinancing : 0)
                .AddPaymentValues(paymentDetail.PaymentValue, paymentDetail.CreditValuePayment,
                    paymentDetail.InterestValuePayment, paymentDetail.ChargesValuePayment, paymentDetail.ArrearsValuePayment,
                    paymentDetail.AssuranceValuePayment, paymentDetail.LastPaymentFee)
                .SetAdjustmentValues(paymentDetail.PreviousArrears, paymentDetail.PreviousInterest, paymentDetail.ActiveFeeValuePaid)
                .SetDates(paymentDetail.NextDueDate, paymentDomainRequest.PaymentCreditRequest.CalculationDate, paymentDetail.LastPaymentDate)
                .SetAdditionalInfo(paymentDomainRequest.PaymentCreditRequest.BankAccount, paymentDomainRequest.PaymentCreditRequest.Location,
                    new UserInfo(paymentDomainRequest.PaymentCreditRequest.UserName, paymentDomainRequest.PaymentCreditRequest.UserId))
                .SetTransactionReference(paymentDomainRequest.PaymentCreditRequest.TransactionId)
                .SetArrears(paymentDetail.ArrearsDays));
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetMinimumPayment(List{PaymentFee}, int, decimal,
        /// decimal, bool, decimal)"/>
        /// </summary>
        /// <param name="paymentFee"></param>
        /// <param name="arrearsFees"></param>
        /// <param name="totalPayment"></param>
        /// <param name="updatedPaymentPlanValue"></param>
        /// <param name="hasUpdatedPaymentPlan"></param>
        /// <param name="maximumUpdatedPaymentPlanGapRate"></param>
        /// <returns></returns>
        public decimal GetMinimumPayment(List<PaymentFee> paymentFee, int arrearsFees, decimal totalPayment,
            decimal updatedPaymentPlanValue, bool hasUpdatedPaymentPlan, decimal maximumUpdatedPaymentPlanGapRate)
        {
            if (!hasUpdatedPaymentPlan)
            {
                return paymentFee.FirstOrDefault(fee => fee.Fees == Math.Max(arrearsFees, 1))?.Payment ?? 0;
            }

            return updatedPaymentPlanValue * (1 + maximumUpdatedPaymentPlanGapRate) > totalPayment
                ? totalPayment : updatedPaymentPlanValue;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetPaymentFees(CreditMaster, Credit, AppParameters,
        /// DateTime, out decimal)"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="credit"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <param name="totalPayment"></param>
        /// <returns></returns>
        public PaymentFeesResponse GetPaymentFees(CreditMaster creditMaster, Credit credit, AppParameters parameters, DateTime calculationDate,
            out decimal totalPayment)
        {
            int decimalNumbersRound = parameters.DecimalNumbersRound;

            decimal originalInterestRate = _creditsUseCase.GetInterestRate(creditMaster.GetEffectiveAnnualRate, credit.GetFrequency).Round(parameters.InterestRateDecimalNumbers);

            AmortizationScheduleResponse amortizationSchedule = _creditsUseCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate, originalInterestRate),
                decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, credit.GetBalance, credit.GetAssuranceBalance)
                .DateInfo(calculationDate.Date, credit.CreditPayment.GetLastPaymentDate)
                .CreditParametersInfo(
                    effectiveAnnualRate: _creditsUseCase.GetValidEffectiveAnnualRate(creditMaster.GetEffectiveAnnualRate, creditMaster.Store.GetEffectiveAnnualRate),
                    parameters.ArrearsEffectiveAnnualRate, parameters.ArrearsGracePeriod)
                .ArrearsAdjustmentDate(parameters.ArrearsAdjustmentDate)
                .Build();

            decimal interestPayment = GetInterestPayment(paymentMatrix, credit.CreditPayment.GetPreviousInterest).Round(decimalNumbersRound);

            decimal arrearsPayment = GetArrearsPayment(paymentMatrix, credit.GetArrearsCharge, credit.GetHasArrearsCharge,
                credit.CreditPayment.GetPreviousArrears).Round(decimalNumbersRound);

            totalPayment = GetFullPayment(credit.GetBalance, interestPayment, arrearsPayment, credit.GetChargeValue, paymentMatrix.PayableAssurance,
                amortizationSchedule, calculationDate);

            return new PaymentFeesResponse()
            {
                PendingFees = paymentMatrix.PendingFees,
                ArrearsPayment = arrearsPayment,
                PaymentFees = CreatePaymentFees(
                    paymentMatrix,
                    firstFeeAdditions: interestPayment + arrearsPayment + credit.GetChargeValue,
                    totalPayment,
                    decimalNumbersRound
                ).ToList(),
                ArrearsFees = paymentMatrix.ArrearsFees
            };
        }

        /// <summary>
        /// Create payment fees
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="firstFeeAdditions"></param>
        /// <param name="totalPayment"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private IEnumerable<PaymentFee> CreatePaymentFees(PaymentMatrix paymentMatrix, decimal firstFeeAdditions, decimal totalPayment, int decimalNumbersRound)
        {
            int pendingFees = paymentMatrix.PendingFees;

            decimal payment = firstFeeAdditions;

            for (int feesToPay = 1; feesToPay <= pendingFees; feesToPay += 1)
            {
                PaymentMatrixFee currentFee = paymentMatrix.GetFeeByFeeNumber(paymentMatrix.LastFeePaidNumber + feesToPay);

                payment = Math.Min(
                    payment + currentFee.CreditValuePayment + currentFee.AssuranceValuePayment,
                    totalPayment
                ).Round(decimalNumbersRound);

                yield return new PaymentFee(feesToPay, payment);
            }
        }

        /// <summary>
        /// Get payment detail by payment type
        /// </summary>
        /// <param name="paymentType"></param>
        /// <param name="creditMaster"></param>
        /// <param name="totalValuePaid"></param>
        /// <param name="totalPayment"></param>
        /// <param name="calculationDate"></param>
        /// <param name="parameters"></param>
        private PaymentDetail GetPaymentDetailByPaymentType(PaymentType paymentType, CreditMaster creditMaster,
            decimal totalValuePaid, decimal totalPayment, DateTime calculationDate, AppParameters parameters)
        {
            PaymentStrategy paymentStrategy;

            switch (paymentType.Type)
            {
                case PaymentTypeTypes.DownPayment:
                    paymentStrategy = new DownPaymentStrategy(this, _creditsUseCase, creditMaster, totalValuePaid, calculationDate,
                        parameters);
                    break;

                case PaymentTypeTypes.CreditValueOnly:
                    paymentStrategy = new CreditValueOnlyPaymentStrategy(this, _creditsUseCase, creditMaster, totalValuePaid, calculationDate,
                        paymentType, parameters);
                    break;

                default:
                    paymentStrategy = new OrdinaryPaymentStrategy(this, _creditsUseCase, creditMaster, totalValuePaid, totalPayment, calculationDate,
                        parameters);
                    break;
            }

            return paymentStrategy.GetPaymentDetail();
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetAssuranceOrdinaryPayment(PaymentMatrix, decimal,
        /// AmortizationScheduleResponse, decimal, decimal, DateTime, decimal, decimal, decimal,
        /// decimal, int, decimal)"/>
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="pendingPaymentValue"></param>
        /// <param name="amortizationScheduleResponse"></param>
        /// <param name="creditValueBalance"></param>
        /// <param name="maximumPaymentAdjustmentResidue"></param>
        /// <param name="calculationDate"></param>
        /// <param name="paymentValue"></param>
        /// <param name="interestValuePayment"></param>
        /// <param name="arrearsValuePayment"></param>
        /// <param name="chargeValue"></param>
        /// <param name="arrearsGracePeriod"></param>
        /// <param name="totalPayment"></param>
        /// <returns></returns>
        public decimal GetAssuranceOrdinaryPayment(PaymentMatrix paymentMatrix, decimal pendingPaymentValue, AmortizationScheduleResponse amortizationScheduleResponse, decimal creditValueBalance, decimal maximumPaymentAdjustmentResidue,
            DateTime calculationDate, decimal paymentValue, decimal interestValuePayment, decimal arrearsValuePayment, decimal chargeValue, int arrearsGracePeriod, decimal totalPayment)
        {
            decimal payableAssurance = paymentMatrix.PayableAssurance;

            if (totalPayment == paymentValue)
            {
                return payableAssurance;
            }

            int nextLastPaymentFee = GetLastPaymentFeeFromPaymentMatrix(paymentMatrix, pendingPaymentValue);

            int lastPaymentFee = GetLastPaymentFee(amortizationScheduleResponse, creditValueBalance, out DateTime _);

            IEnumerable<PaymentMatrixFee> paymentFees = paymentMatrix.Fees.Where(fee => fee.FeeNumber > lastPaymentFee && fee.FeeNumber <= nextLastPaymentFee);

            decimal assuranceFinalPayment = paymentFees.Sum(fee => fee.AssuranceValuePayment);
            decimal creditValueEstimatedPay = paymentFees.Sum(fee => fee.CreditValuePayment);

            decimal assuranceValueNextPaymentFee =
                nextLastPaymentFee == paymentMatrix.Fees.Last().FeeNumber ? 0 :
                    paymentMatrix.Fees.First(fee => fee.FeeNumber == nextLastPaymentFee + 1).AssuranceValuePayment;

            decimal estimatedPending = pendingPaymentValue - assuranceFinalPayment - creditValueEstimatedPay;

            decimal assuranceFinalPaymentReturn = assuranceFinalPayment + Math.Min(estimatedPending, assuranceValueNextPaymentFee);

            return assuranceFinalPaymentReturn;
        }

        /// <summary>
        /// Get Interest Payment
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="previousInterest"></param>
        /// <returns></returns>
        public decimal GetInterestPayment(PaymentMatrix paymentMatrix, decimal previousInterest)
        {
            decimal interestPayment = paymentMatrix.PayableInterest - previousInterest;

            return interestPayment > 0 ? interestPayment : 0;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetArrearsPayment(PaymentMatrix, decimal, bool, decimal)"/>
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="arrearsCharges"></param>
        /// <param name="hasArrearsCharge"></param>
        /// <param name="previousArrears"></param>
        /// <returns></returns>
        public decimal GetArrearsPayment(PaymentMatrix paymentMatrix, decimal arrearsCharges, bool hasArrearsCharge, decimal previousArrears)
        {
            if (hasArrearsCharge)
                return arrearsCharges;

            decimal arrearsPayment = paymentMatrix.PayableArrears - previousArrears;

            return arrearsPayment > 0 ? arrearsPayment : 0;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetFullPayment(decimal, decimal, decimal, decimal,
        /// decimal, AmortizationScheduleResponse, DateTime)"/>
        /// </summary>
        /// <param name="creditValueBalance"></param>
        /// <param name="interestPaymentValue"></param>
        /// <param name="arrearsPayment"></param>
        /// <param name="chargesPayment"></param>
        /// <param name="payableAssurance"></param>
        /// <param name="amortizationSchedule"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        public decimal GetFullPayment(decimal creditValueBalance, decimal interestPaymentValue, decimal arrearsPayment, decimal chargesPayment,
            decimal payableAssurance, AmortizationScheduleResponse amortizationSchedule, DateTime calculationDate)
        {
            int lastPaymentFee = GetLastPaymentFee(amortizationSchedule, creditValueBalance, out DateTime nextDueDate);

            if (lastPaymentFee == 0 && calculationDate.Date < nextDueDate.Date)
            {
                return creditValueBalance;
            }

            return creditValueBalance + interestPaymentValue + arrearsPayment + chargesPayment + payableAssurance;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetFirstDueFee(int, int)"/>
        /// </summary>
        /// <param name="lastPaymentFee"></param>
        /// <param name="totalFees"></param>
        /// <returns></returns>
        public int GetFirstDueFee(int lastPaymentFee, int totalFees) => lastPaymentFee == totalFees ? 0 : lastPaymentFee + 1;

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetLastPaymentFee(AmortizationScheduleResponse,
        /// decimal, out DateTime)"/>
        /// </summary>
        /// <param name="amortizationSchedule"></param>
        /// <param name="creditValueBalance"></param>
        /// <param name="nextDueDate"></param>
        /// <returns></returns>
        public int GetLastPaymentFee(AmortizationScheduleResponse amortizationSchedule, decimal creditValueBalance,
            out DateTime nextDueDate)
        {
            AmortizationScheduleFee firstAmortizationScheduleFee = amortizationSchedule.AmortizationScheduleFees.First();

            nextDueDate = firstAmortizationScheduleFee.FeeDate;

            decimal creditValue = firstAmortizationScheduleFee.Balance;
            decimal paymentValue = creditValue - creditValueBalance;

            int lastPaymentFee = 0;
            foreach (AmortizationScheduleFee amortizationScheduleFee in amortizationSchedule.AmortizationScheduleFees)
            {
                paymentValue -= amortizationScheduleFee.CreditValuePayment;
                bool isFeePaid = paymentValue >= 0;

                if (!isFeePaid)
                {
                    nextDueDate = amortizationScheduleFee.FeeDate;
                    break;
                }

                lastPaymentFee++;
            }

            if (lastPaymentFee == 0)
            {
                nextDueDate = amortizationSchedule.AmortizationScheduleFees[1].FeeDate;
                return lastPaymentFee;
            }

            return lastPaymentFee - 1;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetLastDueFee(DateTime, AmortizationScheduleResponse)"/>
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <param name="amortizationScheduleResponse"></param>
        /// <returns></returns>
        public int GetLastDueFee(DateTime calculationDate, AmortizationScheduleResponse amortizationScheduleResponse)
        {
            int lastDueFee = 0;

            amortizationScheduleResponse.AmortizationScheduleFees.ForEach(fee =>
                {
                    if (fee.FeeDate <= calculationDate)
                    {
                        lastDueFee = fee.Fee;
                    }
                }
            );
            return lastDueFee;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetDueCreditBalance(PaymentMatrix, int, int)"/>
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="lastDueFee"></param>
        /// <param name="firstDueFee"></param>
        /// <returns></returns>
        public decimal GetDueCreditBalance(PaymentMatrix paymentMatrix, int lastDueFee, int firstDueFee) =>
            paymentMatrix.Fees
                .Where(fee => fee.FeeNumber >= Math.Min(firstDueFee, lastDueFee) && fee.FeeNumber <= Math.Max(lastDueFee, firstDueFee))
                    .Sum(fee => fee.CreditValuePayment);

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetValueFromTax(decimal, decimal)"/>
        /// </summary>
        /// <param name="totalValue"></param>
        /// <param name="taxValue"></param>
        /// <returns></returns>
        public decimal GetValueFromTax(decimal totalValue, decimal taxValue) =>
            totalValue / (1 + taxValue);

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetPaymentAlternatives(CreditMaster,
        /// Credit, AppParameters, DateTime)"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="credit"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        public PaymentAlternatives GetPaymentAlternatives(CreditMaster creditMaster, Credit credit, AppParameters parameters, DateTime calculationDate)
        {
            PaymentFeesResponse paymentFeesResponse = GetPaymentFees(creditMaster, credit, parameters, calculationDate, out decimal totalPayment);

            return new PaymentAlternatives
            {
                MinimumPayment = GetMinimumPayment(
                    paymentFeesResponse.PaymentFees, paymentFeesResponse.ArrearsFees, totalPayment, credit.GetUpdatedPaymentPlanValue ?? 0,
                    credit.HasUpdatedPaymentPlan(), parameters.MaximumUpdatedPaymentPlanGapRate),
                TotalPayment = totalPayment,
                PaymentFees = paymentFeesResponse
            };
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetLastPaymentFeeFromPaymentMatrix(PaymentMatrix, decimal)"/>
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="pendingPaymentValue"></param>
        /// <returns></returns>
        public int GetLastPaymentFeeFromPaymentMatrix(PaymentMatrix paymentMatrix, decimal pendingPaymentValue)
        {
            int lastPaymentFee = 0;
            foreach (var paymentMatrixFee in paymentMatrix.Fees)
            {
                pendingPaymentValue -= (paymentMatrixFee.CreditValuePayment + paymentMatrixFee.AssuranceValuePayment);
                if (pendingPaymentValue < 0) break;

                lastPaymentFee++;
            }

            return lastPaymentFee == 0 ? lastPaymentFee : lastPaymentFee - 1;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetCurrentAmortizationSchedule(CurrentAmortizationScheduleRequest, DateTime, AppParameters)"/>
        /// </summary>
        /// <param name="currentAmortizationScheduleRequest"></param>
        /// <param name="calculationDate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CurrentAmortizationScheduleResponse GetCurrentAmortizationSchedule(CurrentAmortizationScheduleRequest currentAmortizationScheduleRequest,
            DateTime calculationDate, AppParameters parameters)
        {
            int decimalNumbersRound = parameters.DecimalNumbersRound;
            decimal arrearsEffectiveAnnualRate = parameters.ArrearsEffectiveAnnualRate;
            int graceDays = parameters.ArrearsGracePeriod;
            decimal assuranceTax = parameters.AssuranceTax;
            DateTime arrearsAdjustmentDate = parameters.ArrearsAdjustmentDate;

            AmortizationScheduleResponse originalAmortizationSchedule = _creditsUseCase.GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCurrent(currentAmortizationScheduleRequest), decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(originalAmortizationSchedule, currentAmortizationScheduleRequest.Balance, currentAmortizationScheduleRequest.AssuranceBalance)
                .DateInfo(calculationDate, currentAmortizationScheduleRequest.LastPaymentDate.Date)
                .CreditParametersInfo(effectiveAnnualRate: currentAmortizationScheduleRequest.CurrentEffectiveAnnualRate, arrearsEffectiveAnnualRate, graceDays)
                .ArrearsAdjustmentDate(arrearsAdjustmentDate)
                .Build();

            CurrentAmortizationScheduleResponse currentAmortizationScheduleResponse =
                new CurrentAmortizationScheduleResponse
                {
                    CreditValue = currentAmortizationScheduleRequest.CreditValue.Round(decimalNumbersRound),
                    AssuranceValue = currentAmortizationScheduleRequest.AssuranceValue.Round(decimalNumbersRound),
                    DownPayment = currentAmortizationScheduleRequest.DownPayment.Round(decimalNumbersRound),
                    DaysSinceLastPayment = Math.Max(calculationDate.Subtract(currentAmortizationScheduleRequest.LastPaymentDate).Days, 0),
                    CurrentAmortizationScheduleFees =
                        GetCurrentAmortizationScheduleFees(currentAmortizationScheduleRequest, originalAmortizationSchedule, paymentMatrix,
                            decimalNumbersRound).ToList(),
                    CurrentAmortizationScheduleAssuranceFees =
                        GetCurrentAmortizationScheduleAssuranceFees(paymentMatrix, assuranceTax, decimalNumbersRound).ToList()
                };

            currentAmortizationScheduleResponse.CurrentAmortizationScheduleFees.ForEach(fee =>
            {
                AmortizationScheduleAssuranceFee amortizationScheduleAssuranceFee =
                    currentAmortizationScheduleResponse.CurrentAmortizationScheduleAssuranceFees.Single(assuranceFee => assuranceFee.Fee == fee.Fee);
                fee.TotalFeeValue = fee.FeeValue + amortizationScheduleAssuranceFee.AssurancePaymentValue;
            });

            return currentAmortizationScheduleResponse;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetCurrentPaymentSchedule(CurrentPaymentScheduleRequest, DateTime, AppParameters)"/>
        /// </summary>
        /// <param name="currentPaymentScheduleRequest"></param>
        /// <param name="calculationDate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CurrentPaymentScheduleResponse GetCurrentPaymentSchedule(CurrentPaymentScheduleRequest currentPaymentScheduleRequest,
            DateTime calculationDate, AppParameters parameters)
        {
            int decimalNumbersRound = parameters.DecimalNumbersRound;
            decimal arrearsEffectiveAnnualRate = parameters.ArrearsEffectiveAnnualRate;
            int graceDays = parameters.ArrearsGracePeriod;
            decimal assuranceTax = parameters.AssuranceTax;
            DateTime arrearsAdjustmentDate = parameters.ArrearsAdjustmentDate;

            AmortizationScheduleResponse originalAmortizationSchedule = _creditsUseCase.GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCurrent(currentPaymentScheduleRequest), decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(originalAmortizationSchedule, currentPaymentScheduleRequest.Balance, currentPaymentScheduleRequest.AssuranceBalance)
                .DateInfo(calculationDate, currentPaymentScheduleRequest.LastPaymentDate.Date)
                .CreditParametersInfo(effectiveAnnualRate: currentPaymentScheduleRequest.CurrentEffectiveAnnualRate, arrearsEffectiveAnnualRate, graceDays)
                .ArrearsAdjustmentDate(arrearsAdjustmentDate)
                .Build();

            decimal interestPayment = GetInterestPayment(paymentMatrix, currentPaymentScheduleRequest.PreviousInterest).Round(decimalNumbersRound);

            decimal arrearsPayment = GetArrearsPayment(paymentMatrix, currentPaymentScheduleRequest.ArrearsCharges, currentPaymentScheduleRequest.HasArrearsCharge,
                                       currentPaymentScheduleRequest.PreviousArrears).Round(decimalNumbersRound);

            decimal totalPayment = GetFullPayment(currentPaymentScheduleRequest.Balance, interestPayment, arrearsPayment, currentPaymentScheduleRequest.ChargeValue, paymentMatrix.PayableAssurance,
                                                  originalAmortizationSchedule, calculationDate);

            List<PaymentFee> paymentFees = CreatePaymentFees(
                    paymentMatrix,
                    firstFeeAdditions: interestPayment + arrearsPayment + currentPaymentScheduleRequest.ChargeValue,
                    totalPayment,
                    decimalNumbersRound
                ).ToList();

            int daysSinceLastPayment = Math.Max(calculationDate.Subtract(currentPaymentScheduleRequest.LastPaymentDate).Days, 0);

            decimal updatedPaymentPlanValue = currentPaymentScheduleRequest.UpdatedPaymentPlanValue;
            bool hasUpdatedPaymentPlan = updatedPaymentPlanValue > 0;

            decimal minimumPayment = GetMinimumPayment(paymentFees, paymentMatrix.ArrearsFees, totalPayment, updatedPaymentPlanValue,
                hasUpdatedPaymentPlan, parameters.MaximumUpdatedPaymentPlanGapRate);

            CurrentPaymentScheduleResponse currentPaymentScheduleResponse =
                new CurrentPaymentScheduleResponse
                {
                    DaysSinceLastPayment = daysSinceLastPayment,
                    PendingFees = paymentMatrix.PendingFees,
                    PaymentFees = paymentFees,
                    MinimumPayment = minimumPayment,
                    TotalPayment = totalPayment,
                    PaymentCreditScheduleFees =
                        GetCurrentPaymentScheduleFees(currentPaymentScheduleRequest, originalAmortizationSchedule, paymentMatrix,
                            decimalNumbersRound).ToList(),
                    PaymentAssuranceScheduleFees =
                        GetCurrentAmortizationScheduleAssuranceFees(paymentMatrix, assuranceTax, decimalNumbersRound).Skip(paymentMatrix.FirstUnpaidFeeNumber).ToList()
                };

            currentPaymentScheduleResponse.PaymentCreditScheduleFees.ForEach(fee =>
            {
                AmortizationScheduleAssuranceFee amortizationScheduleAssuranceFee =
                    currentPaymentScheduleResponse.PaymentAssuranceScheduleFees.Single(assuranceFee => assuranceFee.Fee == fee.Fee);
                fee.TotalFeeValue = fee.FeeValue + amortizationScheduleAssuranceFee.AssurancePaymentValue;
            });

            return currentPaymentScheduleResponse;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetActiveCredits(List{CreditMaster}, AppParameters, DateTime)"/>
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        public List<CreditStatus> GetActiveCredits(List<CreditMaster> creditMasters, AppParameters parameters, DateTime calculationDate)
        {
            if (!creditMasters.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);
            }

            List<CreditStatus> listActiveCredits = CreateActiveCredits(creditMasters, parameters, calculationDate);

            return listActiveCredits;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetDetailedActiveCredits(List{CreditMaster}, AppParameters, DateTime)"/>
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        public List<DetailedCreditStatus> GetDetailedActiveCredits(List<CreditMaster> creditMasters, AppParameters parameters, DateTime calculationDate)
        {
            if (!creditMasters.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);
            }

            List<DetailedCreditStatus> listDetailedActiveCredits = CreateDetailedActiveCredits(creditMasters, parameters, calculationDate);

            return listDetailedActiveCredits;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetCompleteCreditsData(List{CreditMaster}, AppParameters, DateTime)"/>
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        public List<DetailedCreditStatus> GetCompleteCreditsData(List<CreditMaster> creditMasters, AppParameters parameters, DateTime calculationDate)
        {
            if (!creditMasters.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);
            }

            List<DetailedCreditStatus> listDetailedActiveCredits = CreateDetailedActiveCredits(creditMasters, parameters, calculationDate);

            foreach (var creditInfo in listDetailedActiveCredits)
            {
                CurrentAmortizationScheduleRequest currentAmortizationScheduleRequest = new CurrentAmortizationScheduleRequest()
                {
                    CreditValue = creditInfo.CreditValue,
                    InterestRate = creditInfo.InterestRate,
                    Frequency = creditInfo.Frequency,
                    Fees = creditInfo.Fees,
                    AssuranceValue = creditInfo.AssuranceValue,
                    DownPayment = creditInfo.DownPayment,
                    InitialDate = calculationDate,
                    FeeValue = creditInfo.FeeValue,
                    AssuranceFeeValue = creditInfo.AssuranceFeeValue,
                    AssuranceTotalFeeValue = creditInfo.AssuranceTotalFeeValue,
                    Balance = creditInfo.Balance,
                    AssuranceBalance = creditInfo.AsuranceBalance,
                    HasArrearsCharge = creditInfo.HasArrearsCharge,
                    ArrearsCharges = creditInfo.ArrearsCharge,
                    LastPaymentDate = creditInfo.LastPaymentDate,
                    CurrentEffectiveAnnualRate = creditInfo.CurrentEffectiveAnnualRate,
                    ChargeValue = creditInfo.ChargeValue,
                    PreviousInterest = creditInfo.PreviousInterest,
                    PreviousArrears = creditInfo.PreviousArrears
                };

                creditInfo.AmortizationSchedule = GetCurrentAmortizationSchedule(currentAmortizationScheduleRequest, DateTime.Today, parameters);
            }

            return listDetailedActiveCredits;
        }

        /// <summary>
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        public ClientCompromise GetDetailedActiveCreditsCompromise(List<CreditMaster> creditMasters, AppParameters parameters, string ClientName)
        {
            if (!creditMasters.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);
            }

            ClientCompromise clientCompromise = new ClientCompromise();

            clientCompromise.FullName = ClientName;

            clientCompromise.detailedCreditCompromises = CreateDetailedActiveCredits(creditMasters, parameters);

            return clientCompromise;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetStatusCredit(CreditMaster, AppParameters)"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="parameters"></param>rc
        /// <returns></returns>
        public CreditStatusResponse GetStatusCredit(CreditMaster creditMaster, AppParameters parameters)
        {
            CreditStatusResponse creditStatus = CreateStatusCredit(creditMaster, parameters);

            return creditStatus;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetPayMailNotification(PaymentCreditResponse, int, decimal)"/>
        /// </summary>
        /// <param name="paymentCreditResponse"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="taxValue"></param>
        /// <returns></returns>
        public MailNotificationRequest GetPayMailNotification(PaymentCreditResponse paymentCreditResponse, int decimalNumbersRound, decimal taxValue)
        {
            Customer customer = paymentCreditResponse.Credit.Customer;
            Store store = paymentCreditResponse.Store;
            Credit credit = paymentCreditResponse.Credit;

            decimal assuranceValueFeePaid = GetValueFromTax(credit.CreditPayment.GetAssuranceValuePaid, taxValue);
            decimal assuranceFeeTaxValuePaid = credit.CreditPayment.GetAssuranceValuePaid - assuranceValueFeePaid;

            string nexDueDateString = paymentCreditResponse.NextDueDate?.ToShortDateString();
            if (paymentCreditResponse.NextDueDate == null)
            {
                nexDueDateString = "PAGADO";
            }
            else if (paymentCreditResponse.NextDueDate <= DateTime.Today)
            {
                nexDueDateString = "INMEDIATO";
            }

            List<TemplateValue> templateValues = new List<TemplateValue>
            {
                new TemplateValue
                {
                    Key = "{{customer.FullName}}",
                    Value = customer.GetFullName
                },
                new TemplateValue
                {
                    Key = "{{customer.Email}}",
                    Value = customer.GetEmail
                },
                new TemplateValue
                {
                    Key = "{{payValueLetter}}",
                    Value = NumberConversions.ToLettersSpanish(credit.CreditPayment.GetTotalValuePaid.ToString())
                },
                new TemplateValue
                {
                    Key = "{{payCreditResponse.PaymentId}}",
                    Value = paymentCreditResponse.PaymentId.ToString()
                },
                new TemplateValue
                {
                    Key = "{{valuePaidToCapital}}",
                    Value = NumberFormat.Currency(paymentCreditResponse.CreditValuePaid, decimalNumbersRound)
                },
                new TemplateValue
                {
                    Key = "{{valuePaidToInterest}}",
                    Value = NumberFormat.Currency(paymentCreditResponse.InterestValuePaid, decimalNumbersRound)
                },
                new TemplateValue
                {
                    Key = "{{valuePaidToArrears}}",
                    Value = NumberFormat.Currency(paymentCreditResponse.ArrearsValuePaid, decimalNumbersRound)
                },
                new TemplateValue
                {
                    Key = "{{valuePaidToAssurance}}",
                    Value = NumberFormat.Currency(assuranceValueFeePaid, decimalNumbersRound)
                },
                new TemplateValue
                {
                    Key = "{{valuePaidToCharges}}",
                    Value = NumberFormat.Currency(paymentCreditResponse.ChargeValuePaid, decimalNumbersRound)
                },
                new TemplateValue
                {
                    Key = "{{valuePaidToAssuranceTax}}",
                    Value = NumberFormat.Currency(assuranceFeeTaxValuePaid, decimalNumbersRound)
                },
                new TemplateValue
                {
                    Key = "{{AvalCompany}}",
                    Value = store.AssuranceCompany.Name
                },
                new TemplateValue
                {
                    Key = "{{payValue}}",
                    Value = NumberFormat.Currency(credit.CreditPayment.GetTotalValuePaid, decimalNumbersRound)
                },
                new TemplateValue
                {
                    Key = "{{date}}",
                    Value = credit.CreditPayment.GetLastPaymentDate.ToShortDateString()
                },
                new TemplateValue
                {
                    Key = "{{store.StoreName}}",
                    Value = store.StoreName
                },
                new TemplateValue
                {
                    Key = "{{TypId}}",
                    Value = customer.DocumentType
                },
                new TemplateValue
                {
                    Key = "{{DocumentId}}",
                    Value = customer.IdDocument
                },
                new TemplateValue
                {
                    Key = "{{paymentNumber}}",
                    Value = paymentCreditResponse.PaymentNumber.ToString()
                },
                new TemplateValue
                {
                    Key = "{{nextDueDate}}",
                    Value = nexDueDateString
                },
                new TemplateValue
                {
                    Key = "{{creditNumber}}",
                    Value = credit.GetCreditNumber.ToString()
                },
            };

            MailNotificationRequest mailNotificationRequest = new MailNotificationRequest
            {
                TemplateInfo = new TemplateInfo
                {
                    TemplateId = _credinetAppSettings.PayNotificationMailTemplate,
                    TemplateValues = templateValues
                },
                Recipients = new List<string> { customer.GetEmail },
                Subject = _credinetAppSettings.MailMessageNotification
            };

            return mailNotificationRequest;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetMultiplePayMailNotification(List{PaymentCreditResponse}, int, decimal)"/>
        /// </summary>
        /// <param name="paymentCreditResponse"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="taxValue"></param>
        /// <returns></returns>
        public MailNotificationRequest GetMultiplePayMailNotification(List<PaymentCreditResponse> paymentCreditResponse, int decimalNumbersRound, decimal taxValue)
        {
            var defaultData = paymentCreditResponse.FirstOrDefault();

            Customer customer = defaultData.Credit.Customer;
            Store store = defaultData.Store;
            string data = "";
            int count = 1;

            foreach (var item in paymentCreditResponse)
            {
                data += GetMultiplePaymentTemplate(item, decimalNumbersRound, taxValue, count);
                count++;
            }

            List<TemplateValue> templateValues = new List<TemplateValue>
            {
                new TemplateValue
                {
                    Key = "{{FullName}}",
                    Value = customer.GetFullName
                },
                new TemplateValue
                {
                    Key = "{{multiplePayments}}",
                    Value = data
                },
                new TemplateValue
                {
                    Key = "{{date}}",
                    Value = DateTime.Now.ToString()
                },
                new TemplateValue
                {
                    Key = "{{Email}}",
                    Value = customer.GetEmail
                }
            };

            MailNotificationRequest mailNotificationRequest = new MailNotificationRequest
            {
                TemplateInfo = new TemplateInfo
                {
                    TemplateId = _credinetAppSettings.MultiplePaymentsNotificationMailTemplate,
                    TemplateValues = templateValues
                },
                Recipients = new List<string> { customer.GetEmail },
                Subject = _credinetAppSettings.MailMessageNotification
            };

            return mailNotificationRequest;
        }

        /// <summary>
        /// Create active credits
        /// </summary>
        /// <param name="activeCredits"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        private List<CreditStatus> CreateActiveCredits(List<CreditMaster> activeCredits, AppParameters parameters, DateTime calculationDate)
        {
            List<CreditStatus> activeCreditsResponse = new List<CreditStatus>();

            int decimalNumbersRound = parameters.DecimalNumbersRound;
            decimal maximumResidueValue = parameters.MaximumResidueValue;
            int arrearsGracePeriod = parameters.ArrearsGracePeriod;
            Customer customer = activeCredits.FirstOrDefault().Customer;

            foreach (var activeCredit in activeCredits.OrderBy(item => item.Current.CreditPayment.GetDueDate))
            {
                CreditMaster creditMaster = activeCredit;
                Credit credit = activeCredit.Current;
                DateTime dueDate = credit.CreditPayment.GetDueDate;
                PaymentAlternatives paymentAlternatives = GetPaymentAlternatives(creditMaster, credit, parameters, calculationDate);
                bool hasArrears = _creditsUseCase.HasArrears(calculationDate, dueDate, arrearsGracePeriod);
                int arrearsDays = _creditsUseCase.GetArrearsDays(calculationDate, dueDate);
                bool isNextFee = !hasArrears && calculationDate < dueDate;

                activeCreditsResponse.Add(new CreditStatus
                {
                    TypeDocument = credit.Customer.DocumentType,
                    IdDocument = credit.Customer.IdDocument,
                    CustomerFullName = credit.Customer.GetFullName,
                    CreditId = creditMaster.Id,
                    CreditNumber = credit.GetCreditNumber,
                    StoreId = creditMaster.Store.Id,
                    CreateDate = creditMaster.GetCreditDate,
                    CreditValue = credit.GetCreditValue.Round(decimalNumbersRound),
                    ArrearsDays = arrearsDays,
                    MinimumPayment = paymentAlternatives.MinimumPayment.Round(decimalNumbersRound),
                    IsNextFee = isNextFee,
                    TotalPayment = paymentAlternatives.TotalPayment.Round(decimalNumbersRound),
                    FeeValue = credit.GetFeeValue.Round(decimalNumbersRound),
                    StoreName = creditMaster.Store.StoreName,
                    Balance = credit.GetBalance.Round(decimalNumbersRound),
                    DueDate = dueDate,
                    UpdatedPaymentPlan = credit.HasUpdatedPaymentPlan(),
                    MaximumResidueValue = maximumResidueValue,
                    Mobile = customer.GetMobile,
                    Email = customer.GetEmail
                });
            }

            return activeCreditsResponse;
        }

        /// <summary>
        /// Create detailed active credits
        /// </summary>
        /// <param name="activeCredits"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        private List<DetailedCreditStatus> CreateDetailedActiveCredits(List<CreditMaster> activeCredits, AppParameters parameters, DateTime calculationDate)
        {
            List<DetailedCreditStatus> detailedActiveCreditsResponse = new List<DetailedCreditStatus>();

            int decimalNumbersRound = parameters.DecimalNumbersRound;
            decimal maximumResidueValue = parameters.MaximumResidueValue;
            int arrearsGracePeriod = parameters.ArrearsGracePeriod;
            int interestRateDecimalNumbersRound = parameters.InterestRateDecimalNumbers;
            decimal arrearsEffectiveAnnualRate = parameters.ArrearsEffectiveAnnualRate;

            foreach (var activeCredit in activeCredits.OrderBy(item => item.Current.CreditPayment.GetDueDate))
            {
                CreditMaster creditMaster = activeCredit;
                Credit credit = activeCredit.Current;
                decimal effectiveAnnualRate = creditMaster.GetEffectiveAnnualRate;
                decimal currentEffectiveAnnualRate = Math.Min(effectiveAnnualRate, creditMaster.Store.GetEffectiveAnnualRate);
                DateTime dueDate = credit.CreditPayment.GetDueDate;
                PaymentAlternatives paymentAlternatives = GetPaymentAlternatives(creditMaster, credit, parameters, calculationDate);
                bool hasArrears = _creditsUseCase.HasArrears(calculationDate, dueDate, arrearsGracePeriod);
                bool isNextFee = !hasArrears && calculationDate < dueDate;
                int frequency = credit.GetFrequency;

                detailedActiveCreditsResponse.Add(new DetailedCreditStatus
                {
                    TypeDocument = credit.Customer.DocumentType,
                    IdDocument = credit.Customer.IdDocument,
                    CreditId = creditMaster.Id,
                    CreditNumber = credit.GetCreditNumber,
                    StoreId = creditMaster.Store.Id,
                    CreateDate = creditMaster.GetCreditDate,
                    CreditValue = credit.GetCreditValue.Round(decimalNumbersRound),
                    ArrearsDays = _creditsUseCase.GetArrearsDays(calculationDate, dueDate),
                    MinimumPayment = paymentAlternatives.MinimumPayment.Round(decimalNumbersRound),
                    IsNextFee = isNextFee,
                    TotalPayment = paymentAlternatives.TotalPayment.Round(decimalNumbersRound),
                    FeeValue = credit.GetFeeValue.Round(decimalNumbersRound),
                    StoreName = creditMaster.Store.StoreName,
                    Balance = credit.GetBalance.Round(decimalNumbersRound),
                    DueDate = dueDate,
                    UpdatedPaymentPlan = credit.HasUpdatedPaymentPlan(),
                    MaximumResidueValue = maximumResidueValue,
                    DownPayment = credit.GetDownPayment,
                    AssuranceValue = credit.GetAssuranceValue,
                    InterestRate = _creditsUseCase.GetInterestRate(effectiveAnnualRate, frequency).Round(interestRateDecimalNumbersRound),
                    Frequency = credit.GetFrequency,
                    Fees = credit.GetFees,
                    AssuranceFeeValue = credit.GetAssuranceFee,
                    AssuranceTotalFeeValue = credit.GetAssuranceTotalFeeValue,
                    CustomerFullName = creditMaster.Customer.GetFullName,
                    LastPaymentDate = credit.CreditPayment.GetLastPaymentDate,
                    DaysSinceLastPayment = Math.Max(calculationDate.Subtract(credit.CreditPayment.GetLastPaymentDate).Days, 0),
                    AssurancePercentage = credit.GetAssurancePercentage,
                    EffectiveAnnualRate = effectiveAnnualRate,
                    MonthlyArrearsRate = (arrearsEffectiveAnnualRate / DateTimeHelper.MonthsInAYear).Round(interestRateDecimalNumbersRound),
                    AsuranceBalance = credit.GetAssuranceBalance,
                    ChargeValue = credit.GetChargeValue,
                    HasArrearsCharge = credit.GetHasArrearsCharge,
                    ArrearsCharge = credit.GetArrearsCharge,
                    PreviousInterest = credit.CreditPayment.GetPreviousInterest,
                    PreviousArrears = credit.CreditPayment.GetPreviousArrears,
                    CurrentEffectiveAnnualRate = currentEffectiveAnnualRate,
                    CurrentInterestRate = _creditsUseCase.GetInterestRate(currentEffectiveAnnualRate, frequency).Round(interestRateDecimalNumbersRound),
                    TotalFeeValue = credit.GetTotalFeeValue.Round(decimalNumbersRound),
                    UpdatedPaymentPlanValue = Math.Min(credit.GetUpdatedPaymentPlanValue ?? 0, paymentAlternatives.TotalPayment).Round(decimalNumbersRound),
                    ArrearsFees = paymentAlternatives.PaymentFees.ArrearsFees,
                    ArrearsPayment = paymentAlternatives.PaymentFees.ArrearsPayment
                });
            }

            return detailedActiveCreditsResponse;
        }

        /// <summary>
        /// Create credit status
        /// </summary>
        /// <param name="activeCredit"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private CreditStatusResponse CreateStatusCredit(CreditMaster activeCredit, AppParameters parameters)
        {
            CreditMaster creditMaster = activeCredit;
            Credit credit = activeCredit.Current;

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            PaymentAlternatives paymentAlternatives = GetPaymentAlternatives(creditMaster, credit, parameters, DateTime.Today);
            DateTime dueDate = credit.CreditPayment.GetDueDate;
            int arrearsDays = _creditsUseCase.GetArrearsDays(DateTime.Today, dueDate);

            return new CreditStatusResponse
            {
                TypeDocument = creditMaster.Customer.DocumentType,
                IdDocument = creditMaster.Customer.IdDocument,
                CreditId = creditMaster.Id,
                CreditNumber = credit.GetCreditNumber,
                StoreId = credit.Store.Id,
                CreateDate = creditMaster.GetCreditDate,
                CreditValue = credit.GetCreditValue.Round(decimalNumbersRound),
                ArrearsDays = arrearsDays,
                MinimumPayment = paymentAlternatives.MinimumPayment.Round(decimalNumbersRound),
                TotalPayment = paymentAlternatives.TotalPayment.Round(decimalNumbersRound),
                FeeValue = credit.GetFeeValue.Round(decimalNumbersRound)
            };
        }

        /// <summary>
        /// Get current amortization schedule fees
        /// </summary>
        /// <param name="currentAmortizationScheduleRequest"></param>
        /// <param name="originalAmortizationSchedule"></param>
        /// <param name="paymentMatrix"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private IEnumerable<CurrentAmortizationScheduleFee> GetCurrentAmortizationScheduleFees(CurrentAmortizationScheduleRequest currentAmortizationScheduleRequest,
            AmortizationScheduleResponse originalAmortizationSchedule, PaymentMatrix paymentMatrix, int decimalNumbersRound)
        {
            decimal previousInterest = currentAmortizationScheduleRequest.PreviousInterest;
            decimal previousArrears = currentAmortizationScheduleRequest.PreviousArrears;
            DateTime lastPaymentDate = currentAmortizationScheduleRequest.LastPaymentDate;

            foreach (var paymentMatrixFee in paymentMatrix.Fees)
            {
                int feeNumber = paymentMatrixFee.FeeNumber;
                DateTime feeDate = paymentMatrixFee.FeeDate;

                bool isFeePaid = paymentMatrixFee.CreditValuePayment == 0;
                if (isFeePaid)
                {
                    yield return CurrentAmortizationScheduleFee.PaidFee(feeNumber, feeDate);
                    continue;
                }

                bool isFirstUnpaidFeeNumber = feeNumber == paymentMatrix.FirstUnpaidFeeNumber;

                int interestDays =
                  GetFeeInterestDays(paymentMatrix, paymentMatrixFee, lastPaymentDate, isFirstUnpaidFeeNumber, currentAmortizationScheduleRequest.Frequency);

                yield return GetCurrentAmortizationScheduleFee(currentAmortizationScheduleRequest, originalAmortizationSchedule, decimalNumbersRound, ref previousInterest,
                                                                ref previousArrears, paymentMatrixFee, isFirstUnpaidFeeNumber, interestDays);
            }
        }

        /// <summary>
        /// Get current payment schedule fees
        /// </summary>
        /// <param name="currentPaymentScheduleRequest"></param>
        /// <param name="originalAmortizationSchedule"></param>
        /// <param name="paymentMatrix"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private IEnumerable<CurrentAmortizationScheduleFee> GetCurrentPaymentScheduleFees(CurrentPaymentScheduleRequest currentPaymentScheduleRequest,
            AmortizationScheduleResponse originalAmortizationSchedule, PaymentMatrix paymentMatrix, int decimalNumbersRound)
        {
            decimal previousInterest = currentPaymentScheduleRequest.PreviousInterest;
            decimal previousArrears = currentPaymentScheduleRequest.PreviousArrears;

            foreach (var paymentMatrixFee in paymentMatrix.Fees)
            {
                int feeNumber = paymentMatrixFee.FeeNumber;
                bool isFeePaid = paymentMatrixFee.CreditValuePayment != 0;
                bool isFirstUnpaidFeeNumber = feeNumber == paymentMatrix.FirstUnpaidFeeNumber;

                int interestDays = paymentMatrixFee.InterestDays;

                if (isFeePaid)
                {
                    yield return GetCurrentAmortizationScheduleFee(currentPaymentScheduleRequest, originalAmortizationSchedule, decimalNumbersRound, ref previousInterest,
                                                                       ref previousArrears, paymentMatrixFee, isFirstUnpaidFeeNumber, interestDays);
                }
            }
        }

        /// <summary>
        /// Get current amortization schedule fee
        /// </summary>
        /// <param name="currentAmortizationScheduleRequest"></param>
        /// <param name="originalAmortizationSchedule"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="previousInterest"></param>
        /// <param name="previousArrears"></param>
        /// <param name="paymentMatrixFee"></param>
        /// <param name="isFirstUnpaidFeeNumber"></param>
        /// <param name="interestDays"></param>
        /// <returns></returns>
        private CurrentAmortizationScheduleFee GetCurrentAmortizationScheduleFee(CurrentAmortizationScheduleRequest currentAmortizationScheduleRequest, AmortizationScheduleResponse originalAmortizationSchedule, int decimalNumbersRound,
                                                             ref decimal previousInterest, ref decimal previousArrears, PaymentMatrixFee paymentMatrixFee, bool isFirstUnpaidFeeNumber, int interestDays)
        {
            decimal creditValuePayment = paymentMatrixFee.CreditValuePayment;
            int arrearDays = paymentMatrixFee.ArrearsDays;

            bool isFeeLate = arrearDays > 0;
            decimal finalBalance = originalAmortizationSchedule.AmortizationScheduleFees[paymentMatrixFee.FeeNumber].FinalBalance;
            decimal balance = finalBalance + creditValuePayment;

            decimal interestValue =
                (_creditsUseCase.GetInterestRate(currentAmortizationScheduleRequest.CurrentEffectiveAnnualRate, interestDays) * balance).Round(decimalNumbersRound)
                - previousInterest;

            previousInterest = 0;
            if (interestValue < 0)
            {
                previousInterest = -interestValue;
                interestValue = 0;
            }

            decimal chargeValue = 0;
            if (isFirstUnpaidFeeNumber)
            {
                chargeValue = currentAmortizationScheduleRequest.ChargeValue +
                    (currentAmortizationScheduleRequest.HasArrearsCharge
                        ? currentAmortizationScheduleRequest.ArrearsCharges
                        : 0);
            }

            decimal arrearPaymentValue = 0;

            if (!currentAmortizationScheduleRequest.HasArrearsCharge)
            {
                arrearPaymentValue = paymentMatrixFee.ArrearsPayment.Round(decimalNumbersRound);
                arrearPaymentValue -= previousArrears;

                previousArrears = 0;
                if (arrearPaymentValue < 0)
                {
                    previousArrears = -arrearPaymentValue;
                    arrearPaymentValue = 0;
                }
            }

            decimal feevalue = creditValuePayment + interestValue + arrearPaymentValue + chargeValue;

            return new CurrentAmortizationScheduleFee
            {
                Fee = paymentMatrixFee.FeeNumber,
                FeeDate = paymentMatrixFee.FeeDate,
                FeeStatus = isFeeLate ? FeeStatuses.LATE : FeeStatuses.CURRENT,
                Balance = balance.Round(decimalNumbersRound),
                InterestDays = interestDays,
                InterestValue = interestValue,
                CreditValuePayment = creditValuePayment.Round(decimalNumbersRound),
                FinalBalance = finalBalance.Round(decimalNumbersRound),
                ArrearDays = arrearDays,
                ArrearPaymentValue = arrearPaymentValue,
                ChargeValue = chargeValue.Round(decimalNumbersRound),
                FeeValue = feevalue.Round(decimalNumbersRound)
            };
        }

        /// <summary>
        /// Get fee interest days
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="paymentMatrixFee"></param>
        /// <param name="lastPaymentDate"></param>
        /// <param name="isFirstUnpaidFeeNumber"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        private int GetFeeInterestDays(PaymentMatrix paymentMatrix, PaymentMatrixFee paymentMatrixFee, DateTime lastPaymentDate, bool isFirstUnpaidFeeNumber, int frequency)
        {
            int interestDays;
            if (paymentMatrixFee.FeeDate < paymentMatrix.FirstUnpaidFee.FeeDate)
            {
                interestDays = 0;
            }
            else if (paymentMatrixFee.FeeNumber > paymentMatrix.ActiveFee.FeeNumber)
            {
                interestDays = frequency;
            }
            else
            {
                DateTime minDate = isFirstUnpaidFeeNumber ? lastPaymentDate
                                              : DateTimeHelper.LatestDate(lastPaymentDate, paymentMatrix.GetFeeByFeeNumber(paymentMatrixFee.FeeNumber - 1).FeeDate);
                DateTime maxDate = paymentMatrixFee.FeeDate;

                interestDays = DateTimeHelper.Difference360BetweenDates(minDate, maxDate);
            }

            return interestDays;
        }

        /// <summary>
        /// Get current amortization schedule fees
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="assuranceTax"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private IEnumerable<AmortizationScheduleAssuranceFee> GetCurrentAmortizationScheduleAssuranceFees(PaymentMatrix paymentMatrix,
            decimal assuranceTax, int decimalNumbersRound)
        {
            IEnumerable<PaymentMatrixFee> paymentMatrixFees = paymentMatrix.Fees;

            foreach (var paymentMatrixFee in paymentMatrixFees)
            {
                int fee = paymentMatrixFee.FeeNumber;

                decimal assuranceBalanceFeeValue = paymentMatrixFees.Where(feeMatrix => feeMatrix.FeeNumber >= fee).Sum(feeMatrix => feeMatrix.AssuranceValuePayment);

                DateTime feeDate = paymentMatrixFee.FeeDate;
                decimal balance = 0;
                decimal assuranceFeeValue = 0;
                decimal assuranceTaxValue = 0;
                decimal assurancePaymentValue = 0;
                decimal finalBalance = 0;

                if (fee > paymentMatrix.LastFeePaidNumber)
                {
                    balance = assuranceBalanceFeeValue / (1 + assuranceTax);
                    assurancePaymentValue = paymentMatrixFee.AssuranceValuePayment;
                    assuranceFeeValue = paymentMatrixFee.AssuranceValuePayment / (1 + assuranceTax);
                    assuranceTaxValue = assurancePaymentValue - assuranceFeeValue;
                    finalBalance = balance - assuranceFeeValue;
                }

                yield return new AmortizationScheduleAssuranceFee
                {
                    Fee = fee,
                    FeeDate = feeDate,
                    Balance = balance.Round(decimalNumbersRound),
                    AssuranceFeeValue = assuranceFeeValue.Round(decimalNumbersRound),
                    AssuranceTaxValue = assuranceTaxValue.Round(decimalNumbersRound),
                    AssurancePaymentValue = assurancePaymentValue.Round(decimalNumbersRound),
                    FinalBalance = finalBalance.Round(decimalNumbersRound),
                };
            }
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetSmsNotification(string, Store, Credit, int)"/>
        /// </summary>
        /// <param name="template"></param>
        /// <param name="store"></param>
        /// <param name="credit"></param>
        /// <param name="decimalNumberRound"></param>
        /// <returns></returns>
        public SmsNotificationRequest GetSmsNotification(string template, Store store, Credit credit, int decimalNumberRound)
        {
            string[] fullName = credit.Customer.GetFullName.Split(" ");

            string message = string.Format(template, fullName[0],
                NumberFormat.CurrencySms(credit.CreditPayment.GetTotalValuePaid, decimalNumberRound,
                _credinetAppSettings.SmsCurrencyCode),
                _credinetAppSettings.UrlSmsNotificationPay);

            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest
            {
                Message = message,
                Mobile = credit.Customer.GetMobile
            };

            return smsNotificationRequest;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.GetSmsNotification(string, Store, Credit, int)"/>
        /// </summary>
        /// <param name="template"></param>
        /// <param name="customer"></param>
        /// <param name="total"></param>
        /// <param name="decimalNumberRound"></param>
        /// <returns></returns>
        public SmsNotificationRequest GetSmsNotification(string template, Customer customer, decimal total, int decimalNumberRound)
        {

            string[] fullName = customer.GetFullName.Split(" ");

            string message = string.Format(template, fullName[0],
                NumberFormat.Currency(total, decimalNumberRound),
                _credinetAppSettings.UrlSmsNotificationPay);

            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest
            {
                Message = message,
                Mobile = customer.GetMobile
            };

            return smsNotificationRequest;
        }

        /// <summary>
        ///<see cref="ICreditPaymentUsesCase.GetPaymentTemplate(PaymentTemplateResponse, int, bool)"/>
        /// </summary>
        /// <param name="paymentTemplateResponse"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        public string GetPaymentTemplate(PaymentTemplateResponse paymentTemplateResponse, int decimalNumbersRound, bool reprint)
        {
            string templateHtml = paymentTemplateResponse.Template;
            string format = "${{{0}}}";

            templateHtml = templateHtml.Replace(string.Format(format, "GenerationDate"), paymentTemplateResponse.GenerationDate.ToString());
            templateHtml = templateHtml.Replace(string.Format(format, "TotalValuePaid"), NumberFormat.Currency(paymentTemplateResponse.TotalValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "Nit"), paymentTemplateResponse.Nit);
            templateHtml = templateHtml.Replace(string.Format(format, "CellPhone"), paymentTemplateResponse.CellPhone);
            templateHtml = templateHtml.Replace(string.Format(format, "CustomerFullName"), paymentTemplateResponse.CustomerFullName);
            templateHtml = templateHtml.Replace(string.Format(format, "CustomerDocumentType"), paymentTemplateResponse.CustomerDocumentType);
            templateHtml = templateHtml.Replace(string.Format(format, "CustomerIdDocument"), paymentTemplateResponse.CustomerIdDocument);
            templateHtml = templateHtml.Replace(string.Format(format, "PaymentDate"), paymentTemplateResponse.PaymentDate.ToShortDateString());
            templateHtml = templateHtml.Replace(string.Format(format, "StorePaymentName"), paymentTemplateResponse.StorePaymentName);
            templateHtml = templateHtml.Replace(string.Format(format, "StoreCreditName"), paymentTemplateResponse.StoreCreditName);
            templateHtml = templateHtml.Replace(string.Format(format, "StorePhone"), paymentTemplateResponse.StorePhone);
            templateHtml = templateHtml.Replace(string.Format(format, "PaymentNumber"), paymentTemplateResponse.PaymentNumber.ToString());
            templateHtml = templateHtml.Replace(string.Format(format, "CreditNumber"), paymentTemplateResponse.CreditNumber.ToString());
            templateHtml = templateHtml.Replace(string.Format(format, "CreditValuePaid"), NumberFormat.Currency(paymentTemplateResponse.CreditValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "InterestValuePaid"), NumberFormat.Currency(paymentTemplateResponse.InterestValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "ArrearsValuePaid"), NumberFormat.Currency(paymentTemplateResponse.ArrearsValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "AssuranceValuePaid"), NumberFormat.Currency(paymentTemplateResponse.AssuranceValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "ChargeValuePaid"), NumberFormat.Currency(paymentTemplateResponse.ChargeValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "AssuranceTax"), NumberFormat.Currency(paymentTemplateResponse.AssuranceTax, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "TotalBalance"), NumberFormat.Currency(paymentTemplateResponse.TotalBalance, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "AvailableCreditLimit"), NumberFormat.Currency(paymentTemplateResponse.AvailableCreditLimit, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "NextDueDate"), paymentTemplateResponse.NextDueDate?.ToShortDateString());
            templateHtml = templateHtml.Replace(string.Format(format, "Reprint"), reprint ? "block" : "none");

            return templateHtml;
        }

        /// <summary>
        /// <see cref="ICreditPaymentUsesCase.CreatePaymentHistory(List{Credit},
        /// List{RequestCancelPayment}, AppParameters)"/>
        /// </summary>
        /// <param name="payments"></param>
        /// <param name="requestCancelPaymentsNotDismissed"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<PaymentHistoryResponse> CreatePaymentHistory(List<Credit> payments, List<RequestCancelPayment> requestCancelPaymentsNotDismissed,
            AppParameters parameters)
        {
            const string ACTIVE_PAYMENT_STATUS_NAME = @"Activo";
            const int ACTIVE_PAYMENT_STATUS_ID = 0;

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            return payments
                .OrderByDescending(payment => payment.GetTransactionDateComplete)
                .Select(payment =>
                {
                    RequestCancelPayment requestCancelPayment =
                        requestCancelPaymentsNotDismissed
                            .FirstOrDefault(request => request.GetCreditId == payment.Id);

                    return new PaymentHistoryResponse
                    {
                        CancelDate = requestCancelPayment?.GetProcessDate,
                        CreationDate = payment.GetTransactionDate,
                        CreditId = payment.GetCreditMasterId,
                        PaymentId = payment.Id,
                        CreditNumber = payment.GetCreditNumber,
                        PaymentNumber = payment.CreditPayment.GetPaymentNumber,
                        Status = requestCancelPayment?.RequestStatus.Name ?? ACTIVE_PAYMENT_STATUS_NAME,
                        StatusId = requestCancelPayment?.GetRequestStatusId ?? ACTIVE_PAYMENT_STATUS_ID,
                        StoreId = payment.GetStoreId,
                        StoreName = payment.Store.StoreName,
                        ValuePaid = payment.CreditPayment.GetTotalValuePaid.Round(decimalNumbersRound)
                    };
                })
                .ToList();
        }

        /// <summary>
        /// Validate payment date
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="credit"></param>
        private void ValidatePaymentDate(PaymentCreditRequest paymentCreditRequest, Credit credit)
        {
            TimeSpan oneMinute = new TimeSpan(0, 1, 0);
            DateTime lastCreationDate = credit.GetCreateDate + credit.GetCreateTime;

            bool isLastPaymentVeryRecent = DateTime.Now < lastCreationDate.Add(oneMinute);
            bool lastTransactionHasPaymentType = (TransactionTypes)credit.CreditPayment.GetTransactionTypeId == TransactionTypes.Payment;
            bool isSameTotalValuePaid = paymentCreditRequest.TotalValuePaid.Equals(credit.CreditPayment.GetTotalValuePaid);

            if (lastTransactionHasPaymentType && isSameTotalValuePaid && isLastPaymentVeryRecent)
            {
                throw new BusinessException(nameof(BusinessResponse.CreditPaymentAlreadyPaid), (int)BusinessResponse.CreditPaymentAlreadyPaid);
            }
        }

        /// <summary>
        /// Create detailed active credits
        /// </summary>
        /// <param name="activeCredits"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private List<DetailedCreditCompromise> CreateDetailedActiveCredits(List<CreditMaster> activeCredits, AppParameters parameters)
        {
            List<DetailedCreditCompromise> detailedActiveCreditsResponse = new List<DetailedCreditCompromise>();

            int decimalNumbersRound = parameters.DecimalNumbersRound;
            decimal maximumResidueValue = parameters.MaximumResidueValue;
            int arrearsGracePeriod = parameters.ArrearsGracePeriod;
            int interestRateDecimalNumbersRound = parameters.InterestRateDecimalNumbers;
            decimal arrearsEffectiveAnnualRate = parameters.ArrearsEffectiveAnnualRate;

            DateTime calculationDate = DateTime.Now;

            foreach (var activeCredit in activeCredits.OrderBy(item => item.Current.CreditPayment.GetDueDate))
            {
                CreditMaster creditMaster = activeCredit;
                Credit credit = activeCredit.Current;
                DateTime dueDate = credit.CreditPayment.GetDueDate;
                var paymentAlternatives = GetPaymentAlternatives(creditMaster, credit, parameters, calculationDate);
                PaymentFeesResponse paymentFeesResponse = GetPaymentFees(creditMaster, credit, parameters, calculationDate, out decimal totalPayment);

                detailedActiveCreditsResponse.Add(new DetailedCreditCompromise
                {
                    StoreName = creditMaster.Store.StoreName,
                    CreditNumber = credit.GetCreditNumber,
                    CreditMasterId = credit.GetCreditMasterId,
                    CreateDate = creditMaster.GetCreditDate,
                    ArrearsDays = _creditsUseCase.GetArrearsDays(calculationDate, dueDate),
                    ArrearsFees = paymentAlternatives.PaymentFees.ArrearsFees,
                    PaymentFees = paymentFeesResponse.PaymentFees.Count,
                    ArrearsPayment = paymentAlternatives.PaymentFees.ArrearsPayment.Round(decimalNumbersRound),
                    Fees = credit.GetFees,
                    CreditValue = credit.GetCreditValue.Round(decimalNumbersRound),
                    MinimumPayment = paymentAlternatives.MinimumPayment.Round(decimalNumbersRound),
                    ChargeValue = credit.GetChargeValue,
                    TotalPayment = paymentAlternatives.TotalPayment.Round(decimalNumbersRound),
                    UpdatedPaymentPlan = credit.HasUpdatedPaymentPlan(),
                    UpdatedPaymentPlanValue = Math.Min(credit.GetUpdatedPaymentPlanValue ?? 0, paymentAlternatives.TotalPayment).Round(decimalNumbersRound)
                });
            }

            return detailedActiveCreditsResponse;
        }

        /// <summary>
        /// <param name="paymentCreditResponse"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="taxValue"></param>
        /// <returns></returns>
        public string GetMultiplePaymentTemplate(PaymentCreditResponse paymentCreditResponse, int decimalNumbersRound, decimal taxValue, int payNumber)
        {
            decimal assuranceValueFeePaid = GetValueFromTax(paymentCreditResponse.Credit.CreditPayment.GetAssuranceValuePaid, taxValue);
            decimal assuranceFeeTaxValuePaid = paymentCreditResponse.Credit.CreditPayment.GetAssuranceValuePaid - assuranceValueFeePaid;

            string nexDueDateString = paymentCreditResponse.NextDueDate?.ToShortDateString();
            if (paymentCreditResponse.NextDueDate == null)
            {
                nexDueDateString = "PAGADO";
            }
            else if (paymentCreditResponse.NextDueDate <= DateTime.Today)
            {
                nexDueDateString = "INMEDIATO";
            }

            #region template

            string templateHtml = @"<!--[if mso | IE]></td></tr></table><table align='center' border='0' cellpadding='0' cellspacing='0' class='' role='presentation' style='width:600px;' width='600' ><tr><td style='line-height:0px;font-size:0px;mso-line-height-rule:exactly;'><![endif]-->
            <div style='margin:0px auto;max-width:600px;'>
              <table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='width:100%;'>
                <tbody>
                  <tr>
                    <td style='direction:ltr;font-size:0px;padding:20px 0;text-align:center;'>
                      <!--[if mso | IE]><table role='presentation' border='0' cellpadding='0' cellspacing='0'><tr><td class='' style='vertical-align:top;width:600px;' ><![endif]-->
                      <div class='mj-column-per-100 mj-outlook-group-fix' style='font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;'>
                        <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='vertical-align:top;' width='100%'>
                          <tbody>
                            <tr>
                              <td align='left' class='table-payment' style='font-size:0px;padding:25px 30px;word-break:break-word; border-radius:8px;'>
                                <div style='font-family:Red Hat Display, sans-serif;font-size:20px;font-weight:bold;line-height:22px;text-align:left;color:#2c2a29;'>Pago No. {{payNumber}}</div>
                              </td>
                            </tr>
                            <tr>
                              <td align='left' class='table-payment' style='font-size:0px;padding:25px 30px;word-break:break-word; border-radius:8px;'>
                                <table cellpadding='0' cellspacing='0' width='100%' border='0' style='color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:22px;table-layout:auto;width:100%;border:none;'>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>Recibo de caja</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{paymentNumber}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>Valor cancelado Capital</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{valuePaidToInterest}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>Valor cancelado Financiación</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{valuePaidToCapital}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>Valor cancelado Mora</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{valuePaidToArrears}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>Valor Cancelado Aval</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{valuePaidToAssurance}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>Valor Cancelado Cargos</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{valuePaidToCharges}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>Valor Iva Aval</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{valuePaidToAssuranceTax}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px; vertical-align=top'>Compañía Aval</td>
                                    <td width='60%' css-class=' value-table' align='right'>{{AvalCompany}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>Valor del pago</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{payValue}}<br>{{payValueLetter}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>Pago realizado en</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{StoreName}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>Fecha límite próximo pago</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{nextDueDate}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>Código del crédito</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{creditNumber}}</td>
                                  </tr>
                                  <tr valign='top' height='30px'>
                                    <td width='40%' align='left' style='color: #2c2a29; font-weight:700; font-size:14px;' padding-bottom='12px'>ID Pago</td>
                                    <td width='60%' css-class='value-table' align='right' style='color: #2c2a29; font-size:14px;'>{{paymentId}}</td>
                                  </tr>
                                </table>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                      <!--[if mso | IE]></td></tr></table><![endif]-->
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>";

            #endregion template

            templateHtml = templateHtml.Replace("{{payNumber}}", payNumber.ToString());
            templateHtml = templateHtml.Replace("{{payValueLetter}}", NumberConversions.ToLettersSpanish(paymentCreditResponse.Credit.CreditPayment.GetTotalValuePaid.ToString()));
            templateHtml = templateHtml.Replace("{{paymentId}}", paymentCreditResponse.PaymentId.ToString());
            templateHtml = templateHtml.Replace("{{valuePaidToInterest}}", NumberFormat.Currency(paymentCreditResponse.CreditValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace("{{valuePaidToCapital}}", NumberFormat.Currency(paymentCreditResponse.InterestValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace("{{valuePaidToArrears}}", NumberFormat.Currency(paymentCreditResponse.ArrearsValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace("{{valuePaidToAssurance}}", NumberFormat.Currency(assuranceValueFeePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace("{{valuePaidToCharges}}", NumberFormat.Currency(paymentCreditResponse.ChargeValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace("{{valuePaidToAssuranceTax}}", NumberFormat.Currency(assuranceFeeTaxValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace("{{AvalCompany}}", paymentCreditResponse.Store.AssuranceCompany.Name);
            templateHtml = templateHtml.Replace("{{payValue}}", NumberFormat.Currency(paymentCreditResponse.Credit.CreditPayment.GetTotalValuePaid, decimalNumbersRound));
            templateHtml = templateHtml.Replace("{{StoreName}}", paymentCreditResponse.Store.StoreName);
            templateHtml = templateHtml.Replace("{{paymentNumber}}", paymentCreditResponse.PaymentNumber.ToString());
            templateHtml = templateHtml.Replace("{{nextDueDate}}", nexDueDateString);
            templateHtml = templateHtml.Replace("{{creditNumber}}", paymentCreditResponse.Credit.GetCreditNumber.ToString());

            return templateHtml;
        }
    }
}