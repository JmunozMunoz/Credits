using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.Model.Base
{
    /// <summary>
    /// Base contract for transaction repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ITransactionRepository<TEntity>
        : ICommandRepository<TEntity>
        where TEntity : IAggregateRoot
    {
        /// <summary>
        /// Execute on transaction
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        Task ExcecuteOnTransactionAsync(Func<Transaction, Task> func);

        /// <summary>
        /// Execute on transaction
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<TResult> ExcecuteOnTransactionAsync<TResult>(Func<Transaction, Task<TResult>> func);
    }
}