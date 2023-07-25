using Sc.Credits.Domain.Model.Refinancings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.Dapper.Map
{
    public class RefinancingLogMapper
        : Mapper
    {
        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static RefinancingLogMapper New()
            => new RefinancingLogMapper();

        /// <summary>
        /// New request cancel payment mapper
        /// </summary>
        protected RefinancingLogMapper()
        {
        }

        public IEnumerable<RefinancingLogDetail> MastersFromDynamicQuery(IEnumerable<dynamic> dynamicQuery)
        {
            if (dynamicQuery is ICollection<object> rows && rows.Any())
            {
                foreach (object row in rows)
                {
                    yield return FromDynamicRow(row);
                }
            }
        }

        public RefinancingLogDetail FromDynamicRow(object row)
        {
            RefinancingLogDetail refinancingLog = null;

            if (row is IDictionary<string, object> dictionary)
            {
                ICollection<IDictionary<string, object>> dictionaries = SplitDictionary(dictionary);

                refinancingLog = RefinancingLogDetail.New();

                IEnumerator<IDictionary<string, object>> enumerator = dictionaries.GetEnumerator();

                enumerator.MoveNext();

                MapEntity(refinancingLog, enumerator);

              
            }
            return refinancingLog;
        }

    }

}
