using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Builders;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Domain.Model.Customers.Queries.Commands;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Sequences.Gateway;
using Sc.Credits.Domain.UseCase.Credits.Strategy.NextFee;
using Sc.Credits.Helpers.Commons.Extensions;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Sc.Credits.Domain.Model.Fraud;
using Microsoft.AspNetCore.WebUtilities;
using System.Data;

namespace Sc.Credits.Domain.UseCase.Credits
{
    /// <summary>
    /// Credit uses case is an implementation of <see cref="ICreditUsesCase"/>
    /// </summary>
    public class CreditUsesCase : ICreditUsesCase
    {
        private readonly ICreditMasterRepository _creditMasterRepository;
        private readonly ISequenceRepository _sequenceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly CredinetAppSettings _credinetAppSettings;
        private readonly ICreditOperationsUseCase _creditOperations;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditUsesCase"/> class.
        /// </summary>
        /// <param name="creditMasterRepository">The credit master repository.</param>
        /// <param name="sequenceRepository">The sequence repository.</param>
        /// <param name="customerRepository">The customer repository.</param>
        /// <param name="appSettings">The application settings.</param>
        /// <param name="creditOperations">The credit operations.</param>
        public CreditUsesCase(ICreditMasterRepository creditMasterRepository,
            ISequenceRepository sequenceRepository,
            ICustomerRepository customerRepository,
            ISettings<CredinetAppSettings> appSettings,
            ICreditOperationsUseCase creditOperations)
        {
            _creditMasterRepository = creditMasterRepository;
            _sequenceRepository = sequenceRepository;
            _customerRepository = customerRepository;
            _credinetAppSettings = appSettings.Get();
            _creditOperations = creditOperations;
        }

        #region ICreditUsesCase Members

        /// <summary>
        /// <see cref="ICreditUsesCase.GetCreditDetails(CreditDetailDomainRequest)"/>
        /// </summary>
        /// <param name="creditDetailDomainRequest"></param>
        /// <returns>CreditDetailResponse with all the operations applied</returns>
        public CreditDetailResponse GetCreditDetails(CreditDetailDomainRequest creditDetailDomainRequest)
        {
            ValidateCustomer(creditDetailDomainRequest.Customer);
            ValidateStore(creditDetailDomainRequest.Store);

            int limitInMoths = GetTimeLimitInMonths(creditDetailDomainRequest.Customer, creditDetailDomainRequest.CreditValue,
                creditDetailDomainRequest.Store, creditDetailDomainRequest.DecimalNumbersRound, creditDetailDomainRequest.PartialCreditLimit);

            if (creditDetailDomainRequest.GetMonths() > limitInMoths)
            {
                throw new BusinessException(nameof(BusinessResponse.MonthsNumberNotValid), (int)BusinessResponse.MonthsNumberNotValid);
            }

            creditDetailDomainRequest.SetdownPayment(GetDownPayment(creditDetailDomainRequest.Customer, creditDetailDomainRequest.CreditValue,
                creditDetailDomainRequest.Store));

            CreditDetailResponse creditDetail = _creditOperations.CreateCreditDetails(creditDetailDomainRequest);

            return creditDetail;
        }

