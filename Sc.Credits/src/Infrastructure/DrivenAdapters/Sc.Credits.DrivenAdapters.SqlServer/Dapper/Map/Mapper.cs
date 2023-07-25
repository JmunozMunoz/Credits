using Sc.Credits.Helpers.ObjectsUtils;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sc.Credits.DrivenAdapters.SqlServer.Dapper.Map
{
    /// <summary>
    /// Mapper
    /// </summary>
    public class Mapper
    {
        /// <summary>
        /// Map entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="enumerator"></param>
        protected void MapEntity<T>(T entity, IEnumerator<IDictionary<string, object>> enumerator)
        {
            IDictionary<string, object> dictionary = enumerator.Current;

            ReflectionUtils.MapDictionaryToEntity(dictionary, entity);
        }

        /// <summary>
        /// Split dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        protected ICollection<IDictionary<string, object>> SplitDictionary(IDictionary<string, object> dictionary)
        {
            Collection<IDictionary<string, object>> collection = new Collection<IDictionary<string, object>>();

            Dictionary<string, object> itemDictionary = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> valuePair in dictionary)
            {
                bool newItem = valuePair.Key == "Id";

                if (newItem)
                {
                    itemDictionary = new Dictionary<string, object>();
                    collection.Add(itemDictionary);
                }

                itemDictionary.Add(valuePair.Key, valuePair.Value);
            }

            return collection;
        }
    }
}