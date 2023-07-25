using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using System;

namespace Sc.Credits.Domain.Model.Customers
{
    /// <summary>
    /// Customer class
    /// </summary>
    public class Customer
        : Entity<Guid>, IAggregateRoot
    {
        /// <summary>
        /// Gets the document's id
        /// </summary>
        public string IdDocument { get; private set; }

        /// <summary>
        /// Gets the document's type
        /// </summary>
        public string DocumentType { get; private set; }

        /// <summary>
        /// Gets the credit limit
        /// </summary>
        [Write(false)]
        public decimal GetCreditLimit => _creditLimit;

        /// <summary>
        /// Gets the validated mail indicator
        /// </summary>
        [Write(false)]
        public bool GetValidatedMail => _validatedMail;

        /// <summary>
        /// Gets the full name
        /// </summary>
        [Write(false)]
        public string GetFullName => _fullName;

        /// <summary>
        /// Gets the profile id
        /// </summary>
        [Write(false)]
        public int? GetProfileId => _profileId;

        /// <summary>
        /// Gets the available credit limit
        /// </summary>
        [Write(false)]
        public decimal GetAvailableCreditLimit => _availableCreditLimit;

        /// <summary>
        /// Gets the send token mail indicator
        /// </summary>
        [Write(false)]
        public bool GetSendTokenMail => _sendTokenMail;

        /// <summary>
        /// Gets the send token sms indicator
        /// </summary>
        [Write(false)]
        public bool GetSendTokenSms => _sendTokenSms;

        /// <summary>
        /// Gets the mobile number
        /// </summary>
        [Write(false)]
        public string GetMobile => _mobile;

        /// <summary>
        /// Gets the email
        /// </summary>
        [Write(false)]
        public string GetEmail => _email;

        /// <summary>
        /// Gets the status
        /// </summary>
        [Write(false)]
        public int GetStatus => _status;

        /// <summary>
        /// Gets the update date
        /// </summary>
        [Write(false)]
        public DateTime GetUpdateDate => _updateDate;

        /// <summary>
        /// Gets the update time
        /// </summary>
        [Write(false)]
        public TimeSpan GetUpdateTime => _updateTime;

        /// <summary>
        /// Gets the create date
        /// </summary>
        [Write(false)]
        public DateTime GetCreateDate => _createDate;

        /// <summary>
        /// Gets the create time
        /// </summary>
        [Write(false)]
        public TimeSpan GetCreateTime => _createTime;

        /// <summary>
        /// Gets the credit limit update date
        /// </summary>
        [Write(false)]
        public DateTime GetCreditLimitUpdateDate => _creditLimitUpdateDate;


        /// <summary>
        /// Gets or sets the type document.
        /// </summary>
        [Write(false)]
        public string CompleteDocumentType =>
            DocumentTypes.Names.TryGetValue(DocumentType, out string name) ? name : "No definido";

        /// <summary>
        /// Gets the profile
        /// </summary>
        [Write(false)]
        public Profile Profile { get; private set; }

        /// <summary>
        /// Gets the name
        /// </summary>
        [Write(false)]
        public CustomerName Name { get; private set; }

        private decimal _creditLimit;
        private decimal _availableCreditLimit;
        private DateTime _createDate;
        private TimeSpan _createTime;
        private DateTime _updateDate;
        private TimeSpan _updateTime;
        private bool _validatedMail;
        private string _fullName;
        private int _profileId;
        private string _mobile;
        private string _email;
        private bool _sendTokenMail;
        private bool _sendTokenSms;
        private int _status;
        private DateTime _creditLimitUpdateDate;

        [Write(false)]
        private bool _creditLimitUpdated;

        [Write(false)]
        private bool _ignoreCreditLimitUpdate;

        [Write(false)]
        private bool _validateCreditLimit;

        /// <summary>
        /// Creates a new instance of <see cref="Customer"/>
        /// </summary>
        protected Customer()
        {
            _creditLimitUpdated = false;
            _ignoreCreditLimitUpdate = false;
            _validateCreditLimit = true;
            Name = CustomerName.Empty;
        }

        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static Customer New() =>
            new Customer();

        /// <summary>
        /// Set basic info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="name"></param>
        /// <param name="mobile"></param>
        /// <param name="email"></param>
        internal void SetBasicInfo(Guid id, string idDocument, string documentType,
            CustomerName name, string mobile, string email)
        {
            Id = id;
            IdDocument = idDocument;
            DocumentType = documentType;
            Name = name;
            _createDate = DateTime.Today;
            _createTime = DateTime.Now.TimeOfDay;
            _updateDate = _createDate;
            _updateTime = _createTime;
            _fullName = name.ToString();
            _mobile = mobile;
            _email = email;
        }

        /// <summary>
        /// Set validation info
        /// </summary>
        /// <param name="validatedMail"></param>
        /// <param name="profileId"></param>
        /// <param name="sendTokenMail"></param>
        /// <param name="sendTokenSms"></param>
        /// <param name="status"></param>
        internal void SetValidationInfo(bool validatedMail, int profileId, bool sendTokenMail, bool sendTokenSms, int status)
        {
            _validatedMail = validatedMail;
            _profileId = profileId;
            _sendTokenMail = sendTokenMail;
            _sendTokenSms = sendTokenSms;
            _status = status;
        }

        /// <summary>
        /// Set credit limit info
        /// </summary>
        /// <param name="creditLimit"></param>
        /// <param name="availableCreditLimit"></param>
        /// <param name="eventDate"></param>
        internal void SetCreditLimitInfo(decimal creditLimit, decimal availableCreditLimit, DateTime eventDate)
        {
            _creditLimit = creditLimit;
            _availableCreditLimit = availableCreditLimit;
            _creditLimitUpdateDate = eventDate;
        }

        /// <summary>
        /// Ignore credit limit update
        /// </summary>
        /// <param name="ignore"></param>
        internal void IgnoreCreditLimitUpdate(bool ignore)
        {
            _ignoreCreditLimitUpdate = ignore;
            _creditLimitUpdated = false;
        }

        /// <summary>
        /// Validate credit limit
        /// </summary>
        /// <param name="validate"></param>
        public void SetCreditLimitValidation(bool validate)
        {
            _validateCreditLimit = validate;
        }

        /// <summary>
        /// Credit limit increase
        /// </summary>
        /// <param name="value"></param>
        public void CreditLimitIncrease(decimal value)
        {
            if (_ignoreCreditLimitUpdate)
                return;

            _availableCreditLimit += value;
            _updateDate = DateTime.Today;
            _updateTime = DateTime.Now.TimeOfDay;
            _creditLimitUpdated = true;
            _creditLimitUpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Reduce credit limit
        /// </summary>
        /// <param name="value"></param>
        public void ReduceCreditLimit(decimal value)
        {
            _availableCreditLimit -= value;
            _creditLimit -= value;
            _updateDate = DateTime.Today;
            _updateTime = DateTime.Now.TimeOfDay;
            _creditLimitUpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Credit limit decrease
        /// </summary>
        /// <param name="value"></param>
        internal void CreditLimitDecrease(decimal value)
        {
            if (_ignoreCreditLimitUpdate)
                return;

            _availableCreditLimit -= value;
            _updateDate = DateTime.Today;
            _updateTime = DateTime.Now.TimeOfDay;
            _creditLimitUpdated = true;
            _creditLimitUpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="createCustomerRequest"></param>
        public void Update(CustomerRequest createCustomerRequest)
        {
            Name = CustomerName.New(createCustomerRequest.FirstName, createCustomerRequest.SecondName,
                createCustomerRequest.FirstLastName, createCustomerRequest.SecondLastName);

            _fullName = Name.ToString();
            _profileId = createCustomerRequest.ProfileId ?? _profileId;
            _mobile = createCustomerRequest.MovileNumber ?? _mobile;
            _email = createCustomerRequest.Email ?? _email;
            _status = createCustomerRequest.Status;
            _validatedMail = _validatedMail || (createCustomerRequest.VerifiedEmail ?? _validatedMail);
            _sendTokenMail = createCustomerRequest.SendTokenMail ?? _sendTokenMail;
            _sendTokenSms = createCustomerRequest.SendTokenSms ?? _sendTokenSms;
            _updateDate = DateTime.Today;
            _updateTime = DateTime.Now.TimeOfDay;
        }

        /// <summary>
        /// Set Profile
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public Customer SetProfile(Profile profile)
        {
            Profile = profile;
            _profileId = profile.Id;
            return this;
        }

        /// <summary>
        /// Indicates is available credit limit
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="partialCreditLimit"></param>
        /// <param name="availableCreditLimit"></param>
        /// <returns></returns>
        public bool IsAvailableCreditLimit(decimal creditValue, decimal partialCreditLimit, out decimal availableCreditLimit)
        {
            availableCreditLimit = !_validatedMail && partialCreditLimit < _availableCreditLimit
                ? partialCreditLimit
                : _availableCreditLimit;

            if (!_validateCreditLimit)
            {
                return true;
            }

            decimal currentCreditValue = _creditLimit - _availableCreditLimit;

            if (!_validatedMail && currentCreditValue != 0)
            {
                availableCreditLimit = 0;
            }

            return creditValue <= availableCreditLimit;
        }

        /// <summary>
        /// Try credit limit update
        /// </summary>
        /// <param name="creditLimit"></param>
        /// <param name="availableCreditLimit"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        public bool TryCreditLimitUpdate(decimal creditLimit, decimal availableCreditLimit, DateTime eventDate)
        {
            if (IsValidCreditLimitUpdate(eventDate))
            {
                _availableCreditLimit = availableCreditLimit;
                _creditLimit = creditLimit;
                _creditLimitUpdated = true;
                _creditLimitUpdateDate = eventDate;
                _updateDate = DateTime.Today;
                _updateTime = DateTime.Now.TimeOfDay;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Is valid credit limit update
        /// </summary>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        private bool IsValidCreditLimitUpdate(DateTime eventDate)
        {
            return eventDate > _creditLimitUpdateDate;
        }

        /// <summary>
        /// Indicates if credit limit is updated
        /// </summary>
        /// <returns></returns>
        public bool CreditLimitIsUpdated()
        {
            return _creditLimitUpdated;
        }

        /// <summary>
        /// Ignore credit limit update
        /// </summary>
        /// <returns></returns>
        public bool IgnoreCreditLimitUpdate()
        {
            return _ignoreCreditLimitUpdate;
        }

        /// <summary>
        /// Set credit limit updated
        /// </summary>
        /// <param name="updated"></param>
        /// <returns></returns>
        public void SetCreditLimitUpdated(bool updated)
        {
            _creditLimitUpdated = updated;
        }

        /// <summary>
        /// Set status
        /// </summary>
        /// <param name="status"></param>
        public void SetStatus(int status)
        {
            _status = status;
            _updateDate = DateTime.Today;
            _updateTime = DateTime.Now.TimeOfDay;
        }

        /// <summary>
        /// Update mail
        /// </summary>
        /// <param name="email"></param>
        /// <param name="validatedMail"></param>
        public void UpdateMail(string email, bool validatedMail)
        {
            _email = email;
            _validatedMail = _validatedMail || validatedMail;
            _updateDate = DateTime.Today;
            _updateTime = DateTime.Now.TimeOfDay;
        }

        /// <summary>
        /// Update mobile
        /// </summary>
        /// <param name="mobile"></param>
        public void UpdateMobile(string mobile)
        {
            _mobile = mobile;
            _updateDate = DateTime.Today;
            _updateTime = DateTime.Now.TimeOfDay;
        }

        /// <summary>
        /// Update all study
        /// </summary>
        /// <returns></returns>
        public bool UpdateAllStudy()
        {
            switch ((CustomerStatuses)_status)
            {
                case CustomerStatuses.IncompleteRequest:
                case CustomerStatuses.Pending:
                case CustomerStatuses.StudyAgent:
                case CustomerStatuses.Denied:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Is active
        /// </summary>
        /// <returns></returns>
        public bool IsActive() =>
            _status == (int)CustomerStatuses.Approved;

        /// <summary>
        /// Is incomplete
        /// </summary>
        /// <returns></returns>
        public bool IsIncomplete() =>
            _status == (int)CustomerStatuses.IncompleteRequest;

        /// <summary>
        /// Allow credit limit increase
        /// </summary>
        /// <returns></returns>
        public bool AllowCreditLimitIncrease() =>
             _validatedMail && IsActive();
    }
}