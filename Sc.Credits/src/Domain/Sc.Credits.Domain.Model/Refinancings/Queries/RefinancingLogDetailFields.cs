using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sc.Credits.Domain.Model.Refinancings.Queries
{
    public class RefinancingLogDetailFields
        : EntityFields
    {
        protected RefinancingLogDetailFields(string alias) : base(alias)
        {
        }

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field RefinancingLogId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field CreditMasterId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field CreditId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field Value => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field Balance => GetField(MethodBase.GetCurrentMethod().Name);
    }

}
