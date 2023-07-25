using Dapper;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sc.Credits.DrivenAdapters.SqlServer.Dapper.Extensions
{
    /// <summary>
    /// Sql extensions for Dapper
    /// </summary>
    public static partial class SqlMapperExtensions
    {
        /// <summary>
        /// Build insert composition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityToInsert"></param>
        /// <param name="sqlAdapter"></param>
        /// <param name="parameters"></param>
        /// <param name="columnList"></param>
        /// <param name="parameterList"></param>
        /// <param name="entitiesForMerge"></param>
        private static void BuildInsertComposition<T>(T entityToInsert, ISqlAdapter sqlAdapter, DynamicParameters parameters, out string columnList, out string parameterList,
            params object[] entitiesForMerge)
        {
            BuildInsertCompositionFromObject(entityToInsert, sqlAdapter, parameters, out string entityColumnList, out string entityParameterList);

            entitiesForMerge = entitiesForMerge ?? Array.Empty<object>();

            StringBuilder sbColumnList = new StringBuilder(entityColumnList);
            StringBuilder sbParameterList = new StringBuilder(entityParameterList);

            foreach (var entityForMerge in entitiesForMerge)
            {
                BuildInsertCompositionFromObject(entityForMerge, sqlAdapter, parameters, out string entityMergeColumnList, out string entityMergeParameterList);

                entityMergeColumnList = string.Join(SqlMapperConstants.COMMA,
                    entityMergeColumnList.Split(SqlMapperConstants.COMMA).Where(c => !sbColumnList.ToString().Contains(c)));

                entityMergeParameterList = string.Join(SqlMapperConstants.COMMA,
                    entityMergeParameterList.Split(SqlMapperConstants.COMMA).Where(c => !sbParameterList.ToString().Contains(c)));

                sbColumnList.Append($"{SqlMapperConstants.COMMA}{entityMergeColumnList}");
                sbParameterList.Append($"{SqlMapperConstants.COMMA}{entityMergeParameterList}");
            }

            columnList = sbColumnList.ToString();
            parameterList = sbParameterList.ToString();
        }

        /// <summary>
        /// Build insert composition from object
        /// </summary>
        /// <param name="object"></param>
        /// <param name="sqlAdapter"></param>
        /// <param name="parameters"></param>
        /// <param name="columnList"></param>
        /// <param name="parameterList"></param>
        private static void BuildInsertCompositionFromObject(object @object, ISqlAdapter sqlAdapter, DynamicParameters parameters, out string columnList,
            out string parameterList)
        {
            Type type = @object.GetType();

            List<PropertyInfo> allProperties = TypePropertiesCache(type);
            List<PropertyInfo> computedProperties = ComputedPropertiesCache(type);
            List<PropertyInfo> allPropertiesExceptComputed = allProperties.Except(computedProperties).ToList();
            List<FieldInfo> fields = TypeFieldsCache(type);

            var columnsAndValues =
                allPropertiesExceptComputed
                    .Select(p => new
                    {
                        p.Name,
                        Value = p.GetValue(@object)
                    })
                    .Union(fields
                        .Select(f => new
                        {
                            f.Name,
                            Value = f.GetValue(@object)
                        }));

            columnList =
                string.Join(SqlMapperConstants.COMMA,
                    columnsAndValues.Select(column => sqlAdapter.FormatColumnName(column.Name)));

            parameterList =
                string.Join(SqlMapperConstants.COMMA,
                    columnsAndValues.Select(column => $"@{sqlAdapter.SetColumnName(column.Name)}"));

            foreach (var columnAndValue in columnsAndValues)
            {
                string parameterName = $"@{sqlAdapter.SetColumnName(columnAndValue.Name)}";

                if (!parameters.ParameterNames.Any(p => p == parameterName))
                {
                    parameters.Add(parameterName, columnAndValue.Value);
                }
            }
        }

        /// <summary>
        /// The function to get a database type from the given <see cref="IDbConnection"/>.
        /// </summary>
        /// <param name="connection">The connection to get a database type name from.</param>
        public delegate string GetDatabaseTypeDelegate(IDbConnection connection);

        /// <summary>
        /// The function to get a a table name from a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get a table name for.</param>
        public delegate string TableNameMapperDelegate(Type type);

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<FieldInfo>> TypeFields = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<FieldInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ComputedProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ISqlAdapter DefaultAdapter = new SqlServerAdapter();

        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary
            = new Dictionary<string, ISqlAdapter>
            {
                ["sqlconnection"] = new SqlServerAdapter()
            };

        /// <summary>
        /// Computed properties cache
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<PropertyInfo> ComputedPropertiesCache(Type type)
        {
            if (ComputedProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pi))
            {
                return pi.ToList();
            }

            var computedProperties = TypePropertiesCache(type).Where(p => p.GetCustomAttributes(true).Any(a => a is ComputedAttribute)).ToList();

            ComputedProperties[type.TypeHandle] = computedProperties;
            return computedProperties;
        }

        /// <summary>
        /// Type properties cache
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<PropertyInfo> TypePropertiesCache(Type type)
        {
            if (TypeProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pis))
            {
                return pis.ToList();
            }

            var properties = type.GetProperties().Where(IsWriteable).ToArray();
            TypeProperties[type.TypeHandle] = properties;
            return properties.ToList();
        }

        /// <summary>
        /// Is writeable
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static bool IsWriteable(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false).AsList();
            if (attributes.Count != 1) return true;

            var writeAttribute = (WriteAttribute)attributes[0];
            return writeAttribute.Write;
        }

        /// <summary>
        /// Is writeable
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        private static bool IsWriteable(FieldInfo fi)
        {
            var attributes = fi.GetCustomAttributes(typeof(WriteAttribute), false).AsList();
            if (attributes.Count != 1) return true;

            var writeAttribute = (WriteAttribute)attributes[0];
            return writeAttribute.Write;
        }

        /// <summary>
        /// Type fields cache
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<FieldInfo> TypeFieldsCache(Type type)
        {
            if (TypeFields.TryGetValue(type.TypeHandle, out IEnumerable<FieldInfo> fields))
            {
                return fields.ToList();
            }

            var privateFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField)
                .Where(field => !field.Name.Contains("BackingField") && IsWriteable(field));
            TypeFields[type.TypeHandle] = privateFields;
            return privateFields.ToList();
        }

        /// <summary>
        /// Specify a custom table name mapper based on the POCO type name
        /// </summary>
        public static readonly TableNameMapperDelegate TableNameMapper;

        private static string GetTableName(Type type)
        {
            if (TypeTableName.TryGetValue(type.TypeHandle, out string name)) return name;

            if (TableNameMapper != null)
            {
                name = TableNameMapper(type);
            }
            else
            {
                //NOTE: This as dynamic trick falls back to handle both our own Table-attribute as well as the one in EntityFramework
                var tableAttrName =
                    type.GetCustomAttribute<TableAttribute>(false)?.Name
                    ?? (type.GetCustomAttributes(false).FirstOrDefault(attr => attr.GetType().Name == "TableAttribute") as dynamic)?.Name;

                if (tableAttrName != null)
                {
                    name = tableAttrName;
                }
                else
                {
                    name = type.Name + "s";
                    if (type.IsInterface && name.StartsWith("I"))
                        name = name.Substring(1);
                }
            }

            TypeTableName[type.TypeHandle] = name;
            return name;
        }

        /// <summary>
        /// Specifies a custom callback that detects the database type instead of relying on the
        /// default strategy (the name of the connection type object). Please note that this
        /// callback is global and will be used by all the calls that require a database specific adapter.
        /// </summary>
        public static readonly GetDatabaseTypeDelegate GetDatabaseType;

        private static ISqlAdapter GetFormatter(IDbConnection connection)
        {
            var name = GetDatabaseType?.Invoke(connection).ToLower()
                       ?? connection.GetType().Name.ToLower();

            return AdapterDictionary.TryGetValue(name, out var adapter)
                ? adapter
                : DefaultAdapter;
        }
    }

    /// <summary>
    /// Sql mapper constants
    /// </summary>
    public static class SqlMapperConstants
    {
        public const string COMMA = ",";

        public const string UNDERLINE = "_";
    }

    /// <summary>
    /// Sql adapter
    /// </summary>
    public partial interface ISqlAdapter
    {
        /// <summary>
        /// Set column name
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        string SetColumnName(string columnName);

        /// <summary>
        /// Format column name
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        string FormatColumnName(string columnName);

        /// <summary>
        /// Format column name equals value
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        string FormatColumnNameEqualsValue(string columnName);

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="parameterList"></param>
        /// <param name="entityToInsert"></param>
        /// <returns></returns>
        int Insert(IDbConnection connection, IDbTransaction transaction,
            int? commandTimeout, string tableName, string columnList, string parameterList,
            object entityToInsert);
    }

    /// <summary>
    /// <see cref="ISqlAdapter"/>
    /// </summary>
    public partial class SqlServerAdapter : ISqlAdapter
    {
        /// <summary>
        /// <see cref="ISqlAdapter.SetColumnName(string)"/>
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string SetColumnName(string columnName) =>
            columnName.Replace(SqlMapperConstants.UNDERLINE, "").Trim().ToLower();

        /// <summary>
        /// <see cref="ISqlAdapter.FormatColumnName(string)"/>
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string FormatColumnName(string columnName) =>
             $"[{SetColumnName(columnName)}]";

        /// <summary>
        /// <see cref="ISqlAdapter.FormatColumnNameEqualsValue(string)"/>
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string FormatColumnNameEqualsValue(string columnName) =>
             $"{FormatColumnName(columnName)} = @{SetColumnName(columnName)}";

        /// <summary>
        /// <see cref="ISqlAdapter.Insert(IDbConnection, IDbTransaction, int?, string, string,
        /// string, object)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="parameterList"></param>
        /// <param name="entityToInsert"></param>
        /// <returns></returns>
        public int Insert(IDbConnection connection, IDbTransaction transaction,
            int? commandTimeout, string tableName, string columnList, string parameterList,
            object entityToInsert)
        {
            var cmd = $"INSERT INTO {tableName} ({columnList}) VALUES ({parameterList});";
            var result = connection.Execute(cmd, entityToInsert, transaction, commandTimeout);

            return result;
        }
    }
}