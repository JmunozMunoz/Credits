namespace Sc.Credits.Helper.Test.Model
{
    using Domain.Model.Customers;
    using Sc.Credits.Domain.Model.Enums;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Customer Helper Test
    /// </summary>
    public static class CustomerHelperTest
    {
        /// <summary>
        /// Create customer
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="creditLimit"></param>
        /// <param name="availableCreditLimit"></param>
        /// <param name="validatedMail"></param>
        /// <param name="profileId"></param>
        /// <param name="mobile"></param>
        /// <param name="email"></param>
        /// <param name="sendTokenMail"></param>
        /// <param name="sendTokenSms"></param>
        /// <param name="status"></param>
        /// <param name="customerId"></param>
        /// <param name="eventDate"></param>
        /// <param name="firstName"></param>
        /// <param name="secondName"></param>
        /// <param name="firstLastName"></param>
        /// <param name="secondLastName"></param>
        /// <returns></returns>
        public static Customer CreateCustomer(string idDocument, string documentType, decimal creditLimit, decimal availableCreditLimit, bool validatedMail,
            int profileId, string mobile, string email, bool sendTokenMail, bool sendTokenSms, int status, Guid customerId, DateTime eventDate,
            string firstName, string secondName, string firstLastName, string secondLastName)
        {
            return
                CustomerBuilder.CreateBuilder()
                    .BasicInfo(customerId, idDocument, documentType, CustomerName.New(firstName, secondName, firstLastName, secondLastName), mobile, email)
                    .ValidationInfo(validatedMail, profileId, sendTokenMail, sendTokenSms, status)
                    .CreditLimitInfo(creditLimit, availableCreditLimit, eventDate)
                    .Build();
        }

        /// <summary>
        /// Get customer
        /// </summary>
        /// <returns></returns>
        public static Customer GetCustomer(Guid? customerId = null, string idDocument = null)
        {
            customerId = customerId ?? Guid.Parse("55094b4d-5d5e-cd3b-435f-08d72284f7ae");
            return CreateCustomer(idDocument ?? "1033340573", "CE", 500000.0M, 500000.0M, true, 1, "3205875958", "correo@correo.com", sendTokenMail: true,
                sendTokenSms: true, 1, (Guid)customerId, DateTime.Now, "Juan", "", "Perez", "")
                .SetProfile(ProfileHelperTest.GetProfile());
        }

        /// <summary>
        /// Get customer
        /// </summary>
        /// <returns></returns>
        public static Customer GetCustomerWithoutCreditLimit(Guid? customerId = null)
        {
            customerId = customerId ?? Guid.Parse("55094b4d-5d5e-cd3b-435f-08d72284f7ae");
            return CreateCustomer("1033340573", "CE", creditLimit: 0M, 500000.0M, true, 1, "3205875958", "correo@correo.com", sendTokenMail: true,
                sendTokenSms: true, 1, (Guid)customerId, DateTime.Now, "Juan", "", "Perez", "")
                .SetProfile(ProfileHelperTest.GetProfile());
        }

        /// <summary>
        /// Get customer unvalidated mail
        /// </summary>
        /// <returns></returns>
        public static Customer GetCustomerUnvalidatedMail()
        {
            return CreateCustomer(idDocument: "1033340573", documentType: "CE", creditLimit: 500000.0M, availableCreditLimit: 500000.0M,
                validatedMail: false, profileId: 1, mobile: "3205875958", email: "correo@correo.com", sendTokenMail: true,
                sendTokenSms: true, status: 1, customerId: new Guid("55094b4d-5d5e-cd3b-435f-08d72284f7ae"), eventDate: DateTime.Now,
                 "Juan", "", "Perez", "")
                .SetProfile(ProfileHelperTest.GetProfile());
        }

        /// <summary>
        /// Get customer custom
        /// </summary>
        /// <param name="creditLimit"></param>
        /// <param name="availableCreditLimit"></param>
        /// <param name="mandatoryDownPayment"></param>
        /// <returns></returns>
        public static Customer GetCustomerCustom(decimal creditLimit, decimal availableCreditLimit, DownPayments mandatoryDownPayment)
        {
            return CreateCustomer("1033340573", "CE", creditLimit, availableCreditLimit, true, 1, "3205875958", "correo@correo.com", sendTokenMail: true,
                sendTokenSms: true, 1, new Guid("55094b4d-5d5e-cd2b-457f-08d72284f7ae"), DateTime.Now, "Juan", "", "Perez", "")
                .SetProfile(ProfileHelperTest.GetCustomProfile("Ordinary", (int)mandatoryDownPayment));
        }

        /// <summary>
        /// Get customer list
        /// </summary>
        /// <returns></returns>
        public static List<Customer> GetCustomerList()
        {
            return new List<Customer>
            {
                CreateCustomer("1033340573", "CE", 500000.0M, 500000.0M,  true, 1, "3205875958", "correo@correo.com", sendTokenMail: true,
                    sendTokenSms: true, 1,new Guid("48094b4d-5d5e-cd2b-435f-08d72284f7ae"), DateTime.Now,  "Juan", "", "Perez", "")
                    .SetProfile(ProfileHelperTest.GetCustomProfile("Ordinary", (int)DownPayments.Never)),
                CreateCustomer("1037610201", "CE", 500000.0M, 35000.00M, true, 1, "3205875958", "correo@correo.com", sendTokenMail: true,
                    sendTokenSms: true, 1,new Guid("94094b4d-5d5e-cd2b-435f-08d72284f7ae"), DateTime.Now,  "Alonso", "", "Gonzales", "")
                    .SetProfile(ProfileHelperTest.GetCustomProfile("Ordinary", (int)DownPayments.Never)),
                CreateCustomer("1037610201", "CE", 500000.0M, 35000.00M, true, 1, "3205875958", "correo@correo.com", sendTokenMail: true,
                    sendTokenSms: true, 1, default, DateTime.Now,  "Alonso", "", "Gonzales", "")
                    .SetProfile(ProfileHelperTest.GetCustomProfile("Ordinary", (int)DownPayments.Never))
            };
        }

        /// <summary>
        /// Get Customer Credit Limit Client
        /// </summary>
        /// <returns></returns>
        public static Customer GetCustomerCreditLimit()
        {
            return CreateCustomer("1037610201", "CE", 500000.0M, 35000.00M, true, 1, "3205875958", "correo@correo.com", sendTokenMail: true,
                sendTokenSms: true, 1, new Guid("55094b4d-5d5e-cd2b-655f-08d72284f7ae"), DateTime.Now, "Juan", "", "Perez", "")
                .SetProfile(ProfileHelperTest.GetCustomProfile("Ordinary", (int)DownPayments.Always));
        }

        /// <summary>
        /// Get Customer Credit Limit Client True
        /// </summary>
        /// <returns></returns>
        public static Customer GetCustomerCreditLimitTrue()
        {
            return CreateCustomer("1037610201", "CE", creditLimit: 3500000.00M, availableCreditLimit: 500000.0M, true, 1, "3205875958", "correo@correo.com", sendTokenMail: true,
                sendTokenSms: true, 1, new Guid("55094b4d-5d5e-cd3b-435f-08d72284f7ae"), DateTime.Now, "Juan", "", "Perez", "")
                .SetProfile(ProfileHelperTest.GetCustomProfile("Ordinary", (int)DownPayments.Always));
        }

        /// <summary>
        /// Get customer
        /// </summary>
        /// <returns></returns>
        public static Customer GetCustomerByStatus(CustomerStatuses status)
        {
            return CreateCustomer("1033340573", "CE", 500000.0M, 500000.0M, true, 1, "3205875958", "correo@correo.com", sendTokenMail: true,
                sendTokenSms: true, (int)status, new Guid("55094b4d-5d5e-cd2b-874f-08d72284f7ae"), DateTime.Now, "Juan", "", "Perez", "")
                .SetProfile(ProfileHelperTest.GetProfile());
        }

        public static Customer GetCustomerWithtoutCustomerId()
        {
            return CreateCustomer("1033340573", "CE", 500000.0M, 500000.0M, true, 1, "3205875958", "juanPerez@sistecredito.com", sendTokenMail: true,
                sendTokenSms: true, 1, Guid.Empty, DateTime.Now, "Juan", "", "Perez", "");
        }

        /// <summary>
        /// Get Customer Credit Limit Invalid
        /// </summary>
        /// <returns></returns>
        public static Customer GetCustomerCreditLimitInvalid()
        {
            return CreateCustomer("1037610201", "CE", creditLimit: 3000000.00M, availableCreditLimit: 10000.00M, true, 1, "3205875958", "correo@correo.com", sendTokenMail: true,
                sendTokenSms: true, 1, new Guid("55094b4d-5d5e-cd3b-435f-08d72284f7ae"), DateTime.Now, "Juan", "", "Perez", "")
                .SetProfile(ProfileHelperTest.GetCustomProfile("Ordinary", (int)DownPayments.Always));
        }

        /// <summary>
        /// Get Customer Credit Limit valid
        /// </summary>
        /// <returns></returns>
        public static Customer GetCustomerCreditLimitValid()
        {
            return CreateCustomer("1037610201", "CE", creditLimit: 1000000.0M, availableCreditLimit: 500000.0M, true, 1, "3205875958", "correo@correo.com", sendTokenMail: true,
                sendTokenSms: true, 1, new Guid("55094b4d-5d5e-cd3b-435f-08d72284f7ae"), DateTime.Now, "Juan", "", "Perez", "")
                .SetProfile(ProfileHelperTest.GetCustomProfile("Ordinary", (int)DownPayments.Always));
        }

        /// <summary>
        /// Get customer with notifications
        /// </summary>
        /// <param name="sendTokenSms"></param>
        /// <param name="sendTokenMail"></param>
        /// <returns></returns>
        public static Customer GetCustomerWithNotifications(bool sendTokenSms, bool sendTokenMail)
        {
            Guid customerId = Guid.Parse("55094b4d-5d5e-cd3b-435f-08d72284f7ae");

            return CreateCustomer("1033340573", "CE", 500000.0M, 500000.0M, true, 1, "3205875958", "correo@correo.com", sendTokenMail,
               sendTokenSms, 1, customerId, DateTime.Now, "Juan", "", "Perez", "");
        }
    }
}