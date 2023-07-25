using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The transaction type entity
    /// </summary>
    public class TransactionType
        : Entity<int>
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="TransactionType"/>
        /// </summary>
        public TransactionType()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="TransactionType"/>
        /// </summary>
        /// <param name="name"></param>
        public TransactionType(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Set Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new TransactionType SetId(int id)
        {
            base.SetId(id);
            return this;
        }
    }
}