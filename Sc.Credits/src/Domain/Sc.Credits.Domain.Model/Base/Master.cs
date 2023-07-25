using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.Commons.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Base
{
    /// <summary>
    /// Base class for master PTA (Point in Time Architecture) entity
    /// </summary>
    /// <typeparam name="TEntity">Child entity</typeparam>
    public class Master<TEntity> : Entity<Guid>, IAggregateRoot
        where TEntity : Entity<Guid>
    {
        private static readonly object _syncLock = new object();

        /// <summary>
        /// Gets the create date
        /// </summary>
        public DateTime CreateDate { get; protected set; }

        /// <summary>
        /// Gets the create time
        /// </summary>
        public TimeSpan CreateTime { get; protected set; }

        /// <summary>
        /// Gets the last date
        /// </summary>
        public DateTime LastDate { get; protected set; }

        /// <summary>
        /// Gets the last time
        /// </summary>
        public TimeSpan LastTime { get; protected set; }

        /// <summary>
        /// Gets the last id
        /// </summary>
        public Guid LastId { get; protected set; }

        /// <summary>
        /// Gets the current
        /// </summary>
        [Write(false)]
        public TEntity Current => History.SingleOrDefault(h => h.Id == LastId);

        /// <summary>
        /// Gets the history
        /// </summary>
        [Write(false)]
        public List<TEntity> History { get; protected set; }

        /// <summary>
        /// Creates a new instance of <see cref="Master{TEntity}"/>
        /// </summary>
        protected Master()
        {
            History = new List<TEntity>();
        }

        /// <summary>
        /// Create new child entity in master
        /// </summary>
        /// <param name="entity"></param>
        protected void CreateChild(TEntity entity)
        {
            CreateDate = DateTime.Today;
            CreateTime = DateTime.Now.TimeOfDay;

            SetLastId(entity.Id);

            History.Add(entity);

            SetUpdated();
        }

        /// <summary>
        /// Get new id
        /// </summary>
        /// <returns></returns>
        protected Guid NewId()
        {
            lock (_syncLock)
            {
                return IdentityGenerator.NewSequentialGuid();
            }
        }

        /// <summary>
        /// Set the id to entity
        /// </summary>
        /// <param name="id"></param>
        internal void SetLastId(Guid id)
        {
            LastId = id;
        }

        /// <summary>
        /// Set updated
        /// </summary>
        public void SetUpdated()
        {
            LastDate = DateTime.Today;
            LastTime = DateTime.Now.TimeOfDay;
        }

        /// <summary>
        /// Clone record
        /// </summary>
        /// <param name="record"></param>
        public void CloneRecordAndSetLast(TEntity record)
        {
            var newEntity = (TEntity)record.Clone();

            newEntity.SetId(IdentityGenerator.NewSequentialGuid());

            SetLastId(newEntity.Id);

            History.Add(newEntity);

            SetUpdated();
        }
    }

    /// <summary>
    /// Master extensions
    /// </summary>
    public static class MasterExtensions
    {
        /// <summary>
        /// Set current
        /// </summary>
        /// <typeparam name="TMaster"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="master"></param>
        /// <param name="entity"></param>
        public static TMaster SetCurrent<TMaster, TEntity>(this TMaster master, TEntity entity)
        where TMaster : Master<TEntity>
        where TEntity : Entity<Guid>
        {
            if (!master.History.Any(h => h.Id == entity.Id))
            {
                master.History.Add(entity);
            }

            master.SetLastId(entity.Id);

            return master;
        }

        /// <summary>
        /// Update master and child entity via master event an create a new PTA (Point in Time Architecture) record
        /// </summary>
        /// <typeparam name="TMaster"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="master"></param>
        /// <param name="updateEvent"></param>
        public static void HandleEvent<TMaster, TEntity>(this TMaster master, MasterEvent<TMaster, TEntity> updateEvent)
            where TMaster : Master<TEntity>
            where TEntity : Entity<Guid>
        {
            master.HandleEvent(updateEvent, updateEvent.Master.Current);
        }

        /// <summary>
        /// Update master and child entity via master event an create a new PTA (Point in Time Architecture) record, whith specific clonable record
        /// </summary>
        /// <typeparam name="TMaster"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="master"></param>
        /// <param name="updateEvent"></param>
        /// <param name="clonableRecord"></param>
        public static void HandleEvent<TMaster, TEntity>(this TMaster master, MasterEvent<TMaster, TEntity> updateEvent, TEntity clonableRecord)
            where TMaster : Master<TEntity>
            where TEntity : Entity<Guid>
        {
            TEntity newEntity = (TEntity)clonableRecord.Clone();

            newEntity.SetId(IdentityGenerator.NewSequentialGuid());

            updateEvent.Handle(newEntity);

            master.SetCurrent(newEntity);

            master.SetUpdated();
        }
    }
}