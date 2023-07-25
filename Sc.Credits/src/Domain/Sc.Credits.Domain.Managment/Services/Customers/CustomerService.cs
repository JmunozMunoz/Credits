using credinet.comun.models.Credits;
using credinet.comun.models.Study;
using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Domain.Model.Customers.Queries.Commands;
using Sc.Credits.Domain.Model.Customers.Queries.Reading;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.UseCase.Customers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.Managment.Services.Customers
{
    /// <summary>
    /// Customer service is an implementation of <see cref="ICustomerService"/>
    /// </summary>
    public class CustomerService
        : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerUsesCase _customerUsesCase;
        private readonly ICustomerEventsRepository _customerEventsRepository;

        /// <summary>
        /// Creates a new instance of <see cref="CustomerService"/>
        /// </summary>
        /// <param name="customerRepository"></param>
        /// <param name="customerUsesCase"></param>
        public CustomerService(ICustomerRepository customerRepository,
            ICustomerUsesCase customerUsesCase,
            ICustomerEventsRepository customerEventsRepository)
        {
            _customerRepository = customerRepository;
            _customerUsesCase = customerUsesCase;
            _customerEventsRepository = customerEventsRepository;
        }

        /// <summary>
        /// <see cref="ICustomerService.CreateOrUpdateAsync(CustomerRequest, DateTime)"/>
        /// </summary>
        /// <param name="createCustomerRequest"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateAsync(CustomerRequest createCustomerRequest, DateTime eventDate)
        {
            Customer customer = await _customerRepository.GetByDocumentInfoAsync(createCustomerRequest.IdDocument,
                createCustomerRequest.TypeDocument);

            if (customer == null)
            {
                customer = _customerUsesCase.Create(createCustomerRequest, eventDate);

                await _customerRepository.AddAsync(customer);
            }
            else
            {
                _customerUsesCase.Update(createCustomerRequest, customer);

                await _customerRepository.UpdateAsync(customer, CustomerCommandsFields.Update, entitiesForMerge: customer.Name);
            }
        }

        /// <summary>
        /// <see cref="ICustomerService.UpdateAsync(Customer, IEnumerable{Field}, Transaction)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Customer customer, IEnumerable<Field> fields, Transaction transaction = null) =>
            await _customerRepository.UpdateAsync(customer, fields, transaction);

        /// <summary>
        /// <see cref="ICustomerService.GetAsync(Guid, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="profileFields"></param>
        /// <returns></returns>
        public async Task<Customer> GetAsync(Guid id, IEnumerable<Field> fields,
            IEnumerable<Field> profileFields = null)
        {
            Customer customer =
                await _customerRepository.GetByIdAsync(id, fields, profileFields)
                ??
                throw new BusinessException(nameof(BusinessResponse.CustomerNotFound), (int)BusinessResponse.CustomerNotFound);

            return customer;
        }

        /// <summary>
        /// <see cref="ICustomerService.GetAsync(string, string, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="fields"></param>
        /// <param name="profileFields"></param>
        /// <returns></returns>
        public async Task<Customer> GetAsync(string idDocument, string documentType, IEnumerable<Field> fields,
            IEnumerable<Field> profileFields = null)
        {
            ValidateFieldsEmpty(documentType, idDocument);

            Customer customer =
                await _customerRepository.GetByDocumentInfoAsync(idDocument, documentType,
                    fields, profileFields)
                ??
                throw new BusinessException(nameof(BusinessResponse.CustomerNotFound), (int)BusinessResponse.CustomerNotFound);

            return customer;
        }

        /// <summary>
        /// <see cref="ICustomerService.GetActiveAsync(string, string, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="fields"></param>
        /// <param name="profileFields"></param>
        /// <returns></returns>
        public async Task<Customer> GetActiveAsync(string idDocument, string documentType, IEnumerable<Field> fields,
            IEnumerable<Field> profileFields = null)
        {
            ValidateFieldsEmpty(documentType, idDocument);

            Customer customer = await GetAsync(idDocument, documentType, fields, profileFields);

            ValidateCustomerStatus(customer);

            return customer;
        }

        /// <summary>
        /// <see cref="ICustomerService.TrySendCreditLimitUpdateAsync(Customer)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task TrySendCreditLimitUpdateAsync(Customer customer)
        {
            if (customer.CreditLimitIsUpdated())
            {
                await _customerEventsRepository.NotifyCreditLimitUpdateAsync(customer);
            }
        }

        /// <summary>
        /// <see cref="ICustomerService.TryCreditLimitUpdateAsync(CreditLimitResponse, DateTime)"/>
        /// </summary>
        /// <param name="creditLimitResponse"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        public async Task TryCreditLimitUpdateAsync(CreditLimitResponse creditLimitResponse, DateTime eventDate)
        {
            Customer customer = await _customerRepository.GetByDocumentInfoAsync(creditLimitResponse.IdDocument,
                creditLimitResponse.TypeDocument, CustomerReadingFields.CreditLimit);

            if (customer != null
                &&
                customer.TryCreditLimitUpdate(creditLimitResponse.CreditLimit, creditLimitResponse.AvailableCreditLimit,
                    eventDate))
            {
                await _customerRepository.UpdateAsync(customer, CustomerCommandsFields.UpdateCreditLimit);
            }
        }

        /// <summary>
        /// <see cref="ICustomerService.ResendCreditLimitUpdateAsync(CreditLimitResponse, DateTime)"/>
        /// </summary>
        /// <param name="creditLimitResponse"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        public async Task ResendCreditLimitUpdateAsync(Customer customer)
        {
            customer.SetCreditLimitUpdated(true);
            await TrySendCreditLimitUpdateAsync(customer);
        }

        /// <summary>
        /// <see cref="ICustomerService.UpdateStatusAsync(StudyResponse)"/>
        /// </summary>
        /// <param name="studyResponse"></param>
        /// <returns></returns>
        public async Task UpdateStatusAsync(StudyResponse studyResponse)
        {
            Customer customer = await _customerRepository.GetByDocumentInfoAsync(studyResponse.IdDocument, studyResponse.TypeDocument,
                CustomerReadingFields.BasicInfo);

            if (customer != null)
            {
                customer.SetStatus(studyResponse.Status);

                await _customerRepository.UpdateAsync(customer, CustomerCommandsFields.UpdateStatus);
            }
        }

        /// <summary>
        /// <see cref="ICustomerService.UpdateMailAsync(string, string, string, bool)"/>
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="email"></param>
        /// <param name="validatedMail"></param>
        /// <returns></returns>
        public async Task UpdateMailAsync(string idDocument, string documentType, string email, bool validatedMail)
        {
            Customer customer = await _customerRepository.GetByDocumentInfoAsync(idDocument, documentType,
                CustomerReadingFields.UpdateMail);

            if (customer != null)
            {
                customer.UpdateMail(email, validatedMail);

                await _customerRepository.UpdateAsync(customer, CustomerCommandsFields.UpdateMail);
            }
        }

        /// <summary>
        /// <see cref="ICustomerService.UpdateMobileAsync(string, string, string)"/>
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public async Task UpdateMobileAsync(string idDocument, string documentType, string mobile)
        {
            Customer customer = await _customerRepository.GetByDocumentInfoAsync(idDocument, documentType,
                CustomerReadingFields.UpdateMobile);

            if (customer != null)
            {
                customer.UpdateMobile(mobile);

                await _customerRepository.UpdateAsync(customer, CustomerCommandsFields.UpdateMobile);
            }
        }

        /// <summary>
        /// <see cref="ICustomerService.CreateCustomerOrUpdateStatusAsync(CustomerRequest, DateTime)"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateStatusAsync(CustomerRequest request, DateTime eventDate)
        {
            Customer customer = await _customerRepository.GetByDocumentInfoAsync(request.IdDocument, request.TypeDocument);

            if (customer == null)
            {
                customer = _customerUsesCase.Create(request, eventDate);

                await _customerRepository.AddAsync(customer);
            }
            else
            {
                IEnumerable<Field> updateFields = CustomerCommandsFields.UpdateStatus;

                if (customer.UpdateAllStudy())
                {
                    _customerUsesCase.Update(request, customer);
                    updateFields = CustomerCommandsFields.Update;
                }
                else
                {
                    customer.SetStatus(request.Status);
                }

                await _customerRepository.UpdateAsync(customer, updateFields, entitiesForMerge: customer.Name);
            }
        }

        /// <summary>
        /// <see cref="ICustomerService.RejectCreditLimit(string, string, string, string)"/>
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> RejectCreditLimit(string idDocument, string documentType, string userName, string userId)
        {
            ValidateFieldsEmpty(documentType, idDocument, userName, userId);

            Customer customer =
                await _customerRepository.GetByDocumentInfoAsync(idDocument, documentType) ??
                throw new BusinessException(BusinessResponse.CustomerNotFound.ToString(), (int)BusinessResponse.CustomerNotFound);

            _customerUsesCase.RejectCreditLimit(customer);

            await _customerRepository.UpdateAsync(customer, CustomerCommandsFields.UpdateCreditLimit);

            await _customerEventsRepository.NotifyCreditLimitUpdateAsync(customer);

            return true;
        }


        /// <summary>
        /// Gets the customer by document.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <returns></returns>
        public async Task<Customer> GetCustomerByDocument(string idDocument, IEnumerable<Field> fields, IEnumerable<Field> profileFields = null)
        {
            Customer customer = await _customerRepository.GetCustomerByDocument(idDocument,
                fields, profileFields);

            return customer;
        }
        #region Private Methods

        /// <summary>
        /// Validate fields empty
        /// </summary>
        /// <param name="firstField"></param>
        /// <param name="secondField"></param>
        private void ValidateFieldsEmpty(params string[] fields)
        {
            foreach (string field in fields)
            {
                if (string.IsNullOrEmpty(field?.Trim()))
                {
                    throw new BusinessException(BusinessResponse.RequestValuesInvalid.ToString(), (int)BusinessResponse.RequestValuesInvalid);
                }
            }
        }

        /// <summary>
        /// Validate customer status
        /// </summary>
        /// <param name="customer"></param>
        private void ValidateCustomerStatus(Customer customer)
        {
            if (customer.IsIncomplete())
            {
                throw new BusinessException(nameof(BusinessResponse.CustomerWithIncompleteInfo), (int)BusinessResponse.CustomerWithIncompleteInfo);
            }

            if (!customer.IsActive())
            {
                throw new BusinessException(nameof(BusinessResponse.CustomerNotActive), (int)BusinessResponse.CustomerNotActive);
            }
        }

        #endregion Private Methods
    }
}