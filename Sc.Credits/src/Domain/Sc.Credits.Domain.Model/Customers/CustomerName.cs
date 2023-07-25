using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using System.Linq;

namespace Sc.Credits.Domain.Model.Customers
{
    /// <summary>
    /// The customer name entity
    /// </summary>
    public class CustomerName
    {
        private string _firstName;
        private string _secondName;
        private string _firstLastName;
        private string _secondLastName;

        /// <summary>
        /// Gets first name
        /// </summary>
        [Write(false)]
        public string GetFirstName => _firstName;

        /// <summary>
        /// Gets the second name
        /// </summary>
        [Write(false)]
        public string GetSecondName => _secondName;

        /// <summary>
        /// Gets the first last name
        /// </summary>
        [Write(false)]
        public string GetFirstLastName => _firstLastName;

        /// <summary>
        /// Gets the second last name
        /// </summary>
        [Write(false)]
        public string GetSecondLastName => _secondLastName;

        /// <summary>
        /// Gets the names combined (first name and second name)
        /// </summary>
        [Write(false)]
        public string GetNames => FormatNameParts(_firstName, _secondName);

        /// <summary>
        /// Gets the last names
        /// </summary>
        [Write(false)]
        public string GetLastNames => FormatNameParts(_firstLastName, _secondLastName);

        /// <summary>
        /// Creates an empty <see cref="CustomerName"/>
        /// </summary>
        [Write(false)]
        public static CustomerName Empty => new CustomerName();

        /// <summary>
        /// Creates a new instance of <see cref="CustomerName"/>
        /// </summary>
        protected CustomerName()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="CustomerName"/>
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="secondName"></param>
        /// <param name="firstLastName"></param>
        /// <param name="secondLastName"></param>
        protected CustomerName(string firstName, string secondName, string firstLastName, string secondLastName)
        {
            SetName(firstName, secondName, firstLastName, secondLastName);
        }

        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static CustomerName New(string firstName, string secondName, string firstLastName, string secondLastName) =>
            new CustomerName(firstName, secondName, firstLastName, secondLastName);

        /// <summary>
        /// Set name
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="secondName"></param>
        /// <param name="firstLastName"></param>
        /// <param name="secondLastName"></param>
        internal void SetName(string firstName, string secondName, string firstLastName, string secondLastName)
        {
            _firstName = firstName ?? string.Empty;
            _secondName = secondName ?? string.Empty;
            _firstLastName = firstLastName ?? string.Empty;
            _secondLastName = secondLastName ?? string.Empty;
        }

        /// <summary>
        /// Gets the full name
        /// </summary>
        /// <returns></returns>
        public override string ToString() => FormatNameParts(_firstName, _secondName, _firstLastName, _secondLastName);

        /// <summary>
        /// Format the full name
        /// </summary>
        /// <param name="nameParts"></param>
        /// <returns></returns>
        private string FormatNameParts(params string[] nameParts) =>
            string.Join(" ", nameParts.Where(namePart => !string.IsNullOrEmpty(namePart)));
    }
}