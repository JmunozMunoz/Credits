using SqlKata.Compilers;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata
{
    /// <summary>
    /// Compilers
    /// </summary>
    internal static class Compilers
    {
        /// <summary>
        /// Sql server
        /// </summary>
        public static readonly Compiler SqlServer = new SqlServerCompiler();
    }
}