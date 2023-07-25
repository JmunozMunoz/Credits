using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The payment type entity
    /// </summary>
    public class PaymentType
        : Entity<int>
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="PaymentType"/>
        /// </summary>
        public PaymentType()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="PaymentType"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public PaymentType(int id, string name, string type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Set id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new PaymentType SetId(int id)
        {
            base.SetId(id);
            return this;
        }
    }
}