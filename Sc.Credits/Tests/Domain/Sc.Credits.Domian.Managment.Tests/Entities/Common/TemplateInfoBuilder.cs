using Sc.Credits.Domain.Model.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Managment.Tests.Entities.Common
{
    public class TemplateInfoBuilder
    {
        #region Properties
        public string TemplateId { get; set; }
        public List<TemplateValue> TemplateValues { get; set; }
        #endregion

        #region Inicialization
        private const string _templateId = "";
        private readonly List<TemplateValue> _templateValues = new TemplateValueBuilder().Build();
        #endregion

        public TemplateInfoBuilder()
        {
            TemplateId = _templateId;
            TemplateValues = _templateValues;
        }

        public TemplateInfoBuilder WithTemplateId(string templateId)
        {
            this.TemplateId = templateId;
            return this;
        }

        public TemplateInfoBuilder WithTemplateValues(List<TemplateValue> templateValues)
        {
            this.TemplateValues = templateValues;
            return this;
        }

        public TemplateInfo Build()
        {
            return new TemplateInfo
            {
                TemplateId = TemplateId,
                TemplateValues = TemplateValues
            };
        }
    }
}
