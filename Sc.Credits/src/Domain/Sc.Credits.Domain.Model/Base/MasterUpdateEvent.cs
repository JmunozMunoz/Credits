using System;

namespace Sc.Credits.Domain.Model.Base
{
    /// <summary>
    /// Base class for event handler to update master and child entities
    /// </summary>
    /// <typeparam name="TMaster"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class MasterEvent<TMaster, TEntity>
        where TMaster : Master<TEntity>
        where TEntity : Entity<Guid>
    {
        /// <summary>
        /// Gets the master
        /// </summary>
        public TMaster Master { get; }

        /// <summary>
        /// Creates a new instance of <see cref="MasterEvent{TMaster, TEntity}"/>
        /// </summary>
        /// <param name="master"></param>
        protected MasterEvent(TMaster master)
        {
            Master = master;
        }

        /// <summary>
        /// Handle the update event to master and child
        /// </summary>
        public abstract void Handle(TEntity newEntity);
    }
}