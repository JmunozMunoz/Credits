using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sc.Credits.Domain.Model.Refinancings.Queries
{
    public class RefinancingLogFields
         : EntityFields
    {
        protected RefinancingLogFields(string alias) 
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field Date => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field ApplicationId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field CreditMasterId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field Value => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field StoreId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field CustomerId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field ReferenceText => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field UserId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the 
        /// </summary>
        public Field UserName => GetField(MethodBase.GetCurrentMethod().Name);
    }


}
