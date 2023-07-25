using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;

namespace Sc.Credits.Domain.Model.Parameters
{
    /// <summary>
    /// Parameter class
    /// </summary>
    public class Parameter
        : Entity<int>, IAggregateRoot
    {
        /// <summary>
        /// Gets the key
        /// </summary>
        [Write(false)]
        public string GetKey => _key;

        private string _key;

        /// <summary>
        /// Gets the value
        /// </summary>
        [Write(false)]
        public string GetValue => _value;

        private string _value;

        /// <summary>
        /// Creates a new instance of <see cref="Parameter"/>
        /// </summary>
        public Parameter()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="Parameter"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Parameter(string key, string value)
        {
            _key = key;
            _value = value;
        }

        /// <summary>
        /// Set the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new Parameter SetId(int id)
        {
            base.SetId(id);
            return this;
        }
    }
}