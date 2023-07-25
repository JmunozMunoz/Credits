using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Enums;

namespace Sc.Credits.Domain.Model.Customers
{
    /// <summary>
    /// The profile entity
    /// </summary>
    public class Profile : Entity<int>, IAggregateRoot
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Mandatory down pyment
        /// </summary>
        public int MandatoryDownPayment { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Profile"/>
        /// </summary>
        public Profile()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="Profile"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mandatoryDownPayment"></param>
        public Profile(string name, int mandatoryDownPayment)
        {
            Name = name;
            MandatoryDownPayment = mandatoryDownPayment;
        }

        /// <summary>
        /// Set Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new Profile SetId(int id)
        {
            base.SetId(id);
            return this;
        }

        /// <summary>
        /// Indicates is always down payment
        /// </summary>
        /// <returns></returns>
        public bool IsAlwaysDownPayment() => MandatoryDownPayment == (int)DownPayments.Always;

        /// <summary>
        /// Indicates is store down payment
        /// </summary>
        /// <returns></returns>
        public bool IsStoreDownPayment() => MandatoryDownPayment == (int)DownPayments.Store;
    }
}