using Sc.Credits.Domain.Model.Base;
using System;

namespace Sc.Credits.Domain.Model.Sequences
{
    /// <summary>
    /// The sequence entity
    /// </summary>
    public class Sequence
        : Entity<Guid>
    {
        /// <summary>
        /// Gets the last number
        /// </summary>
        public long LastNumber { get; private set; }

        /// <summary>
        /// Gets the store's id
        /// </summary>
        public string StoreId { get; private set; }

        /// <summary>
        /// Gets the type
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Sequence"/>
        /// </summary>
        public Sequence()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="Sequence"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lastNumber"></param>
        /// <param name="storeId"></param>
        /// <param name="type"></param>
        public Sequence(Guid id, long lastNumber, string storeId, string type)
        {
            Id = id;
            LastNumber = lastNumber;
            StoreId = storeId;
            Type = type;
        }

        /// <summary>
        /// Sets the last number
        /// </summary>
        /// <param name="lastNumber"></param>
        public void SetLastNumber(long lastNumber)
        {
            LastNumber = lastNumber;
        }
    }
}