        /// <summary>
        /// Overload that doesnt required a customer to calculate
        /// <see cref="ICreditUsesCase.GetCreditDetails(SimulatedCreditRequest)"/>
        /// </summary>
        /// <param name="simulatedCreditDetailRequest">The simulation detail request.</param>
        /// <returns>CreditDetailResponse with operations made</returns>
        public CreditDetailResponse GetCreditDetails(SimulatedCreditRequest simulatedCreditDetailRequest)
        {
            ValidateStore(simulatedCreditDetailRequest.Store);

            int limitInMoths = GetTimeLimitInMonths(simulatedCreditDetailRequest.CreditValue,
                simulatedCreditDetailRequest.Store, simulatedCreditDetailRequest.DecimalNumbersRound);

            CheckMonths(simulatedCreditDetailRequest, limitInMoths);

            simulatedCreditDetailRequest.SetdownPayment(GetDownPayment(simulatedCreditDetailRequest.CreditValue,
                simulatedCreditDetailRequest.Store));

            CreditDetailResponse creditDetail = _creditOperations.CreateCreditDetails(simulatedCreditDetailRequest);

            return creditDetail;
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetTimeLimitInMonths(Customer, decimal, Store, int, decimal)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="creditValue"></param>
        /// <param name="store"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="partialCreditLimit"></param>
        /// <returns>Limit of months allow for the store</returns>
        public int GetTimeLimitInMonths(Customer customer, decimal creditValue, Store store, int decimalNumbersRound, decimal partialCreditLimit)
        {
            if (!customer.IsAvailableCreditLimit(creditValue, partialCreditLimit, out _))
            {
                throw new BusinessException(nameof(BusinessResponse.NotAvailableCreditLimit), (int)BusinessResponse.NotAvailableCreditLimit);
            }

            return store.GetTimeLimitInMonths(creditValue, decimalNumbersRound);
        }

        /// <summary>
        /// Overload that doesnt need customer to work
        /// <see cref="ICreditUsesCase.GetTimeLimitInMonths(decimal, Store, int)"/>
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="store"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns>Limit of months allow for the store</returns>
        public int GetTimeLimitInMonths(decimal creditValue, Store store, int decimalNumbersRound) =>
            store.GetTimeLimitInMonths(creditValue, decimalNumbersRound);

        /// <summary>
        /// Gets the store minimum and maximum credit limit by store identifier.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="decimalNumbersRound">The decimal numbers round.</param>
        /// <returns>range of value from storecategory of store</returns>
        public StoreCategoryRange GetStoreMinAndMaxCreditLimitByStoreId(Store store, int decimalNumbersRound)
        {
            return new StoreCategoryRange()
            {
                GetMinimumFeeValue = store.StoreCategory.GetMinimumFeeValue.Round(decimalNumbersRound),
                GetMaximumCreditValue = store.StoreCategory.GetMaximumCreditValue.Round(decimalNumbersRound),
            };
        }

        /// <summary>
        /// Create credit
        /// </summary>
        /// <param name="createCreditDomainRequest"></param>
        /// <param name="transaction"></param>
        /// <param name="setCreditLimit"></param>
        /// <returns></returns>
        /// <exception cref="credinet.exception.middleware.models.BusinessException">TokenAlreadyUsed</exception>
        public async Task<CreateCreditResponse> CreateAsync(CreateCreditDomainRequest createCreditDomainRequest, Transaction transaction = null, bool setCreditLimit = true)
        {
            Customer customer = createCreditDomainRequest.Customer;
            CreateCreditRequest createCreditRequest = createCreditDomainRequest.CreateCreditRequest;
            bool duplicatedCredit = await _creditMasterRepository.IsDuplicatedAsync(createCreditRequest.Token, DateTime.Today, customer.Id);

            if (duplicatedCredit)
            {
                throw new BusinessException(nameof(BusinessResponse.TokenAlreadyUsed), (int)BusinessResponse.TokenAlreadyUsed);
            }

            Store store = createCreditDomainRequest.Store;
            AppParameters parameters = createCreditDomainRequest.Parameters;

            createCreditDomainRequest.SetCreditValue(createCreditRequest.CreditValue.Round(parameters.DecimalNumbersRound));

            CreditDetailDomainRequest detailRequest = new CreditDetailDomainRequest(
                    customer, store, createCreditRequest.CreditValue, createCreditRequest.Frequency, parameters);

            detailRequest.SetFees(createCreditRequest.Fees);

            CreditDetailResponse creditDetails = GetCreditDetails(detailRequest);

            DateTime dueDate = GetNextFeeDateByFrequency(fee: 1, (Frequencies)createCreditRequest.Frequency, createCreditDomainRequest.CreditDate,
                previousFeeDate: createCreditDomainRequest.CreditDate);

            long creditNumber = await _sequenceRepository.GetNextAsync(store.Id, nameof(SequenceTypes.Credits));

            CreditMaster creditMaster = CreditBuilder.CreateBuilder()
                .Init(createCreditDomainRequest.Source, customer, createCreditRequest.CreditValue, creditNumber,
                    new UserInfo(createCreditRequest.UserName, createCreditRequest.UserId), _credinetAppSettings, setCreditLimit)
                .Rates(creditDetails.InterestRate, store.GetEffectiveAnnualRate)
                .CreditDate(createCreditDomainRequest.CreditDate)
                .SellerInfo(createCreditRequest.Seller, createCreditRequest.Products, createCreditRequest.Invoice)
                .AdditionalInfo(store, createCreditRequest.Token, createCreditRequest.Location, createCreditRequest.CreditRiskLevel)
                .FeesInfo(createCreditRequest.Fees, createCreditRequest.Frequency, creditDetails.FeeCreditValue)
                .AdditionalDetailInfo(createCreditDomainRequest.Status, createCreditDomainRequest.AuthMethod,
                    creditDetails.DownPayment, creditDetails.TotalDownPayment)
                .InitialPayment(createCreditDomainRequest.CreateCreditTransactionType, createCreditDomainRequest.OrdinaryPaymentType,
                    dueDate, createCreditDomainRequest.CreditDate)
                .AssuranceValues(store.GetAssurancePercentage, creditDetails.AssuranceValue, creditDetails.AssuranceFeeValue,
                    creditDetails.AssuranceTotalFeeValue, creditDetails.AssuranceTotalValue)
                .Build();

            await _creditMasterRepository.AddAsync(creditMaster, transaction);

            if (customer.CreditLimitIsUpdated())
            {
                await _customerRepository.UpdateAsync(customer, CustomerCommandsFields.UpdateCreditLimit, transaction);
            }

            return new CreateCreditResponse
            {
                CreditId = creditMaster.Id,
                CreditNumber = creditMaster.GetCreditNumber,
                CreditValue = createCreditRequest.CreditValue,
                EffectiveAnnualRate = store.GetEffectiveAnnualRate,
                Fees = createCreditRequest.Fees,
                TotalFeeValue = creditDetails.TotalFeeValue,
                AssuranceValue = creditDetails.AssuranceValue,
                IdDocument = createCreditRequest.IdDocument,
                TypeDocument = createCreditRequest.TypeDocument,
                DownPayment = creditDetails.DownPayment,
                InterestRate = creditDetails.InterestRate,
                TotalInterestValue = creditDetails.TotalInterestValue,
                TotalDownPayment = creditDetails.TotalDownPayment,
                FeeCreditValue = creditDetails.FeeCreditValue,
                AssuranceFeeValue = creditDetails.AssuranceFeeValue,
                AssuranceTotalValue = creditDetails.AssuranceTotalValue,
                AssuranceTaxValue = creditDetails.AssuranceTaxValue,
                AssuranceTaxFeeValue = creditDetails.AssuranceTaxFeeValue,
                DownPaymentPercentage = creditDetails.DownPaymentPercentage,
                AssurancePercentage = creditDetails.AssurancePercentage,
                AssuranceTotalFeeValue = creditDetails.AssuranceTotalFeeValue,
                AlternatePayment = creditMaster.Current.GetAlternatePayment,
                CreditMaster = creditMaster,
                StoreId = store.Id,
                TotalPaymentValue = creditDetails.TotalPaymentValue
            };
        }

        /// <summary>
        /// Delete credit
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(CreditMaster creditMaster, Transaction transaction = null)
        {
            return await _creditMasterRepository.DeleteAsync(creditMaster, transaction);
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetCustomerCreditLimitAsync(Customer, decimal, int, int,
        /// decimal, string)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="partialCreditLimit"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="arrearsGracePeriod"></param>
        /// <param name="minimumCreditValue"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public async Task<CustomerCreditLimitResponse> GetCustomerCreditLimitAsync(Customer customer, decimal partialCreditLimit, int decimalNumbersRound,
            int arrearsGracePeriod, decimal minimumCreditValue, string vendorId)
        {
            bool customerIsDefaulter = await CustomerIsDefaulterAsync(customer, vendorId, arrearsGracePeriod);
            bool isAvailableCreditLimit = customer.IsAvailableCreditLimit(minimumCreditValue, partialCreditLimit, out decimal availableCreditLimit);

            CustomerCreditLimitResponse customerCreditLimitResponse = new CustomerCreditLimitResponse
            {
                NewCreditButtonEnabled =
                    isAvailableCreditLimit
                    &&
                    !customerIsDefaulter,
                CreditLimit = customer.GetCreditLimit.Round(decimalNumbersRound),
                AvailableCreditLimit = availableCreditLimit.Round(decimalNumbersRound),
                ValidatedMail = customer.GetValidatedMail,
                Email = customer.GetEmail,
                Mobile = customer.GetMobile,
                FullName = customer.GetFullName,
                FirstName = customer.Name.GetFirstName,
                SecondName = customer.Name.GetSecondName,
                Defaulter = customerIsDefaulter,
                CreditLimitIncrease = customer.AllowCreditLimitIncrease(),
                IsAvailableCreditLimit = isAvailableCreditLimit,
                IsActive = customer.IsActive(),
                Status = customer.GetStatus,
                StatusName = ((CustomerStatuses)customer.GetStatus).ToString()
            };

            return customerCreditLimitResponse;
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetOriginalAmortizationSchedule(AmortizationScheduleRequest, int)"/>
        /// </summary>
        /// <param name="amortizationScheduleRequest"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        public AmortizationScheduleResponse GetOriginalAmortizationSchedule(AmortizationScheduleRequest amortizationScheduleRequest, int decimalNumbersRound)
        {
            AmortizationScheduleResponse amortizationScheduleResponse =
                new AmortizationScheduleResponse
                {
                    CreditValue = amortizationScheduleRequest.CreditValue,
                    AssuranceValue = amortizationScheduleRequest.AssuranceValue,
                    DownPayment = amortizationScheduleRequest.DownPayment,
                    AmortizationScheduleFees =
                        GetOriginalAmortizationScheduleFees(amortizationScheduleRequest, decimalNumbersRound).ToList(),
                    AmortizationScheduleAssuranceFees =
                        GetOriginalAmortizationScheduleAssuranceFees(amortizationScheduleRequest, decimalNumbersRound).ToList()
                };

            amortizationScheduleResponse.AmortizationScheduleFees.ForEach(fee =>
            {
                AmortizationScheduleAssuranceFee amortizationScheduleAssuranceFee =
                    amortizationScheduleResponse.AmortizationScheduleAssuranceFees.Single(assuranceFee => assuranceFee.Fee == fee.Fee);
                fee.TotalFeeValue = fee.FeeValue + amortizationScheduleAssuranceFee.AssurancePaymentValue;
            });

            return amortizationScheduleResponse;
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.UpdateExtraFields(UpdateCreditExtraFieldsRequest, CreditMaster)"/>
        /// </summary>
        /// <param name="updateCreditExtraFieldsRequest"></param>
        /// <param name="creditMaster"></param>
        public void UpdateExtraFields(UpdateCreditExtraFieldsRequest updateCreditExtraFieldsRequest, CreditMaster creditMaster)
        {
            creditMaster.SetSellerInfo(string.IsNullOrEmpty(updateCreditExtraFieldsRequest.Seller) ? creditMaster.GetCreditSeller : updateCreditExtraFieldsRequest.Seller, updateCreditExtraFieldsRequest.Products, updateCreditExtraFieldsRequest.Invoice);
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.UpdateStatus(int, CreditMaster)"/>
        /// </summary>
        /// <param name="newStatus"></param>
        /// <param name="creditMaster"></param>
        public void UpdateStatus(int newStatus, CreditMaster creditMaster)
        {
            creditMaster.SetStatusId(newStatus);
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.UpdateScCode(string, CreditMaster)"/>
        /// </summary>
        /// <param name="scCode"></param>
        /// <param name="creditMaster"></param>
        public void UpdateScCode(string scCode, CreditMaster creditMaster)
        {
            creditMaster.SetScCode(scCode);
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetTokenSmsNotification(string, Customer, Store,
        /// CreditDetailResponse, int, int, string, int)"/>
        /// </summary>
        /// <param name="template"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="creditDetails"></param>
        /// <param name="months"></param>
        /// <param name="frequency"></param>
        /// <param name="token"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        public SmsNotificationRequest GetTokenSmsNotification(string template, Customer customer, Store store, CreditDetailResponse creditDetails, int months,
            int frequency, string token, int decimalNumbersRound)
        {
            string message = template;

            message = message.Replace("{customer.FullName}", $"{customer.Name.GetFirstName} {customer.Name.GetFirstLastName}");
            message = message.Replace("{creditValue}", NumberFormat.CurrencySms(creditDetails.CreditValue, decimalNumbersRound, _credinetAppSettings.SmsCurrencyCode));
            message = message.Replace("{token}", token);

            string url = GetAmortizationScheduleUrl(customer, creditDetails, frequency);

            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest
            {
                Message = message,
                Mobile = customer.GetMobile,
                UrlTemplateValues = new List<UrlTemplateValue>
                {
                    new UrlTemplateValue
                    {
                        Key = "{url}",
                        Value = url
                    }
                }
            };

            return smsNotificationRequest;
        }

        /// <summary>
        /// Sends the SMS notification.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="customer">The customer.</param>
        /// <returns></returns>
        public SmsNotificationRequest SendSmsNotification(string template, Customer customer)
        {
            string message = template;
            message = message.Replace("{{customer.FirstName}}", $"{customer?.Name.GetFirstName}");
            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest
            {
                Message = message,
                Mobile = customer?.GetMobile
            };

            return smsNotificationRequest;
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetTokenMailNotification(Customer, Store,
        /// CreditDetailResponse, int, int, string, int)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="creditDetails"></param>
        /// <param name="months"></param>
        /// <param name="frequency"></param>
        /// <param name="token"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        public MailNotificationRequest GetTokenMailNotification(Customer customer, Store store, CreditDetailResponse creditDetails, int months, int frequency,
            string token, int decimalNumbersRound)
        {
            List<TemplateValue> templateValues = new List<TemplateValue>
            {
                new TemplateValue
                {
                    Key = "{{customer.FullName}}",
                    Value = customer.GetFullName
                },
                new TemplateValue
                {
                    Key = "{{date}}",
                    Value = DateTime.Now.ToShortDateString()
                },
                new TemplateValue
                {
                    Key = "{{creditValue}}",
                    Value = NumberFormat.Currency(creditDetails.CreditValue, decimalNumbersRound)
                },
                new TemplateValue
                {
                    Key = "{{store.StoreName}}",
                    Value = store.StoreName
                },

                new TemplateValue
                {
                    Key = "{{amortizationScheduleFees}}",
                    Value = GetOriginalAmortizationScheduleHtml(creditDetails, frequency, decimalNumbersRound, DateTime.Now, out DateTime lastFeeDate)
                },

                new TemplateValue
                {
                    Key = "{{token}}",
                    Value = token
                }
            };

            MailNotificationRequest mailNotificationRequest = new MailNotificationRequest
            {
                TemplateInfo = new TemplateInfo
                {
                    TemplateId = _credinetAppSettings.CreditTokenNotificationTemplateId,
                    TemplateValues = templateValues
                },
                Recipients = new List<string> { customer.GetEmail },
                Subject = $"Información sobre el crédito que estás generando en el almacén {store.StoreName}"
            };

            return mailNotificationRequest;
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetRiskyCreditRequestNotification(Customer, Store, GenerateTokenRequest, CustomerRiskLevel, int)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="generateTokenRequest"></param>
        /// <param name="riskLevel"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public MailNotificationRequest GetRiskyCreditRequestNotification(
            Customer customer,
            Store store,
            GenerateTokenRequest generateTokenRequest,
            CustomerRiskLevel riskLevel,
            int decimalNumbersRound, Guid transactionId)
        {
            CustomerFraudModel customerFraudModel = new CustomerFraudModel()
            {
                StoreName = store.StoreName,
                CustomerDocumentId = customer.IdDocument,
                CreditValue = generateTokenRequest.CreditValue.ToString(),
                Observations = string.Join(",", riskLevel.Observations),
                Source = generateTokenRequest.Source,
                Frequency = generateTokenRequest.Frequency,
                Months = generateTokenRequest.Months,
                StoreId = generateTokenRequest.StoreId,
                TransactionId = transactionId
            };

            List<TemplateValue> templateValues = new List<TemplateValue>{
                new TemplateValue {
                    Key = "{{{CustomerDocumentId}}}",
                    Value = customer.IdDocument
                },
                new TemplateValue
                {
                    Key = "{{{CreditValue}}}",
                    Value = NumberFormat.Currency(generateTokenRequest.CreditValue, decimalNumbersRound)
                },
                new TemplateValue
                {
                    Key = "{{{Observacion}}}",
                    Value = $"* {string.Join("<br/>* ", riskLevel.Observations)}"
                },
                new TemplateValue
                {
                    Key = "{{{StoreName}}}",
                    Value = store.StoreName
                },
                new TemplateValue
                {
                    Key = "{{{MailCustomer}}}",
                    Value = customer.GetEmail
                },
                new TemplateValue
                {
                    Key = "{{{Url}}}",
                    Value = GetQueryString(customerFraudModel)
                },
                new TemplateValue
                {
                    Key = "{{{MailFooter}}}",
                    Value = _credinetAppSettings.MailFooterTemplate
                }
            };

            MailNotificationRequest mailNotificationRequest = new MailNotificationRequest
            {
                TemplateInfo = new TemplateInfo
                {
                    TemplateId = _credinetAppSettings.RiskyCreditRequestTemplateId,
                    TemplateValues = templateValues
                },
                Recipients = new List<string> { _credinetAppSettings.RiskyCreditNotificationRecipient },
                Subject = "¡Alerta! Detectamos un posible Fraude. Cédula " + customer.IdDocument,
            };

            return mailNotificationRequest;
        }

        #region Fraud

        /// <summary>
        /// Gets the query string.
        /// </summary>
        /// <param name="customerFraudModel">The customerfraudemodel.</param>
        /// <returns></returns>
        private string GetQueryString(CustomerFraudModel customerFraudModel)
        {
            Dictionary<string, string> queryParams = QueryHelpersStrinBuild(new CustomerFraudModel()
            {
                StoreName = customerFraudModel.StoreName,
                CustomerDocumentId = customerFraudModel.CustomerDocumentId,
                CreditValue = customerFraudModel.CreditValue,
                Observations = customerFraudModel.Observations,
                Source = customerFraudModel.Source,
                Frequency = customerFraudModel.Frequency,
                Months = customerFraudModel.Months,
                StoreId = customerFraudModel.StoreId,
                TransactionId = customerFraudModel.TransactionId,
            });

            UriBuilder url = new UriBuilder($"{_credinetAppSettings.UrlSiteFraud}");

            string serviceurl = AddQueryString(url, queryParams);

            return serviceurl;
        }

        /// <summary>
        /// Adds the query string.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="queryParams">The query parameters.</param>
        /// <returns></returns>
        private string AddQueryString(UriBuilder url, Dictionary<string, string> queryParams)
        {
            return QueryHelpers.AddQueryString(url.Uri.ToString(), queryParams);
        }

        /// <summary>
        /// Queries the helpers strin build.
        /// </summary>
        /// <param name="QueryStringModel">The query string model.</param>
        /// <returns></returns>
        private Dictionary<string, string> QueryHelpersStrinBuild(CustomerFraudModel QueryStringModel)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();

            queryParams.Add("storeName", QueryStringModel.StoreName);
            queryParams.Add("customerDocumentId", QueryStringModel.CustomerDocumentId);
            queryParams.Add("creditValue", QueryStringModel.CreditValue);
            queryParams.Add("observations", QueryStringModel.Observations);
            queryParams.Add("source", QueryStringModel.Source);
            queryParams.Add("frequency", QueryStringModel.Frequency.ToString());
            queryParams.Add("months", QueryStringModel.Months.ToString());
            queryParams.Add("storeId", QueryStringModel.StoreId);
            queryParams.Add("transactionId", QueryStringModel.TransactionId.ToString());
            return queryParams;
        }

        #endregion Fraud

        /// <summary>
        /// <see cref="ICreditUsesCase.GetCreateCreditMailNotification(Customer, CreditMaster,
        /// Credit, CreateCreditResponse, Store, string, int)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="creditMaster"></param>
        /// <param name="credit"></param>
        /// <param name="createCreditResponse"></param>
        /// <param name="store"></param>
        /// <param name="promissoryNoteFileName"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        public MailNotificationRequest GetCreateCreditMailNotification(Customer customer, CreditMaster creditMaster, Credit credit, CreateCreditResponse createCreditResponse,
            Store store, string promissoryNoteFileName, int decimalNumbersRound)
        {
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
                    Key = "{{date}}",
                    Value = creditMaster.GetCreditDate.ToShortDateString()
                },
                new TemplateValue
                {
                    Key = "{{creditValue}}",
                    Value = NumberFormat.Currency(createCreditResponse.CreditValue, decimalNumbersRound)
                },
                new TemplateValue
                {
                    Key = "{{store.StoreName}}",
                    Value = store.StoreName
                },

                new TemplateValue
                {
                    Key = "{{creditLimitDate}}",
                    Value = credit.CreditPayment.GetDueDate.ToShortDateString()
                },

                new TemplateValue
                {
                    Key = "{{credit.Id}}",
                    Value = creditMaster.GetCreditNumber.ToString()
                }
            };

            MailNotificationRequest mailNotificationRequest = new MailNotificationRequest
            {
                TemplateInfo = new TemplateInfo
                {
                    TemplateId = _credinetAppSettings.CreateCreditNotificationTemplateId,
                    TemplateValues = templateValues
                },
                Recipients = new List<string> { customer.GetEmail },
                Subject = $"Pagaré de tu crédito en el almacén {store.StoreName}",
                BlobStorageAttachments = new List<BlobStorageAttachment>
                {
                    new BlobStorageAttachment
                    {
                        BlobContainerName = _credinetAppSettings.PdfBlobContainerName,
                        Path = _credinetAppSettings.PromissoryNotePath,
                        FileName = promissoryNoteFileName
                    }
                }
            };

            return mailNotificationRequest;
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetValidEffectiveAnnualRate(decimal, decimal)"/>
        /// </summary>
        /// <param name="effectiveAnualRate1"></param>
        /// <param name="effectiveAnualRate2"></param>
        /// <returns></returns>
        public decimal GetValidEffectiveAnnualRate(decimal effectiveAnualRate1, decimal effectiveAnualRate2)
        {
            return Math.Min(effectiveAnualRate1, effectiveAnualRate2);
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetInterestRate(decimal, int)"/>
        /// </summary>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        public decimal GetInterestRate(decimal effectiveAnnualRate, int frequency) =>
            _creditOperations.GetInterestRate(effectiveAnnualRate, frequency);

        /// <summary>
        /// <see cref="ICreditUsesCase.HasArrears(DateTime, DateTime, int)"/>
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <param name="dueDate"></param>
        /// <param name="graceDays"></param>
        /// <returns></returns>
        public bool HasArrears(DateTime calculationDate, DateTime dueDate, int graceDays)
        {
            bool hasArrearsWithGraceDays = calculationDate.Date > dueDate.AddDays(graceDays);

            return hasArrearsWithGraceDays;
        }

        /// <summary>
        /// Checks the months.
        /// </summary>
        /// <param name="simulatedCredit">The simulated credit.</param>
        /// <param name="limitInMoths">The limit in moths.</param>
        /// <exception cref="BusinessException">MonthsNumberNotValid</exception>
        private void CheckMonths(GeneralCreditDetailDomainRequest simulatedCredit, int limitInMoths)
        {
            if (simulatedCredit.Fees == 0)
            {
                simulatedCredit.SetFeesByMonths(limitInMoths);
            }
            else
            {
                if (simulatedCredit.GetMonths() > limitInMoths)
                {
                    throw new BusinessException(nameof(BusinessResponse.MonthsNumberNotValid), (int)BusinessResponse.MonthsNumberNotValid);
                }
            }
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetArrearsDays(DateTime,  DateTime)"/>
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public int GetArrearsDays(DateTime calculationDate, DateTime dueDate)
        {
            calculationDate = calculationDate.Date;

            if (calculationDate < dueDate)
                return 0;

            return calculationDate.Subtract(dueDate).Days;
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetPromissoryNoteInfo(CreditMaster, AppParameters, string, bool)"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="parameters"></param>
        /// <param name="template"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        public PromissoryNoteInfo GetPromissoryNoteInfo(CreditMaster creditMaster, AppParameters parameters,
            string template, bool reprint)
        {
            Credit credit = creditMaster.Current;

            int decimalNumbersRound = parameters.DecimalNumbersRound;
            int interestRateDecimalNumbers = parameters.InterestRateDecimalNumbers;

            decimal originalInterestRate = GetInterestRate(creditMaster.GetEffectiveAnnualRate, credit.GetFrequency).Round(interestRateDecimalNumbers);

            AmortizationScheduleResponse amortizationScheduleResponse = GetOriginalAmortizationSchedule(
                AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate, originalInterestRate),
                decimalNumbersRound);

            string cellPhone = parameters.CellPhone;
            string nit = parameters.Nit;

            PromissoryNoteInfo promissoryNoteInfo = new PromissoryNoteInfo
            {
                AssuranceCompany = creditMaster.Store.AssuranceCompany?.Name,
                CellPhone = cellPhone,
                CreditDate = creditMaster.GetCreditDate,
                CreditNumber = creditMaster.GetCreditNumber,
                CreditValue = credit.GetCreditValue.Round(decimalNumbersRound),
                CustomerDocumentType = creditMaster.Customer.DocumentType,
                CustomerFullName = creditMaster.Customer.GetFullName,
                CustomerIdDocument = creditMaster.Customer.IdDocument,
                DownPayment = credit.GetDownPayment.Round(decimalNumbersRound),
                EffectiveAnnualRate = creditMaster.Store.GetEffectiveAnnualRate,
                InterestRate = credit.GetInterestRate,
                Invoice = creditMaster.GetCreditInvoice,
                LettersCreditValue = $"{NumberConversions.ToLettersSpanish(credit.GetCreditValue.ToString())} Pesos",
                Nit = nit,
                StoreName = creditMaster.Store.StoreName,
                StorePhone = creditMaster.Store.GetPhone ?? string.Empty,
                Token = creditMaster.GetToken,
                Template = template,
                PaymentPlan = amortizationScheduleResponse.AmortizationScheduleFees.Select(amortizationFee =>
                {
                    AmortizationScheduleAssuranceFee assuranceFee = amortizationScheduleResponse.AmortizationScheduleAssuranceFees[amortizationFee.Fee];
                    return new PromissoryNotePaymentPlan
                    {
                        AssuranceFeeValue = assuranceFee.AssuranceFeeValue,
                        AssuranceTaxValue = assuranceFee.AssuranceTaxValue,
                        AssuranceTotalFeeValue = assuranceFee.AssurancePaymentValue,
                        Balance = amortizationFee.Balance,
                        CreditValuePayment = amortizationFee.CreditValuePayment,
                        DueDate = amortizationFee.FeeDate,
                        Fee = amortizationFee.Fee,
                        FeeValue = amortizationFee.FeeValue,
                        InterestValue = amortizationFee.InterestValue,
                        TotalFeeValue = amortizationFee.TotalFeeValue
                    };
                }).ToList()
            };

            promissoryNoteInfo.Template = GetPromissoryNoteTemplate(promissoryNoteInfo, decimalNumbersRound, reprint);

            return promissoryNoteInfo;
        }

        /// <summary>
        /// <see
        /// cref="ICreditUsesCase.GetPaidCreditCertificateTemplate(PaidCreditCertificateResponse, bool)"/>
        /// </summary>
        /// <param name="paidCreditCertificateResponse"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        public string GetPaidCreditCertificateTemplate(PaidCreditCertificateResponse paidCreditCertificateResponse, bool reprint)
        {
            string templateHtml = paidCreditCertificateResponse.Template;
            string format = "${{{0}}}";

            templateHtml = templateHtml.Replace(string.Format(format, "CurrentDate"), paidCreditCertificateResponse.CurrentDate.ToShortDateString());
            templateHtml = templateHtml.Replace(string.Format(format, "FullName"), paidCreditCertificateResponse.FullName);
            templateHtml = templateHtml.Replace(string.Format(format, "TypeDocument"), paidCreditCertificateResponse.TypeDocument);
            templateHtml = templateHtml.Replace(string.Format(format, "IdDocument"), paidCreditCertificateResponse.IdDocument);
            templateHtml = templateHtml.Replace(string.Format(format, "StoreName"), paidCreditCertificateResponse.StoreName);
            templateHtml = templateHtml.Replace(string.Format(format, "CreditDate"), paidCreditCertificateResponse.CreditDate.ToShortDateString());
            templateHtml = templateHtml.Replace(string.Format(format, "CreditNumber"), paidCreditCertificateResponse.CreditNumber.ToString());
            templateHtml = templateHtml.Replace(string.Format(format, "TransactionDate"), paidCreditCertificateResponse.TransactionDate.ToShortDateString());
            templateHtml = templateHtml.Replace(string.Format(format, "CellPhone"), paidCreditCertificateResponse.CellPhone);
            templateHtml = templateHtml.Replace(string.Format(format, "Reprint"), reprint ? "block" : "none");

            return templateHtml;
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.CustomerIsDefaulterAsync(Customer, string, int)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="vendorId"></param>
        /// <param name="arrearsGracePeriod"></param>
        /// <returns></returns>
        public async Task<bool> CustomerIsDefaulterAsync(Customer customer, string vendorId, int arrearsGracePeriod)
        {
            List<CreditMaster> creditMasters =
                await _creditMasterRepository.GetActiveAndCancelRequestCreditsAsync(customer);

            foreach (var creditMaster in creditMasters)
            {
                Credit credit = creditMaster.Current;

                bool hasArrearsWithoutGraceDays = HasArrears(DateTime.Today, credit.CreditPayment.GetDueDate, graceDays: 0);
                bool hasArrears = HasArrears(DateTime.Today, credit.CreditPayment.GetDueDate, arrearsGracePeriod);
                bool creditBelongsToVendor = creditMaster.Store.GetVendorId == vendorId;

                if ((creditBelongsToVendor && hasArrearsWithoutGraceDays) || hasArrears)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.CreateCreditHistory(List{CreditMaster},
        /// List{RequestCancelCredit}, AppParameters)"/>
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="requestCancelCreditsCanceled"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<CreditHistoryResponse> CreateCreditHistory(List<CreditMaster> creditMasters, List<RequestCancelCredit> requestCancelCreditsCanceled,
            AppParameters parameters)
        {
            int decimalNumbersRound = parameters.DecimalNumbersRound;

            return creditMasters
                .OrderByDescending(creditMaster => creditMaster.GetCreditDateComplete)
                .Select(creditMaster =>
                {
                    int arrearsDays = 0;
                    if (creditMaster.IsActiveOrCancelRequest())
                    {
                        arrearsDays = GetArrearsDays(DateTime.Today, creditMaster.Current.CreditPayment.GetDueDate);
                    }

                    RequestCancelCredit requestCancelCredit =
                        requestCancelCreditsCanceled
                            .FirstOrDefault(request => request.GetCreditMasterId == creditMaster.Id);

                    return new CreditHistoryResponse
                    {
                        ArrearsDays = arrearsDays,
                        CancelDate = requestCancelCredit?.GetProcessDate,
                        CreditDate = creditMaster.GetCreditDate,
                        CreditId = creditMaster.Id,
                        CreditNumber = creditMaster.GetCreditNumber,
                        CreditValue = creditMaster.Current.GetCreditValue.Round(decimalNumbersRound),
                        Status = creditMaster.Status.Name,
                        StatusId = creditMaster.GetStatusId,
                        StoreId = creditMaster.GetStoreId,
                        StoreName = creditMaster.Store.StoreName
                    };
                })
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetNextFeeDateByFrequency(int, Frequencies, DateTime, DateTime?)"/>
        /// </summary>
        /// <param name="fee"></param>
        /// <param name="frequency"></param>
        /// <param name="firstFeeDate"></param>
        /// <param name="previousFeeDate"></param>
        /// <returns></returns>
        public DateTime GetNextFeeDateByFrequency(int fee, Frequencies frequency, DateTime firstFeeDate, DateTime? previousFeeDate = null)
        {
            NextFeeStrategy nextFeeStrategy;

            switch (frequency)
            {
                case Frequencies.ScBiweekly:
                    nextFeeStrategy = new ScBiWeeklyNextFeeStrategy(fee, firstFeeDate, previousFeeDate);
                    break;

                case Frequencies.Monthly:
                    nextFeeStrategy = new MonthlyNextFeeStrategy(fee, firstFeeDate);
                    break;

                default:
                    nextFeeStrategy = new OrdinaryNextFeeStrategy(fee, (int)frequency, firstFeeDate);
                    break;
            }

            return nextFeeStrategy.GetNextDate();
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.CalculateAssuranceToBalance(decimal, decimal, decimal)"/>
        /// </summary>
        /// <param name="creditValuePayment"></param>
        /// <param name="assurancePercentage"></param>
        /// <param name="assuranceTax"></param>
        /// <returns></returns>
        public decimal CalculateAssuranceToBalance(decimal creditValuePayment, decimal assurancePercentage, decimal assuranceTax) =>
                        _creditOperations.CalculateAssuranceValue(creditValuePayment, assurancePercentage) * (1 + assuranceTax);

        #endregion ICreditUsesCase Members

        #region Private Methods

        /// <summary>
        /// Get promissory note template
        /// </summary>
        /// <param name="promissoryNoteInfo"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        private string GetPromissoryNoteTemplate(PromissoryNoteInfo promissoryNoteInfo, int decimalNumbersRound, bool reprint)
        {
            int decimalNumbersRoundPercentage = 2;
            string templateHtml = promissoryNoteInfo.Template;
            string format = "${{{0}}}";
            templateHtml = templateHtml.Replace(string.Format(format, "CreditValue"), NumberFormat.Currency(promissoryNoteInfo.CreditValue, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "Nit"), promissoryNoteInfo.Nit);
            templateHtml = templateHtml.Replace(string.Format(format, "ScPhone"), promissoryNoteInfo.CellPhone);
            templateHtml = templateHtml.Replace(string.Format(format, "FullName"), promissoryNoteInfo.CustomerFullName);
            templateHtml = templateHtml.Replace(string.Format(format, "TypeDocument"), promissoryNoteInfo.CustomerDocumentType);
            templateHtml = templateHtml.Replace(string.Format(format, "Document"), promissoryNoteInfo.CustomerIdDocument);
            templateHtml = templateHtml.Replace(string.Format(format, "PurchaseDate"), promissoryNoteInfo.CreditDate.ToShortDateString());
            templateHtml = templateHtml.Replace(string.Format(format, "StoreName"), promissoryNoteInfo.StoreName);
            templateHtml = templateHtml.Replace(string.Format(format, "StoreNumber"), promissoryNoteInfo.StorePhone);
            templateHtml = templateHtml.Replace(string.Format(format, "CreditNumber"), promissoryNoteInfo.CreditNumber.ToString());
            templateHtml = templateHtml.Replace(string.Format(format, "BillNumber"), promissoryNoteInfo.Invoice);
            templateHtml = templateHtml.Replace(string.Format(format, "CreditValue"), NumberFormat.Currency(promissoryNoteInfo.CreditValue, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "LettersCreditValue"), promissoryNoteInfo.LettersCreditValue);
            templateHtml = templateHtml.Replace(string.Format(format, "InteresRate"), (promissoryNoteInfo.EffectiveAnnualRate * 100).Round(decimalNumbersRoundPercentage).ToString());
            templateHtml = templateHtml.Replace(string.Format(format, "TablePayment"), GenerateTablePaymentHtmlTemplate(promissoryNoteInfo, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "TableInterests"), GenerateTableInterestsHtmlTemplate(promissoryNoteInfo, decimalNumbersRound));
            templateHtml = templateHtml.Replace(string.Format(format, "FooterClass"), promissoryNoteInfo.PaymentPlan.Count > 5 ? "sc-footer" : "");
            templateHtml = templateHtml.Replace(string.Format(format, "Reprint"), reprint ? "block" : "none");

            return templateHtml;
        }

        /// <summary>
        /// Generate Table Payment Html Template
        /// </summary>
        /// <param name="promissoryNoteInfo"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private string GenerateTablePaymentHtmlTemplate(PromissoryNoteInfo promissoryNoteInfo, int decimalNumbersRound)
        {
            string TablePayment = string.Empty;
            string trFormat = "<tr>{0}{1}{2}</tr>";
            string tdFormat = "<td>{0}</td>";

            promissoryNoteInfo.PaymentPlan.ForEach(promissoryNotePaymetnPlan =>
            {
                string feeTd = string.Format(tdFormat, promissoryNotePaymetnPlan.Fee == 0 ? "INICIAL" : promissoryNotePaymetnPlan.Fee.ToString());
                string totalFeeValue = string.Format(tdFormat, NumberFormat.Currency(promissoryNotePaymetnPlan.TotalFeeValue, decimalNumbersRound));
                string dueDate = string.Format(tdFormat, promissoryNotePaymetnPlan.DueDate.ToShortDateString());

                TablePayment += string.Format(trFormat, feeTd, totalFeeValue, dueDate);
            });

            return TablePayment;
        }

        /// <summary>
        /// Generate Table Interests Html Template
        /// </summary>
        /// <param name="promissoryNoteInfo"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private string GenerateTableInterestsHtmlTemplate(PromissoryNoteInfo promissoryNoteInfo, int decimalNumbersRound)
        {
            string TableInterests = string.Empty;
            string trFormat = "<tr>{0}{1}{2}{3}</tr>";
            string tdFormat = "<td>{0}</td>";

            promissoryNoteInfo.PaymentPlan.ForEach(promissoryNotePaymetnPlan =>
            {
                string feeTd = string.Format(tdFormat, promissoryNotePaymetnPlan.Fee == 0 ? "INICIAL" : promissoryNotePaymetnPlan.Fee.ToString());
                string creditValuePayment = string.Format(tdFormat, NumberFormat.Currency(promissoryNotePaymetnPlan.CreditValuePayment, decimalNumbersRound));
                string interestValue = string.Format(tdFormat, NumberFormat.Currency(promissoryNotePaymetnPlan.InterestValue, decimalNumbersRound));
                string assuranceTotalFeeValue = string.Format(tdFormat, NumberFormat.Currency(promissoryNotePaymetnPlan.AssuranceTotalFeeValue, decimalNumbersRound));

                TableInterests += string.Format(trFormat, feeTd, creditValuePayment, interestValue, assuranceTotalFeeValue);
            });

            return TableInterests;
        }

        /// <summary>
        /// Validate down payment store
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        private bool ValidateDownPaymentStore(Customer customer, Store store) =>
            customer.Profile.IsStoreDownPayment() && store.GetMandatoryDownPayment;

        /// <summary>
        /// Get initial fee
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="creditValue"></param>
        /// <param name="store"></param>
        private decimal GetDownPayment(Customer customer, decimal creditValue, Store store)
        {
            if (customer.Profile.IsAlwaysDownPayment() || ValidateDownPaymentStore(customer, store))
            {
                return store.DownPayment(creditValue);
            }
            return 0;
        }

        /// <summary>
        /// overload doesnt need a customer to calculate down payment.
        /// </summary>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="store">The store.</param>
        /// <returns>the downpayment for the store</returns>
        private decimal GetDownPayment(decimal creditValue, Store store)
        {
            if (store.GetMandatoryDownPayment)
            {
                return store.DownPayment(creditValue);
            }
            return 0;
        }

        /// <summary>
        /// Get original amortization schedule fees
        /// </summary>
        /// <param name="amortizationScheduleRequest"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private IEnumerable<AmortizationScheduleFee> GetOriginalAmortizationScheduleFees(AmortizationScheduleRequest amortizationScheduleRequest,
            int decimalNumbersRound)
        {
            decimal creditValue = amortizationScheduleRequest.CreditValue;

            DateTime? previousFeeDate = null;
            for (int fee = 0; fee <= amortizationScheduleRequest.Fees; fee++)
            {
                DateTime feeDate = GetNextFeeDateByFrequency(fee, (Frequencies)amortizationScheduleRequest.Frequency,
                    amortizationScheduleRequest.InitialDate, previousFeeDate);

                bool isDownPayment = fee == 0;
                decimal currentFeeValue = isDownPayment ? amortizationScheduleRequest.DownPayment : amortizationScheduleRequest.FeeValue;
                decimal interestValue = isDownPayment ?
                    0
                    : CalculateInterestValue(creditValue, amortizationScheduleRequest.InterestRate).Round(decimalNumbersRound);

                bool isLastFee = fee == amortizationScheduleRequest.Fees;

                if (isLastFee)
                {
                    currentFeeValue = creditValue + interestValue;
                }

                decimal creditValuePayment = currentFeeValue - interestValue;
                decimal finalBalance = creditValue - creditValuePayment;

                yield return new AmortizationScheduleFee
                {
                    Fee = fee,
                    FeeDate = feeDate,
                    Balance = creditValue.Round(decimalNumbersRound),
                    FeeValue = currentFeeValue.Round(decimalNumbersRound),
                    InterestValue = interestValue.Round(decimalNumbersRound),
                    CreditValuePayment = creditValuePayment.Round(decimalNumbersRound),
                    FinalBalance = finalBalance.Round(decimalNumbersRound)
                };

                creditValue = finalBalance;
                previousFeeDate = feeDate;
            }
        }

        /// <summary>
        /// Calculate interest value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="interestRate"></param>
        /// <returns></returns>
        private decimal CalculateInterestValue(decimal value, decimal interestRate) =>
            value * interestRate;

        /// <summary>
        /// Get original amortization schedule assurance fees
        /// </summary>
        /// <param name="amortizationScheduleRequest"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private IEnumerable<AmortizationScheduleAssuranceFee> GetOriginalAmortizationScheduleAssuranceFees(AmortizationScheduleRequest amortizationScheduleRequest,
            int decimalNumbersRound)
        {
            decimal assuranceValue = amortizationScheduleRequest.AssuranceValue;
            decimal assuranceFeeValue = amortizationScheduleRequest.AssuranceFeeValue;
            decimal assuranceTotalFeeValue = amortizationScheduleRequest.AssuranceTotalFeeValue;
            decimal assuranceTaxValue = amortizationScheduleRequest.AssuranceTotalFeeValue - assuranceFeeValue;
            bool hasDownPayment = amortizationScheduleRequest.DownPayment > 0;

            DateTime? previousFeeDate = null;
            for (int fee = 0; fee <= amortizationScheduleRequest.Fees; fee++)
            {
                bool isDownPayment = fee == 0;
                bool isDownPaymentAndNotHasDownPayment = isDownPayment && !hasDownPayment;
                decimal finalBalance = isDownPaymentAndNotHasDownPayment ? assuranceValue : assuranceValue - assuranceFeeValue;
                DateTime feeDate = GetNextFeeDateByFrequency(fee, (Frequencies)amortizationScheduleRequest.Frequency,
                    amortizationScheduleRequest.InitialDate, previousFeeDate);

                bool isLastFee = fee == amortizationScheduleRequest.Fees;

                if (isLastFee)
                {
                    assuranceFeeValue += finalBalance;
                    finalBalance = assuranceValue - assuranceFeeValue;
                    assuranceTotalFeeValue = assuranceFeeValue + assuranceTaxValue;
                }

                yield return new AmortizationScheduleAssuranceFee
                {
                    Fee = fee,
                    FeeDate = feeDate,
                    Balance = assuranceValue.Round(decimalNumbersRound),
                    AssuranceFeeValue = isDownPaymentAndNotHasDownPayment ? 0 : assuranceFeeValue.Round(decimalNumbersRound),
                    AssuranceTaxValue = isDownPaymentAndNotHasDownPayment ? 0 : assuranceTaxValue.Round(decimalNumbersRound),
                    AssurancePaymentValue = isDownPaymentAndNotHasDownPayment ? 0 : assuranceTotalFeeValue.Round(decimalNumbersRound),
                    FinalBalance = finalBalance.Round(decimalNumbersRound)
                };

                assuranceValue = finalBalance;
                previousFeeDate = feeDate;
            }
        }

        /// <summary>
        /// <see cref="ICreditUsesCase.GetAmortizationScheduleUrl(Customer, CreditDetailResponse, int)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="creditDetails"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        public string GetAmortizationScheduleUrl(Customer customer, CreditDetailResponse creditDetails, int frequency)
        {
            string url = _credinetAppSettings.AmortizationScheduleUrlTemplate;

            url = url.Replace("{creditValue}", creditDetails.CreditValue.ToString().Replace(",", "."));
            url = url.Replace("{interestRate}", creditDetails.InterestRate.ToString().Replace(",", "."));
            url = url.Replace("{frequency}", frequency.ToString());
            url = url.Replace("{fees}", creditDetails.Fees.ToString());
            url = url.Replace("{assuranceValue}", creditDetails.AssuranceValue.ToString().Replace(",", "."));
            url = url.Replace("{downPayment}", creditDetails.DownPayment.ToString().Replace(",", "."));
            url = url.Replace("{initialDate}", DateTime.Now.ToString("yyyy/MM/dd"));
            url = url.Replace("{feeValue}", creditDetails.FeeCreditValue.ToString().Replace(",", "."));
            url = url.Replace("{assuranceFeeValue}", creditDetails.AssuranceFeeValue.ToString().Replace(",", "."));
            url = url.Replace("{assuranceTotalFeeValue}", creditDetails.AssuranceTotalFeeValue.ToString().Replace(",", "."));
            url = url.Replace("{customer.FullName}", customer.GetFullName);

            return url;
        }

        /// <summary>
        /// Get original amortization schedule Html
        /// </summary>
        /// <returns></returns>
        public string GetOriginalAmortizationScheduleHtml(CreditDetailResponse creditDetails, int frequency, int decimalNumbersRound,
                                                            DateTime createDate, out DateTime lastFeeDate)
        {
            AmortizationScheduleResponse amortizationSchedule = GetOriginalAmortizationSchedule(
                new AmortizationScheduleRequest
                {
                    CreditValue = creditDetails.CreditValue,
                    InitialDate = createDate,
                    FeeValue = creditDetails.FeeCreditValue,
                    InterestRate = creditDetails.InterestRate,
                    Frequency = frequency,
                    Fees = creditDetails.Fees,
                    DownPayment = creditDetails.DownPayment,
                    AssuranceValue = creditDetails.AssuranceValue,
                    AssuranceFeeValue = creditDetails.AssuranceFeeValue,
                    AssuranceTotalFeeValue = creditDetails.AssuranceTotalFeeValue
                },
                decimalNumbersRound);

            lastFeeDate = amortizationSchedule.AmortizationScheduleFees.LastOrDefault().FeeDate;

            string amortizationScheduleHtml = string.Empty;
            string trFormat = "<tr class='table__content-payment'>{0}{1}{2}{3}{4}{5}{6}</tr>";
            string tdFormat = "<td>{0}</td>";
            int index = 0;

            amortizationSchedule.AmortizationScheduleFees.ForEach(fee =>
            {
                string feeTd = string.Format(tdFormat, fee.Fee == 0 ? "Inicial" : fee.Fee.ToString());
                string feeDateTd = string.Format(tdFormat, fee.FeeDate.ToShortDateString());
                string balanceTd = string.Format(tdFormat, NumberFormat.Currency(fee.CreditValuePayment, decimalNumbersRound));
                string interestTd = string.Format(tdFormat, NumberFormat.Currency(fee.InterestValue, decimalNumbersRound));
                string assuranceTd = string.Format(tdFormat, NumberFormat.Currency(amortizationSchedule.AmortizationScheduleAssuranceFees[index].AssuranceFeeValue, decimalNumbersRound));
                string assuranceTaxTd = string.Format(tdFormat, NumberFormat.Currency(amortizationSchedule.AmortizationScheduleAssuranceFees[index].AssuranceTaxValue, decimalNumbersRound));
                string totalTd = string.Format(tdFormat, NumberFormat.Currency(fee.TotalFeeValue, decimalNumbersRound));

                amortizationScheduleHtml += string.Format(trFormat, feeTd, feeDateTd, balanceTd, interestTd, assuranceTd, assuranceTaxTd, totalTd);

                index++;
            });

            return amortizationScheduleHtml;
        }

        /// <summary>
        /// Validate Customer
        /// </summary>
        /// <param name="customer"></param>
        private void ValidateCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new BusinessException(nameof(BusinessResponse.CustomerNotFound), (int)BusinessResponse.CustomerNotFound);
            }
        }

        /// <summary>
        /// Validate Store
        /// </summary>
        /// <param name="store"></param>
        private void ValidateStore(Store store)
        {
            if (store == null)
            {
                throw new BusinessException(BusinessResponse.StoreNotFound.ToString(), (int)BusinessResponse.StoreNotFound);
            }
        }

        #endregion Private Methods
    }
}