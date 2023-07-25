using Sc.Credits.Domain.Model.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Managment.Tests.Entities.Common
{
    public class TemplateValueBuilder
    {
        #region Properties		
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
        #endregion

        #region Inicialization
        private const string _key = "";
        private const string _value = "";
        #endregion
        public TemplateValueBuilder()
        {
            Key = _key;
            Value = _value;
        }

        public TemplateValueBuilder WithKey(string key)
        {
            this.Key = key;
            return this;
        }

        public TemplateValueBuilder WithValue(string value)
        {
            this.Value = value;
            return this;
        }

        public List<TemplateValue> Build()
        {
            return new List<TemplateValue>
            {
                new TemplateValue()
                {
                    Key = Key,
                    Value = Value
                }
            };
        }
    }
}
