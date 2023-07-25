using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Helpers.Commons.Domain;
using Sc.Credits.Helpers.ObjectsUtils;
using System;

namespace Sc.Credits.Domain.Model.Refinancings
{
    /// <summary>
    /// Refinancing application
    /// </summary>
    public class RefinancingApplication : Entity<Guid>, IAggregateRoot
    {
        private string _name;
        private DateTime _creationDateFrom;
        private DateTime _creationDateTo;
        private int _arrearsDaysFrom;
        private int _arrearsDaysTo;
        private decimal _valueFrom;
        private decimal _valueTo;
        private bool _allowRefinancingCredits;

        /// <summary>
        /// Init
        /// </summary>
        /// <returns></returns>
        public RefinancingApplication Init(string name, bool allowRefinancingCredits)
        {
            Id = IdentityGenerator.NewSequentialGuid();
            _name = name;
            _allowRefinancingCredits = allowRefinancingCredits;

            return this;
        }

        /// <summary>
        /// Set creation range
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public RefinancingApplication SetCreationRange(DateTime from, DateTime to)
        {
            _creationDateFrom = from;
            _creationDateTo = to;
            return this;
        }

        /// <summary>
        /// Set arrears range
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public RefinancingApplication SetArrearsRange(int from, int to)
        {
            _arrearsDaysFrom = from;
            _arrearsDaysTo = to;
            return this;
        }

        /// <summary>
        /// Set value range
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public RefinancingApplication SetValueRange(decimal from, decimal to)
        {
            _valueFrom = from;
            _valueTo = to;
            return this;
        }

        /// <summary>
        /// Credit is valid
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="totalPayment"></param>
        /// <param name="credinetAppSettings"></param>
        /// <returns></returns>
        public bool CreditIsValid(CreditMaster creditMaster, decimal totalPayment, CredinetAppSettings credinetAppSettings)
            =>
            creditMaster.GetCreditDate >= _creationDateFrom && creditMaster.GetCreditDate <= _creationDateTo
            &&
            creditMaster.Current.GetArrearsDays >= _arrearsDaysFrom && creditMaster.Current.GetArrearsDays <= _arrearsDaysTo
            &&
            totalPayment >= _valueFrom && totalPayment <= _valueTo
            &&
            creditMaster.IsRefinancingAllowed(_allowRefinancingCredits, credinetAppSettings);

        /// <summary>
        /// Validate credit
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="totalPayment"></param>
        /// <param name="credinetAppSettings"></param>
        public void ValidateCredit(CreditMaster creditMaster, decimal totalPayment, CredinetAppSettings credinetAppSettings)
        {
            if (!CreditIsValid(creditMaster, totalPayment, credinetAppSettings))
            {
                throw new BusinessException(nameof(BusinessResponse.CreditIsNotRefinancingAllowed), (int)BusinessResponse.CreditIsNotRefinancingAllowed);
            }
        }
    }
}